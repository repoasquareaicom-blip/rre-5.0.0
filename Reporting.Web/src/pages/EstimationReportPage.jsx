import TransactionReportPage from './TransactionReportPage'
import {
  emptyEstimationSummary,
  fetchEstimationDetail,
  fetchEstimationReport,
  formatMoney,
  formatStock,
} from '../services/estimationReport'

const config = {
  id: 'estimation',
  pageClass: 'estimation-report-page',
  kicker: 'Estimation Report',
  title: 'Estimations',
  entityLabel: 'Estimation',
  idField: 'estimationId',
  searchPlaceholder: 'Estimation, quotation, customer, city, assist',
  emptySummary: emptyEstimationSummary,
  fetchReport: fetchEstimationReport,
  fetchDetail: (branch, row, signal) => fetchEstimationDetail(branch, row.estimationId, signal),
  exportPrefix: 'Estimation_Report',
  exportSheetName: 'Estimation Report',
  tableColumns: [
    { key: 'estimationId', label: 'Estimation ID', link: true, render: (row) => row.estimationId },
    { key: 'customerName', label: 'Customer', render: (row) => row.customerName },
    { key: 'itemCount', label: 'Items', numeric: true, render: (row) => formatStock(row.itemCount) },
    { key: 'totalQuantity', label: 'Quantity', numeric: true, render: (row) => formatStock(row.totalQuantity) },
    { key: 'taxableValue', label: 'Taxable', numeric: true, render: (row) => formatMoney(row.taxableValue) },
    { key: 'gstAmount', label: 'GST', numeric: true, render: (row) => formatMoney(row.gstAmount) },
    { key: 'estimationValue', label: 'Value', numeric: true, render: (row) => formatMoney(row.estimationValue) },
  ],
  exportColumns: [
    { header: 'S.No', value: (row) => row.serialNumber },
    { header: 'Branch', value: (row) => row.branchName },
    { header: 'Estimation ID', value: (row) => row.estimationId },
    { header: 'Customer', value: (row) => row.customerName },
    { header: 'Items', value: (row) => row.itemCount },
    { header: 'Quantity', value: (row) => row.totalQuantity },
    { header: 'Taxable', value: (row) => row.taxableValue },
    { header: 'GST', value: (row) => row.gstAmount },
    { header: 'Value', value: (row) => row.estimationValue },
  ],
  getSummaryCards: (summary) => [
    { label: 'Nos', value: formatStock(summary.totalEstimations) },
    { label: 'Taxable', value: formatMoney(summary.taxableValue) },
    { label: 'GST', value: formatMoney(summary.gstAmount) },
    { label: 'Value', value: formatMoney(summary.estimationValue) },
  ],
  sumSummary: (total, { result }) => ({
    totalEstimations: total.totalEstimations + result.summary.totalEstimations,
    taxableValue: total.taxableValue + result.summary.taxableValue,
    gstAmount: total.gstAmount + result.summary.gstAmount,
    estimationValue: total.estimationValue + result.summary.estimationValue,
    cancelled: total.cancelled + result.summary.cancelled,
  }),
  mobileValue: (row) => formatMoney(row.estimationValue),
  mobileMeta: (row) => `Items ${formatStock(row.itemCount)} / Qty ${formatStock(row.totalQuantity)}`,
  detailSubline: (row) => `${row.city} / ${row.updatedOnLabel}`,
}

function EstimationReportPage(props) {
  return <TransactionReportPage config={config} {...props} />
}

export default EstimationReportPage
