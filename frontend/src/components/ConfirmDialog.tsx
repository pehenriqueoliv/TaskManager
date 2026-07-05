import { useEffect, useRef } from 'react'
import { createPortal } from 'react-dom'

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = 'Excluir',
  cancelLabel = 'Cancelar',
  pending = false,
  onConfirm,
  onCancel,
}: {
  open: boolean
  title: string
  message: string
  confirmLabel?: string
  cancelLabel?: string
  pending?: boolean
  onConfirm: () => void
  onCancel: () => void
}) {
  const cancelRef = useRef<HTMLButtonElement>(null)

  function requestClose() {
    if (!pending) onCancel()
  }

  useEffect(() => {
    if (!open) return

    cancelRef.current?.focus()

    function handleKey(event: KeyboardEvent) {
      if (event.key === 'Escape' && !pending) onCancel()
    }

    document.body.style.overflow = 'hidden'
    window.addEventListener('keydown', handleKey)

    return () => {
      document.body.style.overflow = ''
      window.removeEventListener('keydown', handleKey)
    }
  }, [open, pending, onCancel])

  if (!open) return null

  return createPortal(
    <div className="overlay" onClick={requestClose}>
      <div
        className="dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirm-dialog-title"
        onClick={(event) => event.stopPropagation()}
      >
        <h2 id="confirm-dialog-title" className="dialog__title">
          {title}
        </h2>
        <p className="dialog__message">{message}</p>
        <div className="dialog__actions">
          <button
            ref={cancelRef}
            type="button"
            className="btn btn--ghost"
            onClick={requestClose}
            disabled={pending}
          >
            {cancelLabel}
          </button>
          <button
            type="button"
            className="btn btn--danger-solid"
            onClick={onConfirm}
            disabled={pending}
          >
            {pending ? 'Excluindo…' : confirmLabel}
          </button>
        </div>
      </div>
    </div>,
    document.body,
  )
}
