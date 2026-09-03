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

type BidDialogProps = {
  open: boolean
  onClose: () => void
  itemId: string
  itemTitle: string
  currentPrice: number
}

const BIDDER_NAME_STORAGE_KEY = 'auction:bidderName'

export function BidDialog({ open, onClose, itemId, itemTitle, currentPrice }: BidDialogProps) {
  const queryClient = useQueryClient()

  const [amount, setAmount] = useState('')
  const [bidderName, setBidderName] = useState(() => localStorage.getItem(BIDDER_NAME_STORAGE_KEY) ?? '')
  const [minPrice, setMinPrice] = useState(currentPrice)

  const bidMutation = useMutation({
    mutationFn: () => {
      localStorage.setItem(BIDDER_NAME_STORAGE_KEY, bidderName)
      return placeBid(itemId, Number(amount), bidderName)
    },
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

        <TextField
          label="Your name"
          fullWidth
          autoFocus
          value={bidderName}
          onChange={(e) => setBidderName(e.target.value)}
          sx={{ mb: 2 }}
        />
        <TextField
          label="Your bid"
          type="number"
          fullWidth
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
          disabled={!bidderName || !amount || Number(amount) <= minPrice || bidMutation.isPending}
          onClick={() => bidMutation.mutate()}
        >
          {bidMutation.isPending ? 'Placing bid...' : 'Place bid'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
