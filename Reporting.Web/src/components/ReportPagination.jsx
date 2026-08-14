function ReportPagination({ pageNumber, totalPages, totalRows, pageSize, onPageChange }) {
  const firstRow = totalRows === 0 ? 0 : (pageNumber - 1) * pageSize + 1
  const lastRow = Math.min(pageNumber * pageSize, totalRows)

  return (
    <div className="pagination-row">
      <p>
        {totalRows === 0
          ? 'No rows'
          : `${firstRow.toLocaleString('en-IN')} - ${lastRow.toLocaleString('en-IN')} of ${totalRows.toLocaleString('en-IN')}`}
      </p>
      <div className="pagination-controls">
        <button
          type="button"
          onClick={() => onPageChange(pageNumber - 1)}
          disabled={pageNumber <= 1}
        >
          Previous
        </button>
        <span>
          Page {pageNumber} of {totalPages}
        </span>
        <button
          type="button"
          onClick={() => onPageChange(pageNumber + 1)}
          disabled={pageNumber >= totalPages}
        >
          Next
        </button>
      </div>
    </div>
  )
}

export default ReportPagination
