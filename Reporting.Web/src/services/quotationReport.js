import { runBranchReport } from './branchApi'
import { formatGst, formatMoney, formatStock } from './stockReport'

const fieldNames = {
  quotationId: ['QuotationId', 'QuotationID', 'Quotationid', 'quotationId'],
  updatedOn: ['UpdatedOn', 'updatedOn', 'Updatedon'],
  customerName: ['CustomerName', 'customerName', 'customername'],
  city: ['City', 'city'],
  assistName: ['AssistName', 'assistName'],
  status: ['Status', 'status'],
  itemCount: ['ItemCount', 'itemCount'],
  totalQuantity: ['TotalQuantity', 'totalQuantity'],
  quotationValue: ['QuotationValue', 'quotationValue'],
  alteredItemCount: ['AlteredItemCount', 'alteredItemCount'],
  hasPriceAltered: ['HasPriceAltered', 'hasPriceAltered'],
  isCancelled: ['IsCancelled', 'isCancelled'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryTotalQuotations: ['SummaryTotalQuotations', 'summaryTotalQuotations'],
  summaryQuotationValue: ['SummaryQuotationValue', 'summaryQuotationValue'],
  summaryPriceAltered: ['SummaryPriceAltered', 'summaryPriceAltered'],
  summaryCancelled: ['SummaryCancelled', 'summaryCancelled'],
  rowType: ['RowType', 'rowType'],
  productId: ['ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  displayName: ['DisplayName', 'displayName'],
  uom: ['UOM', 'uom', 'Uom'],
  hsn: ['HSN', 'hsn', 'Hsn'],
  quotedRate: ['QuotedRate', 'quotedRate'],
  masterSalesPrice: ['MasterSalesPrice', 'masterSalesPrice'],
  quantity: ['Quantity', 'quantity'],
  amount: ['Amount', 'amount'],
  gst: ['GST', 'gst', 'Gst'],
  isGstFallback: ['IsGSTFallback', 'isGSTFallback'],
  isPriceAltered: ['IsPriceAltered', 'isPriceAltered'],
  taxableValue: ['TaxableValue', 'taxableValue'],
  gstAmount: ['GSTAmount', 'gstAmount'],
  gstRate: ['GSTRate', 'gstRate'],
  totalTaxable: ['TotalTaxable', 'totalTaxable'],
  totalGst: ['TotalGST', 'totalGst'],
  grandTotal: ['GrandTotal', 'grandTotal'],
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

function normalizeDate(value) {
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

export function normalizeQuotationRows(rows) {
  return rows.map((row, index) => {
    const quotationId = getValue(row, fieldNames.quotationId) || '-'

    return {
      id: `${quotationId}-${index}`,
      quotationId,
      updatedOn: getValue(row, fieldNames.updatedOn),
      updatedOnLabel: normalizeDate(getValue(row, fieldNames.updatedOn)),
      customerName: getValue(row, fieldNames.customerName) || '-',
      city: getValue(row, fieldNames.city) || '-',
      assistName: getValue(row, fieldNames.assistName) || '-',
      status: getValue(row, fieldNames.status) || '-',
      itemCount: toNumber(getValue(row, fieldNames.itemCount)) || 0,
      totalQuantity: toNumber(getValue(row, fieldNames.totalQuantity)) || 0,
      quotationValue: toNumber(getValue(row, fieldNames.quotationValue)) || 0,
      alteredItemCount: toNumber(getValue(row, fieldNames.alteredItemCount)) || 0,
      hasPriceAltered: toBoolean(getValue(row, fieldNames.hasPriceAltered)),
      isCancelled: toBoolean(getValue(row, fieldNames.isCancelled)),
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        totalQuotations: toNumber(getValue(row, fieldNames.summaryTotalQuotations)) || 0,
        quotationValue: toNumber(getValue(row, fieldNames.summaryQuotationValue)) || 0,
        priceAltered: toNumber(getValue(row, fieldNames.summaryPriceAltered)) || 0,
        cancelled: toNumber(getValue(row, fieldNames.summaryCancelled)) || 0,
      },
    }
  })
}

export function normalizeQuotationDetail(rows) {
  const lines = []
  const gstBreakup = []
  let totals = {
    taxableValue: 0,
    gstAmount: 0,
    grandTotal: 0,
  }

  rows.forEach((row, index) => {
    const rowType = String(getValue(row, fieldNames.rowType) || 'DETAIL').toUpperCase()

    if (rowType === 'GST_BREAKUP') {
      gstBreakup.push({
        id: `gst-${index}`,
        gstRate: toNumber(getValue(row, fieldNames.gstRate) ?? getValue(row, fieldNames.gst)),
        taxableValue: toNumber(getValue(row, fieldNames.taxableValue)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      })
      return
    }

    if (rowType === 'TOTAL') {
      totals = {
        taxableValue: toNumber(getValue(row, fieldNames.totalTaxable)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.totalGst)) || 0,
        grandTotal: toNumber(getValue(row, fieldNames.grandTotal)) || 0,
      }
      return
    }

    lines.push({
      id: `${getValue(row, fieldNames.productId) || 'line'}-${index}`,
      productId: getValue(row, fieldNames.productId) || '-',
      productName: getValue(row, fieldNames.productName) || '-',
      displayName: getValue(row, fieldNames.displayName) || '-',
      uom: getValue(row, fieldNames.uom) || '-',
      hsn: getValue(row, fieldNames.hsn) || '-',
      quotedRate: toNumber(getValue(row, fieldNames.quotedRate)),
      masterSalesPrice: toNumber(getValue(row, fieldNames.masterSalesPrice)),
      quantity: toNumber(getValue(row, fieldNames.quantity)),
      amount: toNumber(getValue(row, fieldNames.amount)),
      gst: toNumber(getValue(row, fieldNames.gst)),
      isGstFallback: toBoolean(getValue(row, fieldNames.isGstFallback)),
      isPriceAltered: toBoolean(getValue(row, fieldNames.isPriceAltered)),
      taxableValue: toNumber(getValue(row, fieldNames.taxableValue)),
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)),
    })
  })

  return { lines, gstBreakup, totals }
}

export async function fetchQuotationReport(
  branch,
  { pageNumber, pageSize, searchText, fromDate, toDate },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_quotation',
    {
      PageNumber: pageNumber,
      PageSize: pageSize,
      SearchText: searchText ?? '',
      FromDate: fromDate || null,
      ToDate: toDate || null,
    },
    { signal },
  )

  const rows = normalizeQuotationRows(payload.data)

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: rows.length > 0 ? rows[0].totalRows : 0,
    summary: rows.length > 0
      ? rows[0].summary
      : {
          totalQuotations: 0,
          quotationValue: 0,
          priceAltered: 0,
          cancelled: 0,
        },
  }
}

export async function fetchQuotationDetail(branch, quotationId, signal) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_quotation_detail',
    { QuotationId: quotationId },
    { signal },
  )

  return normalizeQuotationDetail(payload.data)
}

export { formatGst, formatMoney, formatStock }
