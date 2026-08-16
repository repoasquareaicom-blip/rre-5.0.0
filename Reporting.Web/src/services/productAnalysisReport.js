import { runBranchReport } from './branchApi'
import { formatGst, formatMoney, formatStock } from './stockReport'
import { getValue, normalizeDate, toNumber } from './reportUtils'

export const analysisTypes = [
  { id: 'SALES', label: 'Sales' },
  { id: 'QUOTATION', label: 'Quotation' },
  { id: 'ESTIMATION', label: 'Estimation' },
]

const fieldNames = {
  productId: ['ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  transactionCount: ['TransactionCount', 'transactionCount'],
  totalQuantity: ['TotalQuantity', 'totalQuantity'],
  averageRate: ['AverageRate', 'averageRate'],
  minimumRate: ['MinimumRate', 'minimumRate'],
  maximumRate: ['MaximumRate', 'maximumRate'],
  taxableAmount: ['TaxableAmount', 'taxableAmount'],
  gstAmount: ['GSTAmount', 'gstAmount'],
  totalValue: ['TotalValue', 'totalValue'],
  electricalsValue: ['ElectricalsValue', 'electricalsValue'],
  pipesValue: ['PipesValue', 'pipesValue'],
  tradersValue: ['TradersValue', 'tradersValue'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryProducts: ['SummaryProducts', 'summaryProducts'],
  summaryTransactions: ['SummaryTransactions', 'summaryTransactions'],
  summaryQuantity: ['SummaryQuantity', 'summaryQuantity'],
  summaryValue: ['SummaryValue', 'summaryValue'],
  transactionDate: ['TransactionDate', 'transactionDate'],
  documentNo: ['DocumentNo', 'documentNo'],
  customerName: ['CustomerName', 'customerName'],
  division: ['Division', 'division'],
  quantity: ['Quantity', 'quantity'],
  rate: ['Rate', 'rate'],
  gstPercent: ['GSTPercent', 'gstPercent'],
  amount: ['Amount', 'amount'],
}

export const emptyProductAnalysisSummary = {
  products: 0,
  transactions: 0,
  totalQuantity: 0,
  totalValue: 0,
  averageRate: 0,
  minimumRate: 0,
  maximumRate: 0,
}

export function normalizeProductAnalysisRows(rows) {
  return rows.map((row, index) => {
    const productId = getValue(row, fieldNames.productId) || '-'

    return {
      id: `${productId}-${index}`,
      productId,
      productName: getValue(row, fieldNames.productName) || '-',
      transactionCount: toNumber(getValue(row, fieldNames.transactionCount)) || 0,
      totalQuantity: toNumber(getValue(row, fieldNames.totalQuantity)) || 0,
      averageRate: toNumber(getValue(row, fieldNames.averageRate)) || 0,
      minimumRate: toNumber(getValue(row, fieldNames.minimumRate)) || 0,
      maximumRate: toNumber(getValue(row, fieldNames.maximumRate)) || 0,
      taxableAmount: toNumber(getValue(row, fieldNames.taxableAmount)) || 0,
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      totalValue: toNumber(getValue(row, fieldNames.totalValue)) || 0,
      electricalsValue: toNumber(getValue(row, fieldNames.electricalsValue)) || 0,
      pipesValue: toNumber(getValue(row, fieldNames.pipesValue)) || 0,
      tradersValue: toNumber(getValue(row, fieldNames.tradersValue)) || 0,
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        products: toNumber(getValue(row, fieldNames.summaryProducts)) || 0,
        transactions: toNumber(getValue(row, fieldNames.summaryTransactions)) || 0,
        totalQuantity: toNumber(getValue(row, fieldNames.summaryQuantity)) || 0,
        totalValue: toNumber(getValue(row, fieldNames.summaryValue)) || 0,
      },
    }
  })
}

export function normalizeProductAnalysisDetail(rows) {
  return rows.map((row, index) => {
    const documentNo = getValue(row, fieldNames.documentNo) || '-'

    return {
      id: `${documentNo}-${index}`,
      transactionDate: getValue(row, fieldNames.transactionDate),
      transactionDateLabel: normalizeDate(getValue(row, fieldNames.transactionDate)),
      documentNo,
      customerName: getValue(row, fieldNames.customerName) || '-',
      division: getValue(row, fieldNames.division) || '',
      quantity: toNumber(getValue(row, fieldNames.quantity)) || 0,
      rate: toNumber(getValue(row, fieldNames.rate)) || 0,
      gstPercent: toNumber(getValue(row, fieldNames.gstPercent)) || 0,
      taxableAmount: toNumber(getValue(row, fieldNames.taxableAmount)) || 0,
      gstAmount: toNumber(getValue(row, fieldNames.gstAmount)) || 0,
      amount: toNumber(getValue(row, fieldNames.amount)) || 0,
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
    }
  })
}

export function normalizeProductLookupRows(rows) {
  return rows.map((row) => ({
    productId: getValue(row, fieldNames.productId) || '',
    productName: getValue(row, fieldNames.productName) || '',
  })).filter((row) => row.productId && row.productName)
}

export async function fetchProductAnalysisReport(
  branch,
  { analysisType, productId, pageNumber, pageSize, searchText, fromDate, toDate },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_product_analysis',
    {
      AnalysisType: analysisType,
      ProductId: productId || null,
      PageNumber: pageNumber,
      PageSize: pageSize,
      SearchText: searchText ?? '',
      FromDate: fromDate || null,
      ToDate: toDate || null,
    },
    { signal },
  )

  const rows = normalizeProductAnalysisRows(payload.data)

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: rows.length > 0 ? rows[0].totalRows : 0,
    summary: rows.length > 0 ? rows[0].summary : emptyProductAnalysisSummary,
  }
}

export async function fetchProductAnalysisDetail(
  branch,
  { analysisType, productId, pageNumber, pageSize, searchText, fromDate, toDate },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_product_analysis_detail',
    {
      AnalysisType: analysisType,
      ProductId: productId,
      PageNumber: pageNumber,
      PageSize: pageSize,
      SearchText: searchText ?? '',
      FromDate: fromDate || null,
      ToDate: toDate || null,
    },
    { signal },
  )

  const rows = normalizeProductAnalysisDetail(payload.data)

  return {
    branchCode: payload.branchCode,
    rows,
    totalRows: rows.length > 0 ? rows[0].totalRows : 0,
  }
}

export async function fetchProductLookup(branch, searchText, signal) {
  const payload = await runBranchReport(
    branch,
    'dbo.sp_report_product_lookup',
    {
      SearchText: searchText || null,
      Top: 50,
    },
    { signal },
  )

  return normalizeProductLookupRows(payload.data)
}

export { formatGst, formatMoney, formatStock }
