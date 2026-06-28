import type { Branch } from '../api/client';

const KEY = 'rr-cloudeye-branch-selection';

type StoredBranchSelection = {
  selectedCodes: string[];
};

/** Load saved branch codes, or `null` if none / invalid (default = all branches). */
export function loadBranchSelection(): string[] | null {
  const raw = localStorage.getItem(KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as StoredBranchSelection;
    if (!Array.isArray(parsed?.selectedCodes)) return null;
    const codes = parsed.selectedCodes.filter((c) => typeof c === 'string' && c.length > 0);
    return codes.length > 0 ? codes : null;
  } catch {
    return null;
  }
}

/** Persist which branches are selected (by stable branch `code`). */
export function saveBranchSelection(selectedCodes: string[]): void {
  if (!selectedCodes.length) {
    localStorage.removeItem(KEY);
    return;
  }
  const payload: StoredBranchSelection = { selectedCodes };
  localStorage.setItem(KEY, JSON.stringify(payload));
}

/** Map stored codes → branch ids; default all branches when nothing saved. */
export function resolveSelectedBranchIds(
  branchList: Branch[],
  savedCodes: string[] | null
): Set<number> {
  const allIds = branchList.map((b) => b.id);
  if (!allIds.length) return new Set<number>();

  const all = new Set(allIds);
  if (!savedCodes?.length) return all;

  const picked = new Set(
    branchList.filter((b) => savedCodes.includes(b.code)).map((b) => b.id)
  );

  return picked.size > 0 ? picked : all;
}
