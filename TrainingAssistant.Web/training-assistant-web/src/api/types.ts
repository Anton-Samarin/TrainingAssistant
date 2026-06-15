export enum UserSex {
  Male = 0,
  Female = 1,
}

export enum UserGoal {
  LoseWeight = 0,
  GainMuscle = 1,
  Maintain = 2,
}

export enum FitnessLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2,
}

export enum TrainingFocus {
  Strength = 0,
  Endurance = 1,
  Mixed = 2,
}

export interface AuthResponse {
  token: string;
  userId: string;
  name: string;
  email: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  sex: UserSex;
  age: number;
  weightKg: number;
  heightCm: number;
  goal: UserGoal;
  trainingFocus: TrainingFocus;
  fitnessLevel: FitnessLevel;
  sessionsPerWeek: number;
  sessionDurationMin: number;
  activityLevel: number;
  equipment: string[];
  injuries: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ProfileDto {
  name: string;
  sex: UserSex;
  age: number;
  weightKg: number;
  heightCm: number;
  goal: UserGoal;
  trainingFocus: TrainingFocus;
  fitnessLevel: FitnessLevel;
  sessionsPerWeek: number;
  sessionDurationMin: number;
  activityLevel: number;
  equipment: string[];
  injuries: string[];
}

export type UpdateProfileRequest = ProfileDto;

export interface PlanHealthDto {
  bmi: number;
  bmiCategory: string;
  warnings: string[];
  recommendations: string[];
}

export interface MealItemDto {
  id: string;
  foodName: string;
  grams: number;
  calories: number;
  isAlternative?: boolean;
  isCompleted?: boolean;
}

export interface MealDto {
  mealType: string;
  items: MealItemDto[];
}

export interface NutritionDayDto {
  dayIndex: number;
  targetCalories: number;
  targetProteinG: number;
  targetFatG: number;
  targetCarbsG: number;
  meals: MealDto[];
}

export interface TrainingExerciseDto {
  id: string;
  name: string;
  sets: number;
  reps: string;
  restSec: number;
  equipment?: string;
  notes?: string;
  isCompleted?: boolean;
}

export interface TrainingDayDto {
  id: string;
  dayIndex: number;
  dayName: string;
  isRestDay: boolean;
  focus?: string;
  exercises: TrainingExerciseDto[];
}

export interface ReplaceExerciseRequest {
  newName: string;
  rememberForFuture?: boolean;
}

export interface AdjustTrainingDayRequest {
  mode: 'easier' | 'harder';
}

export interface SetCompletedRequest {
  isCompleted: boolean;
}

export interface WeekPlanDto {
  id: string;
  weekStart: string;
  programType?: string;
  programConfidence?: number;
  nutritionDays: NutritionDayDto[];
  trainingDays: TrainingDayDto[];
  health?: PlanHealthDto;
  completedExercises: number;
  totalExercises: number;
  completedMealItems: number;
  totalMealItems: number;
}

export interface WeekSummaryDto {
  id: string;
  weekStart: string;
  isActive: boolean;
  createdAtUtc: string;
  completedExercises: number;
  totalExercises: number;
  completedMealItems: number;
  totalMealItems: number;
}

export interface DayJournalDto {
  date: string;
  weeklyPlanId: string;
  weekStart: string;
  dayIndex: number;
  dayName: string;
  nutrition?: NutritionDayDto;
  training?: TrainingDayDto;
}

export interface BodyMetricLogDto {
  id: string;
  logDate: string;
  weightKg: number;
  note?: string;
}

export interface CreateBodyMetricRequest {
  logDate: string;
  weightKg: number;
  note?: string;
}

export interface WeightProfileSyncDto {
  log?: BodyMetricLogDto;
  profileWeightKg: number;
  bmi: number;
  bmiCategory: string;
}

export interface StrengthRecordDto {
  id: string;
  recordDate: string;
  exerciseName: string;
  weightKg?: number;
  reps?: number;
  sets?: number;
  note?: string;
}

export interface CreateStrengthRecordRequest {
  recordDate: string;
  exerciseName: string;
  weightKg?: number;
  reps?: number;
  sets?: number;
  note?: string;
}

export interface ProgressNoteDto {
  id: string;
  noteDate: string;
  text: string;
}

export interface CreateProgressNoteRequest {
  noteDate: string;
  text: string;
}

export interface ProgressOverviewDto {
  weightLogs: BodyMetricLogDto[];
  strengthRecords: StrengthRecordDto[];
  notes: ProgressNoteDto[];
}
