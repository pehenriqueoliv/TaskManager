import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { createProject, deleteProject, getProject, getProjects } from '../api/projects'

export function useProjects() {
  return useQuery({ queryKey: ['projects'], queryFn: getProjects })
}

export function useProject(id: string) {
  return useQuery({ queryKey: ['projects', id], queryFn: () => getProject(id), enabled: id !== '' })
}

export function useCreateProject() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createProject,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects'] }),
  })
}

export function useDeleteProject() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteProject,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects'] }),
  })
}
