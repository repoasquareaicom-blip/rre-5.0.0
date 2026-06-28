import type { AuthSession } from './types';

const KEY = 'rr-cloudeye-auth';

export function loadSession(): AuthSession | null {
  const raw = localStorage.getItem(KEY) ?? sessionStorage.getItem(KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as AuthSession;
    if (!parsed?.token || !parsed?.user?.username) return null;
    if (parsed.expires_at) {
      const expires = Date.parse(parsed.expires_at);
      if (!Number.isNaN(expires) && expires <= Date.now()) return null;
    }
    return parsed;
  } catch {
    return null;
  }
}

export function saveSession(session: AuthSession): void {
  const payload = JSON.stringify(session);
  clearSession();
  if (session.remember) {
    localStorage.setItem(KEY, payload);
  } else {
    sessionStorage.setItem(KEY, payload);
  }
}

export function clearSession(): void {
  localStorage.removeItem(KEY);
  sessionStorage.removeItem(KEY);
}
