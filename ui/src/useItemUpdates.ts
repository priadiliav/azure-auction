import { useEffect } from 'react'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useQueryClient } from '@tanstack/react-query'

const functionsBaseUrl = import.meta.env.VITE_FUNCTIONS_BASE_URL

export function useItemUpdates() {
  const queryClient = useQueryClient()

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${functionsBaseUrl}/api`)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('itemStatusChanged', () => {
      queryClient.invalidateQueries({ queryKey: ['items'] })
    })

    connection.start().catch((err) => console.error('SignalR connection failed:', err))

    return () => {
      connection.stop()
    }
  }, [queryClient])
}
