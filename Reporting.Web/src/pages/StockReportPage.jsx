import { useEffect, useMemo, useRef, useState } from 'react'
import PageSizeSelector from '../components/PageSizeSelector'
import ReportPagination from '../components/ReportPagination'
import ReportSearch from '../components/ReportSearch'
import LoadingIndicator from '../components/LoadingIndicator'
import { branches, getBranchById } from '../config/branches'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import { fetchStockReport, formatGst, formatMoney, formatStock } from '../services/stockReport'

function getTodayLabel() {
  return new Intl.DateTimeFormat('en-IN', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  }).format(new Date())
}

const emptyFilters = {
  searchText: '',
  stockOperator: '',
  stockValue: '',
  gstValue: '',
}

const filterRequiredMessage = 'Enter a product name to view the stock report. Stock quantity is optional.'
const gstOptions = ['5.00', '12', '14.50', '28', '0', '18']

function StockReportPage({ selectedBranchId, onBranchChange }) {
  const [draftFilters, setDraftFilters] = useState(emptyFilters)
  const [appliedFilters, setAppliedFilters] = useState(emptyFilters)
  const [draftReportScope, setDraftReportScope] = useState(selectedBranchId)
  const [appliedReportScope, setAppliedReportScope] = useState(selectedBranchId)
  const [filterRunId, setFilterRunId] = useState(0)
  const [pageSize, setPageSize] = useState(10)
  const [pageNumber, setPageNumber] = useState(1)
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [status, setStatus] = useState('idle')
  const [error, setError] = useState('')
  const [loadingSteps, setLoadingSteps] = useState([])
  const requestIdRef = useRef(0)
  const debouncedSearchText = useDebouncedValue(appliedFilters.searchText, 400)
  const isCompareMode = appliedReportScope === 'compare'
  const selectedBranch = useMemo(
    () => getBranchById(isCompareMode ? selectedBranchId : appliedReportScope),
    [appliedReportScope, isCompareMode, selectedBranchId],
  )
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))
  const hasAppliedFilter = debouncedSearchText.trim() !== ''

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.stockOperator,
    appliedFilters.stockValue,
    appliedFilters.gstValue,
    debouncedSearchText,
    pageSize,
    appliedReportScope,
  ])

  useEffect(() => {
    if (!hasAppliedFilter) {
      requestIdRef.current += 1
      setRows([])
      setTotalRows(0)
      setStatus('idle')
      setError('')
      setLoadingSteps([])
      return undefined
    }

    const controller = new AbortController()
    const requestId = requestIdRef.current + 1
    requestIdRef.current = requestId

    setStatus('loading')
    setError('')
    setLoadingSteps([])

    const reportRequest = {
      pageNumber,
      pageSize,
      searchText: debouncedSearchText.trim(),
      stockOperator: appliedFilters.stockOperator || null,
      stockValue: appliedFilters.stockValue === '' ? null : Number(appliedFilters.stockValue),
      gstValue: appliedFilters.gstValue === '' ? null : Number(appliedFilters.gstValue),
    }

    const updateLoadingSteps = (steps) => {
      if (requestId === requestIdRef.current && !controller.signal.aborted) {
        setLoadingSteps(steps)
      }
    }

    const request = isCompareMode
      ? fetchCompareStockReport(reportRequest, controller.signal, updateLoadingSteps)
      : fetchBranchStockReport(selectedBranch, reportRequest, controller.signal, updateLoadingSteps)

    request
      .then((result) => {
        if (requestId !== requestIdRef.current) {
          return
        }

        setRows(result.rows)
        setTotalRows(result.totalRows)
        setStatus('ready')
        setLoadingSteps([])
      })
      .catch((apiError) => {
        if (apiError.name === 'AbortError' || requestId !== requestIdRef.current) {
          return
        }

        setRows([])
        setTotalRows(0)
        setStatus('error')
        setLoadingSteps([])
        setError(apiError.message || `${selectedBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [
    appliedFilters.stockOperator,
    appliedFilters.stockValue,
    appliedFilters.gstValue,
    debouncedSearchText,
    filterRunId,
    hasAppliedFilter,
    isCompareMode,
    pageNumber,
    pageSize,
    selectedBranch,
  ])

  function updateDraftFilter(name, value) {
    setDraftFilters((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function applyFilters() {
    if (draftFilters.searchText.trim() === '') {
      setAppliedFilters(emptyFilters)
      setAppliedReportScope(draftReportScope)
      if (draftReportScope !== 'compare') {
        onBranchChange?.(draftReportScope)
      }
      setRows([])
      setTotalRows(0)
      setStatus('idle')
      setError('')
      setLoadingSteps([])
      setPageNumber(1)
      setFilterRunId((current) => current + 1)
      return
    }

    setAppliedFilters({
      searchText: draftFilters.searchText,
      stockOperator: draftFilters.stockOperator,
      stockValue: draftFilters.stockValue,
      gstValue: draftFilters.gstValue,
    })
    setAppliedReportScope(draftReportScope)
    if (draftReportScope !== 'compare') {
      onBranchChange?.(draftReportScope)
    }
    setPageNumber(1)
    setFilterRunId((current) => current + 1)
  }

  function clearFilters() {
    setDraftFilters(emptyFilters)
    setAppliedFilters(emptyFilters)
    setDraftReportScope(selectedBranchId)
    setAppliedReportScope(selectedBranchId)
    setLoadingSteps([])
    setPageNumber(1)
    setFilterRunId((current) => current + 1)
  }

  const branchStyle = {
    '--branch-accent': selectedBranch.accent,
    '--branch-accent-soft': selectedBranch.accentSoft,
    '--branch-accent-border': selectedBranch.accentBorder,
  }

  const reportTitle = isCompareMode ? 'Compare Stock in All Branches' : 'Stock Report'
  const loadingLabel = isCompareMode ? 'Preparing branch comparison' : `Loading ${selectedBranch.label} stock`
  return (
    <section className="report-page" style={branchStyle}>
      <div className="report-sticky-zone">
        <div className="report-title-row">
          <div>
            <p className="section-kicker">Stock Report</p>
            <div className="report-heading-line">
              <h2>{reportTitle}</h2>
              {!isCompareMode && <strong className="selected-branch-title">{selectedBranch.label}</strong>}
            </div>
          </div>
          <div className="report-meta">
            <span className="branch-badge">{isCompareMode ? 'All Branches' : selectedBranch.id}</span>
            <span>As On: {getTodayLabel()}</span>
          </div>
        </div>

        <div className="filter-bar">
          <ReportSearch
            value={draftFilters.searchText}
            onChange={(value) => updateDraftFilter('searchText', value)}
            placeholder="Product name required"
          />
          <div className="field-group operator-field">
            <label htmlFor="stock-operator">Stock Operator</label>
            <select
              id="stock-operator"
              value={draftFilters.stockOperator}
              onChange={(event) => updateDraftFilter('stockOperator', event.target.value)}
            >
              <option value="">Any</option>
              <option value="EQ">Equal To</option>
              <option value="LT">Less Than</option>
              <option value="GT">More Than</option>
            </select>
          </div>
          <div className="field-group quantity-field">
            <label htmlFor="stock-quantity">Stock Quantity</label>
            <input
              id="stock-quantity"
              type="number"
              step="0.001"
              inputMode="decimal"
              value={draftFilters.stockValue}
              onChange={(event) => updateDraftFilter('stockValue', event.target.value)}
            />
          </div>
          <div className="field-group gst-field">
            <label htmlFor="gst-filter">GST %</label>
            <select
              id="gst-filter"
              value={draftFilters.gstValue}
              onChange={(event) => updateDraftFilter('gstValue', event.target.value)}
            >
              <option value="">Any</option>
              {gstOptions.map((gst) => (
                <option key={gst} value={gst}>
                  {gst}
                </option>
              ))}
            </select>
          </div>
          <div className="field-group report-mode-field">
            <label htmlFor="report-mode">Branch / Compare</label>
            <select
              id="report-mode"
              value={draftReportScope}
              onChange={(event) => setDraftReportScope(event.target.value)}
            >
              {branches.map((branch) => (
                <option key={branch.id} value={branch.id}>
                  {branch.label}
                </option>
              ))}
              <option value="compare">Compare All</option>
            </select>
          </div>
          <div className="filter-actions">
            <button type="button" className="apply-button" onClick={applyFilters}>
              Apply
            </button>
            <button type="button" className="clear-button" onClick={clearFilters}>
              Clear
            </button>
          </div>
          <PageSizeSelector value={pageSize} onChange={setPageSize} />
        </div>
      </div>

      {status === 'error' && <div className="alert-error">{error}</div>}

      <div className={status === 'loading' ? 'report-surface is-loading' : 'report-surface'}>
        {status === 'idle' ? (
          <FilterRequiredAlert />
        ) : status === 'loading' && rows.length === 0 ? (
          <LoadingIndicator label={loadingLabel} steps={loadingSteps} />
        ) : (
          <>
            <StockTable rows={rows} isCompareMode={isCompareMode} />
            <StockMobileRows rows={rows} isCompareMode={isCompareMode} />
            {rows.length === 0 && status !== 'error' && (
              <div className="empty-state">No stock rows found for this selection.</div>
            )}
            {status === 'loading' && <LoadingIndicator label={loadingLabel} steps={loadingSteps} overlay />}
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
    </section>
  )
}

function waitForProgress(ms, signal) {
  return new Promise((resolve, reject) => {
    if (signal.aborted) {
      reject(new DOMException('Request aborted', 'AbortError'))
      return
    }

    const timeoutId = window.setTimeout(resolve, ms)
    signal.addEventListener(
      'abort',
      () => {
        window.clearTimeout(timeoutId)
        reject(new DOMException('Request aborted', 'AbortError'))
      },
      { once: true },
    )
  })
}

function createBranchLoadingSteps(branch, checkingStatus, fetchStatus, prepareStatus = 'pending') {
  return [
    {
      id: `${branch.id}-check`,
      label: `Checking ${branch.label} server connectivity`,
      status: checkingStatus,
    },
    {
      id: `${branch.id}-fetch`,
      label: `Fetching ${branch.label} stock data`,
      status: fetchStatus,
    },
    {
      id: `${branch.id}-prepare`,
      label: 'Preparing report rows',
      status: prepareStatus,
    },
  ]
}

function createCompareLoadingSteps(branchStates, prepareStatus = 'pending') {
  return [
    ...branches.flatMap((branch) => {
      const state = branchStates[branch.id] || {}
      return [
        {
          id: `${branch.id}-check`,
          label: `Checking ${branch.label} server connectivity`,
          status: state.check || 'pending',
        },
        {
          id: `${branch.id}-fetch`,
          label: `Fetching ${branch.label} stock data`,
          status: state.fetch || 'pending',
        },
      ]
    }),
    {
      id: 'compare-prepare',
      label: 'Combining branch stock columns',
      status: prepareStatus,
    },
  ]
}

async function fetchBranchStockReport(branch, reportRequest, signal, onProgress) {
  onProgress(createBranchLoadingSteps(branch, 'checking', 'pending'))
  await waitForProgress(220, signal)
  onProgress(createBranchLoadingSteps(branch, 'passed', 'active'))

  try {
    const result = await fetchStockReport(branch, reportRequest, signal)
    onProgress(createBranchLoadingSteps(branch, 'passed', 'passed', 'active'))
    await waitForProgress(220, signal)
    onProgress(createBranchLoadingSteps(branch, 'passed', 'passed', 'passed'))
    await waitForProgress(120, signal)
    return result
  } catch (error) {
    if (error.name === 'AbortError') {
      throw error
    }

    onProgress(createBranchLoadingSteps(branch, 'offline', 'offline'))
    await waitForProgress(220, signal)
    throw error
  }
}

async function fetchCompareStockReport(reportRequest, signal, onProgress) {
  const branchStates = Object.fromEntries(
    branches.map((branch) => [branch.id, { check: 'pending', fetch: 'pending' }]),
  )

  onProgress(createCompareLoadingSteps(branchStates))

  const updateBranchState = (branch, nextState) => {
    branchStates[branch.id] = {
      ...branchStates[branch.id],
      ...nextState,
    }
    onProgress(createCompareLoadingSteps(branchStates))
  }

  const results = await Promise.allSettled(
    branches.map(async (branch) => {
      updateBranchState(branch, { check: 'checking' })
      await waitForProgress(180, signal)
      updateBranchState(branch, { check: 'passed', fetch: 'active' })

      try {
        const result = await fetchStockReport(branch, reportRequest, signal)
        updateBranchState(branch, { check: 'passed', fetch: 'passed' })
        return result
      } catch (error) {
        if (error.name === 'AbortError') {
          throw error
        }

        updateBranchState(branch, { check: 'offline', fetch: 'offline' })
        throw error
      }
    }),
  )

  if (signal.aborted) {
    throw new DOMException('Request aborted', 'AbortError')
  }

  const branchResults = results.map((result, index) => ({
    branch: branches[index],
    result: result.status === 'fulfilled' ? result.value : null,
    isOffline: result.status === 'rejected',
  }))

  onProgress(createCompareLoadingSteps(branchStates, 'active'))
  await waitForProgress(220, signal)
  onProgress(createCompareLoadingSteps(branchStates, 'passed'))
  await waitForProgress(120, signal)

  return {
    rows: mergeCompareRows(branchResults),
    totalRows: Math.max(...branchResults.map(({ result }) => result?.totalRows || 0), 0),
  }
}

function mergeCompareRows(results) {
  const rowMap = new Map()

  const offlineBranchIds = results.filter(({ isOffline }) => isOffline).map(({ branch }) => branch.id)

  results.forEach(({ branch, result }) => {
    if (!result) {
      return
    }

    result.rows.forEach((row) => {
      const key = [
        row.productName,
        row.displayName,
        row.uom,
        row.hsn,
      ].join('|').toLowerCase()

      const compareRow = rowMap.get(key) || {
        ...row,
        branchStocks: {},
        offlineBranchIds,
      }

      compareRow.branchStocks[branch.id] = row.availableStock
      compareRow.offlineBranchIds = offlineBranchIds
      rowMap.set(key, compareRow)
    })
  })

  if (rowMap.size > 0) {
    return Array.from(rowMap.values())
  }

  return [
    {
      id: 'compare-branches-offline',
      productName: '-',
      displayName: '-',
      uom: '-',
      hsn: '-',
      branchStocks: {},
      offlineBranchIds,
    },
  ]
}

function FilterRequiredAlert() {
  return (
    <div className="filter-required-state" role="alert" aria-live="polite">
      <div className="filter-required-icon" aria-hidden="true">
        !
      </div>
      <div>
        <h3>Search required</h3>
        <p>{filterRequiredMessage}</p>
      </div>
    </div>
  )
}

function StockTable({ rows, isCompareMode }) {
  if (rows.length === 0) {
    return null
  }

  if (isCompareMode) {
    return (
      <div className="table-wrap">
        <table className="stock-table compare-stock-table">
          <colgroup>
            <col className="col-product" />
            <col className="col-display" />
            <col className="col-uom" />
            <col className="col-hsn" />
            <col className="col-gst" />
            <col className="col-min" />
            {branches.map((branch) => (
              <col className="col-branch-stock" key={branch.id} />
            ))}
          </colgroup>
          <thead>
            <tr>
              <th>Product Name</th>
              <th>Display Name</th>
              <th>UOM</th>
              <th>HSN</th>
              <th>GST %</th>
              <th className="numeric">Min Stock</th>
              {branches.map((branch) => (
                <th className="numeric" key={branch.id}>{branch.label} Stock</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <tr key={row.id}>
                <td>
                  <div className="product-cell">
                    <span>{row.productName}</span>
                  </div>
                </td>
                <td>{row.displayName}</td>
                <td>{row.uom}</td>
                <td>{row.hsn}</td>
                <td>{formatGst(row.gst)}</td>
                <td className="numeric">
                  <MinStockValue row={row} />
                </td>
                {branches.map((branch) => (
                  <td className={getCompareStockClassName(row, branch)} key={branch.id}>
                    {formatCompareStock(row, branch)}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    )
  }

  return (
    <div className="table-wrap">
      <table className="stock-table">
        <colgroup>
          <col className="col-product" />
          <col className="col-display" />
          <col className="col-uom" />
          <col className="col-hsn" />
          <col className="col-sale" />
          <col className="col-mrp" />
          <col className="col-gst" />
          <col className="col-min" />
          <col className="col-stock" />
        </colgroup>
        <thead>
          <tr>
            <th>Product Name</th>
            <th>Display Name</th>
            <th>UOM</th>
            <th>HSN</th>
            <th className="numeric">Sale Price</th>
            <th className="numeric">MRP</th>
            <th>GST %</th>
            <th className="numeric">Min Stock</th>
            <th className="numeric">Available Stock</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id} className={row.isLowStock ? 'is-low-stock' : undefined}>
              <td>
                <div className="product-cell">
                  <span>{row.productName}</span>
                </div>
              </td>
              <td>{row.displayName}</td>
              <td>{row.uom}</td>
              <td>{row.hsn}</td>
              <td className="numeric">{formatMoney(row.salePrice)}</td>
              <td className="numeric">{formatMoney(row.mrp)}</td>
              <td>{formatGst(row.gst)}</td>
              <td className="numeric">
                <MinStockValue row={row} />
              </td>
              <td className={getStockClassName(row.availableStock)}>
                {formatStock(row.availableStock)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function StockMobileRows({ rows, isCompareMode }) {
  if (rows.length === 0) {
    return null
  }

  return (
    <div className="mobile-stock-list">
      {rows.map((row) => (
        <article className={row.isLowStock ? 'mobile-stock-row is-low-stock' : 'mobile-stock-row'} key={row.id}>
          <div>
            <div className="mobile-product-line">
              <h3>{row.productName}</h3>
            </div>
            <p>{row.displayName}</p>
          </div>
          {isCompareMode ? (
            <div className="mobile-compare-stock">
              {branches.map((branch) => (
                <span key={branch.id}>
                  {branch.label}: <strong className={getMobileCompareStockClassName(row, branch)}>
                    {formatCompareStock(row, branch)}
                  </strong>
                </span>
              ))}
            </div>
          ) : (
            <div className="mobile-stock-metrics">
              <strong className={getMobileStockClassName(row.availableStock)}>
                {formatStock(row.availableStock)}
              </strong>
            </div>
          )}
          <div className="mobile-stock-details">
            <span>UOM: {row.uom}</span>
            <span>HSN: {row.hsn}</span>
            <span>GST: {formatGst(row.gst)}</span>
            <span className="mobile-min-stock">Min: <MinStockValue row={row} /></span>
          </div>
          {!isCompareMode && (
            <div className="mobile-price-line">
              <span>Sale {formatMoney(row.salePrice)}</span>
              <span>MRP {formatMoney(row.mrp)}</span>
            </div>
          )}
        </article>
      ))}
    </div>
  )
}

function MinStockValue({ row }) {
  if (!row.isLowStock) {
    return formatStock(row.minStock)
  }

  return <span className="min-stock-badge" aria-label={`Low stock. Minimum stock ${formatStock(row.minStock)}`}>↓ {formatStock(row.minStock)}</span>
}

function getStockClassName(value) {
  if (value < 0) {
    return 'numeric stock-value is-negative'
  }

  if (value === 0) {
    return 'numeric stock-value is-zero'
  }

  return 'numeric stock-value'
}

function getCompareStockClassName(row, branch) {
  if (row.offlineBranchIds?.includes(branch.id)) {
    return 'numeric stock-value is-offline'
  }

  return getStockClassName(row.branchStocks?.[branch.id])
}

function getMobileCompareStockClassName(row, branch) {
  if (row.offlineBranchIds?.includes(branch.id)) {
    return 'is-offline'
  }

  return getMobileStockClassName(row.branchStocks?.[branch.id])
}

function formatCompareStock(row, branch) {
  if (row.offlineBranchIds?.includes(branch.id)) {
    return 'Branch offline'
  }

  return formatStock(row.branchStocks?.[branch.id])
}

function getMobileStockClassName(value) {
  if (value < 0) {
    return 'is-negative'
  }

  if (value === 0) {
    return 'is-zero'
  }

  return undefined
}

export default StockReportPage
