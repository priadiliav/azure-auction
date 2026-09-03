import { Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar } from '@mui/material'
import CategoryIcon from '@mui/icons-material/Category'
import PersonIcon from '@mui/icons-material/Person'
import CheckroomIcon from '@mui/icons-material/Checkroom'
import DevicesIcon from '@mui/icons-material/Devices'
import ChairIcon from '@mui/icons-material/Chair'
import SportsBasketballIcon from '@mui/icons-material/SportsBasketball'
import { useAuth } from '../auth-context'

export const SIDEBAR_WIDTH = 240

export type ItemsView = 'all' | 'mine'

type SidebarProps = {
  view: ItemsView
  onViewChange: (view: ItemsView) => void
}

const placeholderCategories = [
  { label: 'Fashion', icon: <CheckroomIcon /> },
  { label: 'Electronics', icon: <DevicesIcon /> },
  { label: 'Home', icon: <ChairIcon /> },
  { label: 'Sports', icon: <SportsBasketballIcon /> },
]

export function Sidebar({ view, onViewChange }: SidebarProps) {
  const { user } = useAuth()

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: SIDEBAR_WIDTH,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: SIDEBAR_WIDTH, boxSizing: 'border-box' },
      }}
    >
      <Toolbar />
      <List>
        <ListItemButton selected={view === 'all'} onClick={() => onViewChange('all')}>
          <ListItemIcon>
            <CategoryIcon />
          </ListItemIcon>
          <ListItemText primary="All items" />
        </ListItemButton>
        <ListItemButton
          selected={view === 'mine'}
          disabled={!user}
          onClick={() => onViewChange('mine')}
        >
          <ListItemIcon>
            <PersonIcon />
          </ListItemIcon>
          <ListItemText primary="My items" />
        </ListItemButton>

        {placeholderCategories.map((category) => (
          <ListItemButton key={category.label} disabled>
            <ListItemIcon>{category.icon}</ListItemIcon>
            <ListItemText primary={category.label} />
          </ListItemButton>
        ))}
      </List>
    </Drawer>
  )
}
