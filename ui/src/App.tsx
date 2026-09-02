import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Container,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Paper,
} from '@mui/material'
import { createItem, fetchHealth, fetchItems } from './api'

function App() {
  const queryClient = useQueryClient()

  const [title, setTitle] = useState('')
  const [startingPrice, setStartingPrice] = useState('')

  const healthQuery = useQuery({ queryKey: ['health'], queryFn: fetchHealth })
  const itemsQuery = useQuery({ queryKey: ['items'], queryFn: fetchItems })

  const createItemMutation = useMutation({
    mutationFn: createItem,
    onSuccess: () => {
      setTitle('')
      setStartingPrice('')
      queryClient.invalidateQueries({ queryKey: ['items'] })
    },
  })

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="body2" color="text.secondary" gutterBottom>
        {healthQuery.isPending && 'Checking API status...'}
        {healthQuery.isError && `Health check failed: ${healthQuery.error.message}`}
        {healthQuery.data &&
          `API status: ${healthQuery.data.status} (v${healthQuery.data.version}, ${healthQuery.data.environment})`}
      </Typography>

      <Typography variant="h5" gutterBottom>
        Create item
      </Typography>
      <Stack direction="row" spacing={2} sx={{ mb: 4 }}>
        <TextField
          label="Title"
          size="small"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <TextField
          label="Starting price"
          type="number"
          size="small"
          value={startingPrice}
          onChange={(e) => setStartingPrice(e.target.value)}
        />
        <Button
          variant="contained"
          disabled={!title || !startingPrice || createItemMutation.isPending}
          onClick={() => createItemMutation.mutate({ title, startingPrice: Number(startingPrice) })}
        >
          {createItemMutation.isPending ? 'Creating...' : 'Create item'}
        </Button>
      </Stack>

      {createItemMutation.isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Failed to create item: {createItemMutation.error.message}
        </Alert>
      )}

      <Typography variant="h5" gutterBottom>
        Items
      </Typography>

      {itemsQuery.isPending && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
          <CircularProgress />
        </Box>
      )}
      {itemsQuery.isError && <Alert severity="error">Failed to load items: {itemsQuery.error.message}</Alert>}

      {itemsQuery.data && (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Title</TableCell>
                <TableCell align="right">Starting price</TableCell>
                <TableCell align="right">Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {itemsQuery.data.map((item) => (
                <TableRow key={item.itemId}>
                  <TableCell>{item.title}</TableCell>
                  <TableCell align="right">{item.startingPrice}</TableCell>
                  <TableCell align="right">
                    <Chip label={item.status} size="small" />
                  </TableCell>
                </TableRow>
              ))}
              {itemsQuery.data.length === 0 && (
                <TableRow>
                  <TableCell colSpan={3} align="center">
                    No items yet
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Container>
  )
}

export default App
