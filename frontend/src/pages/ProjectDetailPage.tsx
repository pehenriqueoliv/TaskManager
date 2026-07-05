import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ConfirmDialog } from '../components/ConfirmDialog'
import { NewTaskForm } from '../components/NewTaskForm'
import { PriorityFilter } from '../components/PriorityFilter'
import { TaskBoard } from '../components/TaskBoard'
import { ErrorState, Loading } from '../components/states'
import { useProject } from '../hooks/useProjects'
import { useDeleteTask, useTasks, useUpdateTask } from '../hooks/useTasks'
import { getErrorMessage } from '../lib/utils'
import type { Task, TaskPriority, TaskStatus } from '../types'

export function ProjectDetailPage() {
  const { projectId = '' } = useParams()
  const [priority, setPriority] = useState<TaskPriority | 'all'>('all')
  const [taskTarget, setTaskTarget] = useState<Task | null>(null)

  const project = useProject(projectId)
  const tasks = useTasks(projectId, priority === 'all' ? {} : { priority })
  const updateTask = useUpdateTask(projectId)
  const deleteTask = useDeleteTask(projectId)

  function handleMove(task: Task, status: TaskStatus) {
    updateTask.mutate({
      id: task.id,
      input: {
        title: task.title,
        description: task.description,
        status,
        priority: task.priority,
        dueDate: task.dueDate,
      },
    })
  }

  function confirmDeleteTask() {
    if (!taskTarget) return
    deleteTask.mutate(taskTarget.id, { onSettled: () => setTaskTarget(null) })
  }

  const pendingId = updateTask.isPending
    ? updateTask.variables?.id ?? null
    : deleteTask.isPending
      ? deleteTask.variables ?? null
      : null

  return (
    <>
      <Link to="/" className="back-link">
        ← Projetos
      </Link>

      <div className="page__head">
        <div>
          <span className="eyebrow">Projeto</span>
          <h1 className="page__title">{project.data?.name ?? '…'}</h1>
        </div>
        <PriorityFilter value={priority} onChange={setPriority} />
      </div>

      {project.isError ? (
        <ErrorState message={getErrorMessage(project.error)} />
      ) : (
        <>
          <NewTaskForm projectId={projectId} />
          {tasks.isLoading ? <Loading label="Carregando tasks…" /> : null}
          {tasks.isError ? <ErrorState message={getErrorMessage(tasks.error)} /> : null}
          {tasks.data ? (
            <TaskBoard
              tasks={tasks.data}
              pendingId={pendingId}
              onMove={handleMove}
              onDelete={setTaskTarget}
            />
          ) : null}
        </>
      )}

      <ConfirmDialog
        open={taskTarget !== null}
        title="Excluir task"
        message={`"${taskTarget?.title}" será excluída. Essa ação não pode ser desfeita.`}
        confirmLabel="Excluir task"
        pending={deleteTask.isPending}
        onConfirm={confirmDeleteTask}
        onCancel={() => setTaskTarget(null)}
      />
    </>
  )
}
