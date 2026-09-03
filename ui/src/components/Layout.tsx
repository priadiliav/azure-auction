import type { ReactNode } from 'react'
import { Box, Toolbar } from '@mui/material'
import { Header } from './Header'
import { Sidebar, type ItemsView } from './Sidebar'

type LayoutProps = {
  onSellClick: () => void
  onSettingsClick: () => void
  view: ItemsView
  onViewChange: (view: ItemsView) => void
  children: ReactNode
}

export function Layout({ onSellClick, onSettingsClick, view, onViewChange, children }: LayoutProps) {
  return (
    <Box sx={{ display: 'flex' }}>
      <Header onSellClick={onSellClick} onSettingsClick={onSettingsClick} />
      <Sidebar view={view} onViewChange={onViewChange} />
      <Box component="main" sx={{ flexGrow: 1, p: 3, minWidth: 0 }}>
        <Toolbar />
        {children}
      </Box>
    </Box>
  )
}
