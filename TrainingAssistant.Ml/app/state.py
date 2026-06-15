from functools import lru_cache

from app.config import settings
from app.services.classifier import ProgramClassifier


@lru_cache
def get_classifier() -> ProgramClassifier:
    classifier = ProgramClassifier(settings.model_path)
    classifier.load()
    return classifier
