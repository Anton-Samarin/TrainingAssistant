from __future__ import annotations

from enum import Enum
from typing import List, Literal, Optional
from uuid import UUID

from pydantic import BaseModel, Field, field_validator


class Goal(str, Enum):
    lose_weight = "lose_weight"
    gain_muscle = "gain_muscle"
    maintain = "maintain"


class Sex(str, Enum):
    male = "male"
    female = "female"


class FitnessLevel(str, Enum):
    beginner = "beginner"
    intermediate = "intermediate"
    advanced = "advanced"


class TrainingFocus(str, Enum):
    strength = "strength"
    endurance = "endurance"
    mixed = "mixed"


class ExercisePreference(BaseModel):
    avoid_name: str
    preferred_name: str
    pool_key: Optional[str] = None


GOAL_TO_INT = {Goal.lose_weight: 0, Goal.gain_muscle: 1, Goal.maintain: 2}
SEX_TO_INT = {Sex.male: 0, Sex.female: 1}
LEVEL_TO_INT = {
    FitnessLevel.beginner: 0,
    FitnessLevel.intermediate: 1,
    FitnessLevel.advanced: 2,
}


class GenerateWeekRequest(BaseModel):
    user_id: UUID
    goal: Goal
    sex: Sex
    age: int = Field(ge=14, le=80)
    weight_kg: float = Field(gt=30, le=250)
    height_cm: int = Field(ge=130, le=220)
    fitness_level: FitnessLevel
    sessions_per_week: int = Field(ge=2, le=6)
    session_duration_min: int = Field(default=45, ge=20, le=120)
    activity_level: int = Field(default=3, ge=1, le=5)
    equipment: List[str] = Field(default_factory=list)
    injuries: List[str] = Field(default_factory=list)
    locale: str = "ru"
    training_focus: TrainingFocus = TrainingFocus.mixed
    exercise_preferences: List[ExercisePreference] = Field(default_factory=list)

    @field_validator("equipment", "injuries", mode="before")
    @classmethod
    def lowercase_items(cls, value: Optional[List[str]]) -> List[str]:
        if not value:
            return []
        return [item.strip().lower() for item in value if item and item.strip()]

    def to_feature_dict(self) -> dict:
        height_m = self.height_cm / 100
        bmi = round(self.weight_kg / (height_m**2), 2)
        equipment = set(self.equipment)
        injuries = set(self.injuries)

        has_gym = "gym" in equipment or "full_gym" in equipment
        has_dumbbells = "dumbbells" in equipment
        has_barbell = "barbell" in equipment
        has_pullup_bar = "pullup_bar" in equipment or "pull_up_bar" in equipment

        return {
            "age": self.age,
            "sex": SEX_TO_INT[self.sex],
            "weight_kg": self.weight_kg,
            "height_cm": self.height_cm,
            "bmi": bmi,
            "goal": GOAL_TO_INT[self.goal],
            "fitness_level": LEVEL_TO_INT[self.fitness_level],
            "sessions_per_week": self.sessions_per_week,
            "session_duration_min": self.session_duration_min,
            "activity_level": self.activity_level,
            "has_gym": int(has_gym),
            "has_dumbbells": int(has_dumbbells),
            "has_barbell": int(has_barbell),
            "has_pullup_bar": int(has_pullup_bar),
            "injury_knee": int("knee" in injuries),
            "injury_back": int("back" in injuries),
            "injury_shoulder": int("shoulder" in injuries),
        }


class ExerciseItem(BaseModel):
    name: str
    sets: int
    reps: str
    rest_sec: int
    equipment: Optional[str] = None
    notes: Optional[str] = None


class TrainingDay(BaseModel):
    day_index: int = Field(ge=1, le=7)
    day_name: str
    is_rest_day: bool
    focus: Optional[str] = None
    exercises: List[ExerciseItem] = Field(default_factory=list)


class GenerateWeekResponse(BaseModel):
    program_type: str
    confidence: float
    week: dict

    @classmethod
    def from_days(cls, program_type: str, confidence: float, days: List[TrainingDay]) -> "GenerateWeekResponse":
        return cls(
            program_type=program_type,
            confidence=confidence,
            week={"days": days},
        )


class HealthResponse(BaseModel):
    status: Literal["ok"]
    model_loaded: bool
    model_path: str
