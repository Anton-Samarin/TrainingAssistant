import { apiRequest } from './client';
import type { AuthResponse, LoginRequest, RegisterRequest } from './types';

export function register(data: RegisterRequest) {
  return apiRequest<AuthResponse>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function login(data: LoginRequest) {
  return apiRequest<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
