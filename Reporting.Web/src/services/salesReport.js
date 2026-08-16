import { runBranchReport } from './branchApi'
import { formatGst, formatMoney, formatStock } from './stockReport'
import { getValue, normalizeDate, toBoolean, toNumber } from './reportUtils'

export const salesDivisions = [
  { id: '', label: 'All Divisions' },
  { id: 'MAIN', label: 'Sales' },
  { id: 'PIPES', label: 'Pipes' },
  { id: 'TRADERS', label: 'Traders' },
]

const fieldNames = {
  salesId: ['SalesId', 'SalesID', 'Salesid', 'salesId'],
  divisionCode: ['DivisionCode', 'divisionCode'],
  divisionName: ['DivisionName', 'divisionName'],
  paymentMode: ['PaymentMode', 'paymentMode', 'Paymentmode'],
  gstText: ['GstText', 'gstText', 'GSTText'],
  updatedOn: ['UpdatedOn', 'updatedOn', 'Updatedon'],
  customerName: ['CustomerName', 'customerName', 'customername'],
  city: ['City', 'city'],
  itemCount: ['ItemCount', 'itemCount'],
  totalQuantity: ['TotalQuantity', 'totalQuantity'],
  taxableValue: ['TaxableValue', 'taxableValue', 'TaxableAmount', 'taxableAmount'],
  cgstAmount: ['CGSTAmount', 'cgstAmount'],
  sgstAmount: ['SGSTAmount', 'sgstAmount'],
  igstAmount: ['IGSTAmount', 'igstAmount'],
  gstAmount: ['GSTAmount', 'gstAmount'],
  salesValue: ['SalesValue', 'salesValue', 'GrandTotal', 'grandTotal'],
  isIgst: ['IsIGST', 'isIGST', 'isIgst'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryTotalSales: ['SummaryTotalSales', 'summaryTotalSales'],
  summaryTaxableValue: ['SummaryTaxableValue', 'summaryTaxableValue'],
  summaryCgstAmount: ['SummaryCGSTAmount', 'summaryCGSTAmount', 'summaryCgstAmount'],
  summarySgstAmount: ['SummarySGSTAmount', 'summarySGSTAmount', 'summarySgstAmount'],
  summaryIgstAmount: ['SummaryIGSTAmount', 'summaryIGSTAmount', 'summaryIgstAmount'],
  summaryGstAmount: ['SummaryGSTAmount', 'summaryGSTAmount', 'summaryGstAmount'],
  summarySalesValue: ['SummarySalesValue', 'summarySalesValue'],
  summaryIgstInvoices: ['SummaryIGSTInvoices', 'summaryIGSTInvoices', 'summaryIgstInvoices'],
  rowType: ['RowType', 'rowType'],
  productId: ['ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  rate: ['Rate', 'rate'],
  quantity: ['Quantity', 'quantity'],
  amount: ['Amount', 'amount'],
  gst: ['GST', 'gst', 'Gst'],
  gstRate: ['GSTRate', 'gstRate', 'GST', 'gst', 'Gst'],
  totalTaxable: ['TotalTaxable', 'totalTaxable'],
  totalCgst: ['TotalCGST', 'totalCGST', 'totalCgst'],
  totalSgst: ['TotalSGST', 'totalSGST', 'totalSgst'],
  totalIgst: ['TotalIGST', 'totalIGST', 'totalIgst'],
  totalGst: ['TotalGST', 'totalGst'],
  lessAmount: ['LessAmount', 'lessAmount'],
  others: ['Others', 'others'],
  grandTotal: ['GrandTotal', 'grandTotal'],
}

const emptySummary = {
  totalSales: 0,
  taxableValue: 0,
  cgstAmount: 0,
  sgstAmount: 0,
  igstAmount: 0,
  gstAmount: 0,
  salesValue: 0,
  igstInvoices: 0,
}

export function normalizeSalesRows(rows) {
  return rows.map((row, index) => {
    const salesId = getValue(row, fieldNames.salesId) || '-'
    const divisionCode = getValue(row, fieldNames.divisionCode) || 'MAIN'

    return {
      id: `${divisionCode}-${salesId}-${index}`,
      salesId,
      divisionCode,
      divisionName: getValue(row, fieldNames.divisionName) || 'Sales',
      paymentMode: getValue(row, fieldNames.paymentMode) || '-',
      gstText: getValue(row, fieldNames.gstText) || '',
      updatedOn: getValue(row, fieldNames.updatedOn),
      updatedOnLabel: normalizeDate(getValue(row, fieldNames.updatedOn)),
      customerName: getValue(row, fieldNames.customerName) || '-',
      city: getValue(row, fieldNames.city) || '-',
      itemCount: toNumber(getValue(row, fieldNames.itemCount)) || 0,
      totalQuantity: toNumber(getValue(row, fieldNames.totalQuantity)) || 0,
      taxableValue: toNumber(getValue(row, fieldNames.taxableValue)) || 0,
      cgstAmount: toNumber(getValue(row, fieldNames.cgstAmount)) || 0,
      sgstAmount: toNumber(getValue(row, fieldNames.sgstAmount)) || 0,
      igstAmount: toNumber(getValue(row, fieldNames.igstAmount)) || 0,
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      salesValue: toNumber(getValue(row, fieldNames.salesValue)) || 0,
      isIgst: toBoolean(getValue(row, fieldNames.isIgst)),
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        totalSales: toNumber(getValue(row, fieldNames.summaryTotalSales)) || 0,
        taxableValue: toNumber(getValue(row, fieldNames.summaryTaxableValue)) || 0,
        cgstAmount: toNumber(getValue(row, fieldNames.summaryCgstAmount)) || 0,
        sgstAmount: toNumber(getValue(row, fieldNames.summarySgstAmount)) || 0,
        igstAmount: toNumber(getValue(row, fieldNames.summaryIgstAmount)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.summaryGstAmount)) || 0,
        salesValue: toNumber(getValue(row, fieldNames.summarySalesValue)) || 0,
        igstInvoices: toNumber(getValue(row, fieldNames.summaryIgstInvoices)) || 0,
      },
    }
  })
}

