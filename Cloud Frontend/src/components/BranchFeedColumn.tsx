import { useEffect, useRef, type CSSProperties } from 'react';
import { branchLoggedInUserCount, type Branch, type FeedItem } from '../api/client';
import { branchCode } from './BranchServerBar';
import { isBranchClientOnline } from '../config/branchClient';
import { branchSyncStatusLabel } from '../utils/branchStatus';
import { formatBranchDateTime } from '../utils/date';
import { useIsMobile } from '../hooks/useIsMobile';
import { FeedCard } from './FeedCard';
import { feedItemSortTime } from '../utils/feedSort';

const ACCENTS: [string, string][] = [
  ['#6366f1', '#818cf8'],
  ['#0d9488', '#2dd4bf'],
  ['#d97706', '#fbbf24'],
];

function BranchExpandButton({
  branch,
  onExpand,
  compact = false,
}: {
  branch: Branch;
  onExpand: (branch: Branch) => void;
  compact?: boolean;
}) {
  return (
    <button
      type="button"
      className={`feed-branch-expand-btn ${compact ? 'feed-branch-expand-btn--compact' : ''}`}
      onClick={(e) => {
        e.stopPropagation();
        onExpand(branch);
      }}
      aria-label={`Expand ${branch.name} feed`}
      title="Expand feed"
    >
      <svg width={compact ? 14 : 18} height={compact ? 14 : 18} viewBox="0 0 24 24" fill="none" aria-hidden>
        <path
          d="M15 3h6v6M9 21H3v-6M21 3l-7 7M3 21l7-7"
          stroke="currentColor"
          strokeWidth="2.25"
          strokeLinecap="round"
          strokeLinejoin="round"
        />
      </svg>
      <span className="feed-branch-expand-label">Expand</span>
    </button>
  );
}

type Props = {
  branch: Branch;
  items: FeedItem[];
  newItemIds: Set<number>;
  accentIndex: number;
  pulse?: boolean;
  onItemSelect?: (item: FeedItem) => void;
  /** Mobile: open this branch in full-screen feed view. */
  onExpand?: (branch: Branch) => void;
  /** Mobile accordion: controlled open state from parent. */
  mobileOpen?: boolean;
  onMobileOpenChange?: (open: boolean) => void;
};

