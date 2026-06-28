import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { AuthProvider } from './auth/AuthContext'
import { ThemeProvider } from './theme/ThemeProvider'
import './index.css'
import './theme/premium.css'
import './theme/cloudeye-brand.css'
import './theme/vivid.css'
import './theme/type-select.css'
import './theme/login.css'
import './theme/splash.css'
import './theme/brand-floor.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider>
      <AuthProvider>
        <App />
      </AuthProvider>
    </ThemeProvider>
  </StrictMode>,
)
