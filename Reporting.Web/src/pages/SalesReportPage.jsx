import TransactionReportPage from './TransactionReportPage'
import {
  emptySalesSummary,
  fetchSalesDetail,
  fetchSalesReport,
  formatMoney,
  formatStock,
  salesDivisions,
} from '../services/salesReport'

function formatTax(value) {
  return value ? formatMoney(value) : '-'
}

function SalesIdCell({ row }) {
  return <span className="sales-id-cell"><span>{row.salesId}</span></span>
}

function SalesDetailFooter(data) {
  const isIgst = data.totals.isIgst

  return (
    <footer className="quotation-detail-footer sales-detail-footer">
      {data.gstSummary.length > 0 && (
        <section className="gst-summary-section">
          <h3>GST Summary</h3>
          <table className="gst-summary-table">
            <thead>
              <tr>
                <th className="numeric">GST %</th>
                <th className="numeric">Taxable</th>
                {isIgst ? (
                  <th className="numeric">IGST</th>
                ) : (
                  <>
                    <th className="numeric">CGST</th>
                    <th className="numeric">SGST</th>
                  </>
                )}
                <th className="numeric">Total GST</th>
              </tr>
            </thead>
            <tbody>
              {data.gstSummary.map((row) => (
                <tr key={row.id}>
                  <td className="numeric">{formatStock(row.gst)}%</td>
                  <td className="numeric">{formatMoney(row.taxableValue)}</td>
                  {isIgst ? (
                    <td className="numeric">{formatMoney(row.igstAmount)}</td>
                  ) : (
                    <>
                      <td className="numeric">{formatMoney(row.cgstAmount)}</td>
                      <td className="numeric">{formatMoney(row.sgstAmount)}</td>
                    </>
                  )}
                  <td className="numeric">{formatMoney(row.gstAmount)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      )}
      <div className="bill-total-section">
        <div>
          <span>Taxable Value</span>
          <strong>{formatMoney(data.totals.taxableValue)}</strong>
        </div>
        {isIgst ? (
          <div>
            <span>IGST</span>
            <strong>{formatMoney(data.totals.igstAmount)}</strong>
          </div>
        ) : (
          <>
            <div>
              <span>CGST</span>
              <strong>{formatMoney(data.totals.cgstAmount)}</strong>
            </div>
            <div>
              <span>SGST</span>
              <strong>{formatMoney(data.totals.sgstAmount)}</strong>
            </div>
          </>
        )}
        <div>
          <span>GST Total</span>
          <strong>{formatMoney(data.totals.gstAmount)}</strong>
        </div>
        <div>
          <span>Less Amount</span>
          <strong>{formatMoney(data.totals.lessAmount)}</strong>
        </div>
        <div>
          <span>Others</span>
          <strong>{formatMoney(data.totals.others)}</strong>
        </div>
        <div className="grand-total-line">
          <span>Grand Total</span>
          <strong>{formatMoney(data.totals.grandTotal)}</strong>
        </div>
      </div>
    </footer>
  )
}

const config = {
  id: 'sales',
  kicker: 'Sales Report',
  title: 'Sales',
  pageClass: 'sales-report-page',
  entityLabel: 'Sales',
  idField: 'salesId',
  searchPlaceholder: 'Sales ID, customer, city',
  defaultFilters: { divisionCode: '' },
  emptySummary: emptySalesSummary,
  alwaysShowBranchColumn: true,
  tableClass: 'sales-report-table',
  fetchReport: fetchSalesReport,
  fetchDetail: (branch, row, signal) => fetchSalesDetail(branch, row.salesId, row.divisionCode, signal),
  exportPrefix: 'Sales_Report',
  exportSheetName: 'Sales Report',
  extraFilters: [
    {
      name: 'divisionCode',
      label: 'Division',
      options: salesDivisions,
    },
  ],
  tableColumns: [
    { key: 'divisionName', label: 'Division', render: (row) => row.divisionName },
    { key: 'salesId', label: 'Sales ID', link: true, render: (row) => <SalesIdCell row={row} /> },
    { key: 'customerName', label: 'Customer', render: (row) => row.customerName },
    { key: 'paymentMode', label: 'Payment Mode', render: (row) => row.paymentMode },
    { key: 'itemCount', label: 'Items', numeric: true, render: (row) => formatStock(row.itemCount) },
    { key: 'totalQuantity', label: 'Quantity', numeric: true, render: (row) => formatStock(row.totalQuantity) },
    { key: 'taxableValue', label: 'Taxable', numeric: true, render: (row) => formatMoney(row.taxableValue) },
    { key: 'cgstAmount', label: 'CGST', numeric: true, render: (row) => formatTax(row.cgstAmount) },
    { key: 'sgstAmount', label: 'SGST', numeric: true, render: (row) => formatTax(row.sgstAmount) },
    { key: 'igstAmount', label: 'IGST', numeric: true, render: (row) => formatTax(row.igstAmount) },
    { key: 'salesValue', label: 'Value', numeric: true, render: (row) => formatMoney(row.salesValue) },
  ],
  detailColumns: [
    { key: 'productName', label: 'Product Name', className: 'detail-col-product sales-detail-col-product', render: (line) => <strong>{line.productName}</strong> },
    { key: 'quantity', label: 'Qty', numeric: true, className: 'sales-detail-col-qty', render: (line) => formatStock(line.quantity) },
    { key: 'rate', label: 'Rate', numeric: true, className: 'sales-detail-col-rate', render: (line) => formatMoney(line.rate) },
    { key: 'taxableValue', label: 'Taxable', numeric: true, className: 'sales-detail-col-taxable', render: (line) => formatMoney(line.taxableValue) },
    { key: 'gst', label: 'GST %', numeric: true, className: 'sales-detail-col-gst', render: (line) => `${formatStock(line.gst)}%` },
    { key: 'cgstAmount', label: 'CGST', numeric: true, className: 'sales-detail-col-tax', render: (line) => formatTax(line.cgstAmount) },
    { key: 'sgstAmount', label: 'SGST', numeric: true, className: 'sales-detail-col-tax', render: (line) => formatTax(line.sgstAmount) },
    { key: 'igstAmount', label: 'IGST', numeric: true, className: 'sales-detail-col-tax', render: (line) => formatTax(line.igstAmount) },
    { key: 'amount', label: 'Amount', numeric: true, className: 'sales-detail-col-amount', render: (line) => formatMoney(line.amount) },
  ],
  renderDetailFooter: SalesDetailFooter,
  exportColumns: [
    { header: 'S.No', value: (row) => row.serialNumber },
    { header: 'Branch', value: (row) => row.branchName },
    { header: 'Division', value: (row) => row.divisionName },
    { header: 'Sales ID', value: (row) => row.salesId },
    { header: 'Customer', value: (row) => row.customerName },
    { header: 'Payment Mode', value: (row) => row.paymentMode },
    { header: 'Items', value: (row) => row.itemCount },
    { header: 'Quantity', value: (row) => row.totalQuantity },
    { header: 'Taxable', value: (row) => row.taxableValue },
    { header: 'CGST', value: (row) => row.cgstAmount },
    { header: 'SGST', value: (row) => row.sgstAmount },
    { header: 'IGST', value: (row) => row.igstAmount },
    { header: 'GST Total', value: (row) => row.gstAmount },
    { header: 'Grand Total', value: (row) => row.salesValue },
  ],
  getSummaryCards: (summary) => [
    { label: 'Nos', value: formatStock(summary.totalSales) },
    { label: 'Value', value: formatMoney(summary.salesValue) },
    { label: 'Taxable', value: formatMoney(summary.taxableValue) },
    { label: 'GST', value: formatMoney(summary.gstAmount) },
  ],
  sumSummary: (total, { result }) => ({
    totalSales: total.totalSales + result.summary.totalSales,
    taxableValue: total.taxableValue + result.summary.taxableValue,
    cgstAmount: total.cgstAmount + result.summary.cgstAmount,
    sgstAmount: total.sgstAmount + result.summary.sgstAmount,
    igstAmount: total.igstAmount + result.summary.igstAmount,
    gstAmount: total.gstAmount + result.summary.gstAmount,
    salesValue: total.salesValue + result.summary.salesValue,
    igstInvoices: total.igstInvoices + result.summary.igstInvoices,
  }),
  mobileValue: (row) => formatMoney(row.salesValue),
  mobileMeta: (row) => `${row.divisionName} / ${row.paymentMode} / Items ${formatStock(row.itemCount)} / Qty ${formatStock(row.totalQuantity)}`,
  detailSubline: (row) => `${row.paymentMode} / ${row.divisionName} / ${row.city} / ${row.updatedOnLabel}`,
}

function SalesReportPage(props) {
  return <TransactionReportPage config={config} {...props} />
}

export default SalesReportPage
