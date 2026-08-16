import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import { branches } from '../config/branches'
import { fetchProductSyncPending } from '../services/productSyncPending'
import { formatGst, formatMoney } from '../services/stockReport'

const REFRESH_INTERVAL_MS = 60000
const DRAWER_TRANSITION_MS = 220
const branchOrder = ['RR-SALEM', 'RR-NAMAKKAL', 'RR-KOLATHUR']

function groupRowsByBranch(rows) {
  return Object.fromEntries(
    branchOrder.map((branchId) => [
      branchId,
      rows.filter((row) => row.targetBranchCode === branchId),
    ]),
  )
}

function formatDateTime(value) {
  if (!value) {
    return '-'
  }

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return String(value)
  }

  return new Intl.DateTimeFormat('en-IN', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(date)
}

function getStatusClassName(status) {
  const normalizedStatus = status.trim().toLowerCase()

  if (normalizedStatus === 'pending') {
    return 'sync-status-badge is-pending'
  }

  if (normalizedStatus === 'failed') {
    return 'sync-status-badge is-failed'
  }

  return 'sync-status-badge is-warning'
}

function ProductSyncStatus() {
  const [rows, setRows] = useState([])
  const [status, setStatus] = useState('loading')
  const [isDrawerMounted, setIsDrawerMounted] = useState(false)
  const [isDrawerClosing, setIsDrawerClosing] = useState(false)
  const requestIdRef = useRef(0)
  const activeControllerRef = useRef(null)
  const closeTimeoutRef = useRef(null)

  const openDrawer = useCallback(() => {
    window.clearTimeout(closeTimeoutRef.current)
    setIsDrawerClosing(false)
    setIsDrawerMounted(true)
  }, [])

  const closeDrawer = useCallback(() => {
    setIsDrawerClosing(true)
    window.clearTimeout(closeTimeoutRef.current)
    closeTimeoutRef.current = window.setTimeout(() => {
      setIsDrawerMounted(false)
      setIsDrawerClosing(false)
    }, DRAWER_TRANSITION_MS)
  }, [])

  const refreshSyncStatus = useCallback(() => {
    activeControllerRef.current?.abort()

    const controller = new AbortController()
    activeControllerRef.current = controller
    const requestId = requestIdRef.current + 1
    requestIdRef.current = requestId

    setStatus((current) => (current === 'unavailable' ? 'loading' : current))

    fetchProductSyncPending(controller.signal)
      .then((nextRows) => {
        if (requestId !== requestIdRef.current) {
          return
        }

        activeControllerRef.current = null
        setRows(nextRows)
        setStatus('ready')
      })
      .catch((error) => {
        if (error.name === 'AbortError' || requestId !== requestIdRef.current) {
          return
        }

        activeControllerRef.current = null
        setRows([])
        setStatus('unavailable')
      })

    return () => {
      controller.abort()
      if (activeControllerRef.current === controller) {
        activeControllerRef.current = null
      }
    }
  }, [])

  useEffect(() => {
    const abortInitialRequest = refreshSyncStatus()
    const intervalId = window.setInterval(refreshSyncStatus, REFRESH_INTERVAL_MS)

    return () => {
      abortInitialRequest?.()
      activeControllerRef.current?.abort()
      window.clearTimeout(closeTimeoutRef.current)
      window.clearInterval(intervalId)
      requestIdRef.current += 1
    }
  }, [refreshSyncStatus])

  useEffect(() => {
    if (!isDrawerMounted) {
      return undefined
    }

    const handleKeyDown = (event) => {
      if (event.key === 'Escape') {
        closeDrawer()
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [closeDrawer, isDrawerMounted])

  const groupedRows = useMemo(() => groupRowsByBranch(rows), [rows])
  const totalPending = rows.length
  const hasPending = totalPending > 0 && status === 'ready'
  const isUnavailable = status === 'unavailable'
  const title = isUnavailable
    ? 'Sync status unavailable'
    : hasPending
      ? 'Product sync pending'
      : 'Product sync up to date'

  return (
    <>
      <button
        type="button"
        className={[
          'sync-header-button',
          hasPending ? 'has-pending' : '',
          isUnavailable ? 'is-unavailable' : '',
        ].filter(Boolean).join(' ')}
        title={title}
        aria-label={title}
        onClick={openDrawer}
      >
        <span className="sync-icon" aria-hidden="true">
          <svg viewBox="0 0 24 24" focusable="false">
            <path d="M20 6v5h-5" />
            <path d="M4 18v-5h5" />
            <path d="M18.7 9A7 7 0 0 0 6.8 6.7L4 9.3" />
            <path d="M5.3 15A7 7 0 0 0 17.2 17.3L20 14.7" />
          </svg>
        </span>
        {hasPending && <span className="sync-count-badge">{totalPending}</span>}
        {!hasPending && !isUnavailable && <span className="sync-ok-dot" aria-hidden="true" />}
      </button>

      {isDrawerMounted &&
        createPortal(
          <ProductSyncDrawer
            groupedRows={groupedRows}
            totalPending={totalPending}
            status={status}
            isClosing={isDrawerClosing}
            onClose={closeDrawer}
          />,
          document.body,
        )}
    </>
  )
}

function ProductSyncDrawer({ groupedRows, totalPending, status, isClosing, onClose }) {
  const branchLookup = useMemo(
    () => Object.fromEntries(branches.map((branch) => [branch.id, branch])),
    [],
  )
  const isUnavailable = status === 'unavailable'

  return (
    <div
      className={isClosing ? 'sync-drawer-layer is-closing' : 'sync-drawer-layer'}
      role="presentation"
      onMouseDown={onClose}
    >
      <aside
        className="sync-drawer"
        role="dialog"
        aria-modal="true"
        aria-labelledby="sync-drawer-title"
        onMouseDown={(event) => event.stopPropagation()}
      >
        <div className="sync-drawer-header">
          <div>
            <p className="section-kicker">Product queue</p>
            <h2 id="sync-drawer-title">Product Sync Pending</h2>
          </div>
          <button type="button" className="sync-close-button" aria-label="Close sync status" onClick={onClose}>
            <span aria-hidden="true">x</span>
          </button>
        </div>

        <div className="sync-drawer-body">
          {isUnavailable && (
            <div className="sync-unavailable-message">
              Sync status unavailable. Stock reports can still be used.
            </div>
          )}

          {!isUnavailable && totalPending === 0 && (
            <div className="sync-all-clear-message">All product syncs are up to date.</div>
          )}

          <div className="sync-summary-grid" aria-label="Product sync pending counts">
            {branchOrder.map((branchId) => {
              const branch = branchLookup[branchId]
              const count = groupedRows[branchId]?.length || 0
              return (
                <div
                  className="sync-summary-item"
                  style={{
                    '--sync-branch-accent': branch.accent,
                    '--sync-branch-soft': branch.accentSoft,
                    '--sync-branch-border': branch.accentBorder,
                  }}
                  key={branchId}
                >
                  <span>{branch.label}</span>
                  <strong>{isUnavailable ? '-' : count}</strong>
                </div>
              )
            })}
            <div className="sync-summary-total">
              <span>Total Pending</span>
              <strong>{isUnavailable ? '-' : totalPending}</strong>
            </div>
          </div>

          {branchOrder.map((branchId) => {
            const branch = branchLookup[branchId]
            return (
              <SyncBranchSection
                branch={branch}
                rows={isUnavailable ? [] : groupedRows[branchId] || []}
                key={branchId}
              />
            )
          })}
        </div>
      </aside>
    </div>
  )
}

function SyncBranchSection({ branch, rows }) {
  return (
    <section
      className="sync-branch-section"
      style={{
        '--sync-branch-accent': branch.accent,
        '--sync-branch-soft': branch.accentSoft,
        '--sync-branch-border': branch.accentBorder,
      }}
    >
      <div className="sync-section-heading">
        <h3>{branch.label.toUpperCase()}</h3>
        <span>{rows.length}</span>
      </div>

      {rows.length === 0 ? (
        <p className="sync-empty-text">No pending as of now</p>
      ) : (
        <>
          <SyncPendingTable rows={rows} />
          <SyncPendingMobileList rows={rows} />
        </>
      )}
    </section>
  )
}

function SyncPendingTable({ rows }) {
  return (
    <div className="sync-table-wrap">
      <table className="sync-table">
        <thead>
          <tr>
            <th>Product ID</th>
            <th>Product Name</th>
            <th className="numeric">Sale Price</th>
            <th className="numeric">MRP</th>
            <th>GST</th>
            <th>Status</th>
            <th>Details</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <SyncPendingTableRow row={row} key={row.id} />
          ))}
        </tbody>
      </table>
    </div>
  )
}

