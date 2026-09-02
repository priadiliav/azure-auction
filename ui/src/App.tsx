import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Alert, Box, CircularProgress } from '@mui/material'
import { fetchItems } from './api'
import { Layout } from './components/Layout'
import { ItemGrid } from './components/ItemGrid'
import { CreateItemDialog } from './components/CreateItemDialog'
import { useItemUpdates } from './useItemUpdates'

function App() {
  const [isCreateOpen, setIsCreateOpen] = useState(false)

  const itemsQuery = useQuery({ queryKey: ['items'], queryFn: fetchItems })
  useItemUpdates()

  return (
    <Layout onSellClick={() => setIsCreateOpen(true)}>
      {itemsQuery.isPending && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
          <CircularProgress />
        </Box>
      )}
      {itemsQuery.isError && <Alert severity="error">Failed to load items: {itemsQuery.error.message}</Alert>}
      {itemsQuery.data && <ItemGrid items={itemsQuery.data} />}

      <CreateItemDialog open={isCreateOpen} onClose={() => setIsCreateOpen(false)} />
    </Layout>
  )
}

export default App
