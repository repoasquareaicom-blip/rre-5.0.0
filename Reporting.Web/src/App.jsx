import { useCallback, useEffect, useMemo, useState } from 'react'
import './App.css'
import { isLoggedIn, logout } from './services/auth'
import LoginPage from './pages/LoginPage'
import StockReportPage from './pages/StockReportPage'
import { defaultBranchId, getBranchById } from './config/branches'

const REPORT_ROUTE = '/reports/stock'
const LOGIN_ROUTE = '/login'

function getCurrentPath() {
  return window.location.pathname || LOGIN_ROUTE
}

function navigateTo(path) {
  window.history.pushState({}, '', path)
  window.dispatchEvent(new PopStateEvent('popstate'))
}

function App() {
  const [path, setPath] = useState(getCurrentPath)
  const [selectedBranchId, setSelectedBranchId] = useState(defaultBranchId)
  const authenticated = isLoggedIn()
  const selectedBranch = getBranchById(selectedBranchId)

  useEffect(() => {
    const handlePopState = () => setPath(getCurrentPath())
    window.addEventListener('popstate', handlePopState)
    return () => window.removeEventListener('popstate', handlePopState)
  }, [])

  useEffect(() => {
    if (path === '/') {
      navigateTo(authenticated ? REPORT_ROUTE : LOGIN_ROUTE)
      return
    }

    if (!authenticated && path !== LOGIN_ROUTE) {
      navigateTo(LOGIN_ROUTE)
      return
    }

    if (authenticated && path === LOGIN_ROUTE) {
      navigateTo(REPORT_ROUTE)
    }
  }, [authenticated, path])

  const handleLogin = useCallback(() => {
    navigateTo(REPORT_ROUTE)
  }, [])

  const handleLogout = useCallback(() => {
    logout()
    navigateTo(LOGIN_ROUTE)
  }, [])

  const navItems = useMemo(
    () => [{ label: 'Stock Report', path: REPORT_ROUTE, active: path === REPORT_ROUTE }],
    [path],
  )

  if (!authenticated) {
    return <LoginPage onLogin={handleLogin} />
  }

  return (
    <div
      className="app-shell"
      style={{
        '--branch-accent': selectedBranch.accent,
        '--branch-accent-soft': selectedBranch.accentSoft,
        '--branch-accent-border': selectedBranch.accentBorder,
      }}
    >
      <header className="app-header">
        <div className="brand-block">
          <div className="brand-mark">RR</div>
          <div>
            <p className="brand-kicker">RRE Connect</p>
            <h1>Reporting</h1>
          </div>
        </div>
        <nav className="top-nav" aria-label="Reports">
          {navItems.map((item) => (
            <button
              key={item.path}
              type="button"
              className={item.active ? 'nav-item is-active' : 'nav-item'}
              onClick={() => navigateTo(item.path)}
            >
              {item.label}
            </button>
          ))}
        </nav>
        <button type="button" className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </header>

      <main className="app-main">
        {path === REPORT_ROUTE && (
          <StockReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
      </main>
    </div>
  )
}

export default App
