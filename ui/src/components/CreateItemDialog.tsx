import { useEffect, useRef, useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Step,
  StepLabel,
  Stepper,
  TextField,
  Typography,
} from '@mui/material'
import AddAPhotoOutlinedIcon from '@mui/icons-material/AddAPhotoOutlined'
import { createItem, requestBlobUploadSas, uploadItemImage } from '../api'

type CreateItemDialogProps = {
  open: boolean
  onClose: () => void
}

const steps = ['Details', 'Photo', 'Upload']

export function CreateItemDialog({ open, onClose }: CreateItemDialogProps) {
  const queryClient = useQueryClient()
  const fileInputRef = useRef<HTMLInputElement>(null)

  const [activeStep, setActiveStep] = useState(0)
  const [title, setTitle] = useState('')
  const [startingPrice, setStartingPrice] = useState('')
  const [imageFile, setImageFile] = useState<File | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)

  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl)
    }
  }, [previewUrl])

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] ?? null
    if (previewUrl) URL.revokeObjectURL(previewUrl)
    setImageFile(file)
    setPreviewUrl(file ? URL.createObjectURL(file) : null)
  }

  const createItemMutation = useMutation({
    mutationFn: async () => {
      const { itemId } = await createItem({ title, startingPrice: Number(startingPrice) })
      const sas = await requestBlobUploadSas(itemId, imageFile!.name, imageFile!.type)
      await uploadItemImage(sas.uploadUrl, imageFile!)
      return itemId
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['items'] })
      reset()
      onClose()
    },
  })

  const reset = () => {
    setActiveStep(0)
    setTitle('')
    setStartingPrice('')
    setImageFile(null)
    setPreviewUrl(null)
    createItemMutation.reset()
  }

  const handleClose = () => {
    reset()
    onClose()
  }

  const handleCreate = () => {
    setActiveStep(2)
    createItemMutation.mutate()
  }

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="xs">
      <DialogTitle>Sell an item</DialogTitle>
      <DialogContent>
        <Stepper activeStep={activeStep} sx={{ pt: 1, pb: 3 }}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>

        {activeStep === 0 && (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
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
          </Box>
        )}

        {activeStep === 1 && (
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 1 }}>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              hidden
              onChange={handleFileChange}
            />
            <Box
              onClick={() => fileInputRef.current?.click()}
              sx={{
                border: '2px dashed',
                borderColor: 'grey.300',
                borderRadius: 1,
                width: 160,
                aspectRatio: '1 / 1',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                gap: 0.5,
                color: 'grey.500',
                cursor: 'pointer',
                overflow: 'hidden',
                backgroundImage: previewUrl ? `url(${previewUrl})` : undefined,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
              }}
            >
              {!previewUrl && (
                <>
                  <AddAPhotoOutlinedIcon />
                  <Typography variant="caption">Add photo</Typography>
                </>
              )}
            </Box>
            {imageFile && (
              <Typography variant="caption" color="text.secondary" noWrap>
                {imageFile.name}
              </Typography>
            )}
          </Box>
        )}

        {activeStep === 2 && (
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2, py: 2 }}>
            {createItemMutation.isPending && (
              <>
                <CircularProgress size={32} />
                <Typography variant="body2" color="text.secondary">
                  Creating item and uploading photo...
                </Typography>
              </>
            )}
            {createItemMutation.isError && (
              <Alert severity="error" sx={{ width: '100%' }}>
                Failed to create item: {createItemMutation.error.message}
              </Alert>
            )}
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>

        {activeStep === 0 && (
          <Button variant="contained" disabled={!title || !startingPrice} onClick={() => setActiveStep(1)}>
            Next
          </Button>
        )}

        {activeStep === 1 && (
          <>
            <Button onClick={() => setActiveStep(0)}>Back</Button>
            <Button variant="contained" disabled={!imageFile} onClick={handleCreate}>
              Create item
            </Button>
          </>
        )}

        {activeStep === 2 && createItemMutation.isError && (
          <>
            <Button onClick={() => setActiveStep(1)}>Back</Button>
            <Button variant="contained" onClick={handleCreate}>
              Try again
            </Button>
          </>
        )}
      </DialogActions>
    </Dialog>
  )
}
