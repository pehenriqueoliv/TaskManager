import { AxiosError } from 'axios'

interface ProblemDetails {
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

export function getErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    if (!error.response) return 'Não foi possível conectar à API. Ela está rodando?'

    const data = error.response.data as ProblemDetails | undefined
    if (data?.errors) {
      const first = Object.values(data.errors)[0]
      if (first?.length) return first[0]
    }
    if (data?.detail) return data.detail
    if (data?.title) return data.title
  }
  return 'Algo deu errado. Tente novamente.'
}

export function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  })
}

export function dateInputToIso(value: string): string | null {
  if (!value) return null
  return new Date(`${value}T12:00:00.000Z`).toISOString()
}
