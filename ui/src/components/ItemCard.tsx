import { Box, Card, CardContent, Chip, Stack, Typography } from '@mui/material'
import ImageOutlinedIcon from '@mui/icons-material/ImageOutlined'
import type { Item } from '../api'

type ItemCardProps = {
  item: Item
}

export function ItemCard({ item }: ItemCardProps) {
  return (
    <Card variant="outlined">
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
        {!item.blobUrl && <ImageOutlinedIcon sx={{ fontSize: 48, color: 'grey.400' }} />}
      </Box>
      <CardContent>
        <Typography variant="subtitle1" noWrap title={item.title}>
          {item.title}
        </Typography>
        <Stack direction="row" sx={{ mt: 1, justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">${item.startingPrice}</Typography>
          <Chip label={item.status} size="small" />
        </Stack>
      </CardContent>
    </Card>
  )
}
