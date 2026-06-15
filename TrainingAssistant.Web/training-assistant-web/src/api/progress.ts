import { apiRequest } from './client';
import type {
  CreateBodyMetricRequest,
  WeightProfileSyncDto,
  CreateProgressNoteRequest,
  CreateStrengthRecordRequest,
  ProgressOverviewDto,
} from './types';

export function getProgressOverview() {
  return apiRequest<ProgressOverviewDto>('/api/progress/overview');
}

export function addWeightLog(body: CreateBodyMetricRequest) {
  return apiRequest<WeightProfileSyncDto>('/api/progress/weight', {
    method: 'POST',
    body: JSON.stringify(body),
  });
}

export function deleteWeightLog(id: string) {
  return apiRequest<WeightProfileSyncDto>(`/api/progress/weight/${id}`, { method: 'DELETE' });
}

/** Сигнал для страниц: вес в профиле и ИМТ обновились */
export function notifyProfileWeightUpdated() {
  window.dispatchEvent(new Event('ta-profile-weight-updated'));
}

export function addStrengthRecord(body: CreateStrengthRecordRequest) {
  return apiRequest('/api/progress/strength', { method: 'POST', body: JSON.stringify(body) });
}

export function deleteStrengthRecord(id: string) {
  return apiRequest<void>(`/api/progress/strength/${id}`, { method: 'DELETE' });
}

export function addProgressNote(body: CreateProgressNoteRequest) {
  return apiRequest('/api/progress/notes', { method: 'POST', body: JSON.stringify(body) });
}

export function deleteProgressNote(id: string) {
  return apiRequest<void>(`/api/progress/notes/${id}`, { method: 'DELETE' });
}
