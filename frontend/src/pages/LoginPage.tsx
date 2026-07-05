import { useState, type FormEvent } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { BrandMark } from '../components/BrandMark'
import { getErrorMessage } from '../lib/utils'

type Mode = 'login' | 'register'

export function LoginPage() {
  const { isAuthenticated, login, register } = useAuth()
  const navigate = useNavigate()
  const [mode, setMode] = useState<Mode>('login')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [pending, setPending] = useState(false)

  if (isAuthenticated) return <Navigate to="/" replace />

  const isLogin = mode === 'login'

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setPending(true)
    try {
      await (isLogin ? login : register)({ email, password })
      navigate('/', { replace: true })
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setPending(false)
    }
  }

  return (
    <div className="auth">
      <div className="auth__card">
        <div className="auth__brand">
          <BrandMark />
          Task Manager
        </div>
        <h1 className="auth__title">{isLogin ? 'Bem-vindo de volta' : 'Criar conta'}</h1>
        <p className="auth__subtitle">
          {isLogin
            ? 'Entre para organizar seus projetos e tasks.'
            : 'Crie uma conta para organizar seus projetos e tasks.'}
        </p>
        <form className="auth__form" onSubmit={handleSubmit}>
          <div className="field">
            <label className="field__label" htmlFor="email">
              Email
            </label>
            <input
              id="email"
              type="email"
              className="input"
              autoComplete="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
            />
          </div>
          <div className="field">
            <label className="field__label" htmlFor="password">
              Senha
            </label>
            <input
              id="password"
              type="password"
              className="input"
              autoComplete={isLogin ? 'current-password' : 'new-password'}
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
          </div>
          {error ? <p className="form-error">{error}</p> : null}
          <button type="submit" className="btn btn--block" disabled={pending}>
            {pending ? 'Aguarde…' : isLogin ? 'Entrar' : 'Criar conta'}
          </button>
        </form>
        <p className="auth__switch">
          {isLogin ? 'Ainda não tem conta? ' : 'Já tem conta? '}
          <button
            type="button"
            className="auth__link"
            onClick={() => {
              setMode(isLogin ? 'register' : 'login')
              setError(null)
            }}
          >
            {isLogin ? 'Criar conta' : 'Entrar'}
          </button>
        </p>
      </div>
    </div>
  )
}
