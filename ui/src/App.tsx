import { useEffect, useState } from 'react'

type HealthResponse = {
  status: string
  version: string
  environment: string
}

type ItemResponse = {
  itemId: string
  title: string
  startingPrice: number
  status: string
}

const baseUrl = import.meta.env.VITE_API_BASE_URL

function App() {
  const [health, setHealth] = useState<HealthResponse | null>(null)
  const [healthError, setHealthError] = useState<string | null>(null)

  const [title, setTitle] = useState('')
  const [startingPrice, setStartingPrice] = useState('')
  const [creating, setCreating] = useState(false)
  const [createdItem, setCreatedItem] = useState<ItemResponse | null>(null)
  const [createError, setCreateError] = useState<string | null>(null)

  useEffect(() => {
    fetch(`${baseUrl}/api/health`)
      .then((res) => {
        if (!res.ok) throw new Error(`Request failed: ${res.status}`)
        return res.json() as Promise<HealthResponse>
      })
      .then(setHealth)
      .catch((err: Error) => setHealthError(err.message))
  }, [])

  const createItem = async () => {
    setCreating(true)
    setCreateError(null)
    setCreatedItem(null)
    try {
      const createRes = await fetch(`${baseUrl}/api/items`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, startingPrice: Number(startingPrice) }),
      })
      if (!createRes.ok) throw new Error(`Request failed: ${createRes.status}`)
      const { itemId } = (await createRes.json()) as { itemId: string }

      const itemRes = await fetch(`${baseUrl}/api/items/${itemId}`)
      if (!itemRes.ok) throw new Error(`Request failed: ${itemRes.status}`)
      setCreatedItem((await itemRes.json()) as ItemResponse)
    } catch (err) {
      setCreateError((err as Error).message)
    } finally {
      setCreating(false)
    }
  }

  return (
    <>
      <p>
        {healthError
          ? `Health check failed: ${healthError}`
          : health
            ? `API status: ${health.status} (v${health.version}, ${health.environment})`
            : 'Loading...'}
      </p>

      <h2>Create item</h2>
      <input
        placeholder="Title"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />
      <input
        placeholder="Starting price"
        type="number"
        value={startingPrice}
        onChange={(e) => setStartingPrice(e.target.value)}
      />
      <button disabled={creating || !title || !startingPrice} onClick={createItem}>
        {creating ? 'Creating...' : 'Create item'}
      </button>

      {createdItem && (
        <table>
          <tbody>
            <tr>
              <td>Item ID</td>
              <td>{createdItem.itemId}</td>
            </tr>
            <tr>
              <td>Title</td>
              <td>{createdItem.title}</td>
            </tr>
            <tr>
              <td>Starting price</td>
              <td>{createdItem.startingPrice}</td>
            </tr>
            <tr>
              <td>Status</td>
              <td>{createdItem.status}</td>
            </tr>
          </tbody>
        </table>
      )}
      {createError && <p>Failed to create item: {createError}</p>}
    </>
  )
}

export default App
