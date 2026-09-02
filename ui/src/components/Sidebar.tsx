import { Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar } from '@mui/material'
import CategoryIcon from '@mui/icons-material/Category'
import CheckroomIcon from '@mui/icons-material/Checkroom'
import DevicesIcon from '@mui/icons-material/Devices'
import ChairIcon from '@mui/icons-material/Chair'
import SportsBasketballIcon from '@mui/icons-material/SportsBasketball'

export const SIDEBAR_WIDTH = 240

const categories = [
  { label: 'All items', icon: <CategoryIcon /> },
  { label: 'Fashion', icon: <CheckroomIcon /> },
  { label: 'Electronics', icon: <DevicesIcon /> },
  { label: 'Home', icon: <ChairIcon /> },
  { label: 'Sports', icon: <SportsBasketballIcon /> },
]

export function Sidebar() {
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
        {categories.map((category, index) => (
          <ListItemButton key={category.label} selected={index === 0} disabled={index !== 0}>
            <ListItemIcon>{category.icon}</ListItemIcon>
            <ListItemText primary={category.label} />
          </ListItemButton>
        ))}
      </List>
    </Drawer>
  )
}
