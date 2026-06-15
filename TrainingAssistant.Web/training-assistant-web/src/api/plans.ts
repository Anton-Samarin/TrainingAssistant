import { apiRequest } from './client';
import type { AdjustTrainingDayRequest, ReplaceExerciseRequest, SetCompletedRequest, WeekPlanDto } from './types';

export function generateWeek() {
  return apiRequest<WeekPlanDto>('/api/plans/week/generate', { method: 'POST' });
}

export function getCurrentWeek() {
  return apiRequest<WeekPlanDto>('/api/plans/week/current');
}

export function getExerciseAlternatives(name: string) {
  return apiRequest<string[]>(`/api/plans/exercises/alternatives?name=${encodeURIComponent(name)}`);
}

export function replaceExercise(exerciseId: string, body: ReplaceExerciseRequest) {
  return apiRequest<WeekPlanDto>(`/api/plans/exercises/${exerciseId}`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  });
}

export function adjustTrainingDay(trainingDayId: string, body: AdjustTrainingDayRequest) {
  return apiRequest<WeekPlanDto>(`/api/plans/training-days/${trainingDayId}/intensity`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  });
}

export function setExerciseCompleted(exerciseId: string, body: SetCompletedRequest) {
  return apiRequest<WeekPlanDto>(`/api/plans/exercises/${exerciseId}/completed`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  });
}

export function setMealItemCompleted(mealItemId: string, body: SetCompletedRequest) {
  return apiRequest<WeekPlanDto>(`/api/plans/meal-items/${mealItemId}/completed`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  });
}
