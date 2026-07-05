import { useState, type FormEvent } from 'react'
import { useCreateTask } from '../hooks/useTasks'
import { dateInputToIso, getErrorMessage } from '../lib/utils'
import type { TaskPriority, TaskStatus } from '../types'

export function NewTaskForm({ projectId }: { projectId: string }) {
  const [title, setTitle] = useState('')
  const [status, setStatus] = useState<TaskStatus>('Todo')
  const [priority, setPriority] = useState<TaskPriority>('Medium')
  const [dueDate, setDueDate] = useState('')
  const [error, setError] = useState<string | null>(null)
  const createTask = useCreateTask(projectId)

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    createTask.mutate(
      { title, status, priority, dueDate: dateInputToIso(dueDate) },
      {
        onSuccess: () => {
          setTitle('')
          setStatus('Todo')
          setPriority('Medium')
          setDueDate('')
        },
        onError: (err) => setError(getErrorMessage(err)),
      },
    )
  }

  return (
    <form className="panel" onSubmit={handleSubmit}>
      <p className="panel__title">Nova task</p>
      <div className="task-form">
        <div className="field">
          <label className="field__label" htmlFor="task-title">
            Título
          </label>
          <input
            id="task-title"
            className="input"
            value={title}
            onChange={(event) => setTitle(event.target.value)}
            placeholder="O que precisa ser feito?"
            required
          />
        </div>
        <div className="field">
          <label className="field__label" htmlFor="task-status">
            Coluna
          </label>
          <select
            id="task-status"
            className="select"
            value={status}
            onChange={(event) => setStatus(event.target.value as TaskStatus)}
          >
            <option value="Todo">A fazer</option>
            <option value="InProgress">Em progresso</option>
            <option value="Done">Concluído</option>
          </select>
        </div>
        <div className="field">
          <label className="field__label" htmlFor="task-priority">
            Prioridade
          </label>
          <select
            id="task-priority"
            className="select"
            value={priority}
            onChange={(event) => setPriority(event.target.value as TaskPriority)}
          >
            <option value="Low">Baixa</option>
            <option value="Medium">Média</option>
            <option value="High">Alta</option>
          </select>
        </div>
        <div className="field">
          <label className="field__label" htmlFor="task-due">
            Prazo
          </label>
          <input
            id="task-due"
            type="date"
            className="input"
            value={dueDate}
            onChange={(event) => setDueDate(event.target.value)}
          />
        </div>
        <button type="submit" className="btn" disabled={createTask.isPending || !title.trim()}>
          {createTask.isPending ? 'Adicionando…' : 'Adicionar'}
        </button>
      </div>
      {error ? <p className="form-error" style={{ marginTop: '0.85rem' }}>{error}</p> : null}
    </form>
  )
}
