import { useEffect, useState, type ReactNode } from 'react';
import { useAuth } from '../auth/AuthContext';
import { SPLASH_EXIT_MS, SPLASH_MIN_MS } from '../auth/splash';
import { SplashScreen } from './SplashScreen';

type GatePhase = 'splash' | 'exit' | 'app';

export function AppSplashGate({ children }: { children: ReactNode }) {
  const { status } = useAuth();
  const [phase, setPhase] = useState<GatePhase>('splash');
  const [minElapsed, setMinElapsed] = useState(false);

  useEffect(() => {
    const timer = window.setTimeout(() => setMinElapsed(true), SPLASH_MIN_MS);
    return () => window.clearTimeout(timer);
  }, []);

  useEffect(() => {
    if (phase !== 'splash') return;
    if (!minElapsed || status === 'loading') return;
    setPhase('exit');
  }, [minElapsed, status, phase]);

  useEffect(() => {
    if (phase !== 'exit') return;
    const timer = window.setTimeout(() => setPhase('app'), SPLASH_EXIT_MS);
    return () => window.clearTimeout(timer);
  }, [phase]);

  if (phase !== 'app') {
    return <SplashScreen exiting={phase === 'exit'} />;
  }

  return children;
}
