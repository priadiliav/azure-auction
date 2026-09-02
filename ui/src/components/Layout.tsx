import type { ReactNode } from 'react'
import { Box, Toolbar } from '@mui/material'
import { Header } from './Header'
import { Sidebar } from './Sidebar'

type LayoutProps = {
  onSellClick: () => void
  children: ReactNode
}

export function Layout({ onSellClick, children }: LayoutProps) {
  return (
    <Box sx={{ display: 'flex' }}>
      <Header onSellClick={onSellClick} />
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3, minWidth: 0 }}>
        <Toolbar />
        {children}
      </Box>
    </Box>
  )
}
