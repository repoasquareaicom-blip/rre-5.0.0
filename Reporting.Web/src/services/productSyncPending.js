import { getBranchById } from '../config/branches'

const REPORTING_API_KEY = import.meta.env.VITE_BRANCH_API_KEY || ''
const SYNC_PENDING_QUERY = 'dbo.sp_report_product_sync_pending'
const SALEM_BRANCH_ID = 'RR-SALEM'

function getValue(row, names) {
  for (const name of names) {
    if (Object.prototype.hasOwnProperty.call(row, name)) {
      return row[name]
    }
  }

  const entries = Object.entries(row)
  const match = entries.find(([key]) => names.some((name) => key.toLowerCase() === name.toLowerCase()))
  return match?.[1]
}

function toNumber(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }

  const numberValue = Number(value)
  return Number.isFinite(numberValue) ? numberValue : null
}

function normalizeText(value) {
  if (value === null || value === undefined) {
    return ''
  }

  return String(value).trim()
}

export function normalizeSyncPendingRows(rows) {
  return rows.map((row, index) => {
    const queueId = getValue(row, ['QueueId', 'queueId', 'QUEUEID'])
    const productId = getValue(row, ['ProductId', 'productId', 'PRODUCTID'])

    return {
      id: `${queueId || productId || 'sync-row'}-${index}`,
      queueId,
      productId,
      itemName: normalizeText(getValue(row, ['ItemName', 'itemName', 'ITEMNAME'])) || '-',
      salesPrice: toNumber(getValue(row, ['SalesPrice', 'salesPrice', 'SALESPRICE', 'SalePrice', 'salePrice'])),
      mrp: toNumber(getValue(row, ['MRP', 'mrp', 'Mrp'])),
      gst: toNumber(getValue(row, ['GST', 'gst', 'Gst'])),
      changeType: normalizeText(getValue(row, ['ChangeType', 'changeType', 'CHANGETYPE'])),
      status: normalizeText(getValue(row, ['Status', 'status', 'STATUS'])) || 'Pending',
      attemptCount: toNumber(getValue(row, ['AttemptCount', 'attemptCount', 'ATTEMPTCOUNT'])) || 0,
      lastError: normalizeText(getValue(row, ['LastError', 'lastError', 'LASTERROR'])),
      createdOn: getValue(row, ['CreatedOn', 'createdOn', 'CREATEDON']),
      modifiedOn: getValue(row, ['ModifiedOn', 'modifiedOn', 'MODIFIEDON']),
      lastTriedOn: getValue(row, ['LastTriedOn', 'lastTriedOn', 'LASTTRIEDON']),
      syncedOn: getValue(row, ['SyncedOn', 'syncedOn', 'SYNCEDON']),
      targetBranchCode: normalizeText(
        getValue(row, ['TargetBranchCode', 'targetBranchCode', 'TARGETBRANCHCODE']),
      ),
    }
  })
}

export async function fetchProductSyncPending(signal) {
  const salemBranch = getBranchById(SALEM_BRANCH_ID)
  const requestUrl = `${salemBranch.baseUrl}/api/getdata`

  const response = await fetch(requestUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-Api-Key': REPORTING_API_KEY,
    },
    body: JSON.stringify({
      queryText: SYNC_PENDING_QUERY,
      parameters: {},
    }),
    signal,
  })

  const responseBody = await response.text()
  let payload = null

  try {
    payload = responseBody ? JSON.parse(responseBody) : null
  } catch {
    throw new Error('Invalid sync status response received from Salem.')
  }

  if (!response.ok || payload?.success === false) {
    throw new Error(payload?.message || 'Sync status request failed.')
  }

  if (!Array.isArray(payload?.data)) {
    throw new Error('Invalid sync pending rows received from Salem.')
  }

  return normalizeSyncPendingRows(payload.data)
}
