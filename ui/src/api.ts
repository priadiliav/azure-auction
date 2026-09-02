const baseUrl = import.meta.env.VITE_API_BASE_URL

export type HealthResponse = {
  status: string
  version: string
  environment: string
}

export type Item = {
  itemId: string
  title: string
  startingPrice: number
  status: string
}

export type CreateItemInput = {
  title: string
  startingPrice: number
}

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) throw new Error(`Request failed: ${res.status}`)
  return res.json() as Promise<T>
}

export const fetchHealth = () => fetch(`${baseUrl}/api/health`).then((res) => handle<HealthResponse>(res))

export const fetchItems = () => fetch(`${baseUrl}/api/items`).then((res) => handle<Item[]>(res))

export const createItem = (input: CreateItemInput) =>
  fetch(`${baseUrl}/api/items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input),
  }).then((res) => handle<{ itemId: string }>(res))
