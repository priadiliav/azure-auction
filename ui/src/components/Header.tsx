import { useState } from 'react'
import {
  AppBar,
  Avatar,
  Box,
  Button,
  IconButton,
  InputAdornment,
  Menu,
  MenuItem,
  TextField,
  Toolbar,
  Typography,
} from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import AddIcon from '@mui/icons-material/Add'
import { ApiStatusIndicator } from './ApiStatusIndicator'
import { useAuth } from '../auth-context'

type HeaderProps = {
  onSellClick: () => void
  onSettingsClick: () => void
}

export function Header({ onSellClick, onSettingsClick }: HeaderProps) {
  const { user, buttonRef, signOut } = useAuth()
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null)

  return (
    <AppBar
      position="fixed"
      color="inherit"
      elevation={0}
      sx={{ borderBottom: 1, borderColor: 'divider', zIndex: (theme) => theme.zIndex.drawer + 1 }}
    >
      <Toolbar sx={{ gap: 2 }}>
        <Typography variant="h6" color="primary" noWrap sx={{ fontWeight: 700 }}>
          Auction
        </Typography>

        <Box sx={{ flexGrow: 1, maxWidth: 480 }}>
          <TextField
            fullWidth
            size="small"
            placeholder="Search for items"
            disabled
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
          />
        </Box>

        <Box sx={{ flexGrow: 1 }} />

        {user && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={onSellClick}>
            Sell
          </Button>
        )}

        {user ? (
          <>
            <IconButton onClick={(e) => setMenuAnchor(e.currentTarget)} size="small">
              <Avatar src={user.picture} alt={user.name} sx={{ width: 32, height: 32 }} />
            </IconButton>
            <Menu anchorEl={menuAnchor} open={!!menuAnchor} onClose={() => setMenuAnchor(null)}>
              <MenuItem disabled>{user.name}</MenuItem>
              <MenuItem
                onClick={() => {
                  setMenuAnchor(null)
                  onSettingsClick()
                }}
              >
                Settings
              </MenuItem>
              <MenuItem
                onClick={() => {
                  setMenuAnchor(null)
                  signOut()
                }}
              >
                Sign out
              </MenuItem>
            </Menu>
          </>
        ) : (
          <Box ref={buttonRef} />
        )}

        <ApiStatusIndicator />
      </Toolbar>
    </AppBar>
  )
}
