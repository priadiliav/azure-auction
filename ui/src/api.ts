const baseUrl = import.meta.env.VITE_API_BASE_URL
const functionsBaseUrl = import.meta.env.VITE_FUNCTIONS_BASE_URL

export type HealthResponse = {
  status: string
  version: string
  environment: string
}

export type Item = {
  itemId: string
  title: string
  startingPrice: number
  currentPrice: number
  status: string
  blobUrl: string | null
  endsAt: string
}

export type CreateItemInput = {
  title: string
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

export const fetchItems = () => fetch(`${baseUrl}/api/items`).then((res) => handle<Item[]>(res))

export const createItem = (input: CreateItemInput) =>
  fetch(`${baseUrl}/api/items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input),
  }).then((res) => handle<{ itemId: string }>(res))

export const requestBlobUploadSas = (itemId: string, fileName: string, contentType: string) => {
  const query = new URLSearchParams({ fileName, contentType })
  return fetch(`${functionsBaseUrl}/api/items/${itemId}/blob-sas?${query}`, { method: 'POST' }).then((res) =>
    handle<BlobUploadSas>(res),
  )
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

export const placeBid = async (itemId: string, amount: number, bidderName: string): Promise<PlaceBidResult> => {
  const res = await fetch(`${baseUrl}/api/items/${itemId}/bids`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ amount, bidderName }),
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
