const LOGIN_LOG_STORAGE_KEY = 'rre_reporting_login_log'
const MAX_LOGIN_LOG_ROWS = 100

function readRows() {
  try {
    const raw = localStorage.getItem(LOGIN_LOG_STORAGE_KEY)
    const rows = raw ? JSON.parse(raw) : []
    return Array.isArray(rows) ? rows : []
  } catch {
    return []
  }
}

function writeRows(rows) {
  localStorage.setItem(LOGIN_LOG_STORAGE_KEY, JSON.stringify(rows.slice(0, MAX_LOGIN_LOG_ROWS)))
}

export function recordLogin() {
  const loginAt = new Date().toISOString()
  const rows = readRows()

  writeRows([
    {
      id: `${loginAt}-${Math.random().toString(36).slice(2, 8)}`,
      loginAt,
    },
    ...rows,
  ])
}

export function getRecentLoginLogs() {
  return readRows()
    .filter((row) => row && typeof row.loginAt === 'string')
    .sort((a, b) => new Date(b.loginAt).getTime() - new Date(a.loginAt).getTime())
}
