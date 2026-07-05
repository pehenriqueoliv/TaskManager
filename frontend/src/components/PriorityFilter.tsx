import type { TaskPriority } from '../types'

const options: { value: TaskPriority | 'all'; label: string }[] = [
  { value: 'all', label: 'Todas' },
  { value: 'Low', label: 'Baixa' },
  { value: 'Medium', label: 'Média' },
  { value: 'High', label: 'Alta' },
]

export function PriorityFilter({
  value,
  onChange,
}: {
  value: TaskPriority | 'all'
  onChange: (value: TaskPriority | 'all') => void
}) {
  return (
    <div className="filters">
      <span className="filters__label">Prioridade</span>
      {options.map((option) => (
        <button
          key={option.value}
          type="button"
          className={`filter-btn ${value === option.value ? 'filter-btn--active' : ''}`}
          onClick={() => onChange(option.value)}
        >
          {option.label}
        </button>
      ))}
    </div>
  )
}
