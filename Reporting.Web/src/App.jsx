import { useCallback, useEffect, useMemo, useState } from 'react'
import './App.css'
import { isLoggedIn, logout } from './services/auth'
import LoginPage from './pages/LoginPage'
import StockReportPage from './pages/StockReportPage'
import QuotationReportPage from './pages/QuotationReportPage'
import EstimationReportPage from './pages/EstimationReportPage'
import SalesReportPage from './pages/SalesReportPage'
import ProductAnalysisReportPage from './pages/ProductAnalysisReportPage'
import { defaultBranchId, getBranchById } from './config/branches'
import ProductSyncStatus from './components/ProductSyncStatus'

const STOCK_REPORT_ROUTE = '/reports/stock'
const QUOTATION_REPORT_ROUTE = '/reports/quotation'
const ESTIMATION_REPORT_ROUTE = '/reports/estimation'
const SALES_REPORT_ROUTE = '/reports/sales'
const PRODUCT_ANALYSIS_ROUTE = '/reports/product-analysis'
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
      navigateTo(authenticated ? STOCK_REPORT_ROUTE : LOGIN_ROUTE)
      return
    }

    if (!authenticated && path !== LOGIN_ROUTE) {
      navigateTo(LOGIN_ROUTE)
      return
    }

    if (authenticated && path === LOGIN_ROUTE) {
      navigateTo(STOCK_REPORT_ROUTE)
    }
  }, [authenticated, path])

  const handleLogin = useCallback(() => {
    navigateTo(STOCK_REPORT_ROUTE)
  }, [])

  const handleLogout = useCallback(() => {
    logout()
    navigateTo(LOGIN_ROUTE)
  }, [])

  const navItems = useMemo(
    () => [
      { label: 'Stock Report', path: STOCK_REPORT_ROUTE, active: path === STOCK_REPORT_ROUTE },
      { label: 'Quotation', path: QUOTATION_REPORT_ROUTE, active: path === QUOTATION_REPORT_ROUTE },
      { label: 'Estimation', path: ESTIMATION_REPORT_ROUTE, active: path === ESTIMATION_REPORT_ROUTE },
      { label: 'Sales', path: SALES_REPORT_ROUTE, active: path === SALES_REPORT_ROUTE },
      { label: 'Product Analysis', path: PRODUCT_ANALYSIS_ROUTE, active: path === PRODUCT_ANALYSIS_ROUTE },
    ],
    [path],
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
          <ProductSyncStatus />
          <button type="button" className="logout-button" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </header>

      <main className="app-main">
        {path === STOCK_REPORT_ROUTE && (
          <StockReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {path === QUOTATION_REPORT_ROUTE && (
          <QuotationReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {path === ESTIMATION_REPORT_ROUTE && (
          <EstimationReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {path === SALES_REPORT_ROUTE && (
          <SalesReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
        {path === PRODUCT_ANALYSIS_ROUTE && (
          <ProductAnalysisReportPage
            selectedBranchId={selectedBranchId}
            onBranchChange={setSelectedBranchId}
          />
        )}
      </main>
    </div>
  )
}

export default App
