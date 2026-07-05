export type TaskStatus = 'Todo' | 'InProgress' | 'Done'
export type TaskPriority = 'Low' | 'Medium' | 'High'

export interface Project {
  id: string
  name: string
  description: string | null
  createdAt: string
}

export interface Task {
  id: string
  title: string
  description: string | null
  status: TaskStatus
  priority: TaskPriority
  dueDate: string | null
  projectId: string
  createdAt: string
}

export interface AuthResponse {
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string
  tokenType: string
}

export interface Credentials {
  email: string
  password: string
}

export interface CreateProjectInput {
  name: string
  description?: string | null
}

export interface TaskInput {
  title: string
  description?: string | null
  status: TaskStatus
  priority: TaskPriority
  dueDate?: string | null
}

export interface TaskFilters {
  priority?: TaskPriority
}
