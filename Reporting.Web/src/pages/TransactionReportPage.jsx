import { useEffect, useMemo, useRef, useState } from 'react'
import LoadingIndicator from '../components/LoadingIndicator'
import PageSizeSelector from '../components/PageSizeSelector'
import ReportPagination from '../components/ReportPagination'
import ReportSearch from '../components/ReportSearch'
import { branches, defaultBranchId, getBranchById } from '../config/branches'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import { downloadXlsx } from '../services/excelExport'
import { formatGst, formatMoney, formatStock } from '../services/stockReport'
import {
  ALL_BRANCHES_ID,
  allBranchOption,
  DEFAULT_TRANSACTION_PAGE_SIZE,
  fetchAllRowsForBranch,
  fetchBranchPage,
  getFileDateStamp,
  getTodayInputValue,
  sortByUpdatedOnThenId,
  transactionPageSizes,
  withBranch,
} from '../services/reportUtils'

function createDefaultFilters(extraFilters = {}) {
  const today = getTodayInputValue()

  return {
    searchText: '',
    fromDate: today,
    toDate: today,
    branchId: defaultBranchId,
    ...extraFilters,
  }
}

function TransactionReportPage({ config, selectedBranchId, onBranchChange }) {
  const [draftFilters, setDraftFilters] = useState(() => ({
    ...createDefaultFilters(config.defaultFilters),
    branchId: selectedBranchId || defaultBranchId,
  }))
  const [appliedFilters, setAppliedFilters] = useState(() => ({
    ...createDefaultFilters(config.defaultFilters),
    branchId: selectedBranchId || defaultBranchId,
  }))
  const [pageSize, setPageSize] = useState(DEFAULT_TRANSACTION_PAGE_SIZE)
  const [pageNumber, setPageNumber] = useState(1)
  const [rows, setRows] = useState([])
  const [totalRows, setTotalRows] = useState(0)
  const [summary, setSummary] = useState(config.emptySummary)
  const [status, setStatus] = useState('loading')
  const [error, setError] = useState('')
  const [availabilityNotes, setAvailabilityNotes] = useState([])
  const [selectedRow, setSelectedRow] = useState(null)
  const [detailState, setDetailState] = useState({ status: 'idle', error: '', data: null })
  const [isExporting, setIsExporting] = useState(false)
  const requestIdRef = useRef(0)
  const debouncedSearchText = useDebouncedValue(appliedFilters.searchText, 350)
  const isAllBranches = appliedFilters.branchId === ALL_BRANCHES_ID
  const activeBranch = useMemo(
    () => (isAllBranches ? allBranchOption : getBranchById(appliedFilters.branchId)),
    [appliedFilters.branchId, isAllBranches],
  )
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))

  useEffect(() => {
    setPageNumber(1)
  }, [
    appliedFilters.fromDate,
    appliedFilters.toDate,
    appliedFilters.branchId,
    appliedFilters.divisionCode,
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

    const reportRequest = createReportRequest(appliedFilters, debouncedSearchText, pageNumber, pageSize)
    const request = isAllBranches
      ? fetchAllBranchReport(config, reportRequest, controller.signal)
      : fetchSingleBranchReport(config, activeBranch, reportRequest, controller.signal)

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
        setSummary(config.emptySummary)
        setStatus('error')
        setError(apiError.message || `${activeBranch.label} branch is currently offline or unavailable.`)
      })

    return () => controller.abort()
  }, [
    activeBranch,
    appliedFilters,
    config,
    debouncedSearchText,
    isAllBranches,
    pageNumber,
    pageSize,
  ])

  useEffect(() => {
    if (!selectedRow) {
      setDetailState({ status: 'idle', error: '', data: null })
      return undefined
    }

    const controller = new AbortController()
    setDetailState({ status: 'loading', error: '', data: null })

    config.fetchDetail(selectedRow.branch || activeBranch, selectedRow, controller.signal)
      .then((data) => setDetailState({ status: 'ready', error: '', data }))
      .catch((apiError) => {
        if (apiError.name === 'AbortError') {
          return
        }

        setDetailState({
          status: 'error',
          error: apiError.message || `Unable to load ${config.entityLabel.toLowerCase()} detail.`,
          data: null,
        })
      })

    return () => controller.abort()
  }, [activeBranch, config, selectedRow])

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
    const defaultFilters = createDefaultFilters(config.defaultFilters)
    setDraftFilters(defaultFilters)
    setAppliedFilters(defaultFilters)
    setPageSize(DEFAULT_TRANSACTION_PAGE_SIZE)
    setPageNumber(1)
    onBranchChange?.(defaultFilters.branchId)
  }

  async function exportReport() {
    const controller = new AbortController()
    setIsExporting(true)
    setError('')

    try {
      const reportRequest = createReportRequest(appliedFilters, debouncedSearchText, 1, DEFAULT_TRANSACTION_PAGE_SIZE)
      const result = isAllBranches
        ? await fetchAllBranchReport(config, reportRequest, controller.signal, true)
        : await fetchAllRowsForSingleBranch(config, activeBranch, reportRequest, controller.signal)

      downloadXlsx({
        filename: `${config.exportPrefix}_${getFileDateStamp()}.xlsx`,
        sheetName: config.exportSheetName,
        columns: config.exportColumns,
        rows: result.rows.map((row, index) => ({ ...row, serialNumber: index + 1 })),
      })

      setAvailabilityNotes(result.availabilityNotes || [])
    } catch (apiError) {
      if (apiError.name !== 'AbortError') {
        setError(apiError.message || `Unable to export ${config.title.toLowerCase()}.`)
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
    <section
      className={['report-page', 'quotation-report-page', 'transaction-report-page', config.pageClass]
        .filter(Boolean)
        .join(' ')}
      style={branchStyle}
    >
      <div className="report-sticky-zone quotation-sticky-zone">
        <div className="report-title-row">
          <div>
            <p className="section-kicker">{config.kicker}</p>
            <div className="report-heading-line">
              <h2>{config.title}</h2>
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

        <div className={config.extraFilters ? 'quotation-filter-bar has-extra-filter' : 'quotation-filter-bar'}>
          <ReportSearch
            value={draftFilters.searchText}
            onChange={(value) => updateDraftFilter('searchText', value)}
            placeholder={config.searchPlaceholder}
          />
          <div className="field-group date-from-field">
            <label htmlFor={`${config.id}-from`}>From</label>
            <input
              id={`${config.id}-from`}
              type="date"
              value={draftFilters.fromDate}
              onChange={(event) => updateDraftFilter('fromDate', event.target.value)}
            />
          </div>
          <div className="field-group date-to-field">
            <label htmlFor={`${config.id}-to`}>To</label>
            <input
              id={`${config.id}-to`}
              type="date"
              value={draftFilters.toDate}
              onChange={(event) => updateDraftFilter('toDate', event.target.value)}
            />
          </div>
          {config.extraFilters?.map((filter) => (
            <div className="field-group" key={filter.name}>
              <label htmlFor={`${config.id}-${filter.name}`}>{filter.label}</label>
              <select
                id={`${config.id}-${filter.name}`}
                value={draftFilters[filter.name] || ''}
                onChange={(event) => updateDraftFilter(filter.name, event.target.value)}
              >
                {filter.options.map((option) => (
                  <option key={option.id} value={option.id}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          ))}
          <div className="filter-actions">
            <button type="button" className="apply-button" onClick={applyFilters}>
              Apply
            </button>
            <button type="button" className="clear-button" onClick={clearFilters}>
              Clear
            </button>
          </div>
          <PageSizeSelector value={pageSize} onChange={setPageSize} pageSizes={transactionPageSizes} />
        </div>
      </div>

      <SummaryCards items={config.getSummaryCards(summary)} />

      {status === 'error' && <div className="alert-error">{error}</div>}
      {error && status !== 'error' && <div className="alert-error">{error}</div>}

      <div className={status === 'loading' ? 'report-surface quotation-surface is-loading' : 'report-surface quotation-surface'}>
        {status === 'loading' && rows.length === 0 ? (
          <LoadingIndicator label={`Loading ${activeBranch.label} ${config.title.toLowerCase()}`} />
        ) : (
          <>
            <TransactionTable
              config={config}
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              isAllBranches={isAllBranches}
              onSelect={setSelectedRow}
            />
            <TransactionMobileRows
              config={config}
              rows={rows}
              pageNumber={pageNumber}
              pageSize={pageSize}
              isAllBranches={isAllBranches}
              onSelect={setSelectedRow}
            />
            {rows.length === 0 && status !== 'error' && (
              <div className="empty-state">No {config.title.toLowerCase()} found for this selection.</div>
            )}
            {status === 'loading' && <LoadingIndicator label={`Refreshing ${activeBranch.label} ${config.title.toLowerCase()}`} overlay />}
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

      {selectedRow && (
        <TransactionDetailDrawer
          config={config}
          row={selectedRow}
          detailState={detailState}
          rows={rows}
          onNavigate={setSelectedRow}
          onClose={() => setSelectedRow(null)}
        />
      )}
    </section>
  )
}

function createReportRequest(filters, searchText, pageNumber, pageSize) {
  return {
    pageNumber,
    pageSize,
    searchText: searchText.trim(),
    fromDate: filters.fromDate,
    toDate: filters.toDate,
    divisionCode: filters.divisionCode || null,
  }
}

async function fetchSingleBranchReport(config, branch, reportRequest, signal) {
  const result = await fetchBranchPage(config.fetchReport, branch, reportRequest, signal)
  return {
    ...result,
    summary: result.summary || config.emptySummary,
    rows: result.rows.map((row) => withBranch(row, branch)),
    availabilityNotes: [],
  }
}

async function fetchAllRowsForSingleBranch(config, branch, reportRequest, signal) {
  const result = await fetchAllRowsForBranch(config.fetchReport, branch, reportRequest, config.emptySummary, signal)
  return {
    rows: result.rows.map((row) => withBranch(row, branch)),
    totalRows: result.totalRows,
    summary: result.summary,
    availabilityNotes: [],
  }
}

async function fetchAllBranchReport(config, reportRequest, signal, exportAll = false) {
  const branchResults = await Promise.allSettled(
    branches.map(async (branch) => {
      const result = await fetchAllRowsForBranch(config.fetchReport, branch, reportRequest, config.emptySummary, signal)
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
    .sort(sortByUpdatedOnThenId(config.idField))

  const rows = exportAll
    ? mergedRows
    : mergedRows.slice((reportRequest.pageNumber - 1) * reportRequest.pageSize, reportRequest.pageNumber * reportRequest.pageSize)

  return {
    rows,
    totalRows: mergedRows.length,
    summary: successful.reduce(config.sumSummary, config.emptySummary),
    availabilityNotes,
  }
}

function BranchDropdown({ value, availabilityNotes, onChange }) {
  return (
    <div className="quotation-branch-control">
      <label className="quotation-branch-select">
        <select value={value} onChange={(event) => onChange(event.target.value)} aria-label="Report branch">
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

function SummaryCards({ items }) {
  return (
    <div className="quotation-summary-grid">
      {items.map((item) => (
        <div className={item.tone ? `counter-card is-${item.tone}` : 'counter-card'} key={item.label}>
          <span>{item.label}</span>
          <strong>{item.value}</strong>
        </div>
      ))}
    </div>
  )
}

function TransactionTable({ config, rows, pageNumber, pageSize, isAllBranches, onSelect }) {
  if (rows.length === 0) {
    return null
  }

  const showBranchColumn = isAllBranches || config.alwaysShowBranchColumn
  const tableClassName = ['stock-table', 'quotation-table', 'transaction-table', config.tableClass]
    .filter(Boolean)
    .join(' ')

  return (
    <div className="table-wrap quotation-table-wrap">
      <table className={tableClassName}>
        <thead>
          <tr>
            <th className="numeric transaction-col-serial">S.No</th>
            {showBranchColumn && !config.inlineBranchBadge && <th className="transaction-col-branch">Branch</th>}
            {config.tableColumns.map((column) => (
              <th
                key={column.key}
                className={[
                  column.numeric ? 'numeric' : '',
                  `transaction-col-${column.key}`,
                ].filter(Boolean).join(' ')}
              >
                {column.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id} onClick={() => onSelect(row)}>
              <td className="numeric transaction-col-serial">{getSerialNumber(pageNumber, pageSize, index)}</td>
              {showBranchColumn && !config.inlineBranchBadge && <td className="transaction-col-branch">{row.branchName}</td>}
              {config.tableColumns.map((column) => (
                <td
                  key={column.key}
                  className={[
                    column.numeric ? 'numeric' : '',
                    `transaction-col-${column.key}`,
                  ].filter(Boolean).join(' ')}
                >
                  {column.link ? (
                    <button type="button" className="link-button" onClick={() => onSelect(row)}>
                      {column.render(row)}
                    </button>
                  ) : (
                    column.render(row)
                  )}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function TransactionMobileRows({ config, rows, pageNumber, pageSize, isAllBranches, onSelect }) {
  if (rows.length === 0) {
    return null
  }

  return (
    <div className="mobile-stock-list mobile-quotation-list">
      {rows.map((row, index) => (
        <button type="button" className="mobile-quotation-row" key={row.id} onClick={() => onSelect(row)}>
          <span>
            <strong>{getSerialNumber(pageNumber, pageSize, index)}. {row[config.idField]}</strong>
            {isAllBranches && <span className="quotation-row-branch">{row.branchName}</span>}
          </span>
          <span className="mobile-quotation-value">{config.mobileValue(row)}</span>
          <span className="mobile-quotation-customer">{row.customerName}</span>
          <span className="mobile-quotation-meta">{config.mobileMeta(row)}</span>
        </button>
      ))}
    </div>
  )
}

function TransactionDetailDrawer({ config, row, detailState, rows, onNavigate, onClose }) {
  const data = detailState.data
  const currentIndex = rows.findIndex((current) => current.id === row.id)
  const previousRow = currentIndex > 0 ? rows[currentIndex - 1] : null
  const nextRow = currentIndex >= 0 && currentIndex < rows.length - 1 ? rows[currentIndex + 1] : null

  return (
    <div
      className="quotation-drawer-layer"
      role="dialog"
      aria-modal="true"
      aria-label={`${config.entityLabel} detail`}
      onClick={onClose}
    >
      <aside className="quotation-drawer" onClick={(event) => event.stopPropagation()}>
        <header className="quotation-drawer-header">
          <div>
            <p className="section-kicker">{config.entityLabel} Detail</p>
            <h2>{row[config.idField]}</h2>
            <span>{row.customerName}</span>
            <small>{row.branchName ? `${row.branchName} / ` : ''}{config.detailSubline(row)}</small>
          </div>
          <div className="quotation-drawer-actions">
            <button
              type="button"
              className="detail-nav-button"
              onClick={() => previousRow && onNavigate(previousRow)}
              disabled={!previousRow}
            >
              Previous
            </button>
            <button
              type="button"
              className="detail-nav-button"
              onClick={() => nextRow && onNavigate(nextRow)}
              disabled={!nextRow}
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
            <LoadingIndicator label={`Loading ${config.entityLabel.toLowerCase()} detail`} />
          ) : detailState.status === 'error' ? (
            <div className="alert-error">{detailState.error}</div>
          ) : data ? (
            <>
              <DetailLines lines={data.lines} columns={config.detailColumns} />
              {config.renderDetailFooter ? config.renderDetailFooter(data) : <DetailFooter data={data} />}
            </>
          ) : null}
        </div>
      </aside>
    </div>
  )
}

function DetailLines({ lines, columns }) {
  if (lines.length === 0) {
    return <div className="empty-state">No detail lines found.</div>
  }

  const detailColumns = columns || [
    { key: 'productName', label: 'Product Name', className: 'detail-col-product', render: (line) => <strong>{line.productName}</strong> },
    { key: 'quantity', label: 'Qty', numeric: true, className: 'detail-col-qty', render: (line) => formatStock(line.quantity) },
    { key: 'rate', label: 'Rate', numeric: true, className: 'detail-col-rate', render: (line) => formatMoney(line.rate) },
    { key: 'gst', label: 'GST %', numeric: true, className: 'detail-col-gst', render: (line) => formatGst(line.gst) },
    { key: 'amount', label: 'Amount', numeric: true, className: 'detail-col-amount', render: (line) => formatMoney(line.amount) },
  ]

  return (
    <>
      <div className="quotation-detail-table-wrap">
        <table className="quotation-detail-table">
          <colgroup>
            {detailColumns.map((column) => (
              <col className={column.className} key={column.key} />
            ))}
          </colgroup>
          <thead>
            <tr>
              {detailColumns.map((column) => (
                <th className={column.numeric ? 'numeric' : undefined} key={column.key}>
                  {column.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {lines.map((line) => (
              <tr key={line.id}>
                {detailColumns.map((column) => (
                  <td
                    className={[
                      column.numeric ? 'numeric' : '',
                      column.key === 'amount' ? 'detail-amount' : '',
                    ].filter(Boolean).join(' ')}
                    key={column.key}
                  >
                    {column.render(line)}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="quotation-detail-mobile-list">
        {lines.map((line) => (
          <article className="quotation-detail-mobile-line" key={line.id}>
            <div><strong>{line.productName}</strong></div>
            <small>Qty {formatStock(line.quantity)} x {formatMoney(line.rate)}</small>
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

function getSerialNumber(pageNumber, pageSize, rowIndex) {
  return ((pageNumber - 1) * pageSize) + rowIndex + 1
}

export default TransactionReportPage
export { DetailFooter }
