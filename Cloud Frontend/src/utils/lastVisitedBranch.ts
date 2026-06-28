import type { Branch } from '../api/client';

const KEY = 'rr-cloudeye-last-branch';

export function loadLastVisitedBranchCode(): string | null {
  const raw = localStorage.getItem(KEY);
  return raw && raw.length > 0 ? raw : null;
}

export function saveLastVisitedBranchCode(code: string): void {
  localStorage.setItem(KEY, code);
}

/** Pick last visited branch id, or first branch when none saved / not found. */
export function resolveLastVisitedBranchId(
  branchList: Branch[],
  savedCode: string | null
): number | null {
  if (!branchList.length) return null;
  if (savedCode) {
    const found = branchList.find((b) => b.code === savedCode);
    if (found) return found.id;
  }
  return branchList[0].id;
}
