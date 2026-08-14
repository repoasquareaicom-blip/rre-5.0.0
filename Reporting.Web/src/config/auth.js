export const AUTH_STORAGE_KEY = 'rr.reporting.authenticated'

export const ACCESS_CODE = import.meta.env.VITE_REPORTING_ACCESS_CODE || 'rre'

export function isValidAccessCode(value) {
  return value.trim() === ACCESS_CODE
}
