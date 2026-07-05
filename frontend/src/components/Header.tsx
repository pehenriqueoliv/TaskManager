import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { BrandMark } from './BrandMark'

export function Header() {
  const { email, signOut } = useAuth()

  return (
    <header className="header">
      <Link to="/" className="brand">
        <BrandMark />
        <span className="brand__name">Task Manager</span>
      </Link>
      <div className="header__user">
        <span className="header__email mono">{email}</span>
        <button type="button" className="btn btn--ghost btn--sm" onClick={signOut}>
          Sair
        </button>
      </div>
    </header>
  )
}
