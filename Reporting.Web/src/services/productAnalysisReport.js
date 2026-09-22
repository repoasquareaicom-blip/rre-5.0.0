import { runBranchReport } from './branchApi'
import { formatMoney, formatStock } from './stockReport'
import { getValue, toNumber } from './reportUtils'

export const PRODUCT_ANALYSIS_SORTS = {
  PRODUCT_NAME: 'PRODUCTNAME',
  CUSTOMER_NAME: 'CUSTOMERNAME',
  CITY: 'CITY',
  BRAND: 'BRAND',
  CATEGORY: 'CATEGORY',
  TOTAL_QTY: 'TOTALQTY',
  PRICE: 'PRICE',
  TOTAL_AMOUNT: 'TOTALAMOUNT',
}

const fieldNames = {
  productId: ['Productid', 'ProductId', 'productId'],
  productName: ['ProductName', 'productName'],
  customerName: ['CustomerName', 'customerName'],
  city: ['City', 'city'],
  brand: ['Brand', 'brand'],
  category: ['Category', 'category'],
  totalQty: ['TotalQty', 'TOTALQTY', 'totalQty'],
  price: ['Price', 'price'],
  totalAmount: ['TotalAmount', 'TOTALAMOUNT', 'totalAmount'],
  totalRows: ['TotalRows', 'totalRows'],
  summaryProducts: ['SummaryProducts', 'summaryProducts'],
  summaryCustomers: ['SummaryCustomers', 'summaryCustomers'],
  summaryQuantity: ['SummaryQuantity', 'summaryQuantity'],
  summaryTotalAmount: ['SummaryTotalAmount', 'summaryTotalAmount'],
}

export const emptyProductAnalysisSummary = {
  customers: 0,
  products: 0,
  totalQuantity: 0,
  totalAmount: 0,
}

export function normalizeProductAnalysisRows(rows) {
  return rows.map((row, index) => {
    const productId = String(getValue(row, fieldNames.productId) || '-')
    const customerName = getValue(row, fieldNames.customerName) || ''
    const totalQty = toNumber(getValue(row, fieldNames.totalQty)) || 0
    const totalAmount = toNumber(getValue(row, fieldNames.totalAmount)) || 0

    return {
      id: `${customerName || 'PRODUCT'}-${productId}-${index}`,
      productId,
      productName: getValue(row, fieldNames.productName) || '-',
      customerName,
      city: getValue(row, fieldNames.city) || '',
      brand: getValue(row, fieldNames.brand) || '',
      category: getValue(row, fieldNames.category) || '',
      totalQty,
      price: toNumber(getValue(row, fieldNames.price)) || 0,
      totalAmount,
      totalRows: toNumber(getValue(row, fieldNames.totalRows)) || 0,
      summary: {
        customers: toNumber(getValue(row, fieldNames.summaryCustomers)) || 0,
        products: toNumber(getValue(row, fieldNames.summaryProducts)) || 0,
        totalQuantity: toNumber(getValue(row, fieldNames.summaryQuantity)) || 0,
        totalAmount: toNumber(getValue(row, fieldNames.summaryTotalAmount)) || 0,
      },
    }
  })
}

export async function fetchProductAnalysisReport(
  branch,
  { fromDate, toDate, productSearch, customerSearch, analysisMode, pageNumber, pageSize, sortBy, sortDirection },
  signal,
) {
  const payload = await runBranchReport(
    branch,
    'dbo.Proc_ProductSalesAnalysis',
    {
      FromDate: fromDate,
      ToDate: toDate,
      ProductSearch: productSearch?.trim() || null,
      CustomerSearch: customerSearch?.trim() || null,
      ReportView: analysisMode === 'CUSTOMER' ? 'CUSTOMER' : 'PRODUCT',
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
  const normalized = String(sortBy || PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT).toUpperCase()
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME) return 'PRODUCTNAME'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME) return 'CUSTOMERNAME'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CITY) return 'CITY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.BRAND) return 'BRAND'
  if (normalized === PRODUCT_ANALYSIS_SORTS.CATEGORY) return 'CATEGORY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.TOTAL_QTY) return 'TOTALQTY'
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRICE) return 'PRICE'
  return 'TOTALAMOUNT'
}

export function normalizeCompareSort(sortBy) {
  const normalized = String(sortBy || PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT).toUpperCase()
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME) return PRODUCT_ANALYSIS_SORTS.PRODUCT_NAME
  if (normalized === PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME) return PRODUCT_ANALYSIS_SORTS.CUSTOMER_NAME
  if (normalized === PRODUCT_ANALYSIS_SORTS.CITY) return PRODUCT_ANALYSIS_SORTS.CITY
  if (normalized === PRODUCT_ANALYSIS_SORTS.BRAND) return PRODUCT_ANALYSIS_SORTS.BRAND
  if (normalized === PRODUCT_ANALYSIS_SORTS.CATEGORY) return PRODUCT_ANALYSIS_SORTS.CATEGORY
  if (normalized === PRODUCT_ANALYSIS_SORTS.TOTAL_QTY) return PRODUCT_ANALYSIS_SORTS.TOTAL_QTY
  if (normalized === PRODUCT_ANALYSIS_SORTS.PRICE) return PRODUCT_ANALYSIS_SORTS.PRICE
  return PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT
}

export function normalizeSortDirection(sortDirection) {
  return String(sortDirection || 'DESC').toUpperCase() === 'ASC' ? 'ASC' : 'DESC'
}

export function sortProductAnalysisRows(rows, sortBy, sortDirection) {
  const normalizedSort = normalizeCompareSort(sortBy)
  const direction = normalizeSortDirection(sortDirection) === 'ASC' ? 1 : -1

  return rows.slice().sort((a, b) => {
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

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.TOTAL_QTY) {
      return direction * ((a.totalQty || 0) - (b.totalQty || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.PRICE) {
      return direction * ((a.price || 0) - (b.price || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    if (normalizedSort === PRODUCT_ANALYSIS_SORTS.TOTAL_AMOUNT) {
      return direction * ((a.totalAmount || 0) - (b.totalAmount || 0))
        || String(a.productName).localeCompare(String(b.productName))
    }

    return String(a.productName).localeCompare(String(b.productName))
  })
}

export { formatMoney, formatStock }
