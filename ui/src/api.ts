import { getIdToken } from './auth-context'

const baseUrl = import.meta.env.VITE_API_BASE_URL
const functionsBaseUrl = import.meta.env.VITE_FUNCTIONS_BASE_URL

function authHeaders(): HeadersInit {
  const token = getIdToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}

export type HealthResponse = {
  status: string
  version: string
  environment: string
}

export type RecentBid = {
  bidderId: string
  bidderName: string
  bidderAvatarUrl: string
  amount: number
  placedAt: string
}

export type Item = {
  itemId: string
  title: string
  description: string
  startingPrice: number
  currentPrice: number
  status: string
  blobUrl: string | null
  endsAt: string
  sellerId: string
  recentBids: RecentBid[]
}

export type CreateItemInput = {
  title: string
  description: string
  startingPrice: number
}

export type BlobUploadSas = {
  uploadUrl: string
  blobUrl: string
  expiresOn: string
}

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) throw new Error(`Request failed: ${res.status}`)
  return res.json() as Promise<T>
}

export const fetchHealth = () => fetch(`${baseUrl}/api/health`).then((res) => handle<HealthResponse>(res))

export const fetchItems = () =>
  fetch(`${baseUrl}/api/items`, { headers: authHeaders() }).then((res) => handle<Item[]>(res))

export const fetchMyItems = () =>
  fetch(`${baseUrl}/api/items/mine`, { headers: authHeaders() }).then((res) => handle<Item[]>(res))

export const createItem = (input: CreateItemInput) =>
  fetch(`${baseUrl}/api/items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify(input),
  }).then((res) => handle<{ itemId: string }>(res))

export const requestBlobUploadSas = (itemId: string, fileName: string, contentType: string) => {
  const query = new URLSearchParams({ fileName, contentType })
  return fetch(`${functionsBaseUrl}/api/items/${itemId}/blob-sas?${query}`, {
    method: 'POST',
    headers: authHeaders(),
  }).then((res) => handle<BlobUploadSas>(res))
}

export const uploadItemImage = async (uploadUrl: string, file: File) => {
  const res = await fetch(uploadUrl, {
    method: 'PUT',
    headers: {
      'x-ms-blob-type': 'BlockBlob',
      'Content-Type': file.type,
    },
    body: file,
  })
  if (!res.ok) throw new Error(`Image upload failed: ${res.status}`)
}

export type PlaceBidResult =
  | { status: 'accepted'; currentPrice: number }
  | { status: 'conflict'; reason: 'BidTooLow' | 'AuctionEnded'; currentPrice: number }
  | { status: 'not-found' }

export const placeBid = async (itemId: string, amount: number): Promise<PlaceBidResult> => {
  const res = await fetch(`${baseUrl}/api/items/${itemId}/bids`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify({ amount }),
  })

  if (res.status === 404) {
    return { status: 'not-found' }
  }
  if (res.status === 409) {
    const body = (await res.json()) as { reason: 'BidTooLow' | 'AuctionEnded'; currentPrice: number }
    return { status: 'conflict', reason: body.reason, currentPrice: body.currentPrice }
  }
  if (!res.ok) {
    throw new Error(`Request failed: ${res.status}`)
  }
  const body = (await res.json()) as { currentPrice: number }
  return { status: 'accepted', currentPrice: body.currentPrice }
}
