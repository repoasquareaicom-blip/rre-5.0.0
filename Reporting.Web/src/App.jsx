import { useCallback, useEffect, useMemo, useState } from 'react'
import './App.css'
import {
  canAccessRoute,
  getAuthSession,
  getLandingRoute,
  hasFullAccess,
  isLoggedIn,
  logout,
} from './services/auth'
import LoginPage from './pages/LoginPage'
import StockReportPage from './pages/StockReportPage'
import QuotationReportPage from './pages/QuotationReportPage'
import EstimationReportPage from './pages/EstimationReportPage'
import SalesReportPage from './pages/SalesReportPage'
import ProductAnalysisReportPage from './pages/ProductAnalysisReportPage'
import LoginLogPage from './pages/LoginLogPage'
import { defaultBranchId, getBranchById } from './config/branches'
import ProductSyncStatus from './components/ProductSyncStatus'

const STOCK_REPORT_ROUTE = '/reports/stock'
const QUOTATION_REPORT_ROUTE = '/reports/quotation'
const ESTIMATION_REPORT_ROUTE = '/reports/estimation'
const SALES_REPORT_ROUTE = '/reports/sales'
const PRODUCT_ANALYSIS_ROUTE = '/reports/product-analysis'
const LOGIN_LOG_ROUTE = '/reports/login-log'
const LOGIN_ROUTE = '/login'
const transactionReportRoutes = new Set([
  QUOTATION_REPORT_ROUTE,
  ESTIMATION_REPORT_ROUTE,
  SALES_REPORT_ROUTE,
  PRODUCT_ANALYSIS_ROUTE,
])

function getCurrentPath() {
  return window.location.pathname || LOGIN_ROUTE
}

function navigateTo(path) {
  window.history.pushState({}, '', path)
  window.dispatchEvent(new PopStateEvent('popstate'))
}

function App() {
  const [path, setPath] = useState(getCurrentPath)
  const [authSession, setAuthSession] = useState(getAuthSession)
  const [selectedBranchId, setSelectedBranchId] = useState(defaultBranchId)
  const authenticated = isLoggedIn() && Boolean(authSession)
  const selectedBranch = getBranchById(selectedBranchId)

  useEffect(() => {
    const handlePopState = () => setPath(getCurrentPath())
    window.addEventListener('popstate', handlePopState)
    return () => window.removeEventListener('popstate', handlePopState)
  }, [])

  useEffect(() => {
    const landingRoute = getLandingRoute(authSession?.accessLevel)

    if (path === '/') {
      navigateTo(authenticated ? landingRoute : LOGIN_ROUTE)
      return
    }

    if (!authenticated && path !== LOGIN_ROUTE) {
      navigateTo(LOGIN_ROUTE)
      return
    }

    if (authenticated && path === LOGIN_ROUTE) {
      navigateTo(landingRoute)
      return
    }

    if (authenticated && !canAccessRoute(authSession, path)) {
      navigateTo(landingRoute)
    }
  }, [authSession, authenticated, path])

  const handleLogin = useCallback((session) => {
    setAuthSession(session)
    navigateTo(getLandingRoute(session?.accessLevel))
  }, [])

  const handleLogout = useCallback(() => {
    logout()
    setAuthSession(null)
    navigateTo(LOGIN_ROUTE)
  }, [])

  const navItems = useMemo(
    () => [
      { label: 'Stock Report', path: STOCK_REPORT_ROUTE, active: path === STOCK_REPORT_ROUTE },
      { label: 'Quotation', path: QUOTATION_REPORT_ROUTE, active: path === QUOTATION_REPORT_ROUTE },
      { label: 'Estimation', path: ESTIMATION_REPORT_ROUTE, active: path === ESTIMATION_REPORT_ROUTE },
      { label: 'Sales', path: SALES_REPORT_ROUTE, active: path === SALES_REPORT_ROUTE },
      { label: 'Product Analysis', path: PRODUCT_ANALYSIS_ROUTE, active: path === PRODUCT_ANALYSIS_ROUTE },
      { label: 'Login Log', path: LOGIN_LOG_ROUTE, active: path === LOGIN_LOG_ROUTE },
    ].filter((item) => canAccessRoute(authSession, item.path)),
    [authSession, path],
  )

  if (!authenticated) {
    return <LoginPage onLogin={handleLogin} />
  }

  return (
    <div
      className={transactionReportRoutes.has(path) ? 'app-shell is-quotation-route' : 'app-shell'}
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
        <div className="header-actions">
          {hasFullAccess(authSession) && <ProductSyncStatus />}
          <button type="button" className="logout-button" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </header>

      <main className="app-main">
        {canAccessRoute(authSession, STOCK_REPORT_ROUTE) && path === STOCK_REPORT_ROUTE && (
          <StockReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {canAccessRoute(authSession, QUOTATION_REPORT_ROUTE) && path === QUOTATION_REPORT_ROUTE && (
          <QuotationReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {canAccessRoute(authSession, ESTIMATION_REPORT_ROUTE) && path === ESTIMATION_REPORT_ROUTE && (
          <EstimationReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {canAccessRoute(authSession, SALES_REPORT_ROUTE) && path === SALES_REPORT_ROUTE && (
          <SalesReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {canAccessRoute(authSession, PRODUCT_ANALYSIS_ROUTE) && path === PRODUCT_ANALYSIS_ROUTE && (
          <ProductAnalysisReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {canAccessRoute(authSession, LOGIN_LOG_ROUTE) && path === LOGIN_LOG_ROUTE && <LoginLogPage />}
      </main>
    </div>
  )
}

export default App
