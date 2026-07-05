import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import type { AuthResponse } from '../types'
import { tokenStore } from '../lib/tokenStore'

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5023/api'

export const api = axios.create({ baseURL })

export const session = {
  onSignedOut: null as null | (() => void),
}

api.interceptors.request.use((config) => {
  const token = tokenStore.access
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

type RetriableConfig = InternalAxiosRequestConfig & { retried?: boolean }

let refreshInFlight: Promise<string | null> | null = null

function runRefresh(): Promise<string | null> {
  if (!refreshInFlight) {
    const refreshToken = tokenStore.refresh
    refreshInFlight = axios
      .post<AuthResponse>(`${baseURL}/auth/refresh`, { refreshToken })
      .then((response) => {
        tokenStore.save(response.data)
        return response.data.accessToken
      })
      .catch(() => {
        tokenStore.clear()
        return null
      })
      .finally(() => {
        refreshInFlight = null
      })
  }
  return refreshInFlight
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as RetriableConfig | undefined
    const isAuthRoute = original?.url?.includes('/auth/')

    if (error.response?.status === 401 && original && !original.retried && !isAuthRoute && tokenStore.refresh) {
      original.retried = true
      const newAccess = await runRefresh()

      if (newAccess) {
        original.headers.Authorization = `Bearer ${newAccess}`
        return api(original)
      }

      session.onSignedOut?.()
    }

    return Promise.reject(error)
  },
)