function SyncPendingTableRow({ row }) {
  const [isExpanded, setIsExpanded] = useState(false)
  const detailsId = `sync-details-${row.id}`

  return (
    <>
      <tr>
        <td>{row.productId || '-'}</td>
        <td>
          <div className="sync-product-cell">
            <strong>{row.itemName}</strong>
            <span>{row.changeType || 'Change'} - Attempts {row.attemptCount}</span>
          </div>
        </td>
        <td className="numeric">{formatMoney(row.salesPrice)}</td>
        <td className="numeric">{formatMoney(row.mrp)}</td>
        <td>{formatGst(row.gst)}</td>
        <td>
          <span className={getStatusClassName(row.status)}>{row.status}</span>
        </td>
        <td>
          <button
            type="button"
            className="sync-details-button"
            aria-expanded={isExpanded}
            aria-controls={detailsId}
            onClick={() => setIsExpanded((current) => !current)}
          >
            {isExpanded ? 'Hide' : 'Info'}
          </button>
        </td>
      </tr>
      {isExpanded && (
        <tr className="sync-detail-row" id={detailsId}>
          <td colSpan="7">
            <SyncRowDetails row={row} />
          </td>
        </tr>
      )}
    </>
  )
}

function SyncPendingMobileList({ rows }) {
  return (
    <div className="sync-mobile-list">
      {rows.map((row) => (
        <SyncPendingMobileCard row={row} key={row.id} />
      ))}
    </div>
  )
}

