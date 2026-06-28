import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { fetchMe, login as apiLogin, logout as apiLogout, setAuthToken } from '../api/client';
import type { AuthSession, AuthUser } from './types';
import { clearSession, loadSession, saveSession } from './storage';

type AuthStatus = 'loading' | 'authenticated' | 'unauthenticated';

type AuthContextValue = {
  status: AuthStatus;
  user: AuthUser | null;
  login: (username: string, password: string, remember: boolean) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [status, setStatus] = useState<AuthStatus>('loading');
  const [user, setUser] = useState<AuthUser | null>(null);

  const applySession = useCallback((session: AuthSession | null) => {
    if (!session) {
      setAuthToken(null);
      setUser(null);
      setStatus('unauthenticated');
      return;
    }
    setAuthToken(session.token);
    setUser(session.user);
    setStatus('authenticated');
  }, []);

  useEffect(() => {
    let cancelled = false;

    const bootstrap = async () => {
      const stored = loadSession();
      if (!stored?.token) {
        if (!cancelled) applySession(null);
        return;
      }

      setAuthToken(stored.token);
      try {
        const { user: me } = await fetchMe();
        if (cancelled) return;
        const session: AuthSession = { ...stored, user: me };
        saveSession(session);
        applySession(session);
      } catch {
        if (!cancelled) {
          clearSession();
          applySession(null);
        }
      }
    };

    void bootstrap();
    return () => {
      cancelled = true;
    };
  }, [applySession]);

  const login = useCallback(
    async (username: string, password: string, remember: boolean) => {
      const result = await apiLogin(username, password, remember);
      const session: AuthSession = {
        token: result.token,
        user: result.user,
        remember,
        expires_at: result.expires_at,
      };
      saveSession(session);
      applySession(session);
    },
    [applySession]
  );

  const logout = useCallback(async () => {
    try {
      await apiLogout();
    } catch {
      /* token may already be invalid */
    }
    clearSession();
    applySession(null);
  }, [applySession]);

  const value = useMemo(
    () => ({ status, user, login, logout }),
    [status, user, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
