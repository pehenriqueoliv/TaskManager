export function Loading({ label = 'Carregando…' }: { label?: string }) {
  return (
    <div className="state">
      <div className="spinner" />
      <p className="state__text">{label}</p>
    </div>
  )
}

export function ErrorState({ title = 'Não foi possível carregar', message }: { title?: string; message: string }) {
  return (
    <div className="state state--error">
      <p className="state__title">{title}</p>
      <p className="state__text">{message}</p>
    </div>
  )
}

export function EmptyState({ title, text }: { title: string; text: string }) {
  return (
    <div className="state">
      <p className="state__title">{title}</p>
      <p className="state__text">{text}</p>
    </div>
  )
}
