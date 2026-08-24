import { runBranchReport } from './branchApi'
import { formatMoney, formatStock } from './stockReport'
import { getValue, toNumber } from './reportUtils'

export const PRODUCT_ANALYSIS_SORTS = {
  TRANS_ID: 'TRANSID',
  PRODUCT_NAME: 'PRODUCTNAME',
  CUSTOMER_NAME: 'CUSTOMERNAME',
  CITY: 'CITY',
  BRAND: 'BRAND',
  CATEGORY: 'CATEGORY',
  TRANS_DATE: 'TRANSDATE',
  TRANS_QTY: 'TRANSQTY',
  PRICE: 'PRICE',
  TOTAL_PRICE: 'TOTALPRICE',
}

const fieldNames = {
  transId: ['TransId', 'TRANSID', 'transId'],
  productId: ['Productid', 'ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  customerName: ['CustomerName', 'customerName'],
  city: ['City', 'city'],
  brand: ['Brand', 'brand'],
  category: ['Category', 'category'],
  transDate: ['TransDate', 'transDate'],
  transQty: ['TransQty', 'transQty'],
  price: ['Price', 'price'],
  totalPrice: ['TotalPrice', 'totalPrice'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryProducts: ['SummaryProducts', 'summaryProducts'],
  summaryTransactions: ['SummaryTransactions', 'summaryTransactions'],
  summaryQuantity: ['SummaryQuantity', 'summaryQuantity'],
  summaryTotalPrice: ['SummaryTotalPrice', 'summaryTotalPrice'],
}

export const emptyProductAnalysisSummary = {
  products: 0,
  bills: 0,
  totalQuantity: 0,
  totalSalesAmount: 0,
}

export function normalizeProductAnalysisRows(rows) {
  return rows.map((row, index) => {
    const productId = String(getValue(row, fieldNames.productId) || '-')
    const transId = String(getValue(row, fieldNames.transId) || '-')

    return {
      id: `${transId}-${productId}-${index}`,
      transId,
      productId,
      productName: getValue(row, fieldNames.productName) || '-',
      customerName: getValue(row, fieldNames.customerName) || '',
      city: getValue(row, fieldNames.city) || '',
      brand: getValue(row, fieldNames.brand) || '',
      category: getValue(row, fieldNames.category) || '',
      transDate: getValue(row, fieldNames.transDate) || '',
      transQty: toNumber(getValue(row, fieldNames.transQty)) || 0,
      price: toNumber(getValue(row, fieldNames.price)) || 0,
      totalPrice: toNumber(getValue(row, fieldNames.totalPrice)) || 0,
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        products: toNumber(getValue(row, fieldNames.summaryProducts)) || 0,
        bills: toNumber(getValue(row, fieldNames.summaryTransactions)) || 0,
        totalQuantity: toNumber(getValue(row, fieldNames.summaryQuantity)) || 0,
        totalSalesAmount: toNumber(getValue(row, fieldNames.summaryTotalPrice)) || 0,
      },
    }
  })
}

export async function fetchProductAnalysisReport(
  branch,
  { fromDate, toDate, productSearch, pageNumber, pageSize, sortBy, sortDirection },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.Proc_ProductSalesAnalysis',
    {
      FromDate: fromDate,
      ToDate: toDate,
      ProductSearch: productSearch?.trim() || null,
      PageNumber: pageNumber,
      PageSize: pageSize,
      SortBy: normalizeSingleBranchSort(sortBy),
      SortDirection: normalizeSortDirection(sortDirection),
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

export function normalizeSingleBranchSort(sortBy) {
  const normalized = String(sortBy || PRODUCT_ANALYSIS_SORTS.TRANS_DATE).toUpperCase()
  if (normalized === PRODUCT_ANALYSIS_SORTS.TRANS_ID) return 'TRANSID'
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME) return 'PRODUCTNAME'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME) return 'CUSTOMERNAME'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CITY) return 'CITY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.BRAND) return 'BRAND'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CATEGORY) return 'CATEGORY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.TRANS_QTY) return 'TRANSQTY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRICE) return 'PRICE'
  if (normalized === PRODUCT_ANALYSIS_SORTS.TOTAL_PRICE) return 'TOTALPRICE'
  return 'TRANSDATE'
}

export function normalizeCompareSort(sortBy) {
  const normalized = String(sortBy || PRODUCT_ANALYSIS_SORTS.TRANS_DATE).toUpperCase()
  if (normalized === PRODUCT_ANALYSIS_SORTS.TRANS_ID) return PRODUCT_ANALYSIS_SORTS.TRANS_ID
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME) return PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME
  if (normalized === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME) return PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME
  if (normalized === PRODUCT_ANALYSIS_SORTS.CITY) return PRODUCT_ANALYSIS_SORTS.CITY
  if (normalized === PRODUCT_ANALYSIS_SORTS.BRAND) return PRODUCT_ANALYSIS_SORTS.BRAND
  if (normalized === PRODUCT_ANALYSIS_SORTS.CATEGORY) return PRODUCT_ANALYSIS_SORTS.CATEGORY
  if (normalized === PRODUCT_ANALYSIS_SORTS.TRANS_QTY) return PRODUCT_ANALYSIS_SORTS.TRANS_QTY
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRICE) return PRODUCT_ANALYSIS_SORTS.PRICE
  if (normalized === PRODUCT_ANALYSIS_SORTS.TOTAL_PRICE) return PRODUCT_ANALYSIS_SORTS.TOTAL_PRICE
  return PRODUCT_ANALYSIS_SORTS.TRANS_DATE
}

export function normalizeSortDirection(sortDirection) {
  return String(sortDirection || 'DESC').toUpperCase() === 'ASC' ? 'ASC' : 'DESC'
}

export function sortProductAnalysisRows(rows, sortBy, sortDirection) {
  const normalizedSort = normalizeCompareSort(sortBy)
  const direction = normalizeSortDirection(sortDirection) === 'ASC' ? 1 : -1

  return [...rows].sort((a, b) => {
    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.TRANS_ID) {
      return direction * String(a.transId).localeCompare(String(b.transId))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME) {
      return direction * String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME) {
      return direction * String(a.customerName).localeCompare(String(b.customerName))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.CITY) {
      return direction * String(a.city).localeCompare(String(b.city))
        || String(a.customerName).localeCompare(String(b.customerName))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.BRAND) {
      return direction * String(a.brand).localeCompare(String(b.brand))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.CATEGORY) {
      return direction * String(a.category).localeCompare(String(b.category))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.TRANS_QTY) {
      return direction * ((a.transQty || 0) - (b.transQty || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.PRICE) {
      return direction * ((a.price || 0) - (b.price || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.TOTAL_PRICE) {
      return direction * ((a.totalPrice || 0) - (b.totalPrice || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    return direction * (new Date(a.transDate || 0).getTime() - new Date(b.transDate || 0).getTime())
      || String(a.productName).localeCompare(String(b.productName))
  })
}

export { formatMoney, formatStock }
