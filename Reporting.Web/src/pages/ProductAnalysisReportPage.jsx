import { useEffect, useMemo, useRef, useState } from 'react'
import LoadingIndicator from '../components/LoadingIndicator'
import PageSizeSelector from '../components/PageSizeSelector'
import ReportPagination from '../components/ReportPagination'
import { branches, defaultBranchId, getBranchById } from '../config/branches'
import { downloadXlsx } from '../services/excelExport'
import {
  PRODUCT_ANALYSIS_SORTS,
  emptyProductAnalysisSummary,
  fetchProductAnalysisReport,
  formatMoney,
  formatStock,
  normalizeCompareSort,
  normalizeSortDirection,
  sortProductAnalysisRows,
} from '../services/productAnalysisReport'
import {
  getFileDateStamp,
  getTodayInputValue,
  withBranch,
} from '../services/reportUtils'

const PRODUCT_ANALYSIS_PAGE_SIZE = 500
const PRODUCT_ANALYSIS_PAGE_SIZES = [100, 250, 500]
const EXPORT_FETCH_PAGE_SIZE = 500
const ALL_BRANCHES_ID = 'ALL_BRANCHES'
const MAX_DATE_RANGE_DAYS = 35
const ANALYSIS_MODES = {
  PRODUCT: 'PRODUCT',
  CUSTOMER: 'CUSTOMER',
}

const emptyDashboardData = {
  topByAmount: [],
  topByQuantity: [],
  topByBillCount: [],
  branchSummaries: [],
}

function createDefaultFilters(branchId = defaultBranchId) {
  const today = getTodayInputValue()
  return {
    productSearch: '',
    analysisMode: ANALYSIS_MODES.PRODUCT,
    fromDate: today,
    toDate: today,
    branchId,
  }
}

