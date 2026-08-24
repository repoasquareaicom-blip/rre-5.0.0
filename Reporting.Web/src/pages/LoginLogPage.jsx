import { useMemo } from 'react'
import { getRecentLoginLogs } from '../services/loginLog'

const dateFormatter = new Intl.DateTimeFormat('en-IN', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  second: '2-digit',
})

function formatLoginDate(value) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return value
  }

  return dateFormatter.format(date)
}

function LoginLogPage() {
  const rows = useMemo(() => getRecentLoginLogs(), [])

  return (
    <section className="report-page login-log-page">
      <div className="report-title-row">
        <div>
          <p className="section-kicker">Access</p>
          <h2>Login Log</h2>
        </div>
        <div className="report-meta">
          <span>{rows.length} recent login{rows.length === 1 ? '' : 's'}</span>
        </div>
      </div>

      <div className="report-surface login-log-surface">
        {rows.length === 0 ? (
          <div className="empty-state">
            <h3>No login records yet.</h3>
          </div>
        ) : (
          <div className="table-wrap">
            <table className="stock-table login-log-table">
              <thead>
                <tr>
                  <th>Last Login Date</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={row.id || row.loginAt}>
                    <td>{formatLoginDate(row.loginAt)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </section>
  )
}

export default LoginLogPage