export function normalizeSalesDetail(rows) {
  const lines = []
  const gstSummary = []
  let totals = {
    taxableValue: 0,
    cgstAmount: 0,
    sgstAmount: 0,
    igstAmount: 0,
    gstAmount: 0,
    lessAmount: 0,
    others: 0,
    grandTotal: 0,
    isIgst: false,
  }

  rows.forEach((row, index) => {
    const rowType = String(getValue(row, fieldNames.rowType) || 'DETAIL').toUpperCase()

    if (rowType === 'TOTAL') {
      totals = {
        taxableValue: toNumber(getValue(row, fieldNames.totalTaxable)) || 0,
        cgstAmount: toNumber(getValue(row, fieldNames.totalCgst)) || 0,
        sgstAmount: toNumber(getValue(row, fieldNames.totalSgst)) || 0,
        igstAmount: toNumber(getValue(row, fieldNames.totalIgst)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.totalGst)) || 0,
        lessAmount: toNumber(getValue(row, fieldNames.lessAmount)) || 0,
        others: toNumber(getValue(row, fieldNames.others)) || 0,
        grandTotal: toNumber(getValue(row, fieldNames.grandTotal)) || 0,
        isIgst: toBoolean(getValue(row, fieldNames.isIgst)),
      }
      return
    }

    if (rowType === 'GST_SUMMARY') {
      gstSummary.push({
        id: `gst-${index}`,
        gst: toNumber(getValue(row, fieldNames.gstRate)),
        taxableValue: toNumber(getValue(row, fieldNames.taxableValue)) || 0,
        cgstAmount: toNumber(getValue(row, fieldNames.cgstAmount)) || 0,
        sgstAmount: toNumber(getValue(row, fieldNames.sgstAmount)) || 0,
        igstAmount: toNumber(getValue(row, fieldNames.igstAmount)) || 0,
        gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      })
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
      cgstAmount: toNumber(getValue(row, fieldNames.cgstAmount)),
      sgstAmount: toNumber(getValue(row, fieldNames.sgstAmount)),
      igstAmount: toNumber(getValue(row, fieldNames.igstAmount)),
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)),
    })
  })

  return { lines, gstSummary, totals }
}

export async function fetchSalesReport(
  branch,
  { pageNumber, pageSize, searchText, fromDate, toDate, divisionCode },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_sales',
    {
      PageNumber: pageNumber,
      PageSize: pageSize,
      SearchText: searchText ?? '',
      FromDate: fromDate || null,
      ToDate: toDate || null,
      DivisionCode: divisionCode || null,
    },
    { signal },
  )

  const rows = normalizeSalesRows(payload.data)

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: rows.length > 0 ? rows[0].totalRows : 0,
    summary: rows.length > 0 ? rows[0].summary : emptySummary,
  }
}

export async function fetchSalesDetail(branch, salesId, divisionCode, signal) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_sales_detail',
    { SalesId: salesId, DivisionCode: divisionCode || 'MAIN' },
    { signal },
  )

  return normalizeSalesDetail(payload.data)
}

export { emptySummary as emptySalesSummary, formatGst, formatMoney, formatStock }