function ProductAnalysisReportPage({ selectedBranchId, onBranchChange }) {
  const [draftFilters, setDraftFilters] = useState(() => createDefaultFilters(selectedBranchId || defaultBranchId))
  const [appliedFilters, setAppliedFilters] = useState(() => createDefaultFilters(selectedBranchId || defaultBranchId))
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(PRODUCT_ANALYSIS_PAGE_SIZE)
  const [sortBy, setSortBy] = useState(PRODUCT_ANALYSIS_SORTS.TRANS_DATE)
  const [sortDirection, setSortDirection] = useState('DESC')
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [summary, setSummary] = useState(emptyProductAnalysisSummary)
  const [dashboard, setDashboard] = useState(emptyDashboardData)
  const [showMetrics, setShowMetrics] = useState(false)
  const [status, setStatus] = useState('loading')
  const [error, setError] = useState('')
  const [availabilityNotes, setAvailabilityNotes] = useState([])
  const [isExporting, setIsExporting] = useState(false)
  const requestIdRef = useRef(0)
  const metricsRef = useRef(null)
  const isAllBranches = appliedFilters.branchId === ALL_BRANCHES_ID
  const activeBranch = useMemo(
    () => (isAllBranches ? getBranchById(defaultBranchId) : getBranchById(appliedFilters.branchId)),
    [appliedFilters.branchId, isAllBranches],
  )
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.productSearch,
    appliedFilters.analysisMode,
    appliedFilters.fromDate,
    appliedFilters.toDate,
    appliedFilters.branchId,
    pageSize,
  ])

  useEffect(() => {
    const controller = new AbortController()
    const requestId = requestIdRef.current + 1
    requestIdRef.current = requestId

    setStatus('loading')
    setError('')
    setAvailabilityNotes([])

    const validationError = validateFilters(appliedFilters)
    if (validationError) {
      setRows([])
      setTotalRows(0)
      setSummary(emptyProductAnalysisSummary)
      setDashboard(emptyDashboardData)
      setStatus('error')
      setError(validationError)
      return () => controller.abort()
    }

    const reportRequest = createReportRequest(appliedFilters, pageNumber, pageSize, sortBy, sortDirection)
    const request = isAllBranches
      ? fetchAllBranchesResult(reportRequest, controller.signal, false)
      : fetchSingleBranchResult(activeBranch, reportRequest, controller.signal)

    request
      .then((result) => {
        if (requestId !== requestIdRef.current) {
          return
        }

        setRows(result.rows)
        setTotalRows(result.totalRows)
        setSummary(result.summary)
        setDashboard(result.dashboard || emptyDashboardData)
        setAvailabilityNotes(result.availabilityNotes || [])
        setStatus('ready')
      })
      .catch((apiError) => {
        if (apiError.name === 'AbortError' || requestId !== requestIdRef.current) {
          return
        }

        setRows([])
        setTotalRows(0)
        setSummary(emptyProductAnalysisSummary)
        setDashboard(emptyDashboardData)
        setStatus('error')
        setError(apiError.message || `${activeBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [activeBranch, appliedFilters, isAllBranches, pageNumber, pageSize, sortBy, sortDirection])

  function updateDraftFilter(name, value) {
    setDraftFilters((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function applyFilters() {
    const nextFilters = {
      ...draftFilters,
      productSearch: draftFilters.productSearch.trim(),
    }

    setAppliedFilters(nextFilters)
    setShowMetrics(false)
    setPageNumber(1)
    if (
      nextFilters.analysisMode === ANALYSIS_MODES.PRODUCT &&
      (sortBy === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME || sortBy === PRODUCT_ANALYSIS_SORTS.CITY)
    ) {
      setSortBy(PRODUCT_ANALYSIS_SORTS.TRANS_DATE)
      setSortDirection('DESC')
    }
    if (nextFilters.branchId !== ALL_BRANCHES_ID) {
      onBranchChange?.(nextFilters.branchId)
    }
  }

  function clearFilters() {
    const defaults = createDefaultFilters()
    setDraftFilters(defaults)
    setAppliedFilters(defaults)
    setPageSize(PRODUCT_ANALYSIS_PAGE_SIZE)
    setSortBy(PRODUCT_ANALYSIS_SORTS.TRANS_DATE)
    setSortDirection('DESC')
    setShowMetrics(false)
    setPageNumber(1)
    onBranchChange?.(defaults.branchId)
  }

  function changeBranch(branchId) {
    setRows([])
    setTotalRows(0)
    setSummary(emptyProductAnalysisSummary)
    setDashboard(emptyDashboardData)
    setAvailabilityNotes([])

    const nextFilters = {
      ...draftFilters,
      branchId,
    }

    setDraftFilters(nextFilters)
    setAppliedFilters(nextFilters)
    setShowMetrics(false)
    setSortBy(PRODUCT_ANALYSIS_SORTS.TRANS_DATE)
    setSortDirection('DESC')
    setPageNumber(1)

    if (branchId !== ALL_BRANCHES_ID) {
      onBranchChange?.(branchId)
    }
  }

  function changeSort(nextSortBy) {
    const normalizedSortBy = isAllBranches ? normalizeCompareSort(nextSortBy) : nextSortBy
    if (sortBy === normalizedSortBy) {
      setSortDirection((current) => (current === 'ASC' ? 'DESC' : 'ASC'))
    } else {
      setSortBy(normalizedSortBy)
      setSortDirection(normalizedSortBy === PRODUCT_ANALYSIS_SORTS.TRANS_DATE ? 'DESC' : 'ASC')
    }
    setPageNumber(1)
  }

  function toggleMetrics() {
    setShowMetrics((current) => {
      const next = !current
      if (next) {
        window.setTimeout(() => {
          metricsRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' })
        }, 80)
      }

      return next
    })
  }

  async function exportReport() {
    const controller = new AbortController()
    setIsExporting(true)
    setError('')

    try {
      const request = createReportRequest(appliedFilters, 1, pageSize, sortBy, sortDirection)
      const result = isAllBranches
        ? await fetchAllBranchesResult(request, controller.signal, true)
        : await fetchAllRowsForSingleBranch(activeBranch, request, controller.signal)

      downloadXlsx({
        filename: `Product_Analysis_Detail_${getFileDateStamp()}.xlsx`,
        sheetName: 'Product Analysis',
        columns: createExportColumns(isAllBranches, appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER),
        rows: result.rows.map((row, index) => ({ ...row, serialNumber: index + 1 })),
      })

      setAvailabilityNotes(result.availabilityNotes || [])
    } catch (apiError) {
      if (apiError.name !== 'AbortError') {
        setError(apiError.message || 'Unable to export product analysis.')
      }
    } finally {
      setIsExporting(false)
    }
  }

  const branchStyle = {
    '--branch-accent': isAllBranches ? '#b91c1c' : activeBranch.accent,
    '--branch-accent-soft': isAllBranches ? '#fee2e2' : activeBranch.accentSoft,
    '--branch-accent-border': isAllBranches ? '#fca5a5' : activeBranch.accentBorder,
  }

  return (
    <section className="report-page quotation-report-page product-analysis-page" style={branchStyle}>
      <div className="report-sticky-zone quotation-sticky-zone">
        <div className="report-title-row">
          <div>
            <p className="section-kicker">Product Analysis</p>
            <div className="report-heading-line">
              <h2>{isAllBranches ? 'All Branches' : activeBranch.label}</h2>
              <BranchDropdown value={draftFilters.branchId} availabilityNotes={availabilityNotes} onChange={changeBranch} />
            </div>
          </div>
          <button type="button" className="export-button" onClick={exportReport} disabled={isExporting}>
            {isExporting ? 'Exporting...' : 'Export Excel'}
          </button>
        </div>

        <div className="quotation-filter-bar product-analysis-filter-bar">
          <div className="field-group date-from-field">
            <label htmlFor="product-analysis-from">From</label>
            <input id="product-analysis-from" type="date" value={draftFilters.fromDate} onChange={(event) => updateDraftFilter('fromDate', event.target.value)} required />
          </div>
          <div className="field-group date-to-field">
            <label htmlFor="product-analysis-to">To</label>
            <input id="product-analysis-to" type="date" value={draftFilters.toDate} onChange={(event) => updateDraftFilter('toDate', event.target.value)} required />
          </div>
          <ProductSearch
            value={draftFilters.productSearch}
            onChange={(value) => updateDraftFilter('productSearch', value)}
            onClear={() => updateDraftFilter('productSearch', '')}
          />
          <AnalysisModeSelector
            value={draftFilters.analysisMode}
            onChange={(value) => updateDraftFilter('analysisMode', value)}
          />
          <div className="filter-actions">
            <button type="button" className="apply-button" onClick={applyFilters}>Apply</button>
            <button type="button" className="clear-button" onClick={clearFilters}>Clear</button>
          </div>
          <PageSizeSelector value={pageSize} onChange={setPageSize} pageSizes={PRODUCT_ANALYSIS_PAGE_SIZES} />
        </div>
      </div>

      {status === 'error' && <div className="alert-error">{error}</div>}
      {error && status !== 'error' && <div className="alert-error">{error}</div>}
      {availabilityNotes.length > 0 && <div className="branch-status-message">{availabilityNotes.join(' ')}</div>}

      <div className={status === 'loading' ? 'report-surface quotation-surface is-loading' : 'report-surface quotation-surface'}>
        {status === 'loading' && rows.length === 0 ? (
          <LoadingIndicator label={`Loading ${isAllBranches ? 'all branches' : activeBranch.label + ' product transactions'}`} />
        ) : (
          <>
            <div className="dashboard-section-heading">
              <div>
                <p className="section-kicker">Transaction Detail</p>
                <h3>Completed Estimation Product Rows</h3>
              </div>
            </div>
            <ProductTransactionTable
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              sortBy={sortBy}
              sortDirection={sortDirection}
              onSort={changeSort}
              showBranch={isAllBranches}
              showCustomer={appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER}
            />
            {rows.length === 0 && status !== 'error' && <div className="empty-state">No completed estimation product rows found for the selected criteria.</div>}
            {status === 'loading' && <LoadingIndicator label="Refreshing product analysis" overlay />}
          </>
        )}
      </div>

      <div className="product-analysis-footer-row">
        <ReportPagination pageNumber={pageNumber} totalPages={totalPages} totalRows={totalRows} pageSize={pageSize} onPageChange={setPageNumber} />
        <div className="metrics-launch-row">
          <button
            type="button"
            className="metrics-launch-link"
            onClick={toggleMetrics}
            aria-expanded={showMetrics}
            aria-controls="product-analysis-metrics"
          >
            {showMetrics ? 'Hide Metrics Dashboard' : 'View Metrics Dashboard'}
          </button>
        </div>
      </div>

      <div id="product-analysis-metrics" ref={metricsRef} className={showMetrics ? 'metrics-reveal is-open' : 'metrics-reveal'} aria-hidden={!showMetrics}>
        <div className="metrics-reveal-inner">
          <AnalysisContext filters={appliedFilters} isAllBranches={isAllBranches} activeBranch={activeBranch} />
          <SummaryCards summary={summary} />
          <DashboardGrid dashboard={dashboard} summary={summary} />
          {isAllBranches && (
            <BranchComparison summaries={dashboard.branchSummaries} />
          )}
        </div>
      </div>
    </section>
  )
}

function validateFilters(filters) {
  if (!filters.fromDate || !filters.toDate) {
    return 'From Date and To Date are required.'
  }

  if (filters.fromDate > filters.toDate) {
    return 'From Date must be before or equal to To Date.'
  }

  const fromDate = new Date(`${filters.fromDate}T00:00:00`)
  const toDate = new Date(`${filters.toDate}T00:00:00`)
  if (Number.isNaN(fromDate.getTime()) || Number.isNaN(toDate.getTime())) {
    return 'Please select valid From Date and To Date values.'
  }

  const dayCount = Math.floor((toDate.getTime() - fromDate.getTime()) / 86400000) + 1
  if (dayCount > MAX_DATE_RANGE_DAYS) {
    return `Maximum allowed date range is ${MAX_DATE_RANGE_DAYS} days.`
  }

  return ''
}

function createReportRequest(filters, pageNumber, pageSize, sortBy, sortDirection) {
  return {
    pageNumber,
    pageSize,
    productSearch: filters.productSearch,
    fromDate: filters.fromDate,
    toDate: filters.toDate,
    sortBy,
    sortDirection,
  }
}

async function fetchSingleBranchResult(activeBranch, reportRequest, signal) {
  const [result, allRowsResult] = await Promise.all([
    fetchProductAnalysisReport(activeBranch, reportRequest, signal),
    fetchAllRowsForSingleBranch(activeBranch, reportRequest, signal),
  ])

  const rows = result.rows.map((row) => withBranch(row, activeBranch))
  const dashboard = createDashboard(allRowsResult.rows, [
    {
      branch: activeBranch,
      status: 'online',
      summary: allRowsResult.summary,
    },
  ])

  return {
    rows,
    totalRows: result.totalRows,
    summary: allRowsResult.summary || result.summary || emptyProductAnalysisSummary,
    dashboard,
    availabilityNotes: [],
  }
}

async function fetchAllRowsForSingleBranch(activeBranch, reportRequest, signal) {
  let page = 1
  let rows = []
  let totalRows = 0
  let summary = null

  do {
    const result = await fetchProductAnalysisReport(
      activeBranch,
      { ...reportRequest, pageNumber: page, pageSize: EXPORT_FETCH_PAGE_SIZE },
      signal,
    )
    rows = [...rows, ...result.rows.map((row) => withBranch(row, activeBranch))]
    totalRows = result.totalRows
    summary = result.summary
    page += 1
  } while (rows.length < totalRows)

  return {
    rows,
    totalRows,
    summary: summary || aggregateRows(rows),
    availabilityNotes: [],
  }
}

async function fetchAllBranchesResult(reportRequest, signal, exportAll = false) {
  const branchResults = await Promise.allSettled(
    branches.map(async (branch) => {
      const result = await fetchAllRowsForSingleBranch(branch, reportRequest, signal)
      return { branch, result }
    }),
  )

  if (signal.aborted) {
    throw new DOMException('Request aborted', 'AbortError')
  }

  const availabilityNotes = []
  const successful = []

  branchResults.forEach((settled, index) => {
    if (settled.status === 'fulfilled') {
      successful.push(settled.value)
    } else {
      availabilityNotes.push(`${branches[index].label} branch is currently unavailable.`)
    }
  })

  if (successful.length === 0) {
    throw new Error('All branches are currently unavailable.')
  }

  const merged = sortProductAnalysisRows(
    successful.flatMap(({ result }) => result.rows),
    reportRequest.sortBy,
    reportRequest.sortDirection,
  )
  const start = (reportRequest.pageNumber - 1) * reportRequest.pageSize
  const pageRows = exportAll ? merged : merged.slice(start, start + reportRequest.pageSize)
  const branchSummaries = successful.map(({ branch, result }) => ({
    branch,
    status: 'online',
    summary: result.summary || aggregateRows(result.rows),
  }))
  const offlineSummaries = branches
    .filter((branch) => !successful.some((item) => item.branch.id === branch.id))
    .map((branch) => ({
      branch,
      status: 'offline',
      summary: emptyProductAnalysisSummary,
    }))

  const summary = aggregateRows(merged)

  return {
    rows: pageRows,
    totalRows: merged.length,
    summary,
    dashboard: createDashboard(merged, [...branchSummaries, ...offlineSummaries]),
    availabilityNotes,
    branches,
  }
}

function createDashboard(rows, branchSummaries) {
  const productRows = aggregateProducts(rows)

  return {
    topByAmount: [...productRows].sort((a, b) => b.salesAmount - a.salesAmount || a.productName.localeCompare(b.productName)).slice(0, 10),
    topByQuantity: [...productRows].sort((a, b) => b.salesCount - a.salesCount || a.productName.localeCompare(b.productName)).slice(0, 10),
    topByBillCount: [...productRows].sort((a, b) => b.billCount - a.billCount || a.productName.localeCompare(b.productName)).slice(0, 10),
    branchSummaries,
  }
}

function aggregateRows(rows) {
  const productIds = new Set()
  const transactionIds = new Set()
  let totalQuantity = 0
  let totalSalesAmount = 0

  rows.forEach((row) => {
    productIds.add(String(row.productId || row.productName || '-'))
    transactionIds.add(`${row.branchCode || ''}-${row.transId || '-'}`)
    totalQuantity += row.transQty || 0
    totalSalesAmount += row.totalPrice || 0
  })

  return {
    products: productIds.size,
    bills: transactionIds.size,
    totalQuantity,
    totalSalesAmount,
  }
}

function aggregateProducts(rows) {
  const map = new Map()

  rows.forEach((row) => {
    const key = String(row.productId || row.productName || '-')
    const current = map.get(key) || {
      productId: key,
      productName: row.productName || '-',
      salesCount: 0,
      salesAmount: 0,
      transactionIds: new Set(),
    }

    current.salesCount += row.transQty || 0
    current.salesAmount += row.totalPrice || 0
    current.transactionIds.add(`${row.branchCode || ''}-${row.transId || '-'}`)
    map.set(key, current)
  })

  return Array.from(map.values()).map((row) => ({
    ...row,
    billCount: row.transactionIds.size,
  }))
}

function BranchDropdown({ value, availabilityNotes, onChange }) {
  return (
    <div className="quotation-branch-control">
      <label className="quotation-branch-select">
        <select value={value} onChange={(event) => onChange(event.target.value)} aria-label="Product analysis branch">
          <option value={ALL_BRANCHES_ID}>All Branches</option>
          {branches.map((branch) => <option key={branch.id} value={branch.id}>{branch.label}</option>)}
        </select>
      </label>
      {availabilityNotes.length > 0 && <span className="branch-availability-note">{availabilityNotes.length} branch issue</span>}
    </div>
  )
}

function ProductSearch({ value, onChange, onClear }) {
  return (
    <div className="field-group product-search-field">
      <label htmlFor="product-analysis-product">Product Search</label>
      <div className="product-search-box">
        <input id="product-analysis-product" value={value} onChange={(event) => onChange(event.target.value)} placeholder="Product name" />
        {value && <button type="button" onClick={onClear} aria-label="Clear product search">x</button>}
      </div>
    </div>
  )
}

function AnalysisModeSelector({ value, onChange }) {
  return (
    <div className="field-group analysis-mode-field">
      <label>Report View</label>
      <div className="analysis-mode-options">
        <label>
          <input
            type="radio"
            name="product-analysis-mode"
            value={ANALYSIS_MODES.PRODUCT}
            checked={value === ANALYSIS_MODES.PRODUCT}
            onChange={(event) => onChange(event.target.value)}
          />
          Product Wise
        </label>
        <label>
          <input
            type="radio"
            name="product-analysis-mode"
            value={ANALYSIS_MODES.CUSTOMER}
            checked={value === ANALYSIS_MODES.CUSTOMER}
            onChange={(event) => onChange(event.target.value)}
          />
          Customer Wise
        </label>
      </div>
    </div>
  )
}

function SummaryCards({ summary }) {
  const items = [
    ['Products', formatStock(summary.products)],
    ['Transactions', formatStock(summary.bills)],
    ['Total Quantity', formatStock(summary.totalQuantity)],
    ['Total Price', formatMoney(summary.totalSalesAmount)],
  ]

  return (
    <div className="quotation-summary-grid">
      {items.map(([label, value]) => (
        <div className="counter-card" key={label}>
          <span>{label}</span>
          <strong>{value}</strong>
        </div>
      ))}
    </div>
  )
}

function AnalysisContext({ filters, isAllBranches, activeBranch }) {
  return (
    <div className="analysis-context-strip">
      <span>Analysis Period: <strong>{formatDisplayDate(filters.fromDate)} to {formatDisplayDate(filters.toDate)}</strong></span>
      <span>Branch: <strong>{isAllBranches ? 'All Branches' : activeBranch.label}</strong></span>
    </div>
  )
}

function DashboardGrid({ dashboard, summary }) {
  return (
    <div className="product-dashboard-grid">
      <RankingCard title="Top Products by Total Price" rows={dashboard.topByAmount} valueLabel="Total Price" getValue={(row) => row.salesAmount || 0} renderValue={(value) => formatMoney(value)} showContribution total={summary.totalSalesAmount} className="dashboard-card-feature" />
      <RankingCard title="Top Products by Quantity" rows={dashboard.topByQuantity} valueLabel="Trans Qty" getValue={(row) => row.salesCount || 0} renderValue={(value) => formatStock(value)} className="dashboard-card-feature" />
      <RankingCard title="Top Products by Transactions" rows={dashboard.topByBillCount} valueLabel="Transactions" getValue={(row) => row.billCount || 0} renderValue={(value) => formatStock(value)} />
      <ContributionCard rows={dashboard.topByAmount} total={summary.totalSalesAmount} />
    </div>
  )
}

function RankingCard({ title, rows, valueLabel, getValue, renderValue, total = 0, showContribution = false, className = '' }) {
  const maxValue = rows.reduce((max, row) => Math.max(max, getValue(row)), 0)

  return (
    <div className={`dashboard-card ${className}`}>
      <div className="dashboard-card-title">
        <h3>{title}</h3>
        <span>{valueLabel}</span>
      </div>
      {rows.length === 0 ? (
        <NoData />
      ) : (
        <div className="dashboard-ranking-list">
          {rows.slice(0, 10).map((row, index) => {
            const value = getValue(row)
            const percentOfMax = maxValue > 0 ? Math.max(3, (value / maxValue) * 100) : 0
            const contribution = total > 0 ? (value / total) * 100 : 0

            return (
              <div className="dashboard-ranking-row" key={`${row.productId}-${index}`}>
                <span className="rank-number">{index + 1}</span>
                <div className="rank-product">
                  <div>
                    <strong>{row.productName}</strong>
                    {showContribution && <small>{formatPercent(contribution)} of total</small>}
                  </div>
                  <span className="rank-bar"><i style={{ width: `${percentOfMax}%` }} /></span>
                </div>
                <span className="rank-value">{renderValue(value)}</span>
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}

function ContributionCard({ rows, total }) {
  const topRows = rows.slice(0, 5)
  const topTotal = topRows.reduce((sum, row) => sum + (row.salesAmount || 0), 0)
  const items = topRows.map((row) => ({
    id: row.productId,
    label: row.productName,
    value: row.salesAmount || 0,
  }))
  const others = Math.max(0, total - topTotal)
  if (others > 0) {
    items.push({ id: 'others', label: 'Others', value: others })
  }

  return (
    <div className="dashboard-card">
      <div className="dashboard-card-title">
        <h3>Total Price Contribution</h3>
        <span>Share</span>
      </div>
      {items.length === 0 || total <= 0 ? (
        <NoData />
      ) : (
        <div className="contribution-list">
          {items.map((item) => {
            const contribution = (item.value / total) * 100
            return (
              <div className="contribution-row" key={item.id}>
                <div>
                  <strong>{item.label}</strong>
                  <span>{formatPercent(contribution)}</span>
                </div>
                <span className="rank-bar"><i style={{ width: `${Math.max(3, contribution)}%` }} /></span>
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}

function BranchComparison({ summaries }) {
  return (
    <section className="branch-comparison-section">
      <div className="dashboard-section-heading">
        <div>
          <p className="section-kicker">Branch Comparison</p>
          <h3>Branch Totals</h3>
        </div>
      </div>
      <div className="branch-comparison-grid">
        {summaries.map(({ branch, summary, status }) => (
          <div className={status === 'offline' ? 'branch-summary-card is-offline' : 'branch-summary-card'} key={branch.id}>
            <div>
              <h4>{branch.label}</h4>
              <span>{status === 'offline' ? 'Offline' : 'Online'}</span>
            </div>
            <dl>
              <dt>Products</dt>
              <dd>{formatStock(summary.products)}</dd>
              <dt>Transactions</dt>
              <dd>{formatStock(summary.bills)}</dd>
              <dt>Qty</dt>
              <dd>{formatStock(summary.totalQuantity)}</dd>
              <dt>Total</dt>
              <dd>{formatMoney(summary.totalSalesAmount)}</dd>
            </dl>
          </div>
        ))}
      </div>
    </section>
  )
}

function NoData() {
  return <div className="dashboard-no-data">No Data</div>
}

function formatPercent(value) {
  return `${(Number.isFinite(value) ? value : 0).toLocaleString('en-IN', {
    minimumFractionDigits: 1,
    maximumFractionDigits: 1,
  })}%`
}

function formatDisplayDate(value) {
  if (!value) return '-'
  const [year, month, day] = value.split('-').map(Number)
  const date = new Date(year, month - 1, day)
  if (Number.isNaN(date.getTime())) return value
  return new Intl.DateTimeFormat('en-IN', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

function formatTransactionDate(value) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return String(value)
  return new Intl.DateTimeFormat('en-IN', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

function SortHeader({ children, sortKey, sortBy, sortDirection, onSort, className = '' }) {
  const active = sortBy === sortKey || normalizeCompareSort(sortBy) === sortKey

  return (
    <th className={className}>
      <button type="button" className={active ? 'table-sort-button is-active' : 'table-sort-button'} onClick={() => onSort(sortKey)}>
        <span>{children}</span>
        <small>{active ? normalizeSortDirection(sortDirection) : ''}</small>
      </button>
    </th>
  )
}

function ProductTransactionTable({ rows, pageNumber, pageSize, sortBy, sortDirection, onSort, showBranch, showCustomer }) {
  if (rows.length === 0) return null
  const tableClassName = [
    'stock-table',
    'quotation-table',
    'product-analysis-table',
    'product-analysis-detail-table',
    showBranch ? 'has-branch-column' : '',
    showCustomer ? 'has-customer-columns' : '',
  ].filter(Boolean).join(' ')

  return (
    <div className="table-wrap quotation-table-wrap">
      <table className={tableClassName}>
        <thead>
          <tr>
            <th className="numeric col-product-analysis-serial">S.No</th>
            {showBranch && <th className="col-product-analysis-branch">Branch</th>}
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TRANS_ID} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-trans">Trans ID</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-name">Product Name</SortHeader>
            {showCustomer && <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-customer">Customer Name</SortHeader>}
            {showCustomer && <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CITY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-city">City</SortHeader>}
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.BRAND} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-brand">Brand</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CATEGORY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-category">Category</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TRANS_DATE} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-date">Trans Date</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TRANS_QTY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Trans Qty</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.PRICE} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Price</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TOTAL_PRICE} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Total Price</SortHeader>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id}>
              <td className="numeric">{((pageNumber - 1) * pageSize) + index + 1}</td>
              {showBranch && <td>{row.branchName}</td>}
              <td className="product-analysis-trans-cell">{row.transId}</td>
              <td>{row.productName}</td>
              {showCustomer && <td>{row.customerName}</td>}
              {showCustomer && <td>{row.city}</td>}
              <td>{row.brand}</td>
              <td>{row.category}</td>
              <td>{formatTransactionDate(row.transDate)}</td>
              <td className="numeric">{formatStock(row.transQty)}</td>
              <td className="numeric">{formatMoney(row.price)}</td>
              <td className="numeric">{formatMoney(row.totalPrice)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function createExportColumns(includeBranch, includeCustomer) {
  return [
    { header: 'S.No', value: (row) => row.serialNumber },
    ...(includeBranch ? [{ header: 'Branch', value: (row) => row.branchName }] : []),
    { header: 'TRANS ID', value: (row) => row.transId },
    { header: 'PRODUCT NAME', value: (row) => row.productName },
    ...(includeCustomer ? [
      { header: 'CUSTOMER NAME', value: (row) => row.customerName },
      { header: 'CITY', value: (row) => row.city },
    ] : []),
    { header: 'BRAND', value: (row) => row.brand },
    { header: 'CATEGORY', value: (row) => row.category },
    { header: 'TRANS DATE', value: (row) => formatTransactionDate(row.transDate) },
    { header: 'TRANS QTY', value: (row) => row.transQty },
    { header: 'PRICE', value: (row) => row.price },
    { header: 'TOTAL PRICE', value: (row) => row.totalPrice },
  ]
}

export default ProductAnalysisReportPage
