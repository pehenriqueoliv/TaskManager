import type { CreateProjectInput, Project } from '../types'
import { api } from './client'

export function getProjects(): Promise<Project[]> {
  return api.get<Project[]>('/projects').then((r) => r.data)
}

export function getProject(id: string): Promise<Project> {
  return api.get<Project>(`/projects/${id}`).then((r) => r.data)
}

export function createProject(input: CreateProjectInput): Promise<Project> {
  return api.post<Project>('/projects', input).then((r) => r.data)
}

export function deleteProject(id: string): Promise<void> {
  return api.delete(`/projects/${id}`).then(() => undefined)
}
