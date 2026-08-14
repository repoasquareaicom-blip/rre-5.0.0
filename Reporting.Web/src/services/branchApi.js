const REPORTING_API_KEY = import.meta.env.VITE_BRANCH_API_KEY || ''
const REQUEST_TIMEOUT_MS = 18000

export class BranchApiError extends Error {
  constructor(message, status = 0) {
    super(message)
    this.name = 'BranchApiError'
    this.status = status
  }
}

function createTimeoutSignal(signal, timeoutMs) {
  const controller = new AbortController()
  const timeoutId = window.setTimeout(() => controller.abort(), timeoutMs)

  const abortFromParent = () => controller.abort()
  if (signal) {
    if (signal.aborted) {
      controller.abort()
    } else {
      signal.addEventListener('abort', abortFromParent, { once: true })
    }
  }

  return {
    signal: controller.signal,
    cleanup: () => {
      window.clearTimeout(timeoutId)
      signal?.removeEventListener('abort', abortFromParent)
    },
  }
}

export async function runBranchReport(branch, queryText, parameters, options = {}) {
  const timeout = createTimeoutSignal(options.signal, options.timeoutMs || REQUEST_TIMEOUT_MS)
  const requestUrl = `${branch.baseUrl}/api/getdata`

  try {
    console.info('[BranchApi] report request', {
      url: requestUrl,
      branchCode: branch.id,
      queryText,
      parameters,
    })

    const response = await fetch(requestUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Api-Key': REPORTING_API_KEY,
      },
      body: JSON.stringify({ queryText, parameters }),
      signal: timeout.signal,
    })

    const responseBody = await response.text()
    console.info('[BranchApi] report response', {
      url: requestUrl,
      status: response.status,
      ok: response.ok,
      body: responseBody,
    })

    let payload = null
    try {
      payload = responseBody ? JSON.parse(responseBody) : null
    } catch {
      throw new BranchApiError('Invalid response received from branch API.', response.status)
    }

    if (response.status === 401) {
      throw new BranchApiError('Reporting API authorization failed.', response.status)
    }

    if (!response.ok) {
      throw new BranchApiError(payload?.message || 'Branch report request failed.', response.status)
    }

    if (payload?.success === false) {
      throw new BranchApiError(payload.message || 'Branch report request failed.', response.status)
    }

    if (!Array.isArray(payload?.data)) {
      throw new BranchApiError('Invalid report data received from branch API.', response.status)
    }

    return payload
  } catch (error) {
    if (error.name === 'AbortError') {
      throw error
    }

    if (error instanceof BranchApiError) {
      console.error('[BranchApi] report error', {
        url: requestUrl,
        status: error.status || null,
        message: error.message,
      })
      throw error
    }

    console.error('[BranchApi] report network or CORS error', {
      url: requestUrl,
      message: error.message,
      error,
    })

    throw new BranchApiError(`${branch.label} branch is currently offline or unavailable.`)
  } finally {
    timeout.cleanup()
  }
}
