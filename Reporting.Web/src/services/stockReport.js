import { runBranchReport } from './branchApi'

const fieldNames = {
  productId: ['ProductId', 'productId', 'PRODUCTID'],
  productName: ['ProductName', 'productName', 'PRODUCTNAME', 'ItemName', 'itemName'],
  displayName: ['DisplayName', 'displayName', 'DISPLAYNAME'],
  uom: ['UOM', 'uom', 'Uom', 'Unit', 'unit'],
  hsn: ['HSN', 'hsn', 'Hsn'],
  salePrice: ['SalePrice', 'salePrice', 'SALEPRICE'],
  mrp: ['MRP', 'mrp', 'Mrp'],
  gst: ['GST', 'gst', 'Gst'],
  minStock: ['MinStock', 'minStock', 'MINSTOCK'],
  isLowStock: ['IsLowStock', 'isLowStock', 'ISLOWSTOCK'],
  availableStock: ['AvailableStock', 'availableStock', 'AVAILABLESTOCK', 'Stock', 'stock'],
  totalRows: ['TotalRows', 'totalRows', 'TOTALROWS'],
}

function getValue(row, names) {
  for (const name of names) {
    if (Object.prototype.hasOwnProperty.call(row, name)) {
      return row[name]
    }
  }

  const entries = Object.entries(row)
  const match = entries.find(([key]) => names.some((name) => key.toLowerCase() === name.toLowerCase()))
  return match?.[1]
}

function toNumber(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }

  const numberValue = Number(value)
  return Number.isFinite(numberValue) ? numberValue : null
}

function toBoolean(value) {
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

export function formatStock(value) {
  const numberValue = toNumber(value)
  if (numberValue === null) {
    return '-'
  }

  return new Intl.NumberFormat('en-IN', {
    maximumFractionDigits: 3,
  }).format(numberValue)
}

export function formatMoney(value) {
  const numberValue = toNumber(value)
  if (numberValue === null) {
    return '-'
  }

  return new Intl.NumberFormat('en-IN', {
    maximumFractionDigits: 2,
  }).format(numberValue)
}

export function formatGst(value) {
  const numberValue = toNumber(value)
  if (numberValue === null) {
    return '-'
  }

  return `${new Intl.NumberFormat('en-IN', {
    maximumFractionDigits: 2,
  }).format(numberValue)}%`
}

export function normalizeStockRows(rows) {
  return rows.map((row, index) => ({
    id: `${getValue(row, fieldNames.productId) || getValue(row, fieldNames.productName) || 'row'}-${index}`,
    productId: getValue(row, fieldNames.productId) || null,
    productName: getValue(row, fieldNames.productName) || '-',
    displayName: getValue(row, fieldNames.displayName) || '-',
    uom: getValue(row, fieldNames.uom) || '-',
    hsn: getValue(row, fieldNames.hsn) || '-',
    salePrice: toNumber(getValue(row, fieldNames.salePrice)),
    mrp: toNumber(getValue(row, fieldNames.mrp)),
    gst: toNumber(getValue(row, fieldNames.gst)),
    minStock: toNumber(getValue(row, fieldNames.minStock)),
    isLowStock: toBoolean(getValue(row, fieldNames.isLowStock)),
    availableStock: toNumber(getValue(row, fieldNames.availableStock)),
    totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
  }))
}

export async function fetchStockReport(
  branch,
  { pageNumber, pageSize, searchText, stockOperator, stockValue, gstValue },
  signal,
) {
  let payload
  let shouldFilterGstClientSide = false

  try {
    payload = await runStockReportRequest(
      branch,
      { pageNumber, pageSize, searchText, stockOperator, stockValue, gstValue },
      signal,
    )
  } catch (error) {
    if (gstValue === null || gstValue === undefined || !isUnknownParameterError(error)) {
      throw error
    }

    payload = await runStockReportRequest(
      branch,
      { pageNumber, pageSize, searchText, stockOperator, stockValue },
      signal,
    )
    shouldFilterGstClientSide = true
  }

  let rows = normalizeStockRows(payload.data)
  if (shouldFilterGstClientSide) {
    rows = rows.filter((row) => row.gst === gstValue)
  }

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: shouldFilterGstClientSide ? rows.length : rows.length > 0 ? rows[0].totalRows : 0,
  }
}

async function runStockReportRequest(
  branch,
  { pageNumber, pageSize, searchText, stockOperator, stockValue, gstValue },
  signal,
) {
  const parameters = {
    PageNumber: pageNumber,
    PageSize: pageSize,
    SearchText: searchText,
    StockOperator: stockOperator || null,
    StockValue: stockValue ?? null,
  }

  if (gstValue !== null && gstValue !== undefined) {
    parameters.Gst = gstValue
  }

  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_stock',
    parameters,
    { signal },
  )

  return payload
}

function isUnknownParameterError(error) {
  return /too many arguments|parameter|argument/i.test(error?.message || '')
}
