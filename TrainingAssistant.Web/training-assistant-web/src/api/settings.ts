import { apiRequest } from './client';

export interface ClearWeeksResult {
  deletedCount: number;
}

export function clearAllWeeks() {
  return apiRequest<ClearWeeksResult>('/api/settings/weeks', { method: 'DELETE' });
}

export function deleteAccount(password: string) {
  return apiRequest<void>('/api/settings/account', {
    method: 'DELETE',
    body: JSON.stringify({ password }),
  });
}
