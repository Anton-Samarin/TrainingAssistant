from __future__ import annotations

import argparse
import json
from datetime import datetime, timezone
from pathlib import Path

import joblib
import numpy as np
import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import (
    accuracy_score,
    classification_report,
    confusion_matrix,
    f1_score,
)
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.pipeline import Pipeline

from ml.features import FEATURE_COLUMNS, LABEL_COLUMN, split_features_labels


def train_model(
    df: pd.DataFrame,
    test_size: float = 0.2,
    random_state: int = 42,
) -> tuple[Pipeline, LabelEncoder, dict]:
    x, y_raw = split_features_labels(df)

    label_encoder = LabelEncoder()
    y = label_encoder.fit_transform(y_raw)

    x_train, x_test, y_train, y_test = train_test_split(
        x,
        y,
        test_size=test_size,
        random_state=random_state,
        stratify=y,
    )

    pipeline = Pipeline(
        steps=[
            (
                "clf",
                RandomForestClassifier(
                    n_estimators=300,
                    max_depth=None,
                    min_samples_leaf=2,
                    class_weight="balanced",
                    random_state=random_state,
                    n_jobs=-1,
                ),
            ),
        ]
    )
    pipeline.fit(x_train, y_train)

    y_pred = pipeline.predict(x_test)
    metrics = {
        "accuracy": float(accuracy_score(y_test, y_pred)),
        "f1_macro": float(f1_score(y_test, y_pred, average="macro")),
        "test_size": len(y_test),
        "train_size": len(y_train),
        "class_count": int(len(label_encoder.classes_)),
        "classes": label_encoder.classes_.tolist(),
        "classification_report": classification_report(
            y_test,
            y_pred,
            target_names=label_encoder.classes_,
            output_dict=True,
            zero_division=0,
        ),
        "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
    }

    artifact = {
        "pipeline": pipeline,
        "label_encoder": label_encoder,
        "feature_columns": FEATURE_COLUMNS,
        "label_column": LABEL_COLUMN,
        "trained_at": datetime.now(timezone.utc).isoformat(),
        "metrics": metrics,
    }
    return pipeline, label_encoder, artifact


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--data",
        type=Path,
        default=Path(__file__).resolve().parent.parent / "data" / "training_dataset.csv",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path(__file__).resolve().parent.parent / "artifacts" / "program_classifier.pkl",
    )
    parser.add_argument(
        "--metrics",
        type=Path,
        default=Path(__file__).resolve().parent.parent / "artifacts" / "training_metrics.json",
    )
    parser.add_argument("--test-size", type=float, default=0.2)
    parser.add_argument("--seed", type=int, default=42)
    args = parser.parse_args()

    if not args.data.exists():
        raise FileNotFoundError(
            f"Dataset not found: {args.data}. Run: python -m ml.generate_dataset"
        )

    df = pd.read_csv(args.data)
    missing = [c for c in FEATURE_COLUMNS + [LABEL_COLUMN] if c not in df.columns]
    if missing:
        raise ValueError(f"Dataset missing columns: {missing}")

    _, _, artifact = train_model(df, test_size=args.test_size, random_state=args.seed)

    args.output.parent.mkdir(parents=True, exist_ok=True)
    joblib.dump(artifact, args.output)

    metrics_path = args.metrics
    metrics_path.parent.mkdir(parents=True, exist_ok=True)
    with metrics_path.open("w", encoding="utf-8") as f:
        json.dump(artifact["metrics"], f, ensure_ascii=False, indent=2)

    m = artifact["metrics"]
    print(f"Saved model -> {args.output}")
    print(f"Saved metrics -> {metrics_path}")
    print(f"Accuracy: {m['accuracy']:.4f}")
    print(f"F1 macro: {m['f1_macro']:.4f}")
    print(f"Classes: {m['class_count']}")


if __name__ == "__main__":
    main()
