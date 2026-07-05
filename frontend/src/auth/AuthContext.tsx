import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { login as loginRequest, register as registerRequest } from '../api/auth'
import { session } from '../api/client'
import { readEmailFromToken, tokenStore } from '../lib/tokenStore'
import type { Credentials } from '../types'

interface AuthContextValue {
  email: string | null
  isAuthenticated: boolean
  login: (credentials: Credentials) => Promise<void>
  register: (credentials: Credentials) => Promise<void>
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [authed, setAuthed] = useState(() => tokenStore.access !== null)
  const [email, setEmail] = useState(() => readEmailFromToken(tokenStore.access))

  useEffect(() => {
    session.onSignedOut = () => {
      tokenStore.clear()
      setAuthed(false)
      setEmail(null)
    }
    return () => {
      session.onSignedOut = null
    }
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      email,
      isAuthenticated: authed,
      async login(credentials) {
        const tokens = await loginRequest(credentials)
        tokenStore.save(tokens)
        setAuthed(true)
        setEmail(readEmailFromToken(tokens.accessToken))
      },
      async register(credentials) {
        const tokens = await registerRequest(credentials)
        tokenStore.save(tokens)
        setAuthed(true)
        setEmail(readEmailFromToken(tokens.accessToken))
      },
      signOut() {
        tokenStore.clear()
        setAuthed(false)
        setEmail(null)
      },
    }),
    [authed, email],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth precisa estar dentro de um AuthProvider')
  return context
}
