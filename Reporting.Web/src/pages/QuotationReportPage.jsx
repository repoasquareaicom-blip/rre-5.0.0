import { useEffect, useMemo, useRef, useState } from 'react'
import LoadingIndicator from '../components/LoadingIndicator'
import PageSizeSelector from '../components/PageSizeSelector'
import ReportPagination from '../components/ReportPagination'
import ReportSearch from '../components/ReportSearch'
import { branches, defaultBranchId, getBranchById } from '../config/branches'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import { downloadXlsx } from '../services/excelExport'
import {
  fetchQuotationDetail,
  fetchQuotationReport,
  formatGst,
  formatMoney,
  formatStock,
} from '../services/quotationReport'
import { getFileDateStamp } from '../services/reportUtils'

const ALL_BRANCHES_ID = 'ALL'
const ALL_BRANCH_FETCH_SIZE = 1000
const ALL_BRANCH_PAGE_SIZE = 100
const DEFAULT_PAGE_SIZE = 500
const quotationPageSizes = [10, 25, 50, 100, 500]
const allBranchOption = {
  id: ALL_BRANCHES_ID,
  label: 'All Branches',
  accent: '#b91c1c',
  accentSoft: '#fee2e2',
  accentBorder: '#fca5a5',
}

function getTodayInputValue() {
  const today = new Date()
  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function createDefaultFilters() {
  const today = getTodayInputValue()

  return {
    searchText: '',
    fromDate: today,
    toDate: today,
    branchId: defaultBranchId,
  }
}

function QuotationReportPage({ selectedBranchId, onBranchChange }) {
  const [draftFilters, setDraftFilters] = useState(() => ({
    ...createDefaultFilters(),
    branchId: selectedBranchId || defaultBranchId,
  }))
  const [appliedFilters, setAppliedFilters] = useState(() => ({
    ...createDefaultFilters(),
    branchId: selectedBranchId || defaultBranchId,
  }))
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE)
  const [pageNumber, setPageNumber] = useState(1)
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [summary, setSummary] = useState({
    totalQuotations: 0,
    quotationValue: 0,
    priceAltered: 0,
    cancelled: 0,
  })
  const [status, setStatus] = useState('loading')
  const [error, setError] = useState('')
  const [availabilityNotes, setAvailabilityNotes] = useState([])
  const [selectedQuotation, setSelectedQuotation] = useState(null)
  const [detailState, setDetailState] = useState({ status: 'idle', error: '', data: null })
  const [isExporting, setIsExporting] = useState(false)
  const requestIdRef = useRef(0)
  const debouncedSearchText = useDebouncedValue(appliedFilters.searchText, 350)
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))
  const isAllBranches = appliedFilters.branchId === ALL_BRANCHES_ID
  const activeBranch = useMemo(
    () => (isAllBranches ? allBranchOption : getBranchById(appliedFilters.branchId)),
    [appliedFilters.branchId, isAllBranches],
  )

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.fromDate,
    appliedFilters.toDate,
    appliedFilters.branchId,
    debouncedSearchText,
    pageSize,
  ])

  useEffect(() => {
    const controller = new AbortController()
    const requestId = requestIdRef.current + 1
    requestIdRef.current = requestId

    setStatus('loading')
    setError('')
    setAvailabilityNotes([])

    const reportRequest = {
      pageNumber,
      pageSize,
      searchText: debouncedSearchText.trim(),
      fromDate: appliedFilters.fromDate,
      toDate: appliedFilters.toDate,
    }

    const request = isAllBranches
      ? fetchAllBranchQuotationReport(reportRequest, controller.signal)
      : fetchSingleBranchQuotationReport(activeBranch, reportRequest, controller.signal)

    request
      .then((result) => {
        if (requestId !== requestIdRef.current) {
          return
        }

        setRows(result.rows)
        setTotalRows(result.totalRows)
        setSummary(result.summary)
        setAvailabilityNotes(result.availabilityNotes || [])
        setStatus('ready')
      })
      .catch((apiError) => {
        if (apiError.name === 'AbortError' || requestId !== requestIdRef.current) {
          return
        }

        setRows([])
        setTotalRows(0)
        setSummary({
          totalQuotations: 0,
          quotationValue: 0,
          priceAltered: 0,
          cancelled: 0,
        })
        setStatus('error')
        setError(apiError.message || `${activeBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [
    activeBranch,
    appliedFilters.branchId,
    appliedFilters.fromDate,
    appliedFilters.toDate,
    debouncedSearchText,
    isAllBranches,
    pageNumber,
    pageSize,
  ])

  useEffect(() => {
    if (!selectedQuotation) {
      setDetailState({ status: 'idle', error: '', data: null })
      return undefined
    }

    const controller = new AbortController()
    setDetailState({ status: 'loading', error: '', data: null })

    fetchQuotationDetail(selectedQuotation.branch || activeBranch, selectedQuotation.quotationId, controller.signal)
      .then((data) => setDetailState({ status: 'ready', error: '', data }))
      .catch((apiError) => {
        if (apiError.name === 'AbortError') {
          return
        }

        setDetailState({
          status: 'error',
          error: apiError.message || 'Unable to load quotation detail.',
          data: null,
        })
      })

    return () => controller.abort()
  }, [activeBranch, selectedQuotation])

  function updateDraftFilter(name, value) {
    setDraftFilters((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function applyFilters() {
    setAppliedFilters(draftFilters)
    setPageNumber(1)
    if (draftFilters.branchId !== ALL_BRANCHES_ID) {
      onBranchChange?.(draftFilters.branchId)
    }
  }

  function clearFilters() {
    const defaultFilters = createDefaultFilters()
    setDraftFilters(defaultFilters)
    setAppliedFilters(defaultFilters)
    setPageSize(DEFAULT_PAGE_SIZE)
    setPageNumber(1)
    onBranchChange?.(defaultFilters.branchId)
  }

  async function exportReport() {
    const controller = new AbortController()
    setIsExporting(true)
    setError('')

    try {
      const reportRequest = {
        pageNumber: 1,
        pageSize: DEFAULT_PAGE_SIZE,
        searchText: debouncedSearchText.trim(),
        fromDate: appliedFilters.fromDate,
        toDate: appliedFilters.toDate,
      }

      const result = isAllBranches
        ? await fetchAllBranchQuotationReport(reportRequest, controller.signal, true)
        : await fetchAllRowsForSingleBranch(activeBranch, reportRequest, controller.signal)

      downloadXlsx({
        filename: `Quotation_Report_${getFileDateStamp()}.xlsx`,
        sheetName: 'Quotation Report',
        columns: [
          { header: 'S.No', value: (row) => row.serialNumber },
          { header: 'Branch', value: (row) => row.branchName },
          { header: 'Quotation ID', value: (row) => row.quotationId },
          { header: 'Customer', value: (row) => row.customerName },
          { header: 'Items', value: (row) => row.itemCount },
          { header: 'Quantity', value: (row) => row.totalQuantity },
          { header: 'Value', value: (row) => row.quotationValue },
          { header: 'Price Altered', value: (row) => (row.hasPriceAltered ? 'Yes' : 'No') },
        ],
        rows: result.rows.map((row, index) => ({ ...row, serialNumber: index + 1 })),
      })

      setAvailabilityNotes(result.availabilityNotes || [])
    } catch (apiError) {
      if (apiError.name !== 'AbortError') {
        setError(apiError.message || 'Unable to export quotation report.')
      }
    } finally {
      setIsExporting(false)
    }
  }

  const branchStyle = {
    '--branch-accent': activeBranch.accent,
    '--branch-accent-soft': activeBranch.accentSoft,
    '--branch-accent-border': activeBranch.accentBorder,
  }

  return (
    <section className="report-page quotation-report-page" style={branchStyle}>
      <div className="report-sticky-zone quotation-sticky-zone">
        <div className="report-title-row">
          <div>
            <p className="section-kicker">Quotation Report</p>
            <div className="report-heading-line">
              <h2>Quotations</h2>
              <BranchDropdown
                value={draftFilters.branchId}
                availabilityNotes={availabilityNotes}
                onChange={(value) => updateDraftFilter('branchId', value)}
              />
            </div>
          </div>
          <button type="button" className="export-button" onClick={exportReport} disabled={isExporting}>
            {isExporting ? 'Exporting...' : 'Export Excel'}
          </button>
        </div>

        <div className="quotation-filter-bar">
          <ReportSearch
            value={draftFilters.searchText}
            onChange={(value) => updateDraftFilter('searchText', value)}
            placeholder="Quotation, customer, city, assist"
          />
          <div className="field-group date-from-field">
            <label htmlFor="quotation-from">From</label>
            <input
              id="quotation-from"
              type="date"
              value={draftFilters.fromDate}
              onChange={(event) => updateDraftFilter('fromDate', event.target.value)}
            />
          </div>
          <div className="field-group date-to-field">
            <label htmlFor="quotation-to">To</label>
            <input
              id="quotation-to"
              type="date"
              value={draftFilters.toDate}
              onChange={(event) => updateDraftFilter('toDate', event.target.value)}
            />
          </div>
          <div className="filter-actions">
            <button type="button" className="apply-button" onClick={applyFilters}>
              Apply
            </button>
            <button type="button" className="clear-button" onClick={clearFilters}>
              Clear
            </button>
          </div>
          <PageSizeSelector value={pageSize} onChange={setPageSize} pageSizes={quotationPageSizes} />
        </div>
      </div>

      <SummaryCards summary={summary} />

      {status === 'error' && <div className="alert-error">{error}</div>}

      <div className={status === 'loading' ? 'report-surface quotation-surface is-loading' : 'report-surface quotation-surface'}>
        {status === 'loading' && rows.length === 0 ? (
          <LoadingIndicator label={`Loading ${activeBranch.label} quotations`} />
        ) : (
          <>
            <QuotationTable
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              isAllBranches={isAllBranches}
              onSelect={setSelectedQuotation}
            />
            <QuotationMobileRows
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              isAllBranches={isAllBranches}
              onSelect={setSelectedQuotation}
            />
            {rows.length === 0 && status !== 'error' && (
              <div className="empty-state">No quotations found for this selection.</div>
            )}
            {status === 'loading' && <LoadingIndicator label={`Refreshing ${activeBranch.label} quotations`} overlay />}
          </>
        )}
      </div>

      <ReportPagination
        pageNumber={pageNumber}
        totalPages={totalPages}
        totalRows={totalRows}
        pageSize={pageSize}
        onPageChange={setPageNumber}
      />

      {selectedQuotation && (
        <QuotationDetailDrawer
          quotation={selectedQuotation}
          detailState={detailState}
          rows={rows}
          onNavigate={setSelectedQuotation}
          onClose={() => setSelectedQuotation(null)}
        />
      )}
    </section>
  )
}

function BranchDropdown({ value, availabilityNotes, onChange }) {
  return (
    <div className="quotation-branch-control">
      <label className="quotation-branch-select">
        <select value={value} onChange={(event) => onChange(event.target.value)} aria-label="Quotation branch">
          <option value={ALL_BRANCHES_ID}>All Branches</option>
          {branches.map((branch) => (
            <option key={branch.id} value={branch.id}>
              {branch.label}
            </option>
          ))}
        </select>
      </label>
      {availabilityNotes.length > 0 && (
        <span className="branch-availability-note">{availabilityNotes.join(' / ')}</span>
      )}
    </div>
  )
}

async function fetchSingleBranchQuotationReport(branch, reportRequest, signal) {
  const result = await fetchBranchPage(branch, reportRequest, signal)
  return {
    ...result,
    rows: result.rows.map((row) => withBranch(row, branch)),
    availabilityNotes: [],
  }
}

async function fetchAllRowsForSingleBranch(branch, reportRequest, signal) {
  const result = await fetchAllRowsForBranch(branch, reportRequest, signal)
  return {
    ...result,
    rows: result.rows.map((row) => withBranch(row, branch)),
    availabilityNotes: [],
  }
}

async function fetchBranchPage(branch, reportRequest, signal) {
  if (reportRequest.pageSize <= ALL_BRANCH_PAGE_SIZE) {
    return fetchQuotationReport(branch, reportRequest, signal)
  }

  const firstServerPage = ((reportRequest.pageNumber - 1) * reportRequest.pageSize) / ALL_BRANCH_PAGE_SIZE
  const startServerPage = Math.floor(firstServerPage) + 1
  const pagesToFetch = Math.ceil(reportRequest.pageSize / ALL_BRANCH_PAGE_SIZE)
  let rows = []
  let totalRows = 0
  let summary = null

  for (let pageOffset = 0; pageOffset < pagesToFetch; pageOffset += 1) {
    const result = await fetchQuotationReport(
      branch,
      {
        ...reportRequest,
        pageNumber: startServerPage + pageOffset,
        pageSize: ALL_BRANCH_PAGE_SIZE,
      },
      signal,
    )

    rows = [...rows, ...result.rows]
    totalRows = result.totalRows
    summary = result.summary

    if (rows.length >= reportRequest.pageSize || rows.length >= totalRows) {
      break
    }
  }

  return {
    rows: rows.slice(0, reportRequest.pageSize),
    totalRows,
    summary: summary || {
      totalQuotations: 0,
      quotationValue: 0,
      priceAltered: 0,
      cancelled: 0,
    },
  }
}

async function fetchAllBranchQuotationReport(reportRequest, signal, exportAll = false) {
  const branchResults = await Promise.allSettled(
    branches.map(async (branch) => {
      const result = await fetchAllRowsForBranch(branch, reportRequest, signal)
      return { branch, result }
    }),
  )

  if (signal.aborted) {
    throw new DOMException('Request aborted', 'AbortError')
  }

  const successful = []
  const availabilityNotes = []

  branchResults.forEach((settled, index) => {
    const branch = branches[index]
    if (settled.status === 'fulfilled') {
      successful.push(settled.value)
      return
    }

    availabilityNotes.push(`${branch.label} unavailable`)
  })

  if (successful.length === 0) {
    throw new Error('All branches are currently unavailable.')
  }

  const mergedRows = successful
    .flatMap(({ branch, result }) => result.rows.map((row) => withBranch(row, branch)))
    .sort((a, b) => {
      const bTime = new Date(b.updatedOn || 0).getTime()
      const aTime = new Date(a.updatedOn || 0).getTime()
      return bTime - aTime || String(b.quotationId).localeCompare(String(a.quotationId))
    })

  // ALL mode intentionally uses a controlled larger per-branch fetch and then merges,
  // sorts, and paginates in React. Independent branch pagination cannot be exact
  // global pagination, so single-branch mode remains server-side paginated.
  const startIndex = (reportRequest.pageNumber - 1) * reportRequest.pageSize
  const rows = exportAll ? mergedRows : mergedRows.slice(startIndex, startIndex + reportRequest.pageSize)

  return {
    rows,
    totalRows: mergedRows.length,
    summary: successful.reduce(
      (total, { result }) => ({
        totalQuotations: total.totalQuotations + result.summary.totalQuotations,
        quotationValue: total.quotationValue + result.summary.quotationValue,
        priceAltered: total.priceAltered + result.summary.priceAltered,
        cancelled: total.cancelled + result.summary.cancelled,
      }),
      {
        totalQuotations: 0,
        quotationValue: 0,
        priceAltered: 0,
        cancelled: 0,
      },
    ),
    availabilityNotes,
  }
}

async function fetchAllRowsForBranch(branch, reportRequest, signal) {
  let pageNumber = 1
  let rows = []
  let summary = null
  let totalRows = 0

  do {
    const result = await fetchQuotationReport(
      branch,
      {
        ...reportRequest,
        pageNumber,
        pageSize: ALL_BRANCH_PAGE_SIZE,
      },
      signal,
    )

    rows = [...rows, ...result.rows]
    summary = result.summary
    totalRows = result.totalRows
    pageNumber += 1
  } while (rows.length < Math.min(totalRows, ALL_BRANCH_FETCH_SIZE) && rows.length < totalRows)

  return {
    rows,
    totalRows,
    summary: summary || {
      totalQuotations: 0,
      quotationValue: 0,
      priceAltered: 0,
      cancelled: 0,
    },
  }
}

function withBranch(row, branch) {
  return {
    ...row,
    id: `${branch.id}-${row.id}`,
    branch,
    branchCode: branch.id,
    branchName: branch.label,
  }
}

function SummaryCards({ summary }) {
  return (
    <div className="quotation-summary-grid quotation-summary-three">
      <CounterCard label="Nos" value={formatStock(summary.totalQuotations)} />
      <CounterCard label="Value" value={formatMoney(summary.quotationValue)} />
      <CounterCard label="Altered" value={formatStock(summary.priceAltered)} tone="warning" />
    </div>
  )
}

function CounterCard({ label, value, tone }) {
  return (
    <div className={tone ? `counter-card is-${tone}` : 'counter-card'}>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  )
}

function QuotationTable({ rows, pageNumber, pageSize, isAllBranches, onSelect }) {
  if (rows.length === 0) {
    return null
  }

  return (
    <div className="table-wrap quotation-table-wrap">
      <table className="stock-table quotation-table">
        <colgroup>
          <col className="col-serial" />
          <col className="col-quotation-id" />
          {isAllBranches && <col className="col-branch-name" />}
          <col className="col-customer" />
          <col className="col-count" />
          <col className="col-quantity" />
          <col className="col-value" />
          <col className="col-altered" />
        </colgroup>
        <thead>
          <tr>
            <th className="numeric">S.No</th>
            <th>Quotation ID</th>
            {isAllBranches && <th>Branch Name</th>}
            <th>Customer</th>
            <th className="numeric">Items</th>
            <th className="numeric">Quantity</th>
            <th className="numeric">Value</th>
            <th>Altered</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id} onClick={() => onSelect(row)}>
              <td className="numeric">{getSerialNumber(pageNumber, pageSize, index)}</td>
              <td>
                <button type="button" className="link-button" onClick={() => onSelect(row)}>
                  {row.quotationId}
                </button>
              </td>
              {isAllBranches && <td>{row.branchName}</td>}
              <td>{row.customerName}</td>
              <td className="numeric">{formatStock(row.itemCount)}</td>
              <td className="numeric">{formatStock(row.totalQuantity)}</td>
              <td className="numeric">{formatMoney(row.quotationValue)}</td>
              <td>{row.hasPriceAltered ? <PriceAlteredBadge /> : '-'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function QuotationMobileRows({ rows, pageNumber, pageSize, isAllBranches, onSelect }) {
  if (rows.length === 0) {
    return null
  }

  return (
    <div className="mobile-stock-list mobile-quotation-list">
      {rows.map((row, index) => (
        <button type="button" className="mobile-quotation-row" key={row.id} onClick={() => onSelect(row)}>
          <span>
            <strong>{getSerialNumber(pageNumber, pageSize, index)}. {row.quotationId}</strong>
            {isAllBranches && <BranchBadge row={row} />}
          </span>
          <span className="mobile-quotation-value">{formatMoney(row.quotationValue)}</span>
          <span className="mobile-quotation-customer">{row.customerName}</span>
          <span className="mobile-quotation-meta">
            Items {formatStock(row.itemCount)} / Qty {formatStock(row.totalQuantity)}
          </span>
          {row.hasPriceAltered && <PriceAlteredBadge />}
        </button>
      ))}
    </div>
  )
}

function getSerialNumber(pageNumber, pageSize, rowIndex) {
  return ((pageNumber - 1) * pageSize) + rowIndex + 1
}

function BranchBadge({ row }) {
  return <span className="quotation-row-branch">{row.branchName}</span>
}

function QuotationDetailDrawer({ quotation, detailState, rows, onNavigate, onClose }) {
  const data = detailState.data
  const currentIndex = rows.findIndex((row) => row.id === quotation.id)
  const previousQuotation = currentIndex > 0 ? rows[currentIndex - 1] : null
  const nextQuotation = currentIndex >= 0 && currentIndex < rows.length - 1 ? rows[currentIndex + 1] : null

  return (
    <div
      className="quotation-drawer-layer"
      role="dialog"
      aria-modal="true"
      aria-label="Quotation detail"
      onClick={onClose}
    >
      <aside className="quotation-drawer" onClick={(event) => event.stopPropagation()}>
        <header className="quotation-drawer-header">
          <div>
            <p className="section-kicker">Quotation Detail</p>
            <h2>{quotation.quotationId}</h2>
            <span>{quotation.customerName}</span>
            <small>{quotation.branchName ? `${quotation.branchName} / ` : ''}{quotation.city} / {quotation.updatedOnLabel}</small>
          </div>
          <div className="quotation-drawer-actions">
            <button
              type="button"
              className="detail-nav-button"
              onClick={() => previousQuotation && onNavigate(previousQuotation)}
              disabled={!previousQuotation}
            >
              Previous
            </button>
            <button
              type="button"
              className="detail-nav-button"
              onClick={() => nextQuotation && onNavigate(nextQuotation)}
              disabled={!nextQuotation}
            >
              Next
            </button>
            <button type="button" className="sync-close-button" onClick={onClose} aria-label="Close detail">
              x
            </button>
          </div>
        </header>
        <div className="quotation-drawer-body">
          {detailState.status === 'loading' ? (
            <LoadingIndicator label="Loading quotation detail" />
          ) : detailState.status === 'error' ? (
            <div className="alert-error">{detailState.error}</div>
          ) : data ? (
            <>
              <DetailLines lines={data.lines} />
              <DetailFooter data={data} />
            </>
          ) : null}
        </div>
      </aside>
    </div>
  )
}

function DetailLines({ lines }) {
  if (lines.length === 0) {
    return <div className="empty-state">No quotation lines found.</div>
  }

  return (
    <>
      <div className="quotation-detail-table-wrap">
        <table className="quotation-detail-table">
          <colgroup>
            <col className="detail-col-product" />
            <col className="detail-col-qty" />
            <col className="detail-col-rate" />
            <col className="detail-col-gst" />
            <col className="detail-col-amount" />
          </colgroup>
          <thead>
            <tr>
              <th>Product Name</th>
              <th className="numeric">Qty</th>
              <th className="numeric">Rate</th>
              <th className="numeric">GST %</th>
              <th className="numeric">Amount</th>
            </tr>
          </thead>
          <tbody>
            {lines.map((line) => (
              <tr key={line.id}>
                <td>
                  <div className="bill-product-cell">
                    <strong>{line.productName}</strong>
                    {line.isPriceAltered && <PriceAlteredBadge text={`PRICE ALTERED Original ${formatMoney(line.masterSalesPrice)}`} />}
                  </div>
                </td>
                <td className="numeric">{formatStock(line.quantity)}</td>
                <td className="numeric">{formatMoney(line.quotedRate)}</td>
                <td className="numeric">{formatGst(line.gst)}</td>
                <td className="numeric detail-amount">{formatMoney(line.amount)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="quotation-detail-mobile-list">
        {lines.map((line) => (
          <article className="quotation-detail-mobile-line" key={line.id}>
            <div>
              <strong>{line.productName}</strong>
              {line.isPriceAltered && <PriceAlteredBadge text={`PRICE ALTERED Original ${formatMoney(line.masterSalesPrice)}`} />}
            </div>
            <small>
              Qty {formatStock(line.quantity)} x {formatMoney(line.quotedRate)}
            </small>
            <span>GST {formatGst(line.gst)}</span>
            <b>{formatMoney(line.amount)}</b>
          </article>
        ))}
      </div>
    </>
  )
}

function DetailFooter({ data }) {
  return (
    <footer className="quotation-detail-footer">
      <div className="bill-total-section">
        <div>
          <span>Taxable Value</span>
          <strong>{formatMoney(data.totals.taxableValue)}</strong>
        </div>
        <div>
          <span>GST Amount</span>
          <strong>{formatMoney(data.totals.gstAmount)}</strong>
        </div>
        <div className="grand-total-line">
          <span>Grand Total</span>
          <strong>{formatMoney(data.totals.grandTotal)}</strong>
        </div>
      </div>
    </footer>
  )
}

function PriceAlteredBadge({ text = 'PRICE ALTERED' }) {
  return <span className="price-altered-badge">{text}</span>
}

export default QuotationReportPage
