import { branches } from '../config/branches'

function BranchSelector({ selectedBranchId, onChange }) {
  return (
    <div className="field-group branch-field">
      <label id="branch-label">Branch</label>
      <div className="branch-segmented" role="radiogroup" aria-labelledby="branch-label">
        {branches.map((branch) => (
          <button
            key={branch.id}
            type="button"
            className={branch.id === selectedBranchId ? 'branch-option is-selected' : 'branch-option'}
            onClick={() => onChange(branch.id)}
            role="radio"
            aria-checked={branch.id === selectedBranchId}
          >
            {branch.label}
          </button>
        ))}
      </div>
      <select
        id="branch"
        className="branch-select"
        value={selectedBranchId}
        onChange={(event) => onChange(event.target.value)}
      >
        {branches.map((branch) => (
          <option key={branch.id} value={branch.id}>
            {branch.label}
          </option>
        ))}
      </select>
    </div>
  )
}

export default BranchSelector
