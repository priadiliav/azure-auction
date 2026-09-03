import { Avatar, Box, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material'
import { useAuth } from '../auth-context'

type SettingsDialogProps = {
  open: boolean
  onClose: () => void
}

export function SettingsDialog({ open, onClose }: SettingsDialogProps) {
  const { user } = useAuth()

  if (!user) return null

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Settings</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, py: 1 }}>
          <Avatar src={user.picture} alt={user.name} sx={{ width: 56, height: 56 }} />
          <Box>
            <Typography variant="subtitle1">{user.name}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user.email}
            </Typography>
          </Box>
        </Box>
      </DialogContent>
    </Dialog>
  )
}
