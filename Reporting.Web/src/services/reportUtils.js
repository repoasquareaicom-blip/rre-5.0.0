export const ALL_BRANCHES_ID = 'ALL'
export const ALL_BRANCH_FETCH_SIZE = 1000
export const BRANCH_PAGE_SIZE = 100
export const DEFAULT_TRANSACTION_PAGE_SIZE = 500
export const transactionPageSizes = [10, 25, 50, 100, 500]

export const allBranchOption = {
  id: ALL_BRANCHES_ID,
  label: 'All Branches',
  accent: '#b91c1c',
  accentSoft: '#fee2e2',
  accentBorder: '#fca5a5',
}

export function getTodayInputValue() {
  const today = new Date()
  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

export function getFileDateStamp() {
  return getTodayInputValue().replaceAll('-', '')
}

export function getValue(row, names) {
  for (const name of names) {
    if (Object.prototype.hasOwnProperty.call(row, name)) {
      return row[name]
    }
  }

  const entries = Object.entries(row)
  const match = entries.find(([key]) => names.some((name) => key.toLowerCase() === name.toLowerCase()))
  return match?.[1]
}

export function toNumber(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }

  const numberValue = Number(value)
  return Number.isFinite(numberValue) ? numberValue : null
}

export function toBoolean(value) {
  if (typeof value === 'boolean') {
    return value
  }

  if (typeof value === 'number') {
    return value !== 0
  }

  if (typeof value === 'string') {
    return ['true', '1', 'yes'].includes(value.trim().toLowerCase())
  }

  return false
}

export function normalizeDate(value) {
  if (!value) {
    return '-'
  }

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return String(value)
  }

  return new Intl.DateTimeFormat('en-IN', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  }).format(date)
}

export function withBranch(row, branch) {
  return {
    ...row,
    id: `${branch.id}-${row.id}`,
    branch,
    branchCode: branch.id,
    branchName: branch.label,
  }
}

export async function fetchBranchPage(fetchReport, branch, reportRequest, signal) {
  if (reportRequest.pageSize <= BRANCH_PAGE_SIZE) {
    return fetchReport(branch, reportRequest, signal)
  }

  const firstServerPage = ((reportRequest.pageNumber - 1) * reportRequest.pageSize) / BRANCH_PAGE_SIZE
  const startServerPage = Math.floor(firstServerPage) + 1
  const pagesToFetch = Math.ceil(reportRequest.pageSize / BRANCH_PAGE_SIZE)
  let rows = []
  let totalRows = 0
  let summary = null

  for (let pageOffset = 0; pageOffset < pagesToFetch; pageOffset += 1) {
    const result = await fetchReport(
      branch,
      {
        ...reportRequest,
        pageNumber: startServerPage + pageOffset,
        pageSize: BRANCH_PAGE_SIZE,
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
    summary,
  }
}

export async function fetchAllRowsForBranch(fetchReport, branch, reportRequest, emptySummary, signal) {
  let pageNumber = 1
  let rows = []
  let summary = null
  let totalRows = 0

  do {
    const result = await fetchReport(
      branch,
      {
        ...reportRequest,
        pageNumber,
        pageSize: BRANCH_PAGE_SIZE,
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
    summary: summary || emptySummary,
  }
}

export function sortByUpdatedOnThenId(idField) {
  return (a, b) => {
    const bTime = new Date(b.updatedOn || 0).getTime()
    const aTime = new Date(a.updatedOn || 0).getTime()
    return bTime - aTime || String(b[idField]).localeCompare(String(a[idField]))
  }
}
