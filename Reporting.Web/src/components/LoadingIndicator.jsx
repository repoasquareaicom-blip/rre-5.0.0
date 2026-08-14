function LoadingIndicator({ label = 'Loading stock...', overlay = false, steps = [] }) {
  if (steps.length > 0) {
    return (
      <div className={overlay ? 'loading-indicator is-overlay' : 'loading-indicator'} role="status">
        <div className="loading-panel">
          <span className="loading-spinner" aria-hidden="true" />
          <div className="loading-copy">
            <strong>{label}</strong>
            <ol className="loading-steps">
              {steps.map((step) => (
                <li className={`loading-step is-${step.status || 'pending'}`} key={step.id || step.label}>
                  <span className="step-dot" aria-hidden="true" />
                  <span>{step.label}</span>
                  <small>{getStepStatusLabel(step.status)}</small>
                </li>
              ))}
            </ol>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className={overlay ? 'loading-indicator is-overlay' : 'loading-indicator'} role="status">
      <span className="loading-spinner" aria-hidden="true" />
      <span>{label}</span>
    </div>
  )
}

function getStepStatusLabel(status) {
  if (status === 'checking') {
    return 'Checking'
  }

  if (status === 'active') {
    return 'Running'
  }

  if (status === 'passed') {
    return 'Passed'
  }

  if (status === 'offline') {
    return 'Offline'
  }

  return 'Waiting'
}

export default LoadingIndicator
