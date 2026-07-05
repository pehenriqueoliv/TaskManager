import { formatDate } from '../lib/utils'
import type { Task, TaskStatus } from '../types'
import { PriorityChip } from './PriorityChip'

const order: TaskStatus[] = ['Todo', 'InProgress', 'Done']

export function TaskCard({
  task,
  busy,
  onMove,
  onDelete,
}: {
  task: Task
  busy: boolean
  onMove: (task: Task, status: TaskStatus) => void
  onDelete: (task: Task) => void
}) {
  const index = order.indexOf(task.status)
  const previous = order[index - 1]
  const next = order[index + 1]

  return (
    <article className="task">
      <div className="task__top">
        <span className="task__title">{task.title}</span>
      </div>
      {task.description ? <p className="task__desc">{task.description}</p> : null}
      <div className="task__foot">
        <PriorityChip priority={task.priority} />
        {task.dueDate ? <span className="task__due">{formatDate(task.dueDate)}</span> : null}
        <div className="task__actions">
          <button
            type="button"
            className="btn btn--icon"
            aria-label="Mover para a coluna anterior"
            disabled={!previous || busy}
            onClick={() => previous && onMove(task, previous)}
          >
            ←
          </button>
          <button
            type="button"
            className="btn btn--icon"
            aria-label="Mover para a próxima coluna"
            disabled={!next || busy}
            onClick={() => next && onMove(task, next)}
          >
            →
          </button>
          <button
            type="button"
            className="btn btn--icon btn--danger"
            aria-label="Excluir task"
            disabled={busy}
            onClick={() => onDelete(task)}
          >
            ✕
          </button>
        </div>
      </div>
    </article>
  )
}
