from fastapi import APIRouter, Depends

from app.api.deps import verify_api_key
from app.schemas.training import GenerateWeekRequest, GenerateWeekResponse
from app.services.classifier import ProgramClassifier
from app.services.obesity import apply_obesity_to_program_type
from app.services.program_builder import build_week
from app.state import get_classifier

router = APIRouter()


@router.post(
    "/v1/training/generate-week",
    response_model=GenerateWeekResponse,
    dependencies=[Depends(verify_api_key)],
)
def generate_week(
    request: GenerateWeekRequest,
    classifier: ProgramClassifier = Depends(get_classifier),
) -> GenerateWeekResponse:
    profile = request.to_feature_dict()
    program_type, confidence = classifier.predict(profile)
    program_type = apply_obesity_to_program_type(program_type, profile["bmi"], request)
    days = build_week(program_type, request)
    return GenerateWeekResponse.from_days(program_type, confidence, days)
