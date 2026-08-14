import { AUTH_STORAGE_KEY, isValidAccessCode } from '../config/auth'

export function loginWithAccessCode(accessCode) {
  if (!isValidAccessCode(accessCode)) {
    return false
  }

  sessionStorage.setItem(AUTH_STORAGE_KEY, 'true')
  return true
}

export function isLoggedIn() {
  return sessionStorage.getItem(AUTH_STORAGE_KEY) === 'true'
}

export function logout() {
  sessionStorage.removeItem(AUTH_STORAGE_KEY)
}
