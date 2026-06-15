from pathlib import Path

from pydantic_settings import BaseSettings, SettingsConfigDict

ROOT_DIR = Path(__file__).resolve().parent.parent


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_file_encoding="utf-8", extra="ignore")

    api_key: str = "dev-ml-key"
    model_path: Path = ROOT_DIR / "artifacts" / "program_classifier.pkl"
    app_name: str = "TrainingAssistant.Ml"


settings = Settings()
