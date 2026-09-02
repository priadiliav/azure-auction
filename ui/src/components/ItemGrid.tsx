import { Box, Typography } from '@mui/material'
import type { Item } from '../api'
import { ItemCard } from './ItemCard'

type ItemGridProps = {
  items: Item[]
}

export function ItemGrid({ items }: ItemGridProps) {
  if (items.length === 0) {
    return (
      <Typography color="text.secondary" sx={{ py: 4, textAlign: 'center' }}>
        No items yet
      </Typography>
    )
  }

  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))',
        gap: 2,
      }}
    >
      {items.map((item) => (
        <ItemCard key={item.itemId} item={item} />
      ))}
    </Box>
  )
}