function SyncPendingMobileCard({ row }) {
  const [isExpanded, setIsExpanded] = useState(false)

  return (
    <article className="sync-mobile-card">
      <div>
        <h4>{row.itemName}</h4>
        <p>ID: {row.productId || '-'}</p>
      </div>
      <div className="sync-mobile-money">
        <span>Price {formatMoney(row.salesPrice)}</span>
        <span>MRP {formatMoney(row.mrp)}</span>
        <span>GST {formatGst(row.gst)}</span>
      </div>
      <div className="sync-mobile-status-line">
        <span className={getStatusClassName(row.status)}>{row.status}</span>
        <span>{row.changeType || 'Change'} - Attempts {row.attemptCount}</span>
      </div>
      <button type="button" className="sync-details-button" onClick={() => setIsExpanded((current) => !current)}>
        {isExpanded ? 'Hide details' : 'Details'}
      </button>
      {isExpanded && <SyncRowDetails row={row} />}
    </article>
  )
}

function SyncRowDetails({ row }) {
  return (
    <dl className="sync-row-details">
      <div>
        <dt>Last Error</dt>
        <dd>{row.lastError || '-'}</dd>
      </div>
      <div>
        <dt>Last Tried</dt>
        <dd>{formatDateTime(row.lastTriedOn)}</dd>
      </div>
      <div>
        <dt>Created On</dt>
        <dd>{formatDateTime(row.createdOn)}</dd>
      </div>
    </dl>
  )
}

export default ProductSyncStatus
