import type { AuthResponse, Credentials } from '../types'
import { api } from './client'

export function register(credentials: Credentials): Promise<AuthResponse> {
  return api.post<AuthResponse>('/auth/register', credentials).then((r) => r.data)
}

export function login(credentials: Credentials): Promise<AuthResponse> {
  return api.post<AuthResponse>('/auth/login', credentials).then((r) => r.data)
}
