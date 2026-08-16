import { useEffect, useMemo, useRef, useState } from 'react'
import LoadingIndicator from '../components/LoadingIndicator'
import PageSizeSelector from '../components/PageSizeSelector'
import ReportPagination from '../components/ReportPagination'
import { branches, defaultBranchId, getBranchById } from '../config/branches'
import { downloadXlsx } from '../services/excelExport'
import {
  analysisTypes,
  emptyProductAnalysisSummary,
  fetchProductAnalysisDetail,
  fetchProductAnalysisReport,
  fetchProductLookup,
  formatGst,
  formatMoney,
  formatStock,
} from '../services/productAnalysisReport'
import {
  ALL_BRANCHES_ID,
  allBranchOption,
  DEFAULT_TRANSACTION_PAGE_SIZE,
  fetchAllRowsForBranch,
  fetchBranchPage,
  getFileDateStamp,
  getTodayInputValue,
  transactionPageSizes,
  withBranch,
} from '../services/reportUtils'

const PRODUCT_LOOKUP_DELAY_MS = 300

function createDefaultFilters(branchId = defaultBranchId) {
  const today = getTodayInputValue()
  return {
    analysisType: 'SALES',
    productId: '',
    productName: '',
    productSearch: '',
    fromDate: today,
    toDate: today,
    branchId,
  }
}

