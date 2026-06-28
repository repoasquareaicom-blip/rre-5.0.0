import type { CSSProperties } from 'react';
import { branchLoggedInUserCount, type Branch } from '../api/client';
import { isBranchClientOnline } from '../config/branchClient';
import { branchSyncStatusLabel } from '../utils/branchStatus';

type Props = {
  branches: Branch[];
  selectedIds: Set<number>;
  onToggle: (branchId: number) => void;
  onSelectAll: () => void;
  /** @deprecated use variant */
  compact?: boolean;
  variant?: 'row' | 'inline';
};

const ACCENTS: [string, string][] = [
  ['#6366f1', '#818cf8'],
  ['#0d9488', '#2dd4bf'],
  ['#d97706', '#fbbf24'],
  ['#db2777', '#f472b6'],
  ['#7c3aed', '#a78bfa'],
  ['#0284c7', '#38bdf8'],
];

export function branchCode(branch: Branch): string {
  const code = branch.short_code?.trim().toUpperCase();
  if (code && code.length >= 3) return code.slice(0, 3);
  const segment = branch.code.split('-').pop() ?? branch.code;
  return segment.replace(/[^A-Za-z]/g, '').slice(0, 3).toUpperCase().padEnd(3, 'X');
}

function branchAccent(branchId: number): [string, string] {
  return ACCENTS[branchId % ACCENTS.length];
}

function syncHint(lastSyncAt?: string | null): string {
  if (!lastSyncAt) return 'No sync yet';
  const mins = Math.floor((Date.now() - new Date(lastSyncAt).getTime()) / 60000);
  if (mins < 1) return 'Synced just now';
  if (mins < 60) return `Synced ${mins}m ago`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `Synced ${hrs}h ago`;
  return `Synced ${Math.floor(hrs / 24)}d ago`;
}

export function BranchServerBar({
  branches,
  selectedIds,
  onToggle,
  onSelectAll,
  compact,
  variant = compact ? 'inline' : 'row',
}: Props) {
  const allSelected = branches.length > 0 && branches.every((b) => selectedIds.has(b.id));
  const isRow = variant === 'row';

  return (
    <section
      className={`location-bar ${isRow ? 'location-bar--row' : 'location-bar--inline'}`}
      aria-label="Locations"
    >
      {isRow && (
        <div className="location-bar-head">
          <span className="location-bar-title">Locations</span>
          {branches.length > 0 && (
            <span className="location-bar-count">{branches.length}</span>
          )}
          <span className="location-bar-legend">
            <span className="loc-legend-item">
              <span className="loc-legend-swatch on" /> Active (sync &lt; 5m)
            </span>
            <span className="loc-legend-item">
              <span className="loc-legend-swatch off" /> Inactive
            </span>
            <span className="loc-legend-item">
              <span className="loc-users-icon loc-legend-users" aria-hidden />
              Logged in today
            </span>
          </span>
          {!allSelected && (
            <button type="button" className="location-select-all" onClick={onSelectAll}>
              Show all
            </button>
          )}
        </div>
      )}

      <div className="location-rail">
        {branches.length > 1 && <span className="location-rail-line" aria-hidden />}
        <div className="location-rail-track">
          {branches.length === 0 ? (
            <span className="location-empty muted">No locations</span>
          ) : (
            branches.map((branch, index) => {
              const selected = selectedIds.has(branch.id);
              const clientOnline = isBranchClientOnline(branch);
              const usersLoggedIn = branchLoggedInUserCount(branch);
              const code = branchCode(branch);
              const [accentA, accentB] = branchAccent(branch.id);
              const title = `${branch.name}\nSync: ${branchSyncStatusLabel(branch)}\n${usersLoggedIn} user(s) logged in today\n${syncHint(branch.last_sync_at)}`;

              return (
                <button
                  key={branch.id}
                  type="button"
                  className={`loc-node ${selected ? 'is-selected' : ''} ${clientOnline ? 'client-on' : 'client-off'}`}
                  style={
                    {
                      '--loc-a': accentA,
                      '--loc-b': accentB,
                      animationDelay: `${index * 60}ms`,
                    } as CSSProperties
                  }
                  onClick={() => onToggle(branch.id)}
                  aria-pressed={selected}
                  aria-label={`${branch.name}, ${code}, service ${clientOnline ? 'on' : 'off'}, ${usersLoggedIn} logged in`}
                  title={title}
                >
                  <span className="loc-client-stripe" aria-hidden />
                  <span className="loc-node-glow" aria-hidden />
                  <span className="loc-node-sheen" aria-hidden />
                  {selected && (
                    <span className="loc-node-check" aria-hidden>
                      <svg viewBox="0 0 16 16" fill="none">
                        <path
                          d="M3.5 8.2 6.4 11l6.1-6.5"
                          stroke="currentColor"
                          strokeWidth="2"
                          strokeLinecap="round"
                          strokeLinejoin="round"
                        />
                      </svg>
                    </span>
                  )}
                  <span className={`loc-node-ring ${clientOnline ? 'is-client-live' : ''}`}>
                    <span className="loc-node-avatar" aria-hidden>
                      {code.charAt(0)}
                    </span>
                  </span>
                  <span className="loc-node-meta">
                    <span className="loc-node-code">{code}</span>
                    <span className="loc-node-status-row">
                      <span
                        className={`loc-client-pill ${clientOnline ? 'on' : 'off'}`}
                        title="Branch service / client"
                      >
                        {clientOnline ? 'On' : 'Off'}
                      </span>
                      <span
                        className={`loc-users-pill ${usersLoggedIn > 0 ? 'has-logged-in' : ''}`}
                        title="Users logged in to inventory (login_flag 1)"
                      >
                        <span className="loc-users-icon" aria-hidden />
                        {usersLoggedIn}
                      </span>
                    </span>
                  </span>
                </button>
              );
            })
          )}
        </div>
      </div>
    </section>
  );
}
