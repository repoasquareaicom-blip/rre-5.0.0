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

const emptyMetricsData = {
  rows: [],
  summary: emptyProductAnalysisSummary,
  dashboard: emptyDashboardData,
  availabilityNotes: [],
}

function createDefaultFilters(branchId = defaultBranchId) {
  const today = getTodayInputValue()
  return {
    productSearch: '',
    customerSearch: '',
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
  const [sortBy, setSortBy] = useState(PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT)
  const [sortDirection, setSortDirection] = useState('DESC')
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [metricsData, setMetricsData] = useState(emptyMetricsData)
  const [metricsStatus, setMetricsStatus] = useState('idle')
  const [metricsError, setMetricsError] = useState('')
  const [metricsCacheKey, setMetricsCacheKey] = useState('')
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
  const appliedMetricsKey = useMemo(() => createMetricsCacheKey(appliedFilters), [appliedFilters])
  const metricsSummary = useMemo(
    () => createMetricsSummary(metricsData.rows, metricsData.summary, appliedFilters.analysisMode),
    [appliedFilters.analysisMode, metricsData.rows, metricsData.summary],
  )
  const metricsCharts = useMemo(
    () => createMetricsCharts(metricsData.rows, appliedFilters.analysisMode),
    [appliedFilters.analysisMode, metricsData.rows],
  )

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.productSearch,
    appliedFilters.customerSearch,
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
      setMetricsData(emptyMetricsData)
      setMetricsStatus('idle')
      setMetricsError('')
      setMetricsCacheKey('')
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
        setAvailabilityNotes(result.availabilityNotes || [])
        setStatus('ready')
      })
      .catch((apiError) => {
        if (apiError.name === 'AbortError' || requestId !== requestIdRef.current) {
          return
        }

        setRows([])
        setTotalRows(0)
        setStatus('error')
        setError(apiError.message || `${activeBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [activeBranch, appliedFilters, isAllBranches, pageNumber, pageSize, sortBy, sortDirection])

  useEffect(() => {
    if (!showMetrics || metricsCacheKey === appliedMetricsKey) {
      return undefined
    }

    const validationError = validateFilters(appliedFilters)
    if (validationError) {
      setMetricsData(emptyMetricsData)
      setMetricsStatus('error')
      setMetricsError(validationError)
      setMetricsCacheKey(appliedMetricsKey)
      return undefined
    }

    const controller = new AbortController()
    setMetricsStatus('loading')
    setMetricsError('')

    const reportRequest = createReportRequest(
      appliedFilters,
      1,
      EXPORT_FETCH_PAGE_SIZE,
      PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT,
      'DESC',
    )

    const request = isAllBranches
      ? fetchAllBranchesResult(reportRequest, controller.signal, true)
      : fetchAllRowsForSingleBranch(activeBranch, reportRequest, controller.signal)

    request
      .then((result) => {
        setMetricsData({
          rows: result.rows || [],
          summary: result.summary || aggregateRows(result.rows || []),
          dashboard: result.dashboard || createDashboard(result.rows || [], []),
          availabilityNotes: result.availabilityNotes || [],
        })
        setMetricsCacheKey(appliedMetricsKey)
        setMetricsStatus('ready')
      })
      .catch((apiError) => {
        if (apiError.name === 'AbortError') {
          return
        }

        setMetricsData(emptyMetricsData)
        setMetricsStatus('error')
        setMetricsError(apiError.message || 'Unable to load metrics dashboard.')
      })

    return () => controller.abort()
  }, [activeBranch, appliedFilters, appliedMetricsKey, isAllBranches, metricsCacheKey, showMetrics])

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
      customerSearch: draftFilters.analysisMode === ANALYSIS_MODES.CUSTOMER ? draftFilters.customerSearch.trim() : '',
    }

    setAppliedFilters(nextFilters)
    setShowMetrics(false)
    setMetricsCacheKey('')
    setPageNumber(1)
    if (!isSortAllowedForMode(sortBy, nextFilters.analysisMode)) {
      setSortBy(PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT)
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
    setSortBy(PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT)
    setSortDirection('DESC')
    setShowMetrics(false)
    setMetricsData(emptyMetricsData)
    setMetricsStatus('idle')
    setMetricsError('')
    setMetricsCacheKey('')
    setPageNumber(1)
    onBranchChange?.(defaults.branchId)
  }

  function changeBranch(branchId) {
    setRows([])
    setTotalRows(0)
    setAvailabilityNotes([])

    const nextFilters = {
      ...draftFilters,
      branchId,
    }

    setDraftFilters(nextFilters)
    setAppliedFilters(nextFilters)
    setShowMetrics(false)
    setMetricsData(emptyMetricsData)
    setMetricsStatus('idle')
    setMetricsError('')
    setMetricsCacheKey('')
    setSortBy(PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT)
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
      setSortDirection(normalizedSortBy === PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT ? 'DESC' : 'ASC')
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
        filename: `Product_Analysis_${appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER ? 'Customer_Wise' : 'Product_Wise'}_${getFileDateStamp()}.xlsx`,
        sheetName: 'Product Analysis',
        columns: createExportColumns(appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER),
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
          {draftFilters.analysisMode === ANALYSIS_MODES.CUSTOMER && (
            <CustomerSearch
              value={draftFilters.customerSearch}
              onChange={(value) => updateDraftFilter('customerSearch', value)}
              onClear={() => updateDraftFilter('customerSearch', '')}
            />
          )}
          <AnalysisModeSelector
            value={draftFilters.analysisMode}
            onChange={(value) => {
              setDraftFilters((current) => ({
                ...current,
                analysisMode: value,
                customerSearch: value === ANALYSIS_MODES.CUSTOMER ? current.customerSearch : '',
              }))
              setAppliedFilters((current) => ({
                ...current,
                analysisMode: value,
                customerSearch: value === ANALYSIS_MODES.CUSTOMER ? current.customerSearch : '',
              }))
              setShowMetrics(false)
              setMetricsData(emptyMetricsData)
              setMetricsStatus('idle')
              setMetricsError('')
              setMetricsCacheKey('')
              setSortBy(PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT)
              setSortDirection('DESC')
              setPageNumber(1)
            }}
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
                <p className="section-kicker">{appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER ? 'Customer Wise' : 'Product Wise'}</p>
                <h3>Completed Estimation Summary</h3>
              </div>
            </div>
            <ProductSummaryTable
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              sortBy={sortBy}
              sortDirection={sortDirection}
              onSort={changeSort}
              showBranch={false}
              showCustomer={appliedFilters.analysisMode === ANALYSIS_MODES.CUSTOMER}
            />
            {rows.length === 0 && status !== 'error' && <div className="empty-state">No completed estimation summary rows found for the selected criteria.</div>}
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
          {metricsStatus === 'loading' && <div className="dashboard-loading-state">Loading metrics...</div>}
          {metricsStatus === 'error' && <div className="dashboard-error-state">{metricsError || 'Unable to load metrics dashboard.'}</div>}
          {metricsStatus === 'ready' && (
            <>
              {metricsData.availabilityNotes.length > 0 && <div className="branch-status-message">{metricsData.availabilityNotes.join(' ')}</div>}
              <SummaryCards summary={metricsSummary} mode={appliedFilters.analysisMode} />
              <MetricsDashboard charts={metricsCharts} mode={appliedFilters.analysisMode} summary={metricsSummary} />
            </>
          )}
          {isAllBranches && metricsStatus === 'ready' && (
            <BranchComparison summaries={metricsData.dashboard.branchSummaries} />
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

function isSortAllowedForMode(sortBy, analysisMode) {
  const productSorts = new Set([
    PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME,
    PRODUCT_ANALYSIS_SORTS.BRAND,
    PRODUCT_ANALYSIS_SORTS.CATEGORY,
    PRODUCT_ANALYSIS_SORTS.TOTAL_QTY,
    PRODUCT_ANALYSIS_SORTS.PRICE,
    PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT,
  ])

  if (analysisMode !== ANALYSIS_MODES.CUSTOMER) {
    return productSorts.has(sortBy)
  }

  return productSorts.has(sortBy)
    || sortBy === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME
    || sortBy === PRODUCT_ANALYSIS_SORTS.CITY
}

function createReportRequest(filters, pageNumber, pageSize, sortBy, sortDirection) {
  return {
    pageNumber,
    pageSize,
    productSearch: filters.productSearch,
    customerSearch: filters.analysisMode === ANALYSIS_MODES.CUSTOMER ? filters.customerSearch : '',
    analysisMode: filters.analysisMode,
    fromDate: filters.fromDate,
    toDate: filters.toDate,
    sortBy,
    sortDirection,
  }
}

function createMetricsCacheKey(filters) {
  return [
    filters.branchId,
    filters.fromDate,
    filters.toDate,
    filters.productSearch,
    filters.customerSearch,
    filters.analysisMode,
  ].join('|')
}

async function fetchSingleBranchResult(activeBranch, reportRequest, signal) {
  const result = await fetchProductAnalysisReport(activeBranch, reportRequest, signal)

  const rows = result.rows.map((row) => withBranch(row, activeBranch))
  const summary = result.summary || emptyProductAnalysisSummary
  const dashboard = createDashboard([], [
    {
      branch: activeBranch,
      status: 'online',
      summary,
    },
  ])

  return {
    rows,
    totalRows: result.totalRows,
    summary,
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
    rows = rows.concat(result.rows.map((row) => withBranch(row, activeBranch)))
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

  let allRows = []
  successful.forEach(({ result }) => {
    allRows = allRows.concat(result.rows)
  })

  const merged = sortProductAnalysisRows(
    mergeBranchRows(allRows, reportRequest.analysisMode),
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
    dashboard: createDashboard(merged, branchSummaries.concat(offlineSummaries)),
    availabilityNotes,
    branches,
  }
}

function createDashboard(rows, branchSummaries) {
  const productRows = aggregateProducts(rows)

  return {
    topByAmount: productRows.slice().sort((a, b) => b.salesAmount - a.salesAmount || a.productName.localeCompare(b.productName)).slice(0, 10),
    topByQuantity: productRows.slice().sort((a, b) => b.salesCount - a.salesCount || a.productName.localeCompare(b.productName)).slice(0, 10),
    topByBillCount: productRows.slice().sort((a, b) => b.billCount - a.billCount || a.productName.localeCompare(b.productName)).slice(0, 10),
    branchSummaries,
  }
}

function aggregateRows(rows) {
  const productIds = new Set()
  const customerNames = new Set()
  let totalQuantity = 0
  let totalAmount = 0

  rows.forEach((row) => {
    productIds.add(String(row.productId || row.productName || '-'))
    if (row.customerName) {
      customerNames.add(String(row.customerName).trim().toUpperCase())
    }
    totalQuantity += row.totalQty || 0
    totalAmount += row.totalAmount || 0
  })

  return {
    customers: customerNames.size,
    products: productIds.size,
    totalQuantity,
    totalAmount,
  }
}

function mergeBranchRows(rows, analysisMode) {
  const grouped = new Map()

  rows.forEach((row) => {
    const productKey = String(row.productId || row.productName || '-').trim().toUpperCase()
    const customerKey = String(row.customerName || '').trim().toUpperCase()
    const key = analysisMode === ANALYSIS_MODES.CUSTOMER ? `${customerKey}|${productKey}` : productKey
    const current = grouped.get(key) || {
      ...row,
      totalQty: 0,
      totalAmount: 0,
    }

    current.totalQty += row.totalQty || 0
    current.totalAmount += row.totalAmount || 0
    current.price = current.totalQty !== 0 ? current.totalAmount / current.totalQty : 0
    grouped.set(key, current)
  })

  return Array.from(grouped.values()).map((row, index) => ({
    ...row,
    id: `${row.customerName || 'PRODUCT'}-${row.productId || row.productName}-${index}`,
  }))
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

    current.salesCount += row.totalQty || 0
    current.salesAmount += row.totalAmount || 0
    current.transactionIds.add(`${row.branchCode || ''}-${row.customerName || ''}-${row.productId || row.productName || '-'}`)
    map.set(key, current)
  })

  return Array.from(map.values()).map((row) => ({
    ...row,
    billCount: row.transactionIds.size,
  }))
}

function createMetricsSummary(rows, fallbackSummary, analysisMode) {
  const summary = aggregateRows(rows)
  const baseSummary = rows.length > 0 ? summary : fallbackSummary
  const productCount = baseSummary.products || 0

  return {
    ...baseSummary,
    averageProductValue: productCount > 0 ? (baseSummary.totalAmount || 0) / productCount : 0,
    mode: analysisMode,
  }
}

function createMetricsCharts(rows, analysisMode) {
  if (analysisMode === ANALYSIS_MODES.CUSTOMER) {
    return createCustomerMetricsCharts(rows)
  }

  return createProductMetricsCharts(rows)
}

function createProductMetricsCharts(rows) {
  const productRows = rows.map((row) => ({
    id: row.productId || row.productName,
    label: row.productName || '-',
    productName: row.productName || '-',
    totalQty: safeNumber(row.totalQty),
    totalAmount: safeNumber(row.totalAmount),
    price: safeNumber(row.price),
  }))

  return {
    valueTitle: 'Top 10 Products by Sales Value',
    quantityTitle: 'Top 10 Products by Quantity',
    valueRows: topRows(productRows, (row) => row.totalAmount, 10),
    quantityRows: topRows(productRows, (row) => row.totalQty, 10),
    brandRows: topContributionRows(aggregateBy(rows, (row) => row.brand || 'Unassigned', (row) => row.totalAmount), 10),
    categoryRows: topContributionRows(aggregateBy(rows, (row) => row.category || 'Unassigned', (row) => row.totalAmount), 10),
    extraTitle: 'Top 10 Products by Average Price',
    extraRows: topRows(productRows.filter((row) => row.price > 0), (row) => row.price, 10),
    extraValueLabel: 'Price',
  }
}

function createCustomerMetricsCharts(rows) {
  const customerRows = aggregateCustomerRows(rows)
  const productRows = aggregateProductSalesRows(rows)

  return {
    valueTitle: 'Top 10 Customers by Sales Value',
    quantityTitle: 'Top 10 Customers by Quantity',
    valueRows: topRows(customerRows, (row) => row.totalAmount, 10),
    quantityRows: topRows(customerRows, (row) => row.totalQty, 10),
    brandRows: topContributionRows(customerRows.map((row) => ({
      id: row.id,
      label: row.label,
      value: row.totalAmount,
      totalQty: row.totalQty,
      totalAmount: row.totalAmount,
    })), 10),
    categoryRows: topContributionRows(aggregateBy(rows, (row) => row.category || 'Unassigned', (row) => row.totalAmount), 10),
    extraTitle: 'Top 10 Products in Customer Sales',
    extraRows: topRows(productRows, (row) => row.totalAmount, 10),
    extraValueLabel: 'Total Amount',
  }
}

function aggregateCustomerRows(rows) {
  const map = new Map()

  rows.forEach((row) => {
    const label = String(row.customerName || 'Unassigned').trim() || 'Unassigned'
    const key = label.toUpperCase()
    const current = map.get(key) || {
      id: key,
      label,
      productName: label,
      customerName: label,
      totalQty: 0,
      totalAmount: 0,
    }

    current.totalQty += safeNumber(row.totalQty)
    current.totalAmount += safeNumber(row.totalAmount)
    map.set(key, current)
  })

  return Array.from(map.values())
}

function aggregateProductSalesRows(rows) {
  const map = new Map()

  rows.forEach((row) => {
    const key = String(row.productId || row.productName || '-')
    const current = map.get(key) || {
      id: key,
      productName: row.productName || '-',
      label: row.productName || '-',
      totalQty: 0,
      totalAmount: 0,
    }

    current.totalQty += safeNumber(row.totalQty)
    current.totalAmount += safeNumber(row.totalAmount)
    map.set(key, current)
  })

  return Array.from(map.values())
}

function aggregateBy(rows, getLabel, getValue) {
  const map = new Map()

  rows.forEach((row) => {
    const label = String(getLabel(row) || 'Unassigned').trim() || 'Unassigned'
    const key = label.toUpperCase()
    const current = map.get(key) || {
      id: key,
      label,
      value: 0,
      totalQty: 0,
      totalAmount: 0,
    }

    const amount = safeNumber(getValue(row))
    current.value += amount
    current.totalAmount += amount
    current.totalQty += safeNumber(row.totalQty)
    map.set(key, current)
  })

  return Array.from(map.values())
}

function topRows(rows, getValue, count) {
  return rows
    .filter((row) => Number.isFinite(getValue(row)))
    .slice()
    .sort((a, b) => getValue(b) - getValue(a) || String(a.label || a.productName).localeCompare(String(b.label || b.productName)))
    .slice(0, count)
}

function topContributionRows(rows, count) {
  const positiveRows = rows
    .filter((row) => safeNumber(row.value) > 0)
    .slice()
    .sort((a, b) => safeNumber(b.value) - safeNumber(a.value) || String(a.label).localeCompare(String(b.label)))

  const top = positiveRows.slice(0, count)
  const others = positiveRows.slice(count).reduce((sum, row) => sum + safeNumber(row.value), 0)

  if (others > 0) {
    return top.concat([{
      id: 'OTHERS',
      label: 'Others',
      value: others,
      totalAmount: others,
      totalQty: 0,
    }])
  }

  return top
}

function safeNumber(value) {
  const number = Number(value)
  return Number.isFinite(number) ? number : 0
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

function CustomerSearch({ value, onChange, onClear }) {
  return (
    <div className="field-group product-search-field customer-search-field">
      <label htmlFor="product-analysis-customer">Customer Search</label>
      <div className="product-search-box">
        <input id="product-analysis-customer" value={value} onChange={(event) => onChange(event.target.value)} placeholder="Customer name" />
        {value && <button type="button" onClick={onClear} aria-label="Clear customer search">x</button>}
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

function SummaryCards({ summary, mode }) {
  const items = [
    ...(mode === ANALYSIS_MODES.CUSTOMER ? [['Customers', formatStock(summary.customers)]] : []),
    ['Products', formatStock(summary.products)],
    ['Total Quantity', formatStock(summary.totalQuantity)],
    ['Total Amount', formatMoney(summary.totalAmount)],
    ...(mode === ANALYSIS_MODES.PRODUCT ? [['Average Product Value', formatMoney(summary.averageProductValue)]] : []),
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

function MetricsDashboard({ charts, mode }) {
  const contributionTitle = mode === ANALYSIS_MODES.CUSTOMER ? 'Customer Sales Contribution' : 'Sales by Brand'

  return (
    <div className="metrics-dashboard-layout">
      <div className="metrics-chart-row">
        <BarChartCard
          title={charts.valueTitle}
          rows={charts.valueRows}
          valueLabel="Total Amount"
          getValue={(row) => row.totalAmount}
          renderValue={formatMoney}
          tooltipKind={mode === ANALYSIS_MODES.CUSTOMER ? 'customer' : 'product'}
        />
        <BarChartCard
          title={charts.quantityTitle}
          rows={charts.quantityRows}
          valueLabel="Total Qty"
          getValue={(row) => row.totalQty}
          renderValue={formatStock}
          tooltipKind={mode === ANALYSIS_MODES.CUSTOMER ? 'customer' : 'product'}
        />
      </div>
      <div className="metrics-chart-row">
        <DoughnutChartCard
          title={contributionTitle}
          rows={charts.brandRows}
          note="Contribution chart shows positive sales values only."
        />
        <DoughnutChartCard
          title="Sales by Category"
          rows={charts.categoryRows}
          note="Contribution chart shows positive sales values only."
        />
      </div>
      <div className="metrics-chart-row metrics-chart-row-single">
        <BarChartCard
          title={charts.extraTitle}
          rows={charts.extraRows}
          valueLabel={charts.extraValueLabel}
          getValue={(row) => charts.extraValueLabel === 'Price' ? row.price : row.totalAmount}
          renderValue={formatMoney}
          tooltipKind="product"
        />
      </div>
    </div>
  )
}

function BarChartCard({ title, rows, valueLabel, getValue, renderValue, tooltipKind }) {
  const maxValue = rows.reduce((max, row) => Math.max(max, Math.abs(safeNumber(getValue(row)))), 0)

  return (
    <div className="dashboard-card metric-chart-card">
      <div className="dashboard-card-title">
        <h3>{title}</h3>
        <span>{valueLabel}</span>
      </div>
      {rows.length === 0 ? (
        <NoData />
      ) : (
        <div className="metric-bar-chart" role="list">
          {rows.map((row, index) => {
            const value = safeNumber(getValue(row))
            const width = maxValue > 0 ? Math.max(4, (Math.abs(value) / maxValue) * 100) : 0
            const label = row.label || row.productName || row.customerName || '-'

            return (
              <div
                className={value < 0 ? 'metric-bar-row is-negative' : 'metric-bar-row'}
                key={`${label}-${index}`}
                role="listitem"
                title={createChartTooltip(row, tooltipKind)}
              >
                <div className="metric-bar-label">
                  <span>{index + 1}</span>
                  <strong>{label}</strong>
                </div>
                <div className="metric-bar-track">
                  <i style={{ width: `${width}%` }} />
                </div>
                <div className="metric-bar-value">{renderValue(value)}</div>
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}

function DoughnutChartCard({ title, rows, note }) {
  const total = rows.reduce((sum, row) => sum + safeNumber(row.value), 0)
  const segments = createDoughnutSegments(rows, total)

  return (
    <div className="dashboard-card metric-chart-card">
      <div className="dashboard-card-title">
        <h3>{title}</h3>
        <span>Share</span>
      </div>
      {rows.length === 0 || total <= 0 ? (
        <NoData />
      ) : (
        <div className="metric-doughnut-layout">
          <div
            className="metric-doughnut"
            style={{ background: `conic-gradient(${segments.gradient})` }}
            aria-label={title}
          >
            <span>{formatMoney(total)}</span>
          </div>
          <div className="metric-doughnut-legend">
            {segments.items.map((item) => (
              <div className="metric-legend-row" key={item.id} title={`${item.label}\nAmount: ${formatMoney(item.value)}\nShare: ${formatPercent(item.percent)}`}>
                <i style={{ background: item.color }} />
                <span>{item.label}</span>
                <strong>{formatPercent(item.percent)}</strong>
              </div>
            ))}
          </div>
        </div>
      )}
      {note && rows.length > 0 && <p className="metric-chart-note">{note}</p>}
    </div>
  )
}

function createDoughnutSegments(rows, total) {
  const colors = ['#b91c1c', '#0f766e', '#b45309', '#2563eb', '#7c3aed', '#be123c', '#15803d', '#ca8a04', '#0891b2', '#9333ea', '#6b7280']
  let cursor = 0
  const items = rows.map((row, index) => {
    const value = safeNumber(row.value)
    const percent = total > 0 ? (value / total) * 100 : 0
    const start = cursor
    cursor += percent
    const color = colors[index % colors.length]

    return {
      id: row.id || row.label,
      label: row.label || '-',
      value,
      percent,
      color,
      gradientPart: `${color} ${start}% ${cursor}%`,
    }
  })

  return {
    items,
    gradient: items.map((item) => item.gradientPart).join(', '),
  }
}

function createChartTooltip(row, kind) {
  const nameLabel = kind === 'customer' ? 'Customer' : 'Product Name'
  const nameValue = row.customerName || row.productName || row.label || '-'

  return [
    `${nameLabel}: ${nameValue}`,
    `Total Qty: ${formatStock(row.totalQty)}`,
    `Total Amount: ${formatMoney(row.totalAmount)}`,
  ].join('\n')
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
              <dt>Customers</dt>
              <dd>{formatStock(summary.customers)}</dd>
              <dt>Products</dt>
              <dd>{formatStock(summary.products)}</dd>
              <dt>Qty</dt>
              <dd>{formatStock(summary.totalQuantity)}</dd>
              <dt>Total</dt>
              <dd>{formatMoney(summary.totalAmount)}</dd>
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

function ProductSummaryTable({ rows, pageNumber, pageSize, sortBy, sortDirection, onSort, showBranch, showCustomer }) {
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
            {showCustomer && <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-customer">Customer Name</SortHeader>}
            {showCustomer && <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CITY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-city">City</SortHeader>}
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-name">Product Name</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.BRAND} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-brand">Brand</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.CATEGORY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="col-product-analysis-category">Category</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TOTAL_QTY} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Total Qty</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.PRICE} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Price</SortHeader>
            <SortHeader sortKey={PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT} sortBy={sortBy} sortDirection={sortDirection} onSort={onSort} className="numeric metric-header">Total Amount</SortHeader>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id}>
              <td className="numeric">{((pageNumber - 1) * pageSize) + index + 1}</td>
              {showBranch && <td>{row.branchName}</td>}
              {showCustomer && <td>{row.customerName}</td>}
              {showCustomer && <td>{row.city}</td>}
              <td>{row.productName}</td>
              <td>{row.brand}</td>
              <td>{row.category}</td>
              <td className="numeric">{formatStock(row.totalQty)}</td>
              <td className="numeric">{formatMoney(row.price)}</td>
              <td className="numeric">{formatMoney(row.totalAmount)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function createExportColumns(includeCustomer) {
  return [
    { header: 'S.No', value: (row) => row.serialNumber },
    ...(includeCustomer ? [
      { header: 'CUSTOMER NAME', value: (row) => row.customerName },
      { header: 'CITY', value: (row) => row.city },
    ] : []),
    { header: 'PRODUCT NAME', value: (row) => row.productName },
    { header: 'BRAND', value: (row) => row.brand },
    { header: 'CATEGORY', value: (row) => row.category },
    { header: 'TOTAL QTY', value: (row) => row.totalQty },
    { header: 'PRICE', value: (row) => row.price },
    { header: 'TOTAL AMOUNT', value: (row) => row.totalAmount },
  ]
}

export default ProductAnalysisReportPage
