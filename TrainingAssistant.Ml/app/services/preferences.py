from __future__ import annotations

from app.schemas.training import ExerciseItem, ExercisePreference


def apply_preferences_to_exercises(
    exercises: list[ExerciseItem],
    preferences: list[ExercisePreference],
) -> list[ExerciseItem]:
    if not preferences:
        return exercises

    pref_map = {p.avoid_name.strip().lower(): p.preferred_name for p in preferences}
    avoid = {p.avoid_name.strip().lower() for p in preferences}

    result: list[ExerciseItem] = []
    for ex in exercises:
        key = ex.name.strip().lower()
        if key in pref_map:
            result.append(ex.model_copy(update={"name": pref_map[key]}))
        elif key not in avoid:
            result.append(ex)
    return result