export function BranchFeedColumn({
  branch,
  items,
  newItemIds,
  accentIndex,
  pulse = false,
  onItemSelect,
  onExpand,
  mobileOpen = false,
  onMobileOpenChange,
}: Props) {
  const listRef = useRef<HTMLUListElement>(null);
  const topSortKeyRef = useRef(0);
  const isMobile = useIsMobile();
  const open = isMobile ? mobileOpen : true;

  const [accentA, accentB] = ACCENTS[accentIndex % ACCENTS.length];
  const clientOnline = isBranchClientOnline(branch);
  const usersLoggedIn = branchLoggedInUserCount(branch);
  const code = branchCode(branch);

  useEffect(() => {
    const top = items[0];
    if (!top || !listRef.current) return;
    const sortKey = feedItemSortTime(top);
    if (sortKey > topSortKeyRef.current) {
      listRef.current.scrollTop = 0;
    }
    topSortKeyRef.current = sortKey;
  }, [items]);

  const toggleOpen = () => {
    if (isMobile) onMobileOpenChange?.(!open);
  };

  return (
    <section
      className={`feed-branch-column ${isMobile ? 'feed-branch-column--mobile' : ''} ${open ? 'is-open' : 'is-collapsed'} ${pulse ? 'feed-branch-column--pulse' : ''}`}
      style={{ '--branch-a': accentA, '--branch-b': accentB } as CSSProperties}
      aria-label={`${branch.name} activity`}
    >
      <header
        className={`feed-branch-column-head ${isMobile ? 'feed-branch-column-head--compact' : ''}`}
      >
        <div className="feed-branch-column-head-row">
          {isMobile ? (
            <>
              <button
                type="button"
                className="feed-branch-compact-toggle"
                onClick={toggleOpen}
                aria-expanded={open}
                aria-controls={`feed-branch-body-${branch.id}`}
              >
                <span className="feed-branch-compact-accent" aria-hidden />
                <span className="feed-branch-mini-avatar" aria-hidden>
                  {code.charAt(0)}
                </span>
                <span className="feed-branch-compact-main">
                  <span className="feed-branch-compact-name" title={`${branch.name} · ${code}`}>
                    {branch.name}
                  </span>
                </span>
                <span className="feed-branch-compact-meta">
                  <span
                    className={`feed-branch-compact-status ${clientOnline ? 'on' : 'off'}`}
                    role="img"
                    aria-label={branchSyncStatusLabel(branch)}
                    title={
                      branch.last_sync_at
                        ? `${branchSyncStatusLabel(branch)} · Last sync: ${formatBranchDateTime(branch.last_sync_at)}`
                        : `${branchSyncStatusLabel(branch)} · No sync yet`
                    }
                  >
                    <span className="feed-branch-status-dot" aria-hidden />
                  </span>
                  {usersLoggedIn > 0 && (
                    <span className="feed-branch-compact-users" title="Logged in today">
                      <svg width="10" height="10" viewBox="0 0 16 16" fill="none" aria-hidden>
                        <circle cx="8" cy="5" r="2.5" stroke="currentColor" strokeWidth="1.4" />
                        <path
                          d="M3 14c0-2.8 2.2-4.5 5-4.5s5 1.7 5 4.5"
                          stroke="currentColor"
                          strokeWidth="1.4"
                          strokeLinecap="round"
                        />
                      </svg>
                      {usersLoggedIn}
                    </span>
                  )}
                  <span className="feed-branch-compact-count">{items.length}</span>
                </span>
              </button>
              {onExpand && (
                <BranchExpandButton branch={branch} onExpand={onExpand} compact />
              )}
              <button
                type="button"
                className={`feed-branch-chevron-btn ${open ? 'is-open' : ''}`}
                onClick={toggleOpen}
                aria-label={open ? `Collapse ${branch.name}` : `Expand ${branch.name} list`}
              >
                <svg width="12" height="12" viewBox="0 0 16 16" fill="none" aria-hidden>
                  <path
                    d="M4 6l4 4 4-4"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>
            </>
          ) : (
            <div className="feed-branch-column-brand">
              <span className="feed-branch-avatar" aria-hidden>
                {code.charAt(0)}
              </span>
              <div>
                <h2 className="feed-branch-column-title">{branch.name}</h2>
                <span className="feed-branch-column-code mono">{code}</span>
              </div>
            </div>
          )}
          {!isMobile && onExpand && <BranchExpandButton branch={branch} onExpand={onExpand} />}
        </div>
        {!isMobile && (
          <div className="feed-branch-column-stats">
            <span className={`feed-branch-pill ${clientOnline ? 'on' : 'off'}`}>
              {branchSyncStatusLabel(branch)}
            </span>
            <span className="feed-branch-pill feed-branch-pill--count">
              {items.length} synced
            </span>
            {usersLoggedIn > 0 && (
              <span className="feed-branch-pill feed-branch-pill--users">
                {usersLoggedIn} logged in
              </span>
            )}
          </div>
        )}
      </header>

      <div
        id={`feed-branch-body-${branch.id}`}
        className={`feed-branch-column-body ${isMobile && open ? 'feed-branch-column-body--open' : ''} ${isMobile && !open ? 'feed-branch-column-body--closed' : ''}`}
        aria-hidden={isMobile && !open}
      >
        <div className="feed-branch-column-inner">
          <ul ref={listRef} className="feed-branch-column-list">
            {items.map((item) => (
              <FeedCard
                key={item.id}
                item={item}
                isNew={newItemIds.has(item.id)}
                onSelect={onItemSelect}
              />
            ))}
            {!items.length && (
              <li className="feed-branch-empty">
                <span className="muted">No sync activity for this date</span>
              </li>
            )}
          </ul>
        </div>
      </div>
    </section>
  );
}
