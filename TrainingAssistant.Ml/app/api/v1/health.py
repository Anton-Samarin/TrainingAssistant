from fastapi import APIRouter

from app.config import settings
from app.schemas.training import HealthResponse

router = APIRouter()


@router.get("/health", response_model=HealthResponse)
def health() -> HealthResponse:
    loaded = settings.model_path.exists()
    return HealthResponse(
        status="ok",
        model_loaded=loaded,
        model_path=str(settings.model_path),
    )
