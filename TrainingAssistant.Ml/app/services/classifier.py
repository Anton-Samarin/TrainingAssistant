from __future__ import annotations

from pathlib import Path
from typing import Any, Dict, Optional, Tuple

import joblib
import numpy as np
import pandas as pd
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import LabelEncoder

from ml.features import rows_to_frame


class ProgramClassifier:
    def __init__(self, model_path: Path) -> None:
        self.model_path = model_path
        self._artifact: Optional[Dict[str, Any]] = None

    @property
    def is_loaded(self) -> bool:
        return self._artifact is not None

    def load(self) -> None:
        if not self.model_path.exists():
            raise FileNotFoundError(
                f"Model not found: {self.model_path}. Run: python -m ml.train"
            )
        self._artifact = joblib.load(self.model_path)

    def _require_artifact(self) -> Dict[str, Any]:
        if self._artifact is None:
            self.load()
        assert self._artifact is not None
        return self._artifact

    @property
    def pipeline(self) -> Pipeline:
        return self._require_artifact()["pipeline"]

    @property
    def label_encoder(self) -> LabelEncoder:
        return self._require_artifact()["label_encoder"]

    def predict(self, profile: dict) -> Tuple[str, float]:
        artifact = self._require_artifact()
        frame = rows_to_frame([profile])
        proba = self.pipeline.predict_proba(frame)[0]
        class_idx = int(np.argmax(proba))
        program_type = artifact["label_encoder"].classes_[class_idx]
        confidence = float(proba[class_idx])
        return program_type, confidence
