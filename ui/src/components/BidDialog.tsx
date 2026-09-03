import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  Alert,
  Avatar,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  TextField,
  Typography,
} from '@mui/material'
import { placeBid, type Item } from '../api'
import { useAuth } from '../auth-context'

type BidDialogProps = {
  open: boolean
  onClose: () => void
  item: Item
}

export function BidDialog({ open, onClose, item }: BidDialogProps) {
  const queryClient = useQueryClient()
  const { user } = useAuth()

  const [amount, setAmount] = useState('')

  const bidMutation = useMutation({
    mutationFn: () => placeBid(item.itemId, Number(amount)),
    onSuccess: (result) => {
      if (result.status === 'accepted') {
        queryClient.invalidateQueries({ queryKey: ['items'] })
        setAmount('')
        onClose()
      }
    },
  })

  const result = bidMutation.data

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Bid on &quot;{item.title}&quot;</DialogTitle>
      <DialogContent>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Current price: ${item.currentPrice}
        </Typography>

        {user && (
          <Typography variant="body2" sx={{ mb: 2 }}>
            Bidding as <strong>{user.name}</strong>
          </Typography>
        )}

        <TextField
          label="Your bid"
          type="number"
          fullWidth
          autoFocus
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
        />

        {result?.status === 'conflict' && (
          <Alert severity="warning" sx={{ mt: 2 }}>
            {result.reason === 'AuctionEnded'
              ? 'This auction has ended.'
              : `Someone bid first - current price is now $${result.currentPrice}.`}
          </Alert>
        )}
        {result?.status === 'not-found' && (
          <Alert severity="error" sx={{ mt: 2 }}>
            Item not found.
          </Alert>
        )}
        {bidMutation.isError && (
          <Alert severity="error" sx={{ mt: 2 }}>
            Failed to place bid: {bidMutation.error.message}
          </Alert>
        )}

        {item.recentBids.length > 0 && (
          <>
            <Divider sx={{ mt: 3, mb: 1 }} />
            <Typography variant="subtitle2" color="text.secondary">
              Recent bids
            </Typography>
            <List dense disablePadding>
              {item.recentBids.map((bid) => (
                <ListItem key={`${bid.bidderId}-${bid.placedAt}`} disableGutters>
                  <ListItemAvatar sx={{ minWidth: 40 }}>
                    <Avatar src={bid.bidderAvatarUrl} alt={bid.bidderName} sx={{ width: 28, height: 28 }} />
                  </ListItemAvatar>
                  <ListItemText primary={bid.bidderName} secondary={`$${bid.amount}`} />
                </ListItem>
              ))}
            </List>
          </>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          variant="contained"
          disabled={!amount || Number(amount) <= item.currentPrice || bidMutation.isPending}
          onClick={() => bidMutation.mutate()}
        >
          {bidMutation.isPending ? 'Placing bid...' : 'Place bid'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
