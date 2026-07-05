import type { Task, TaskFilters, TaskInput } from '../types'
import { api } from './client'

export function getTasks(projectId: string, filters: TaskFilters): Promise<Task[]> {
  return api.get<Task[]>(`/projects/${projectId}/tasks`, { params: filters }).then((r) => r.data)
}

export function createTask(projectId: string, input: TaskInput): Promise<Task> {
  return api.post<Task>(`/projects/${projectId}/tasks`, input).then((r) => r.data)
}

export function updateTask(projectId: string, id: string, input: TaskInput): Promise<Task> {
  return api.put<Task>(`/projects/${projectId}/tasks/${id}`, input).then((r) => r.data)
}

export function deleteTask(projectId: string, id: string): Promise<void> {
  return api.delete(`/projects/${projectId}/tasks/${id}`).then(() => undefined)
}
