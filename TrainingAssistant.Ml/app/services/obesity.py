from __future__ import annotations

from app.schemas.training import GenerateWeekRequest, Goal


def apply_obesity_to_program_type(program_type: str, bmi: float, request: GenerateWeekRequest) -> str:
    injuries = set(request.injuries)
    if injuries:
        return program_type

    has_gym = "gym" in request.equipment or "full_gym" in request.equipment
    home = not has_gym

    if bmi >= 35:
        if has_gym:
            return "int_injury_safe_gym_3x"
        return "beg_injury_safe_home_3x"

    if bmi >= 30:
        if program_type.startswith("adv_"):
            if request.goal == Goal.lose_weight:
                return "int_fat_loss_gym_4x" if has_gym else "int_fat_loss_home_4x"
            return "int_injury_safe_gym_3x" if has_gym else "beg_injury_safe_home_3x"
        if request.goal == Goal.gain_muscle and "muscle" in program_type:
            return "beg_fat_loss_home_3x" if home else "beg_fat_loss_gym_3x"

    return program_type
