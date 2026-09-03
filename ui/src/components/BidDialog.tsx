import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  Typography,
} from '@mui/material'
import { placeBid } from '../api'
import { useAuth } from '../auth-context'

type BidDialogProps = {
  open: boolean
  onClose: () => void
  itemId: string
  itemTitle: string
  currentPrice: number
}

export function BidDialog({ open, onClose, itemId, itemTitle, currentPrice }: BidDialogProps) {
  const queryClient = useQueryClient()
  const { user } = useAuth()

  const [amount, setAmount] = useState('')
  const [minPrice, setMinPrice] = useState(currentPrice)

  const bidMutation = useMutation({
    mutationFn: () => placeBid(itemId, Number(amount)),
    onSuccess: (result) => {
      if (result.status === 'accepted') {
        queryClient.invalidateQueries({ queryKey: ['items'] })
        setAmount('')
        onClose()
      } else if (result.status === 'conflict') {
        setMinPrice(result.currentPrice)
      }
    },
  })

  const result = bidMutation.data

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Bid on &quot;{itemTitle}&quot;</DialogTitle>
      <DialogContent>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Current price: ${minPrice}
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
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          variant="contained"
          disabled={!amount || Number(amount) <= minPrice || bidMutation.isPending}
          onClick={() => bidMutation.mutate()}
        >
          {bidMutation.isPending ? 'Placing bid...' : 'Place bid'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
