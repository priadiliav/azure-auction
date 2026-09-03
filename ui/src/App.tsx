import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Alert, Box, CircularProgress } from '@mui/material'
import { fetchItems, fetchMyItems } from './api'
import { Layout } from './components/Layout'
import { ItemGrid } from './components/ItemGrid'
import { CreateItemDialog } from './components/CreateItemDialog'
import { SettingsDialog } from './components/SettingsDialog'
import { useItemUpdates } from './useItemUpdates'
import type { ItemsView } from './components/Sidebar'

function App() {
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [isSettingsOpen, setIsSettingsOpen] = useState(false)
  const [view, setView] = useState<ItemsView>('all')

  const itemsQuery = useQuery({
    queryKey: ['items', view],
    queryFn: view === 'mine' ? fetchMyItems : fetchItems,
  })
  useItemUpdates()

  return (
    <Layout
      onSellClick={() => setIsCreateOpen(true)}
      onSettingsClick={() => setIsSettingsOpen(true)}
      view={view}
      onViewChange={setView}
    >
      {itemsQuery.isPending && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
          <CircularProgress />
        </Box>
      )}
      {itemsQuery.isError && <Alert severity="error">Failed to load items: {itemsQuery.error.message}</Alert>}
      {itemsQuery.data && <ItemGrid items={itemsQuery.data} />}

      <CreateItemDialog open={isCreateOpen} onClose={() => setIsCreateOpen(false)} />
      <SettingsDialog open={isSettingsOpen} onClose={() => setIsSettingsOpen(false)} />
    </Layout>
  )
}

export default App
