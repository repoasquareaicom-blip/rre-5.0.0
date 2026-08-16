const defaultPageSizes = [10, 25, 50, 100]

function PageSizeSelector({ value, onChange, pageSizes = defaultPageSizes }) {
  return (
    <div className="field-group size-field">
      <label htmlFor="page-size">Page Size</label>
      <select
        id="page-size"
        value={value}
        onChange={(event) => onChange(Number(event.target.value))}
      >
        {pageSizes.map((size) => (
          <option key={size} value={size}>
            {size}
          </option>
        ))}
      </select>
    </div>
  )
}

export default PageSizeSelector
