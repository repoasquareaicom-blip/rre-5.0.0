import { useEffect, useRef, type CSSProperties } from 'react';
import { createPortal } from 'react-dom';
import { branchLoggedInUserCount, type Branch, type FeedItem } from '../api/client';
import { branchCode } from './BranchServerBar';
import { isBranchClientOnline } from '../config/branchClient';
import { branchSyncStatusLabel } from '../utils/branchStatus';
import { formatBranchDateTime } from '../utils/date';
import { FeedCard } from './FeedCard';
import { feedItemSortTime } from '../utils/feedSort';

const ACCENTS: [string, string][] = [
  ['#6366f1', '#818cf8'],
  ['#0d9488', '#2dd4bf'],
  ['#d97706', '#fbbf24'],
];

type Props = {
  branch: Branch;
  items: FeedItem[];
  newItemIds: Set<number>;
  accentIndex: number;
  pulse?: boolean;
  onItemSelect?: (item: FeedItem) => void;
  onClose: () => void;
};

export function BranchFeedFullscreen({
  branch,
  items,
  newItemIds,
  accentIndex,
  pulse = false,
  onItemSelect,
  onClose,
}: Props) {
  const listRef = useRef<HTMLUListElement>(null);
  const topSortKeyRef = useRef(0);
  const [accentA, accentB] = ACCENTS[accentIndex % ACCENTS.length];
  const clientOnline = isBranchClientOnline(branch);
  const usersLoggedIn = branchLoggedInUserCount(branch);
  const code = branchCode(branch);

  useEffect(() => {
    const prev = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = prev;
    };
  }, []);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [onClose]);

  useEffect(() => {
    const top = items[0];
    if (!top || !listRef.current) return;
    const sortKey = feedItemSortTime(top);
    if (sortKey > topSortKeyRef.current) {
      listRef.current.scrollTop = 0;
    }
    topSortKeyRef.current = sortKey;
  }, [items]);

  if (typeof document === 'undefined') return null;

  return createPortal(
    <div
      className={`feed-branch-fullscreen ${pulse ? 'feed-branch-fullscreen--pulse' : ''}`}
      style={{ '--branch-a': accentA, '--branch-b': accentB } as CSSProperties}
      role="dialog"
      aria-modal="true"
      aria-label={`${branch.name} activity — full screen`}
    >
      <header className="feed-branch-fullscreen-head">
        <button
          type="button"
          className="feed-branch-fullscreen-back"
          onClick={onClose}
          aria-label="Close full screen"
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
            <path
              d="M15 6l-6 6 6 6"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
          <span>Back</span>
        </button>

        <div className="feed-branch-fullscreen-brand">
          <span className="feed-branch-fullscreen-avatar" aria-hidden>
            {code.charAt(0)}
          </span>
          <div className="feed-branch-fullscreen-titles">
            <h2 className="feed-branch-fullscreen-name">{branch.name}</h2>
            <span className="feed-branch-fullscreen-code mono">{code}</span>
          </div>
        </div>

        <div className="feed-branch-fullscreen-meta">
          <span
            className={`feed-branch-fullscreen-status ${clientOnline ? 'on' : 'off'}`}
            title={
              branch.last_sync_at
                ? `Last sync: ${formatBranchDateTime(branch.last_sync_at)}`
                : 'No sync yet'
            }
          >
            {branchSyncStatusLabel(branch)}
          </span>
          {usersLoggedIn > 0 && (
            <span className="feed-branch-fullscreen-users">{usersLoggedIn} in</span>
          )}
          <span className="feed-branch-fullscreen-count">{items.length}</span>
        </div>
      </header>

      <div className="feed-branch-fullscreen-body">
        <ul ref={listRef} className="feed-branch-fullscreen-list feed-branch-column-list">
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
    </div>,
    document.body
  );
}
