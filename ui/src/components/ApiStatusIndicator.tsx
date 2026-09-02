import { useQuery } from '@tanstack/react-query'
import { CircularProgress, Tooltip } from '@mui/material'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import ErrorOutlinedIcon from '@mui/icons-material/ErrorOutlined'
import { fetchHealth } from '../api'

export function ApiStatusIndicator() {
  const healthQuery = useQuery({ queryKey: ['health'], queryFn: fetchHealth })

  if (healthQuery.isPending) return <CircularProgress size={20} />

  if (healthQuery.isError) {
    return (
      <Tooltip title={`API status: unavailable (${healthQuery.error.message})`}>
        <ErrorOutlinedIcon color="error" />
      </Tooltip>
    )
  }

  return (
    <Tooltip title={`API status: ${healthQuery.data.status} (v${healthQuery.data.version})`}>
      <CheckCircleIcon color="success" />
    </Tooltip>
  )
}
