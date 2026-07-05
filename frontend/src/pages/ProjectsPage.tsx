import { useState } from 'react'
import { ConfirmDialog } from '../components/ConfirmDialog'
import { NewProjectForm } from '../components/NewProjectForm'
import { ProjectCard } from '../components/ProjectCard'
import { EmptyState, ErrorState, Loading } from '../components/states'
import { useDeleteProject, useProjects } from '../hooks/useProjects'
import { getErrorMessage } from '../lib/utils'
import type { Project } from '../types'

export function ProjectsPage() {
  const projects = useProjects()
  const deleteProject = useDeleteProject()
  const [target, setTarget] = useState<Project | null>(null)

  function confirmDelete() {
    if (!target) return
    deleteProject.mutate(target.id, { onSettled: () => setTarget(null) })
  }

  return (
    <>
      <div className="page__head">
        <div>
          <span className="eyebrow">Seu espaço</span>
          <h1 className="page__title">
            Projetos
            {projects.data ? <span className="page__count">{projects.data.length}</span> : null}
          </h1>
        </div>
      </div>

      <NewProjectForm />

      {projects.isLoading ? <Loading label="Carregando projetos…" /> : null}
      {projects.isError ? <ErrorState message={getErrorMessage(projects.error)} /> : null}
      {projects.data && projects.data.length === 0 ? (
        <EmptyState title="Nenhum projeto ainda" text="Crie seu primeiro projeto no formulário acima." />
      ) : null}
      {projects.data && projects.data.length > 0 ? (
        <div className="projects-grid">
          {projects.data.map((project) => (
            <ProjectCard key={project.id} project={project} onDelete={setTarget} />
          ))}
        </div>
      ) : null}

      <ConfirmDialog
        open={target !== null}
        title="Excluir projeto"
        message={`"${target?.name}" e todas as suas tasks serão excluídos. Essa ação não pode ser desfeita.`}
        confirmLabel="Excluir projeto"
        pending={deleteProject.isPending}
        onConfirm={confirmDelete}
        onCancel={() => setTarget(null)}
      />
    </>
  )
}
