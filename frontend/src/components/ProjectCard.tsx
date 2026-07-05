import { useNavigate } from 'react-router-dom'
import { formatDate } from '../lib/utils'
import type { Project } from '../types'

export function ProjectCard({
  project,
  onDelete,
}: {
  project: Project
  onDelete: (project: Project) => void
}) {
  const navigate = useNavigate()

  function open() {
    navigate(`/projects/${project.id}`)
  }

  return (
    <div
      className="project-card"
      role="button"
      tabIndex={0}
      onClick={open}
      onKeyDown={(event) => {
        if (event.key === 'Enter' || event.key === ' ') {
          event.preventDefault()
          open()
        }
      }}
    >
      <button
        type="button"
        className="btn btn--icon btn--danger project-card__delete"
        aria-label={`Excluir projeto ${project.name}`}
        onClick={(event) => {
          event.stopPropagation()
          onDelete(project)
        }}
      >
        ✕
      </button>
      <div className="project-card__name">{project.name}</div>
      {project.description ? <p className="project-card__desc">{project.description}</p> : null}
      <div className="project-card__meta">Criado em {formatDate(project.createdAt)}</div>
    </div>
  )
}
