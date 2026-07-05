import { useState, type FormEvent } from 'react'
import { useCreateProject } from '../hooks/useProjects'
import { getErrorMessage } from '../lib/utils'

export function NewProjectForm() {
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [error, setError] = useState<string | null>(null)
  const createProject = useCreateProject()

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    createProject.mutate(
      { name, description: description || null },
      {
        onSuccess: () => {
          setName('')
          setDescription('')
        },
        onError: (err) => setError(getErrorMessage(err)),
      },
    )
  }

  return (
    <form className="panel" onSubmit={handleSubmit}>
      <p className="panel__title">Novo projeto</p>
      <div className="inline-form">
        <div className="field">
          <label className="field__label" htmlFor="project-name">
            Nome
          </label>
          <input
            id="project-name"
            className="input"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="Ex.: Portfólio"
            required
          />
        </div>
        <div className="field">
          <label className="field__label" htmlFor="project-description">
            Descrição
          </label>
          <input
            id="project-description"
            className="input"
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            placeholder="Opcional"
          />
        </div>
        <button type="submit" className="btn" disabled={createProject.isPending || !name.trim()}>
          {createProject.isPending ? 'Criando…' : 'Criar projeto'}
        </button>
      </div>
      {error ? <p className="form-error" style={{ marginTop: '0.85rem' }}>{error}</p> : null}
    </form>
  )
}
