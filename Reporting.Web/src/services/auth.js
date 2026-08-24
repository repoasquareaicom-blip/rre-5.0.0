import { ACCESS_LEVELS, AUTH_STORAGE_KEY, LOGIN_MESSAGES } from '../config/auth'
import { defaultBranchId, getBranchById } from '../config/branches'
import { runBranchReport } from './branchApi'
import { recordLogin } from './loginLog'

const PRODUCT_ANALYSIS_ROUTE = '/reports/product-analysis'
const FULL_ACCESS_LANDING_ROUTE = '/reports/stock'
const LOGIN_PROCEDURE = 'Getlogindetails'

function readStoredSession() {
  try {
    const stored = sessionStorage.getItem(AUTH_STORAGE_KEY)
    if (!stored) {
      return null
    }

    const session = JSON.parse(stored)
    if (
      session?.accessLevel !== ACCESS_LEVELS.fullAccess &&
      session?.accessLevel !== ACCESS_LEVELS.productAnalysisOnly
    ) {
      return null
    }

    return session
  } catch {
    return null
  }
}

function storeSession(payload) {
  const session = {
    userId: payload.userId || '',
    username: payload.username || '',
    role: payload.role || '',
    branchCode: payload.branchCode || '',
    accessLevel: payload.accessLevel,
  }

  sessionStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session))
  recordLogin()
  return session
}

function normalizeText(value) {
  return String(value || '').replace(/\s+/g, ' ').trim()
}

function deriveAccessLevel(username, role) {
  const normalizedUsername = normalizeText(username).toLowerCase()
  const normalizedRole = normalizeText(role).toLowerCase()

  if (normalizedUsername === 'vanitha') {
    return ACCESS_LEVELS.productAnalysisOnly
  }

  if (normalizedRole === 'admin') {
    return ACCESS_LEVELS.fullAccess
  }

  if (normalizedRole === 'sales order') {
    return ACCESS_LEVELS.productAnalysisOnly
  }

  return ACCESS_LEVELS.denied
}

function createSessionFromUser(row, branchCode) {
  const username = row.UserName || row.username || row.UserFullName || ''
  const role = row.URole || row.uRole || row.role || ''
  const accessLevel = deriveAccessLevel(username, role)

  if (accessLevel === ACCESS_LEVELS.denied) {
    throw new Error(LOGIN_MESSAGES.unauthorized)
  }

  return {
    userId: row.UserId || row.userId || '',
    username,
    role,
    branchCode,
    accessLevel,
  }
}

export async function loginWithCredentials(username, password) {
  const branch = getBranchById(defaultBranchId)
  const payload = await runBranchReport(
    branch,
    LOGIN_PROCEDURE,
    {
      UserName: username.trim(),
      Password: password,
    },
    { timeoutMs: 12000 },
  )

  const user = payload.data[0]
  if (!user) {
    throw new Error(LOGIN_MESSAGES.invalid)
  }

  return storeSession(createSessionFromUser(user, payload.branchCode || branch.id))
}

export function getAuthSession() {
  return readStoredSession()
}

export function isLoggedIn() {
  return Boolean(readStoredSession())
}

export function getLandingRoute(accessLevel) {
  return accessLevel === ACCESS_LEVELS.productAnalysisOnly
    ? PRODUCT_ANALYSIS_ROUTE
    : FULL_ACCESS_LANDING_ROUTE
}

export function hasFullAccess(session) {
  return session?.accessLevel === ACCESS_LEVELS.fullAccess
}

export function canAccessRoute(session, path) {
  if (!session) {
    return false
  }

  if (hasFullAccess(session)) {
    return true
  }

  return session.accessLevel === ACCESS_LEVELS.productAnalysisOnly && path === PRODUCT_ANALYSIS_ROUTE
}

export function logout() {
  sessionStorage.removeItem(AUTH_STORAGE_KEY)
}
