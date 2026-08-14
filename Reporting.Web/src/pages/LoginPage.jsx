import { useState } from 'react'
import { loginWithAccessCode } from '../services/auth'

function LoginPage({ onLogin }) {
  const [accessCode, setAccessCode] = useState('')
  const [error, setError] = useState('')

  function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!loginWithAccessCode(accessCode)) {
      setError('Invalid access code.')
      return
    }

    onLogin()
  }

  return (
    <main className="login-page">
      <section className="login-panel" aria-labelledby="login-title">
        <div className="brand-mark large">RR</div>
        <p className="brand-kicker">RRE Connect</p>
        <h1 id="login-title">Reporting Login</h1>
        <form onSubmit={handleSubmit}>
          <div className="field-group">
            <label htmlFor="access-code">Access Code</label>
            <input
              id="access-code"
              type="password"
              value={accessCode}
              autoComplete="current-password"
              onChange={(event) => setAccessCode(event.target.value)}
            />
          </div>
          {error && <p className="form-error">{error}</p>}
          <button type="submit" className="primary-button">
            Login
          </button>
        </form>
      </section>
    </main>
  )
}

export default LoginPage
