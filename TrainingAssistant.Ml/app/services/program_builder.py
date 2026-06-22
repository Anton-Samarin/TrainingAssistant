from __future__ import annotations

import re

from app.schemas.training import ExerciseItem, GenerateWeekRequest, TrainingDay, TrainingFocus
from app.services.preferences import apply_preferences_to_exercises

DAY_NAMES = [
    "Понедельник",
    "Вторник",
    "Среда",
    "Четверг",
    "Пятница",
    "Суббота",
    "Воскресенье",
]

EXERCISE_POOL = {
    "upper": [
        ("Отжимания", "mat", ("back",)),
        ("Отжимания с колен", "mat", ("back", "shoulder")),
        ("Жим гантелей лёжа", "dumbbells", ("shoulder",)),
        ("Жим гантелей сидя", "dumbbells", ("shoulder",)),
        ("Тяга гантели в наклоне", "dumbbells", ("back",)),
        ("Разведения гантелей", "dumbbells", ("shoulder",)),
        ("Сгибания рук с гантелями", "dumbbells", ()),
        ("Разгибания из-за головы", "dumbbells", ("shoulder",)),
        ("Подтягивания", "pullup_bar", ("shoulder", "back")),
        ("Обратные отжимания от скамьи", "gym", ("shoulder",)),
        ("Жим штанги лёжа", "barbell", ("shoulder", "back")),
        ("Тяга штанги в наклоне", "barbell", ("back",)),
        ("Жим в тренажёре", "gym", ("shoulder",)),
    ],
    "lower": [
        ("Приседания с собственным весом", "mat", ("knee",)),
        ("Приседания с гантелями", "dumbbells", ("knee",)),
        ("Гоблет-присед", "dumbbells", ("knee",)),
        ("Выпады", "mat", ("knee",)),
        ("Выпады с гантелями", "dumbbells", ("knee",)),
        ("Ягодичный мост", "mat", ()),
        ("Мостик на одной ноге", "mat", ("knee",)),
        ("Становая с гантелями", "dumbbells", ("back", "knee")),
        ("Приседания со штангой", "barbell", ("knee", "back")),
        ("Жим ногами в тренажёре", "gym", ("knee",)),
        ("Подъёмы на носки стоя", "mat", ()),
        ("Болгарские выпады", "dumbbells", ("knee",)),
    ],
    "full": [
        ("Планка", "mat", ()),
        ("Боковая планка", "mat", ()),
        ("Берпи (облегчённый)", "mat", ("knee", "back")),
        ("Шаги на месте с высоким коленом", "mat", ("knee",)),
        ("Прыжки на месте", "mat", ("knee",)),
        ("Скакалка", "mat", ("knee",)),
        ("Гоблет-присед", "dumbbells", ("knee",)),
        ("Махи гантелью", "dumbbells", ()),
        ("Гребля в наклоне с гантелями", "dumbbells", ("back",)),
        ("Велотренажёр", "gym", ("knee",)),
        ("Эллипс", "gym", ("knee",)),
    ],
    "safe": [
        ("Планка", "mat", ()),
        ("Боковая планка", "mat", ()),
        ("Ягодичный мост", "mat", ()),
        ("Жим гантелей сидя", "dumbbells", ("shoulder",)),
        ("Сгибания рук с гантелями", "dumbbells", ()),
        ("Велосипед лёжа", "mat", ("back",)),
        ("Подъём коленей лёжа", "mat", ()),
        ("Разведения гантелей лёжа", "dumbbells", ("shoulder",)),
        ("Приседания у стены", "mat", ("knee",)),
        ("Шаги на месте", "mat", ()),
        ("Растяжка задней поверхности бедра", "mat", ()),
    ],
}

FOCUS_BY_SLOT = ["Верх тела", "Низ тела", "Всё тело", "Кардио и кор", "Сила", "Выносливость"]

HIGH_IMPACT = ("прыжк", "берпи", "скакалк")


def _sessions_from_program_type(program_type: str) -> int:
    match = re.search(r"(\d+)x$", program_type)
    if match:
        return int(match.group(1))
    return 3


def _training_day_indices(count: int) -> list[int]:
    patterns = {
        2: [1, 4],
        3: [1, 3, 5],
        4: [1, 2, 4, 6],
        5: [1, 2, 3, 5, 6],
        6: [1, 2, 3, 4, 5, 6],
    }
    return patterns.get(count, patterns[3])


def _injury_tags(request: GenerateWeekRequest) -> set[str]:
    return set(request.injuries)


