import { useState, type FormEvent } from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { BRAND } from '../config/brand';
import { BrandFloor } from '../components/BrandFloor';
import { ThemeToggle } from '../components/ThemeToggle';

export function Login() {
  const { status, login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string } | null)?.from ?? '/';

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [remember, setRemember] = useState(true);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  if (status === 'authenticated') {
    return <Navigate to={from} replace />;
  }

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      await login(username.trim(), password, remember);
      navigate(from, { replace: true });
    } catch {
      setError('Invalid keyword or password.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="login-page login-page--form">
      <div className="login-page-ambient" aria-hidden>
        <span className="app-orb app-orb-1" />
        <span className="app-orb app-orb-2" />
        <span className="app-orb app-orb-3" />
      </div>
      <div className="login-top-actions">
        <ThemeToggle compact />
      </div>
      <div className="login-card login-card--enter">
        <div className="login-brand">
          <span className="cloudeye-mark login-brand-eye" aria-hidden>
            <span className="cloudeye-iris" />
            <span className="cloudeye-shine" />
          </span>
          <div className="login-brand-text">
            <span className="cloudeye-name login-title">{BRAND.productName}</span>
            <span className="cloudeye-client">{BRAND.clientName}</span>
          </div>
        </div>
        <p className="login-sub">Sign in to monitor all branches</p>

        <form className="login-form" onSubmit={onSubmit}>
          <label className="login-field">
            <span>Keyword</span>
            <input
              type="text"
              name="username"
              autoComplete="username"
              autoCapitalize="none"
              autoCorrect="off"
              spellCheck={false}
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Your access keyword"
              required
            />
          </label>
          <label className="login-field">
            <span>Password</span>
            <input
              type="password"
              name="password"
              autoComplete={remember ? 'current-password' : 'password'}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              required
            />
          </label>
          <label className="login-remember">
            <input
              type="checkbox"
              checked={remember}
              onChange={(e) => setRemember(e.target.checked)}
            />
            <span>Keep me signed in until I sign out</span>
          </label>
          {error && (
            <p className="login-error" role="alert">
              {error}
            </p>
          )}
          <button type="submit" className="login-submit" disabled={submitting}>
            {submitting ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
      </div>
      <BrandFloor className="brand-floor--login" />
    </div>
  );
}
