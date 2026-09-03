import { Box, Card, CardContent, Chip, Stack, Typography } from '@mui/material'
import HourglassEmptyOutlinedIcon from '@mui/icons-material/HourglassEmptyOutlined'
import type { Item } from '../api'

type ItemCardProps = {
  item: Item
}

export function ItemCard({ item }: ItemCardProps) {
  const isQueued = !item.blobUrl

  return (
    <Card variant="outlined" sx={{ opacity: isQueued ? 0.6 : 1 }}>
      <Box
        sx={{
          aspectRatio: '1 / 1',
          bgcolor: 'grey.100',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          backgroundImage: item.blobUrl ? `url(${item.blobUrl})` : undefined,
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        {isQueued && <HourglassEmptyOutlinedIcon sx={{ fontSize: 48, color: 'grey.400' }} />}
      </Box>
      <CardContent>
        <Typography variant="subtitle1" noWrap title={item.title}>
          {item.title}
        </Typography>
        <Stack direction="row" sx={{ mt: 1, justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">${item.startingPrice}</Typography>
          <Chip
            label={isQueued ? 'Queued to publish' : item.status}
            size="small"
            color={isQueued ? 'default' : 'primary'}
            icon={isQueued ? <HourglassEmptyOutlinedIcon /> : undefined}
          />
        </Stack>
      </CardContent>
    </Card>
  )
}