def _available_equipment(request: GenerateWeekRequest) -> set[str]:
    tags = {"mat"}
    if "dumbbells" in request.equipment:
        tags.add("dumbbells")
    if "barbell" in request.equipment:
        tags.add("barbell")
    if "pullup_bar" in request.equipment or "pull_up_bar" in request.equipment:
        tags.add("pullup_bar")
    if "gym" in request.equipment or "full_gym" in request.equipment:
        tags.update({"dumbbells", "barbell", "pullup_bar", "gym"})
    return tags


def _equipment_ok(eq_tag: str, equipment: set[str]) -> bool:
    if eq_tag == "mat":
        return True
    if eq_tag == "gym":
        return "gym" in equipment
    return eq_tag in equipment


def _bmi(request: GenerateWeekRequest) -> float:
    h = request.height_cm / 100
    return request.weight_kg / (h * h)


def _avoid_names(request: GenerateWeekRequest) -> set[str]:
    return {p.avoid_name.strip().lower() for p in request.exercise_preferences}


def _pick_exercises(
    pool_key: str,
    request: GenerateWeekRequest,
    count: int = 5,
) -> list[ExerciseItem]:
    injuries = _injury_tags(request)
    equipment = _available_equipment(request)
    bmi = _bmi(request)
    avoid = _avoid_names(request)

    if injuries or bmi >= 30:
        pool_key = "safe" if bmi >= 30 or injuries else pool_key
    pool = EXERCISE_POOL["safe"] if injuries else EXERCISE_POOL[pool_key]

    selected: list[ExerciseItem] = []
    for name, eq_tag, blocked in pool:
        if name.strip().lower() in avoid:
            continue
        if bmi >= 30 and any(x in name.lower() for x in HIGH_IMPACT):
            continue
        if any(tag in injuries for tag in blocked):
            continue
        if not _equipment_ok(eq_tag, equipment):
            continue
        sets = 3 if request.fitness_level.value != "advanced" else 4
        reps = "10-12" if "fat" in pool_key or injuries else "8-10"
        if "Планка" in name or "планка" in name.lower():
            reps = "30-45 сек"
        selected.append(
            ExerciseItem(
                name=name,
                sets=sets,
                reps=reps,
                rest_sec=60 if injuries else 90,
                equipment=eq_tag,
            )
        )
        if len(selected) >= count:
            break

    if not selected:
        selected.append(
            ExerciseItem(name="Планка", sets=3, reps="30-45 сек", rest_sec=45, equipment="mat")
        )
    selected = apply_preferences_to_exercises(selected, request.exercise_preferences)
    return selected


def _pool_for_day(program_type: str, slot: int, injuries: set[str], request: GenerateWeekRequest) -> str:
    if injuries or "injury" in program_type:
        return "safe"
    if request.training_focus == TrainingFocus.endurance:
        return ["full", "full", "lower", "full"][slot % 4]
    if "fat_loss" in program_type:
        pools = ["upper", "lower", "full", "full"]
        if request.training_focus == TrainingFocus.endurance:
            pools = ["full", "full", "lower", "full"]
        return pools[slot % 4]
    if "muscle" in program_type or "strength" in program_type:
        return ["upper", "lower", "upper", "lower"][slot % 4]
    return ["full", "upper", "lower", "full"][slot % 4]


def build_week(program_type: str, request: GenerateWeekRequest) -> list[TrainingDay]:
    sessions = _sessions_from_program_type(program_type)
    training_days = set(_training_day_indices(sessions))
    injuries = _injury_tags(request)
    days: list[TrainingDay] = []
    workout_slot = 0

    for day_index in range(1, 8):
        if day_index not in training_days:
            days.append(
                TrainingDay(
                    day_index=day_index,
                    day_name=DAY_NAMES[day_index - 1],
                    is_rest_day=True,
                    focus="Отдых",
                    exercises=[],
                )
            )
            continue

        pool_key = _pool_for_day(program_type, workout_slot, injuries, request)
        focus = FOCUS_BY_SLOT[workout_slot % len(FOCUS_BY_SLOT)]
        if request.training_focus == TrainingFocus.endurance and workout_slot % 2 == 1:
            focus = "Выносливость"
        bmi = _bmi(request)
        exercise_count = 6 if request.fitness_level.value == "advanced" else 5
        if bmi >= 30:
            exercise_count = min(exercise_count, 4)
        if request.session_duration_min <= 30:
            exercise_count = min(exercise_count, 4)
        days.append(
            TrainingDay(
                day_index=day_index,
                day_name=DAY_NAMES[day_index - 1],
                is_rest_day=False,
                focus=focus,
                exercises=_pick_exercises(pool_key, request, count=exercise_count),
            )
        )
        workout_slot += 1

    return days
