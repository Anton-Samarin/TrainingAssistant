import { apiRequest } from './client';
import type { ProfileDto, UpdateProfileRequest } from './types';

export function getProfile() {
  return apiRequest<ProfileDto>('/api/profile');
}

export function updateProfile(data: UpdateProfileRequest) {
  return apiRequest<ProfileDto>('/api/profile', {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}
