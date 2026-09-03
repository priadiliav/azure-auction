import { useCallback, useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { AuthContext, decodeIdToken, ID_TOKEN_KEY, type AuthUser } from './auth-context'

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID

export function AuthProvider({ children }: { children: ReactNode }) {
  const buttonRef = useRef<HTMLDivElement>(null)
  const [user, setUser] = useState<AuthUser | null>(() => {
    const token = localStorage.getItem(ID_TOKEN_KEY)
    return token ? decodeIdToken(token) : null
  })

  const handleCredentialResponse = useCallback((response: { credential: string }) => {
    localStorage.setItem(ID_TOKEN_KEY, response.credential)
    setUser(decodeIdToken(response.credential))
  }, [])

  useEffect(() => {
    let cancelled = false

    const setup = () => {
      if (cancelled || !window.google) return
      window.google.accounts.id.initialize({
        client_id: GOOGLE_CLIENT_ID,
        callback: handleCredentialResponse,
      })
      if (buttonRef.current) {
        window.google.accounts.id.renderButton(buttonRef.current, { theme: 'outline', size: 'medium' })
      }
    }

    if (window.google) {
      setup()
      return
    }

    // The GIS script tag loads async - poll briefly until it's ready.
    const interval = setInterval(() => {
      if (window.google) {
        clearInterval(interval)
        setup()
      }
    }, 100)

    return () => {
      cancelled = true
      clearInterval(interval)
    }
  }, [handleCredentialResponse])

  const signOut = useCallback(() => {
    localStorage.removeItem(ID_TOKEN_KEY)
    window.google?.accounts.id.disableAutoSelect()
    setUser(null)
  }, [])

  const value = useMemo(() => ({ user, buttonRef, signOut }), [user, signOut])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
