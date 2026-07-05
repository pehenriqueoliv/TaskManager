import type { Task, TaskStatus } from '../types'
import { TaskCard } from './TaskCard'

const columns: { status: TaskStatus; label: string; modifier: string }[] = [
  { status: 'Todo', label: 'A fazer', modifier: 'col--todo' },
  { status: 'InProgress', label: 'Em progresso', modifier: 'col--progress' },
  { status: 'Done', label: 'Concluído', modifier: 'col--done' },
]

export function TaskBoard({
  tasks,
  pendingId,
  onMove,
  onDelete,
}: {
  tasks: Task[]
  pendingId: string | null
  onMove: (task: Task, status: TaskStatus) => void
  onDelete: (task: Task) => void
}) {
  return (
    <div className="board">
      {columns.map((column) => {
        const columnTasks = tasks.filter((task) => task.status === column.status)
        return (
          <section key={column.status} className={`col ${column.modifier}`}>
            <div className="col__head">
              <span className="col__dot" />
              <span className="col__title">{column.label}</span>
              <span className="col__count">{columnTasks.length}</span>
            </div>
            <div className="col__body">
              {columnTasks.length === 0 ? <p className="col__empty">Nada por aqui.</p> : null}
              {columnTasks.map((task) => (
                <TaskCard
                  key={task.id}
                  task={task}
                  busy={pendingId === task.id}
                  onMove={onMove}
                  onDelete={onDelete}
                />
              ))}
            </div>
          </section>
        )
      })}
    </div>
  )
}
