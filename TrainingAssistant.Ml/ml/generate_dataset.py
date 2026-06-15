"""
Synthetic training dataset for program-type classification.

Each row: user profile features -> program_type (expert rules).
Run: python -m ml.generate_dataset [--rows 8000] [--output data/training_dataset.csv]
"""

from __future__ import annotations

import argparse
from pathlib import Path

import numpy as np
import pandas as pd

from ml.features import FEATURE_COLUMNS, LABEL_COLUMN
from ml.program_types import (
    GOAL_GAIN_MUSCLE,
    GOAL_LOSE_WEIGHT,
    GOAL_MAINTAIN,
    LEVEL_ADVANCED,
    LEVEL_BEGINNER,
    LEVEL_INTERMEDIATE,
)

RANDOM_SEED = 42


def _max_sessions_for_duration(duration_min: int) -> int:
    """Короткие сессии — в разметке не назначаем плотные 5–6-дневные программы."""
    if duration_min <= 30:
        return 3
    if duration_min <= 45:
        return 4
    if duration_min <= 60:
        return 5
    return 6


def _effective_sessions(sessions: int, duration_min: int) -> int:
    return min(sessions, _max_sessions_for_duration(duration_min))


def assign_program_type(row: dict) -> str:
    goal = int(row["goal"])
    level = int(row["fitness_level"])
    gym = bool(row["has_gym"])
    duration = int(row["session_duration_min"])
    sessions = _effective_sessions(int(row["sessions_per_week"]), duration)
    knee = bool(row["injury_knee"])
    back = bool(row["injury_back"])

    shoulder = bool(row["injury_shoulder"])
    bmi = float(row.get("bmi", 22))

    if bmi >= 35 and not (knee or back or shoulder):
        if gym:
            return "int_injury_safe_gym_3x"
        return "beg_injury_safe_home_3x"

    if knee or back or shoulder:
        if gym and level >= LEVEL_INTERMEDIATE:
            return "int_injury_safe_gym_3x"
        return "beg_injury_safe_home_3x"

    home = not gym

    if bmi >= 30 and goal == GOAL_LOSE_WEIGHT:
        level = min(level, LEVEL_INTERMEDIATE)

    if goal == GOAL_LOSE_WEIGHT:
        if level == LEVEL_BEGINNER:
            if home:
                return "beg_fat_loss_home_3x" if sessions <= 3 else "beg_fat_loss_home_4x"
            return "beg_fat_loss_gym_3x" if sessions <= 3 else "beg_fat_loss_gym_4x"
        if level == LEVEL_INTERMEDIATE:
            if home:
                return "int_fat_loss_home_4x"
            return "int_fat_loss_gym_4x" if sessions <= 4 else "int_fat_loss_gym_5x"
        return "adv_fat_loss_gym_5x"

    if goal == GOAL_GAIN_MUSCLE:
        if level == LEVEL_BEGINNER:
            if home:
                return "beg_muscle_home_3x"
            return "beg_muscle_gym_3x" if sessions <= 3 else "beg_muscle_gym_4x"
        if level == LEVEL_INTERMEDIATE:
            if home:
                return "int_muscle_home_4x"
            return "int_muscle_gym_4x" if sessions <= 4 else "int_muscle_gym_5x"
        if not gym:
            return "int_muscle_home_4x"
        if sessions >= 5:
            return "adv_strength_gym_5x"
        return "adv_muscle_gym_5x"

    # maintain
    if level == LEVEL_BEGINNER:
        if home:
            return "beg_maintain_home_2x" if sessions <= 2 else "beg_maintain_home_3x"
        return "int_maintain_gym_3x"
    return "int_maintain_gym_4x" if sessions <= 4 else "int_maintain_gym_4x"