function ProductAnalysisReportPage({ selectedBranchId, onBranchChange }) {
  const [draftFilters, setDraftFilters] = useState(() => createDefaultFilters(selectedBranchId || defaultBranchId))
  const [appliedFilters, setAppliedFilters] = useState(() => createDefaultFilters(selectedBranchId || defaultBranchId))
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(DEFAULT_TRANSACTION_PAGE_SIZE)
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [summary, setSummary] = useState(emptyProductAnalysisSummary)
  const [status, setStatus] = useState('loading')
  const [error, setError] = useState('')
  const [availabilityNotes, setAvailabilityNotes] = useState([])
  const [lookupRows, setLookupRows] = useState([])
  const [lookupStatus, setLookupStatus] = useState('idle')
  const [isExporting, setIsExporting] = useState(false)
  const requestIdRef = useRef(0)
  const isAllBranches = appliedFilters.branchId === ALL_BRANCHES_ID
  const isSingleProduct = Boolean(appliedFilters.productId)
  const activeBranch = useMemo(
    () => (isAllBranches ? allBranchOption : getBranchById(appliedFilters.branchId)),
    [appliedFilters.branchId, isAllBranches],
  )
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.analysisType,
    appliedFilters.productId,
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

    const reportRequest = createReportRequest(appliedFilters, pageNumber, pageSize)
    const request = isSingleProduct
      ? fetchDetailResult(activeBranch, reportRequest, isAllBranches, controller.signal)
      : fetchSummaryResult(activeBranch, reportRequest, isAllBranches, controller.signal)

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
        setSummary(emptyProductAnalysisSummary)
        setStatus('error')
        setError(apiError.message || `${activeBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [activeBranch, appliedFilters, isAllBranches, isSingleProduct, pageNumber, pageSize])

  useEffect(() => {
    const searchText = draftFilters.productSearch.trim()
    if (searchText.length < 2 || draftFilters.productId) {
      setLookupRows([])
      setLookupStatus('idle')
      return undefined
    }

    const controller = new AbortController()
    const timerId = window.setTimeout(() => {
      setLookupStatus('loading')
      fetchProductLookup(getBranchById(draftFilters.branchId === ALL_BRANCHES_ID ? defaultBranchId : draftFilters.branchId), searchText, controller.signal)
        .then((products) => {
          setLookupRows(products)
          setLookupStatus('ready')
        })
        .catch((apiError) => {
          if (apiError.name !== 'AbortError') {
            setLookupRows([])
            setLookupStatus('error')
          }
        })
    }, PRODUCT_LOOKUP_DELAY_MS)

    return () => {
      window.clearTimeout(timerId)
      controller.abort()
    }
  }, [draftFilters.branchId, draftFilters.productId, draftFilters.productSearch])

  function updateDraftFilter(name, value) {
    setDraftFilters((current) => ({
      ...current,
      [name]: value,
      ...(name === 'productSearch' ? { productId: '', productName: '' } : {}),
    }))
  }

  function selectProduct(product) {
    setDraftFilters((current) => ({
      ...current,
      productId: product.productId,
      productName: product.productName,
      productSearch: product.productName,
    }))
    setLookupRows([])
  }

  function applyFilters() {
    setAppliedFilters(draftFilters)
    setPageNumber(1)
    if (draftFilters.branchId !== ALL_BRANCHES_ID) {
      onBranchChange?.(draftFilters.branchId)
    }
  }

  function clearFilters() {
    const defaults = createDefaultFilters()
    setDraftFilters(defaults)
    setAppliedFilters(defaults)
    setPageSize(DEFAULT_TRANSACTION_PAGE_SIZE)
    setPageNumber(1)
    onBranchChange?.(defaults.branchId)
  }

  function openProduct(row) {
    const nextFilters = {
      ...appliedFilters,
      productId: row.productId,
      productName: row.productName,
      productSearch: row.productName,
    }
    setDraftFilters(nextFilters)
    setAppliedFilters(nextFilters)
    setPageNumber(1)
  }

  async function exportReport() {
    const controller = new AbortController()
    setIsExporting(true)
    setError('')

    try {
      const request = createReportRequest(appliedFilters, 1, DEFAULT_TRANSACTION_PAGE_SIZE)
      const result = isSingleProduct
        ? await fetchDetailResult(activeBranch, request, isAllBranches, controller.signal, true)
        : await fetchSummaryResult(activeBranch, request, isAllBranches, controller.signal, true)

      downloadXlsx({
        filename: `Product_Analysis_${appliedFilters.analysisType}_${getFileDateStamp()}.xlsx`,
        sheetName: 'Product Analysis',
        columns: isSingleProduct ? detailExportColumns : summaryExportColumns,
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
    '--branch-accent': activeBranch.accent,
    '--branch-accent-soft': activeBranch.accentSoft,
    '--branch-accent-border': activeBranch.accentBorder,
  }

  return (
    <section className="report-page quotation-report-page product-analysis-page" style={branchStyle}>
      <div className="report-sticky-zone quotation-sticky-zone">
        <div className="report-title-row">
          <div>
            <p className="section-kicker">Product Analysis</p>
            <div className="report-heading-line">
              <h2>{isSingleProduct ? appliedFilters.productName : 'All Products'}</h2>
              <BranchDropdown value={draftFilters.branchId} availabilityNotes={availabilityNotes} onChange={(value) => updateDraftFilter('branchId', value)} />
            </div>
          </div>
          <button type="button" className="export-button" onClick={exportReport} disabled={isExporting}>
            {isExporting ? 'Exporting...' : 'Export Excel'}
          </button>
        </div>

        <div className="quotation-filter-bar product-analysis-filter-bar">
          <div className="field-group analysis-type-field">
            <label htmlFor="product-analysis-type">Analysis Type</label>
            <select id="product-analysis-type" value={draftFilters.analysisType} onChange={(event) => updateDraftFilter('analysisType', event.target.value)}>
              {analysisTypes.map((type) => <option key={type.id} value={type.id}>{type.label}</option>)}
            </select>
          </div>
          <ProductSearch
            value={draftFilters.productSearch}
            lookupRows={lookupRows}
            lookupStatus={lookupStatus}
            onChange={(value) => updateDraftFilter('productSearch', value)}
            onSelect={selectProduct}
            onClear={() => updateDraftFilter('productSearch', '')}
          />
          <div className="field-group date-from-field">
            <label htmlFor="product-analysis-from">From</label>
            <input id="product-analysis-from" type="date" value={draftFilters.fromDate} onChange={(event) => updateDraftFilter('fromDate', event.target.value)} />
          </div>
          <div className="field-group date-to-field">
            <label htmlFor="product-analysis-to">To</label>
            <input id="product-analysis-to" type="date" value={draftFilters.toDate} onChange={(event) => updateDraftFilter('toDate', event.target.value)} />
          </div>
          <div className="filter-actions">
            <button type="button" className="apply-button" onClick={applyFilters}>Apply</button>
            <button type="button" className="clear-button" onClick={clearFilters}>Clear</button>
          </div>
          <PageSizeSelector value={pageSize} onChange={setPageSize} pageSizes={transactionPageSizes} />
        </div>
      </div>

      <SummaryCards summary={summary} isSingleProduct={isSingleProduct} />

      {status === 'error' && <div className="alert-error">{error}</div>}
      {error && status !== 'error' && <div className="alert-error">{error}</div>}

      <div className={status === 'loading' ? 'report-surface quotation-surface is-loading' : 'report-surface quotation-surface'}>
        {status === 'loading' && rows.length === 0 ? (
          <LoadingIndicator label={`Loading ${activeBranch.label} product analysis`} />
        ) : (
          <>
            {isSingleProduct ? (
              <DetailTable rows={rows} analysisType={appliedFilters.analysisType} />
            ) : (
              <SummaryTable rows={rows} pageNumber={pageNumber} pageSize={pageSize} onSelect={openProduct} />
            )}
            {rows.length === 0 && status !== 'error' && <div className="empty-state">No product analysis found for this selection.</div>}
            {status === 'loading' && <LoadingIndicator label="Refreshing product analysis" overlay />}
          </>
        )}
      </div>

      <ReportPagination pageNumber={pageNumber} totalPages={totalPages} totalRows={totalRows} pageSize={pageSize} onPageChange={setPageNumber} />
    </section>
  )
}

function createReportRequest(filters, pageNumber, pageSize) {
  return {
    analysisType: filters.analysisType,
    productId: filters.productId,
    pageNumber,
    pageSize,
    searchText: '',
    fromDate: filters.fromDate,
    toDate: filters.toDate,
  }
}

async function fetchSummaryResult(activeBranch, reportRequest, isAllBranches, signal, exportAll = false) {
  if (!isAllBranches) {
    const result = exportAll
      ? await fetchAllRowsForBranch(fetchProductAnalysisReport, activeBranch, reportRequest, emptyProductAnalysisSummary, signal)
      : await fetchBranchPage(fetchProductAnalysisReport, activeBranch, reportRequest, signal)

    return {
      rows: result.rows.map((row) => withBranch(row, activeBranch)),
      totalRows: result.totalRows,
      summary: result.summary || emptyProductAnalysisSummary,
      availabilityNotes: [],
    }
  }

  return fetchAllBranchResult(fetchProductAnalysisReport, reportRequest, signal, exportAll, mergeSummaryRows)
}

async function fetchDetailResult(activeBranch, reportRequest, isAllBranches, signal, exportAll = false) {
  if (!isAllBranches) {
    const [summaryResult, result] = await Promise.all([
      fetchProductAnalysisReport(activeBranch, { ...reportRequest, pageNumber: 1, pageSize: 1 }, signal),
      exportAll
      ? await fetchAllRowsForBranch(fetchProductAnalysisDetail, activeBranch, reportRequest, emptyProductAnalysisSummary, signal)
      : await fetchBranchPage(fetchProductAnalysisDetail, activeBranch, reportRequest, signal),
    ])

    return {
      rows: result.rows.map((row) => withBranch(row, activeBranch)),
      totalRows: result.totalRows,
      summary: summarizeSingleProduct(summaryResult.rows),
      availabilityNotes: [],
    }
  }

  const [summaryResult, detailResult] = await Promise.all([
    fetchAllBranchResult(fetchProductAnalysisReport, reportRequest, signal, true, mergeSummaryRows),
    fetchAllBranchResult(fetchProductAnalysisDetail, reportRequest, signal, exportAll, (rows) => rows.sort(sortDetailRows)),
  ])

  return {
    ...detailResult,
    summary: summarizeSingleProduct(summaryResult.rows),
  }
}

async function fetchAllBranchResult(fetchReport, reportRequest, signal, exportAll, mergeRows) {
  const branchResults = await Promise.allSettled(
    branches.map(async (branch) => {
      const result = await fetchAllRowsForBranch(fetchReport, branch, reportRequest, emptyProductAnalysisSummary, signal)
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
      availabilityNotes.push(`${branches[index].label} unavailable`)
    }
  })

  if (successful.length === 0) {
    throw new Error('All branches are currently unavailable.')
  }

  const merged = mergeRows(successful.flatMap(({ branch, result }) => result.rows.map((row) => withBranch(row, branch))))
  const rows = exportAll ? merged : merged.slice((reportRequest.pageNumber - 1) * reportRequest.pageSize, reportRequest.pageNumber * reportRequest.pageSize)

  return {
    rows,
    totalRows: merged.length,
    summary: summarizeRows(merged),
    availabilityNotes,
  }
}

function mergeSummaryRows(rows) {
  const map = new Map()
  rows.forEach((row) => {
    const current = map.get(row.productId) || { ...row, transactionCount: 0, totalQuantity: 0, taxableAmount: 0, gstAmount: 0, totalValue: 0, weightedRateTotal: 0 }
    current.transactionCount += row.transactionCount
    current.totalQuantity += row.totalQuantity
    current.taxableAmount += row.taxableAmount
    current.gstAmount += row.gstAmount
    current.totalValue += row.totalValue
    current.minimumRate = Math.min(current.minimumRate || row.minimumRate, row.minimumRate)
    current.maximumRate = Math.max(current.maximumRate || row.maximumRate, row.maximumRate)
    current.weightedRateTotal += row.averageRate * row.totalQuantity
    current.averageRate = current.totalQuantity ? current.weightedRateTotal / current.totalQuantity : 0
    map.set(row.productId, current)
  })

  return Array.from(map.values()).sort((a, b) => b.totalValue - a.totalValue || a.productName.localeCompare(b.productName))
}

function summarizeRows(rows) {
  return {
    products: rows.length,
    transactions: rows.reduce((total, row) => total + (row.transactionCount || 1), 0),
    totalQuantity: rows.reduce((total, row) => total + row.totalQuantity, 0),
    totalValue: rows.reduce((total, row) => total + row.totalValue, 0),
  }
}

function summarizeSingleProduct(rows) {
  if (rows.length === 0) {
    return emptyProductAnalysisSummary
  }

  const row = mergeSummaryRows(rows)[0]
  return {
    products: 1,
    transactions: row.transactionCount,
    totalQuantity: row.totalQuantity,
    totalValue: row.totalValue,
    averageRate: row.averageRate,
    minimumRate: row.minimumRate,
    maximumRate: row.maximumRate,
  }
}

function sortDetailRows(a, b) {
  return new Date(b.transactionDate || 0).getTime() - new Date(a.transactionDate || 0).getTime()
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
      {availabilityNotes.length > 0 && <span className="branch-availability-note">{availabilityNotes.join(' / ')}</span>}
    </div>
  )
}

function ProductSearch({ value, lookupRows, lookupStatus, onChange, onSelect, onClear }) {
  return (
    <div className="field-group product-search-field">
      <label htmlFor="product-analysis-product">Product</label>
      <div className="product-search-box">
        <input id="product-analysis-product" value={value} onChange={(event) => onChange(event.target.value)} placeholder="All Products" />
        {value && <button type="button" onClick={onClear} aria-label="Clear product">x</button>}
        {lookupRows.length > 0 && (
          <div className="product-lookup-menu">
            {lookupRows.map((product) => (
              <button type="button" key={product.productId} onClick={() => onSelect(product)}>
                <span>{product.productName}</span>
                <small>{product.productId}</small>
              </button>
            ))}
          </div>
        )}
        {lookupStatus === 'loading' && <span className="product-lookup-hint">Searching...</span>}
      </div>
    </div>
  )
}

function SummaryCards({ summary, isSingleProduct }) {
  const items = isSingleProduct
    ? [
        ['Transactions', formatStock(summary.transactions)],
        ['Total Qty', formatStock(summary.totalQuantity)],
        ['Avg Rate', formatMoney(summary.averageRate)],
        ['Min Rate', formatMoney(summary.minimumRate)],
        ['Max Rate', formatMoney(summary.maximumRate)],
        ['Total Value', formatMoney(summary.totalValue)],
      ]
    : [
        ['Products', formatStock(summary.products)],
        ['Transactions', formatStock(summary.transactions)],
        ['Total Quantity', formatStock(summary.totalQuantity)],
        ['Total Value', formatMoney(summary.totalValue)],
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

function SummaryTable({ rows, pageNumber, pageSize, onSelect }) {
  if (rows.length === 0) return null

  return (
    <div className="table-wrap quotation-table-wrap">
      <table className="stock-table quotation-table product-analysis-table">
        <thead>
          <tr>
            <th className="numeric">S.No</th>
            <th>Product</th>
            <th className="numeric">Transactions</th>
            <th className="numeric">Qty</th>
            <th className="numeric">Avg Rate</th>
            <th className="numeric">Min Rate</th>
            <th className="numeric">Max Rate</th>
            <th className="numeric">Taxable</th>
            <th className="numeric">GST</th>
            <th className="numeric">Value</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id} onClick={() => onSelect(row)}>
              <td className="numeric">{((pageNumber - 1) * pageSize) + index + 1}</td>
              <td><button type="button" className="link-button" onClick={() => onSelect(row)}>{row.productName}</button></td>
              <td className="numeric">{formatStock(row.transactionCount)}</td>
              <td className="numeric">{formatStock(row.totalQuantity)}</td>
              <td className="numeric">{formatMoney(row.averageRate)}</td>
              <td className="numeric">{formatMoney(row.minimumRate)}</td>
              <td className="numeric">{formatMoney(row.maximumRate)}</td>
              <td className="numeric">{formatMoney(row.taxableAmount)}</td>
              <td className="numeric">{formatMoney(row.gstAmount)}</td>
              <td className="numeric">{formatMoney(row.totalValue)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function DetailTable({ rows, analysisType }) {
  if (rows.length === 0) return null
  const showDivision = analysisType === 'SALES'

  return (
    <div className="table-wrap quotation-table-wrap">
      <table className="stock-table quotation-table product-analysis-detail-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Document No</th>
            <th>Customer</th>
            {showDivision && <th>Division</th>}
            <th className="numeric">Qty</th>
            <th className="numeric">Rate</th>
            <th className="numeric">GST %</th>
            <th className="numeric">Taxable</th>
            <th className="numeric">GST</th>
            <th className="numeric">Amount</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id}>
              <td>{row.transactionDateLabel}</td>
              <td>{row.documentNo}</td>
              <td>{row.customerName}</td>
              {showDivision && <td><span className="quotation-row-branch">{row.division}</span></td>}
              <td className="numeric">{formatStock(row.quantity)}</td>
              <td className="numeric">{formatMoney(row.rate)}</td>
              <td className="numeric">{formatGst(row.gstPercent)}</td>
              <td className="numeric">{formatMoney(row.taxableAmount)}</td>
              <td className="numeric">{formatMoney(row.gstAmount)}</td>
              <td className="numeric">{formatMoney(row.amount)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

const summaryExportColumns = [
  { header: 'S.No', value: (row) => row.serialNumber },
  { header: 'Branch', value: (row) => row.branchName },
  { header: 'ProductId', value: (row) => row.productId },
  { header: 'Product', value: (row) => row.productName },
  { header: 'Transactions', value: (row) => row.transactionCount },
  { header: 'Qty', value: (row) => row.totalQuantity },
  { header: 'Avg Rate', value: (row) => row.averageRate },
  { header: 'Min Rate', value: (row) => row.minimumRate },
  { header: 'Max Rate', value: (row) => row.maximumRate },
  { header: 'Taxable', value: (row) => row.taxableAmount },
  { header: 'GST', value: (row) => row.gstAmount },
  { header: 'Value', value: (row) => row.totalValue },
]

const detailExportColumns = [
  { header: 'S.No', value: (row) => row.serialNumber },
  { header: 'Branch', value: (row) => row.branchName },
  { header: 'Date', value: (row) => row.transactionDateLabel },
  { header: 'Document No', value: (row) => row.documentNo },
  { header: 'Customer', value: (row) => row.customerName },
  { header: 'Division', value: (row) => row.division },
  { header: 'Qty', value: (row) => row.quantity },
  { header: 'Rate', value: (row) => row.rate },
  { header: 'GST %', value: (row) => row.gstPercent },
  { header: 'Taxable', value: (row) => row.taxableAmount },
  { header: 'GST', value: (row) => row.gstAmount },
  { header: 'Amount', value: (row) => row.amount },
]

export default ProductAnalysisReportPage
