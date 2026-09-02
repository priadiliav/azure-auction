import { useEffect, useState } from 'react'

type HealthResponse = {
  status: string
  version: string
  environment: string
}

function App() {
  const [health, setHealth] = useState<HealthResponse | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL
    fetch(`${baseUrl}/api/health`)
      .then((res) => {
        if (!res.ok) throw new Error(`Request failed: ${res.status}`)
        return res.json() as Promise<HealthResponse>
      })
      .then(setHealth)
      .catch((err: Error) => setError(err.message))
  }, [])

  if (error) return <>Health check failed: {error}</>
  if (!health) return <>Loading...</>

  return (
    <>
      API status: {health.status} (v{health.version}, {health.environment})
    </>
  )
}

export default App
