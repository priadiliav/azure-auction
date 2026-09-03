import { createContext, useContext, type RefObject } from 'react'

const ID_TOKEN_STORAGE_KEY = 'auction:googleIdToken'

export type AuthUser = {
  name: string
  email: string
  picture: string
}

export function decodeIdToken(token: string): AuthUser | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1])) as AuthUser & { exp: number }
    if (Date.now() >= payload.exp * 1000) return null
    return { name: payload.name, email: payload.email, picture: payload.picture }
  } catch {
    return null
  }
}

export function getIdToken(): string | null {
  const token = localStorage.getItem(ID_TOKEN_STORAGE_KEY)
  return token && decodeIdToken(token) ? token : null
}

export const ID_TOKEN_KEY = ID_TOKEN_STORAGE_KEY

export type AuthContextValue = {
  user: AuthUser | null
  buttonRef: RefObject<HTMLDivElement | null>
  signOut: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used within AuthProvider')
  return context
}

declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize: (config: {
            client_id: string
            callback: (response: { credential: string }) => void
          }) => void
          renderButton: (element: HTMLElement, options: { theme?: string; size?: string }) => void
          disableAutoSelect: () => void
        }
      }
    }
  }
}
