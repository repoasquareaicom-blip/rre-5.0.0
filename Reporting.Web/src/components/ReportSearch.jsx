function ReportSearch({ value, onChange, placeholder = 'Product or display name' }) {
  return (
    <div className="field-group search-field">
      <label htmlFor="report-search">Search</label>
      <input
        id="report-search"
        type="search"
        value={value}
        placeholder={placeholder}
        onChange={(event) => onChange(event.target.value)}
      />
    </div>
  )
}

export default ReportSearch
