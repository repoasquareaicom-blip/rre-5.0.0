import { runBranchReport } from './branchApi'
import { formatGst, formatMoney, formatStock } from './stockReport'
import { getValue, normalizeDate, toBoolean, toNumber } from './reportUtils'

const fieldNames = {
  estimationId: ['EstimationId', 'EstimationID', 'Estimationid', 'estimationId'],
  quotationId: ['QuotationId', 'QuotationID', 'Quotationid', 'quotationId'],
  updatedOn: ['UpdatedOn', 'updatedOn', 'Updatedon'],
  customerName: ['CustomerName', 'customerName', 'customername'],
  city: ['City', 'city'],
  assistName: ['AssistName', 'assistName'],
  itemCount: ['ItemCount', 'itemCount'],
  totalQuantity: ['TotalQuantity', 'totalQuantity'],
  taxableValue: ['TaxableValue', 'taxableValue'],
  gstAmount: ['GSTAmount', 'gstAmount'],
  estimationValue: ['EstimationValue', 'estimationValue', 'GrandTotal', 'grandTotal'],
  isCancelled: ['IsCancelled', 'isCancelled'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryTotalEstimations: ['SummaryTotalEstimations', 'summaryTotalEstimations'],
  summaryTaxableValue: ['SummaryTaxableValue', 'summaryTaxableValue'],
  summaryGstAmount: ['SummaryGSTAmount', 'summaryGSTAmount', 'summaryGstAmount'],
  summaryEstimationValue: ['SummaryEstimationValue', 'summaryEstimationValue'],
  summaryCancelled: ['SummaryCancelled', 'summaryCancelled'],
  rowType: ['RowType', 'rowType'],
  productId: ['ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  rate: ['Rate', 'rate', 'QuotedRate', 'quotedRate'],
  quantity: ['Quantity', 'quantity'],
  amount: ['Amount', 'amount'],
  gst: ['GST', 'gst', 'Gst'],
  totalTaxable: ['TotalTaxable', 'totalTaxable'],
  totalGst: ['TotalGST', 'totalGst'],
  grandTotal: ['GrandTotal', 'grandTotal'],
}

const emptySummary = {
  totalEstimations: 0,
  taxableValue: 0,
  gstAmount: 0,
  estimationValue: 0,
  cancelled: 0,
}

export function normalizeEstimationRows(rows) {
  return rows.map((row, index) => {
    const estimationId = getValue(row, fieldNames.estimationId) || '-'

    return {
      id: `${estimationId}-${index}`,
      estimationId,
      quotationId: getValue(row, fieldNames.quotationId) || '-',
      updatedOn: getValue(row, fieldNames.updatedOn),
      updatedOnLabel: normalizeDate(getValue(row, fieldNames.updatedOn)),
      customerName: getValue(row, fieldNames.customerName) || '-',
      city: getValue(row, fieldNames.city) || '-',
      assistName: getValue(row, fieldNames.assistName) || '-',
      itemCount: toNumber(getValue(row, fieldNames.itemCount)) || 0,
      totalQuantity: toNumber(getValue(row, fieldNames.totalQuantity)) || 0,
      taxableValue: toNumber(getValue(row, fieldNames.taxableValue)) || 0,
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      estimationValue: toNumber(getValue(row, fieldNames.estimationValue)) || 0,
      isCancelled: toBoolean(getValue(row, fieldNames.isCancelled)),
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        totalEstimations: toNumber(getValue(row, fieldNames.summaryTotalEstimations)) || 0,
        taxableValue: toNumber(getValue(row, fieldNames.summaryTaxableValue)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.summaryGstAmount)) || 0,
        estimationValue: toNumber(getValue(row, fieldNames.summaryEstimationValue)) || 0,
        cancelled: toNumber(getValue(row, fieldNames.summaryCancelled)) || 0,
      },
    }
  })
}

export function normalizeEstimationDetail(rows) {
  const lines = []
  let totals = {
    taxableValue: 0,
    gstAmount: 0,
    grandTotal: 0,
  }

  rows.forEach((row, index) => {
    const rowType = String(getValue(row, fieldNames.rowType) || 'DETAIL').toUpperCase()

    if (rowType === 'TOTAL') {
      totals = {
        taxableValue: toNumber(getValue(row, fieldNames.totalTaxable)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.totalGst)) || 0,
        grandTotal: toNumber(getValue(row, fieldNames.grandTotal)) || 0,
      }
      return
    }

    if (rowType !== 'DETAIL') {
      return
    }

    lines.push({
      id: `${getValue(row, fieldNames.productId) || 'line'}-${index}`,
      productId: getValue(row, fieldNames.productId) || '-',
      productName: getValue(row, fieldNames.productName) || '-',
      rate: toNumber(getValue(row, fieldNames.rate)),
      quantity: toNumber(getValue(row, fieldNames.quantity)),
      amount: toNumber(getValue(row, fieldNames.amount)),
      gst: toNumber(getValue(row, fieldNames.gst)),
      taxableValue: toNumber(getValue(row, fieldNames.taxableValue)),
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)),
    })
  })

  return { lines, totals }
}

export async function fetchEstimationReport(
  branch,
  { pageNumber, pageSize, searchText, fromDate, toDate },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_estimation',
    {
      PageNumber: pageNumber,
      PageSize: pageSize,
      SearchText: searchText ?? '',
      FromDate: fromDate || null,
      ToDate: toDate || null,
    },
    { signal },
  )

  const rows = normalizeEstimationRows(payload.data)

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: rows.length > 0 ? rows[0].totalRows : 0,
    summary: rows.length > 0 ? rows[0].summary : emptySummary,
  }
}

export async function fetchEstimationDetail(branch, estimationId, signal) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_estimation_detail',
    { EstimationId: estimationId },
    { signal },
  )

  return normalizeEstimationDetail(payload.data)
}

export { emptySummary as emptyEstimationSummary, formatGst, formatMoney, formatStock }
