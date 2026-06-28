import type { Branch } from '../api/client';
import { isBranchSyncActive } from '../utils/branchStatus';

/**
 * Optional overrides per branch code until the branch service app reports status.
 * Example: { 'RR-KOLATHUR': false }
 * Leave empty to use last sync time (active within 10 minutes).
 */
export const BRANCH_CLIENT_ONLINE_OVERRIDE: Record<string, boolean> = {
  // 'RR-SALEM': true,
  // 'RR-NAMAKKAL': true,
  // 'RR-KOLATHUR': false,
};

/** Branch sync active when last successful /api/sync call was within 10 minutes. */
export function isBranchClientOnline(branch: Branch): boolean {
  const code = branch.code?.toUpperCase();
  if (code && code in BRANCH_CLIENT_ONLINE_OVERRIDE) {
    return BRANCH_CLIENT_ONLINE_OVERRIDE[code];
  }
  if (branch.last_sync_at) {
    return isBranchSyncActive(branch);
  }
  return branch.client_online ?? branch.server_online ?? false;
}
