import { AppBar, Box, Button, IconButton, InputAdornment, TextField, Toolbar, Typography } from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import AddIcon from '@mui/icons-material/Add'
import AccountCircleIcon from '@mui/icons-material/AccountCircle'
import { ApiStatusIndicator } from './ApiStatusIndicator'

type HeaderProps = {
  onSellClick: () => void
}

export function Header({ onSellClick }: HeaderProps) {
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

        <Button variant="contained" startIcon={<AddIcon />} onClick={onSellClick}>
          Sell
        </Button>
        <ApiStatusIndicator />
        <IconButton disabled>
          <AccountCircleIcon />
        </IconButton>
      </Toolbar>
    </AppBar>
  )
}
