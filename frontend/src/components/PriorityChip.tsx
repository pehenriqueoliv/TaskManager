import type { TaskPriority } from '../types'

const labels: Record<TaskPriority, string> = { Low: 'Baixa', Medium: 'Média', High: 'Alta' }
const classes: Record<TaskPriority, string> = {
  Low: 'chip--low',
  Medium: 'chip--medium',
  High: 'chip--high',
}

export function PriorityChip({ priority }: { priority: TaskPriority }) {
  return <span className={`chip ${classes[priority]}`}>{labels[priority]}</span>
}
