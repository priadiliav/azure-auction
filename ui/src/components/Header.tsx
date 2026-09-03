import { AppBar, Avatar, Box, Button, InputAdornment, TextField, Toolbar, Typography } from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import AddIcon from '@mui/icons-material/Add'
import { ApiStatusIndicator } from './ApiStatusIndicator'
import { useAuth } from '../auth-context'

type HeaderProps = {
  onSellClick: () => void
}

export function Header({ onSellClick }: HeaderProps) {
  const { user, buttonRef, signOut } = useAuth()

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
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Avatar src={user.picture} alt={user.name} sx={{ width: 32, height: 32 }} />
            <Typography variant="body2" noWrap sx={{ maxWidth: 120 }}>
              {user.name}
            </Typography>
            <Button size="small" onClick={signOut}>
              Sign out
            </Button>
          </Box>
        ) : (
          <Box ref={buttonRef} />
        )}

        <ApiStatusIndicator />
      </Toolbar>
    </AppBar>
  )
}