def _random_profile(rng: np.random.Generator) -> dict:
    sex = int(rng.integers(0, 2))
    age = int(rng.integers(18, 56))

    if sex == 0:
        height_cm = int(rng.normal(178, 7))
        weight_kg = float(rng.normal(80, 14))
    else:
        height_cm = int(rng.normal(165, 6))
        weight_kg = float(rng.normal(65, 12))

    height_cm = int(np.clip(height_cm, 150, 205))
    weight_kg = round(float(np.clip(weight_kg, 45, 140)), 1)
    bmi = round(weight_kg / ((height_cm / 100) ** 2), 2)

    bmi_preview = weight_kg / ((height_cm / 100) ** 2)
    if bmi_preview >= 28:
        goal = int(rng.choice([0, 1, 2], p=[0.60, 0.25, 0.15]))
    elif bmi_preview < 20:
        goal = int(rng.choice([0, 1, 2], p=[0.20, 0.55, 0.25]))
    else:
        goal = int(rng.choice([0, 1, 2], p=[0.40, 0.35, 0.25]))

    fitness_level = int(rng.choice([0, 1, 2], p=[0.50, 0.35, 0.15]))
    gym_prob = 0.25 + 0.20 * fitness_level
    has_gym = bool(rng.random() < gym_prob)
    has_dumbbells = bool(rng.random() < (0.55 if not has_gym else 0.25))
    has_barbell = bool(has_gym and rng.random() < 0.70)
    has_pullup_bar = bool(rng.random() < (0.20 if has_gym else 0.35))

    if not has_gym and not (has_dumbbells or has_pullup_bar):
        has_dumbbells = bool(rng.random() < 0.60)

    injury_knee = bool(rng.random() < 0.08)
    injury_back = bool(rng.random() < 0.07)
    injury_shoulder = bool(rng.random() < 0.06)

    if fitness_level == LEVEL_BEGINNER:
        sessions_per_week = int(rng.integers(2, 5))
        session_duration_min = int(np.clip(int(rng.normal(38, 8)), 20, 55))
    elif fitness_level == LEVEL_INTERMEDIATE:
        sessions_per_week = int(rng.integers(3, 6))
        session_duration_min = int(np.clip(int(rng.normal(48, 10)), 30, 75))
    else:
        sessions_per_week = int(rng.integers(4, 7))
        session_duration_min = int(np.clip(int(rng.normal(62, 12)), 40, 120))

    activity_level = int(np.clip(int(rng.normal(2.5 + fitness_level, 1)), 1, 5))

    return {
        "age": age,
        "sex": sex,
        "weight_kg": weight_kg,
        "height_cm": height_cm,
        "bmi": bmi,
        "goal": goal,
        "fitness_level": fitness_level,
        "sessions_per_week": sessions_per_week,
        "session_duration_min": session_duration_min,
        "activity_level": activity_level,
        "has_gym": int(has_gym),
        "has_dumbbells": int(has_dumbbells),
        "has_barbell": int(has_barbell),
        "has_pullup_bar": int(has_pullup_bar),
        "injury_knee": int(injury_knee),
        "injury_back": int(injury_back),
        "injury_shoulder": int(injury_shoulder),
    }


def generate_dataset(rows: int, seed: int = RANDOM_SEED) -> pd.DataFrame:
    rng = np.random.default_rng(seed)
    records: list[dict] = []

    for _ in range(rows):
        profile = _random_profile(rng)
        profile[LABEL_COLUMN] = assign_program_type(profile)
        records.append(profile)

    df = pd.DataFrame(records)
    return df[FEATURE_COLUMNS + [LABEL_COLUMN]]


def main() -> None:
    parser = argparse.ArgumentParser(description="Generate synthetic training dataset.")
    parser.add_argument("--rows", type=int, default=8000, help="Number of samples (default: 8000)")
    parser.add_argument(
        "--output",
        type=Path,
        default=Path(__file__).resolve().parent.parent / "data" / "training_dataset.csv",
    )
    parser.add_argument("--seed", type=int, default=RANDOM_SEED)
    args = parser.parse_args()

    args.output.parent.mkdir(parents=True, exist_ok=True)
    df = generate_dataset(args.rows, seed=args.seed)
    df.to_csv(args.output, index=False, encoding="utf-8")

    print(f"Saved {len(df)} rows -> {args.output}")
    print("\nClass distribution:")
    counts = df[LABEL_COLUMN].value_counts().sort_index()
    for label, count in counts.items():
        print(f"  {label}: {count} ({100 * count / len(df):.1f}%)")


if __name__ == "__main__":
    main()
