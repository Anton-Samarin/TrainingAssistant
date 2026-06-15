import { apiRequest } from './client';
import type { DayJournalDto, WeekPlanDto, WeekSummaryDto } from './types';

export function listJournalWeeks(limit = 30) {
  return apiRequest<WeekSummaryDto[]>(`/api/journal/weeks?limit=${limit}`);
}

export function getJournalWeek(planId: string) {
  return apiRequest<WeekPlanDto>(`/api/journal/weeks/${planId}`);
}

export function getJournalDay(date: string, planId?: string) {
  const q = planId ? `&planId=${planId}` : '';
  return apiRequest<DayJournalDto>(`/api/journal/day?date=${date}${q}`);
}
