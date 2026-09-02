import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  Typography,
} from '@mui/material'
import AddAPhotoOutlinedIcon from '@mui/icons-material/AddAPhotoOutlined'
import { createItem } from '../api'

type CreateItemDialogProps = {
  open: boolean
  onClose: () => void
}

export function CreateItemDialog({ open, onClose }: CreateItemDialogProps) {
  const queryClient = useQueryClient()

  const [title, setTitle] = useState('')
  const [startingPrice, setStartingPrice] = useState('')

  const createItemMutation = useMutation({
    mutationFn: createItem,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['items'] })
      setTitle('')
      setStartingPrice('')
      onClose()
    },
  })

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Sell an item</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
          <Box
            sx={{
              border: '2px dashed',
              borderColor: 'grey.300',
              borderRadius: 1,
              width: 120,
              aspectRatio: '1 / 1',
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              gap: 0.5,
              color: 'grey.500',
            }}
          >
            <AddAPhotoOutlinedIcon />
            <Typography variant="caption">Add photo</Typography>
          </Box>

          <TextField
            label="Title"
            autoFocus
            fullWidth
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          <TextField
            label="Starting price"
            type="number"
            fullWidth
            value={startingPrice}
            onChange={(e) => setStartingPrice(e.target.value)}
          />

          {createItemMutation.isError && (
            <Alert severity="error">Failed to create item: {createItemMutation.error.message}</Alert>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          variant="contained"
          disabled={!title || !startingPrice || createItemMutation.isPending}
          onClick={() => createItemMutation.mutate({ title, startingPrice: Number(startingPrice) })}
        >
          {createItemMutation.isPending ? 'Creating...' : 'Create item'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
