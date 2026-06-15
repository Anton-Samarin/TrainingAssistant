"""Feature extraction shared by training and inference."""

from __future__ import annotations

from typing import Any

import numpy as np
import pandas as pd

FEATURE_COLUMNS = [
    "age",
    "sex",
    "weight_kg",
    "height_cm",
    "bmi",
    "goal",
    "fitness_level",
    "sessions_per_week",
    "session_duration_min",
    "activity_level",
    "has_gym",
    "has_dumbbells",
    "has_barbell",
    "has_pullup_bar",
    "injury_knee",
    "injury_back",
    "injury_shoulder",
]

LABEL_COLUMN = "program_type"


def profile_to_row(profile: dict[str, Any]) -> dict[str, Any]:
    """Normalize API/dataset dict and ensure BMI exists."""
    row = {col: profile.get(col) for col in FEATURE_COLUMNS}
    if row["bmi"] is None and row["weight_kg"] and row["height_cm"]:
        h_m = float(row["height_cm"]) / 100
        row["bmi"] = round(float(row["weight_kg"]) / (h_m**2), 2)
    return row


def rows_to_frame(rows: list[dict[str, Any]] | pd.DataFrame) -> pd.DataFrame:
    if isinstance(rows, pd.DataFrame):
        df = rows.copy()
    else:
        df = pd.DataFrame([profile_to_row(r) for r in rows])
    return df[FEATURE_COLUMNS].astype(float)


def split_features_labels(df: pd.DataFrame) -> tuple[pd.DataFrame, pd.Series]:
    x = df[FEATURE_COLUMNS].astype(float)
    y = df[LABEL_COLUMN]
    return x, y
