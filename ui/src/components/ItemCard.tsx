import { useState } from 'react'
import { Box, Button, Card, CardContent, Chip, Stack, Typography } from '@mui/material'
import HourglassEmptyOutlinedIcon from '@mui/icons-material/HourglassEmptyOutlined'
import type { Item } from '../api'
import { BidDialog } from './BidDialog'
import { useAuth } from '../auth-context'

type ItemCardProps = {
  item: Item
}

export function ItemCard({ item }: ItemCardProps) {
  const [bidOpen, setBidOpen] = useState(false)
  const { user } = useAuth()

  const isQueued = !item.blobUrl
  const isEnded = item.status === 'Ended'
  const isBiddable = item.status === 'Listed' && !isEnded
  const canBid = isBiddable && !!user

  return (
    <>
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
            <Typography variant="h6">${item.currentPrice}</Typography>
            <Chip
              label={isQueued ? 'Queued to publish' : item.status}
              size="small"
              color={isQueued ? 'default' : 'primary'}
              icon={isQueued ? <HourglassEmptyOutlinedIcon /> : undefined}
            />
          </Stack>

          {!isQueued && !isEnded && (
            <Typography variant="caption" color="text.secondary" component="div" sx={{ mt: 0.5 }}>
              Ends {new Date(item.endsAt).toLocaleString()}
            </Typography>
          )}

          {isBiddable && (
            <Button
              fullWidth
              variant="outlined"
              sx={{ mt: 1.5 }}
              disabled={!user}
              onClick={() => setBidOpen(true)}
            >
              {user ? 'Place bid' : 'Sign in to bid'}
            </Button>
          )}
        </CardContent>
      </Card>

      {canBid && (
        <BidDialog
          open={bidOpen}
          onClose={() => setBidOpen(false)}
          itemId={item.itemId}
          itemTitle={item.title}
          currentPrice={item.currentPrice}
        />
      )}
    </>
  )
}
