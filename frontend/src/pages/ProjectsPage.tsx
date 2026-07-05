import { NewProjectForm } from '../components/NewProjectForm'
import { ProjectCard } from '../components/ProjectCard'
import { EmptyState, ErrorState, Loading } from '../components/states'
import { useDeleteProject, useProjects } from '../hooks/useProjects'
import { getErrorMessage } from '../lib/utils'
import type { Project } from '../types'

export function ProjectsPage() {
  const projects = useProjects()
  const deleteProject = useDeleteProject()

  function handleDelete(project: Project) {
    if (window.confirm(`Excluir "${project.name}" e todas as suas tasks?`)) {
      deleteProject.mutate(project.id)
    }
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
            <ProjectCard key={project.id} project={project} onDelete={handleDelete} />
          ))}
        </div>
      ) : null}
    </>
  )
}
