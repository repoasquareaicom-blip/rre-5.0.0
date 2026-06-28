import type { Branch } from '../api/client';

/** Branch sync is active when last successful /api/sync call was within this window. */
export const BRANCH_SYNC_ACTIVE_MINUTES = 10;

/** Parse API wall-clock datetime (`Y-m-d H:i:s`) as local time — no UTC shift. */
function parseWallClockMs(value: string): number {
  const normalized = value.trim().replace(' ', 'T');
  const t = Date.parse(normalized);
  return Number.isNaN(t) ? 0 : t;
}

export function isBranchSyncActive(branch: Branch, now = Date.now()): boolean {
  if (!branch.last_sync_at) return false;
  const lastSync = parseWallClockMs(branch.last_sync_at);
  if (!lastSync) return false;
  return now - lastSync < BRANCH_SYNC_ACTIVE_MINUTES * 60 * 1000;
}

export function branchSyncStatusLabel(branch: Branch): string {
  return isBranchSyncActive(branch) ? 'Active' : 'Inactive';
}
