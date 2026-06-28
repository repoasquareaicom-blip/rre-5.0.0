import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { fetchFeed, type Branch, type FeedItem } from '../api/client';
import { BranchFeedColumn } from '../components/BranchFeedColumn';
import { BranchFeedFullscreen } from '../components/BranchFeedFullscreen';
import { FeedDetailSheet } from '../components/FeedDetailSheet';
import { FeedStickyHeader } from '../components/FeedStickyHeader';
import {
  ALL_ACTIVITY_FILTER_KEYS,
  filterKeysToApiTypes,
} from '../config/activityTypes';
import { formatFeedDate, isToday, localDateString } from '../utils/date';
import { sortFeedNewestFirst } from '../utils/feedSort';
import { playNewActivitySound, unlockAudio } from '../utils/alertSound';
import { requestNotificationPermission, showActivityNotification } from '../utils/notifications';
import {
  loadLastVisitedBranchCode,
  resolveLastVisitedBranchId,
  saveLastVisitedBranchCode,
} from '../utils/lastVisitedBranch';

const POLL_MS = 10_000;
const PRESENCE_POLL_MS = 15_000;
const NEW_ITEM_ANIM_MS = 2_500;

function feedItemKey(item: FeedItem): string {
  return `${item.entity_type}:${item.branch_id}:${item.external_id}`;
}

function dedupeFeedItems(rows: FeedItem[]): FeedItem[] {
  const seen = new Set<string>();
  return rows.filter((row) => {
    const key = feedItemKey(row);
    if (seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

export function LiveFeed() {
  const [items, setItems] = useState<FeedItem[]>([]);
  const [branches, setBranches] = useState<Branch[]>([]);
  const [selectedTypes, setSelectedTypes] = useState<Set<string>>(
    () => new Set(ALL_ACTIVITY_FILTER_KEYS)
  );
  const [feedDate, setFeedDate] = useState(localDateString);
  const [loading, setLoading] = useState(true);
  const [live, setLive] = useState(true);
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [soundOn, setSoundOn] = useState(() => localStorage.getItem('rr-sound') === '1');
  const [notifyOn, setNotifyOn] = useState(() => localStorage.getItem('rr-notify') === '1');
  const [error, setError] = useState('');
  const [newItemIds, setNewItemIds] = useState<Set<number>>(() => new Set());
  const [pulseBranchIds, setPulseBranchIds] = useState<Set<number>>(() => new Set());
  const [detailItem, setDetailItem] = useState<FeedItem | null>(null);
  const [fullscreenBranchId, setFullscreenBranchId] = useState<number | null>(null);
  const [mobileExpandedBranchId, setMobileExpandedBranchId] = useState<number | null>(null);

  const latestIdRef = useRef(0);
  const seenIdsRef = useRef<Set<number>>(new Set());
  const soundOnRef = useRef(soundOn);
  const notifyOnRef = useRef(notifyOn);
  const loadGenRef = useRef(0);
  const feedInFlightRef = useRef(false);

  soundOnRef.current = soundOn;
  notifyOnRef.current = notifyOn;

  const typeFilterKey = useMemo(() => {
    if (selectedTypes.size === 0) return 'all';
    if (selectedTypes.size >= ALL_ACTIVITY_FILTER_KEYS.length) return 'all';
    return [...selectedTypes].sort().join(',');
  }, [selectedTypes]);

  const typesForApi = useCallback((): string[] | undefined => {
    if (typeFilterKey === 'all') return undefined;
    return filterKeysToApiTypes(selectedTypes);
  }, [typeFilterKey, selectedTypes]);

  const hasCustomFilters =
    !isToday(feedDate) || selectedTypes.size < ALL_ACTIVITY_FILTER_KEYS.length;

  const itemsByBranch = useMemo(() => {
    const map = new Map<number, FeedItem[]>();
    for (const branch of branches) {
      map.set(branch.id, []);
    }
    for (const item of items) {
      const list = map.get(item.branch_id);
      if (list) {
        list.push(item);
      }
    }
    for (const [branchId, list] of map) {
      map.set(branchId, sortFeedNewestFirst(list));
    }
    return map;
  }, [items, branches]);

  const highlightNewItems = useCallback((novel: FeedItem[]) => {
    if (!novel.length) return;

    const branchIds = new Set(novel.map((i) => i.branch_id));
    setNewItemIds((prev) => {
      const next = new Set(prev);
      novel.forEach((i) => next.add(i.id));
      return next;
    });
    setPulseBranchIds(branchIds);

    window.setTimeout(() => {
      setNewItemIds((prev) => {
        const next = new Set(prev);
        novel.forEach((i) => next.delete(i.id));
        return next;
      });
      setPulseBranchIds(new Set());
    }, NEW_ITEM_ANIM_MS);

    if (soundOnRef.current) void playNewActivitySound();
    if (notifyOnRef.current) novel.forEach(showActivityNotification);
  }, []);

  const markNewItems = useCallback(
    (incoming: FeedItem[]) => {
      const novel = incoming.filter((item) => !seenIdsRef.current.has(item.id));
      highlightNewItems(novel);
    },
    [highlightNewItems]
  );

  const mergeItems = useCallback((incoming: FeedItem[], replace: boolean) => {
    if (!incoming.length && replace) {
      setItems([]);
      return;
    }
    if (!incoming.length) return;

    if (!replace) {
      markNewItems(incoming);
    }

    incoming.forEach((item) => seenIdsRef.current.add(item.id));

    setItems((prev) => {
      const merged = replace ? incoming : [...incoming, ...prev];
      const byKey = new Map<string, FeedItem>();
      for (const row of merged) {
        byKey.set(feedItemKey(row), row);
      }
      return sortFeedNewestFirst(dedupeFeedItems([...byKey.values()]));
    });
  }, [markNewItems]);

  const loadFeed = useCallback(
    async (opts?: { sinceId?: number; replace?: boolean; force?: boolean }) => {
      if (feedInFlightRef.current && !opts?.force) {
        return null;
      }

      feedInFlightRef.current = true;
      const gen = ++loadGenRef.current;

      try {
        const data = await fetchFeed({
          ...(opts?.sinceId && opts.sinceId > 0 ? { since_id: opts.sinceId } : {}),
          types: typesForApi(),
          date: feedDate,
          limit: 120,
        });

        if (gen !== loadGenRef.current) return data;

        setBranches(data.branches);
        if (data.latest_id > latestIdRef.current) {
          latestIdRef.current = data.latest_id;
        }

        const replace = opts?.replace ?? !opts?.sinceId;
        if (replace) {
          seenIdsRef.current = new Set(data.items.map((item) => item.id));
          setNewItemIds(new Set());
          setPulseBranchIds(new Set());
        }

        mergeItems(data.items, replace);
        return data;
      } finally {
        if (gen === loadGenRef.current) {
          feedInFlightRef.current = false;
        }
      }
    },
    [feedDate, typesForApi, mergeItems]
  );

  const loadInitial = useCallback(async () => {
    setError('');
    latestIdRef.current = 0;
    await loadFeed({ replace: true, force: true });
  }, [loadFeed]);

  const pollNew = useCallback(async () => {
    if (!live || !isToday(feedDate)) return;
    const beforeIds = new Set(seenIdsRef.current);
    try {
      const data = await loadFeed({ replace: true });
      if (!data) return;
      const novel = data.items.filter((item) => !beforeIds.has(item.id));
      highlightNewItems(novel);
    } catch {
      /* poll quietly */
    }
  }, [live, feedDate, loadFeed, highlightNewItems]);

  const refreshBranches = useCallback(async () => {
    try {
      const data = await fetchFeed({
        types: typesForApi(),
        date: feedDate,
        limit: 1,
      });
      setBranches(data.branches);
    } catch {
      /* quiet */
    }
  }, [feedDate, typesForApi]);

  useEffect(() => {
    setLoading(true);
    loadInitial()
      .catch(() => setError('Could not load feed. Is the API running?'))
      .finally(() => setLoading(false));
  }, [loadInitial]);

  useEffect(() => {
    if (!live || !isToday(feedDate)) return;
    const id = window.setInterval(() => void pollNew(), POLL_MS);
    return () => clearInterval(id);
  }, [live, feedDate, pollNew]);

  useEffect(() => {
    if (!isToday(feedDate)) return;
    void refreshBranches();
    const id = window.setInterval(() => void refreshBranches(), PRESENCE_POLL_MS);
    return () => clearInterval(id);
  }, [feedDate, refreshBranches]);

  useEffect(() => {
    localStorage.setItem('rr-sound', soundOn ? '1' : '0');
  }, [soundOn]);

  useEffect(() => {
    localStorage.setItem('rr-notify', notifyOn ? '1' : '0');
  }, [notifyOn]);

  useEffect(() => {
    if (!branches.length) return;
    setMobileExpandedBranchId((current) => {
      if (current != null && branches.some((b) => b.id === current)) return current;
      return resolveLastVisitedBranchId(branches, loadLastVisitedBranchCode());
    });
  }, [branches]);

  const visitBranch = useCallback((branch: Branch) => {
    saveLastVisitedBranchCode(branch.code);
    setMobileExpandedBranchId(branch.id);
  }, []);

  const toggleType = (key: string) => {
    setSelectedTypes((prev) => {
      const next = new Set(prev);
      if (next.has(key)) {
        next.delete(key);
        if (next.size === 0) return new Set(ALL_ACTIVITY_FILTER_KEYS);
      } else {
        next.add(key);
      }
      return next;
    });
  };

  const headerSlot =
    typeof document !== 'undefined' ? document.getElementById('feed-header-slot') : null;
  const liveSlot = typeof document !== 'undefined' ? document.getElementById('feed-live-slot') : null;
  const filterToggleSlot =
    typeof document !== 'undefined' ? document.getElementById('feed-filter-toggle-slot') : null;

  const header = (
    <FeedStickyHeader
      feedDate={feedDate}
      onDateChange={setFeedDate}
      selectedTypes={selectedTypes}
      onToggleType={toggleType}
      onSelectAllTypes={() => setSelectedTypes(new Set(ALL_ACTIVITY_FILTER_KEYS))}
      live={live}
      soundOn={soundOn}
      notifyOn={notifyOn}
      onLiveToggle={() => setLive((v) => !v)}
      onSoundToggle={() => setSoundOn(false)}
      onNotifyToggle={() => setNotifyOn(false)}
      onEnableSound={() => {
        unlockAudio();
        setSoundOn(true);
      }}
      onTestSound={() => void playNewActivitySound()}
      onEnableNotify={async () => {
        const p = await requestNotificationPermission();
        setNotifyOn(p === 'granted');
      }}
      filtersOpen={filtersOpen}
    />
  );

  const filterToggle = (
    <button
      type="button"
      className={`filter-toggle-btn ${filtersOpen ? 'is-open' : ''} ${hasCustomFilters ? 'has-filters' : ''}`}
      onClick={() => setFiltersOpen((v) => !v)}
      aria-expanded={filtersOpen}
      aria-controls="feed-filters"
      title={filtersOpen ? 'Hide filters' : 'Show filters'}
    >
      <span className="filter-toggle-bars" aria-hidden>
        <span />
        <span />
        <span />
      </span>
    </button>
  );

  const liveBadge = (
    <span className={`live-pill ${live && isToday(feedDate) ? '' : 'paused'}`}>
      <span className={live && isToday(feedDate) ? 'live-dot' : 'live-dot off'} />
      {live && isToday(feedDate) ? `Live · ${POLL_MS / 1000}s` : 'Paused'}
    </span>
  );

  const displayBranches = branches.length > 0 ? branches : [];

  const fullscreenBranch =
    fullscreenBranchId != null
      ? displayBranches.find((b) => b.id === fullscreenBranchId) ?? null
      : null;

  const fullscreenAccentIndex = fullscreenBranch
    ? displayBranches.findIndex((b) => b.id === fullscreenBranch.id)
    : 0;

  const goLiveToday = () => {
    setFeedDate(localDateString());
    setLive(true);
  };

  return (
    <>
      {headerSlot && createPortal(header, headerSlot)}
      {liveSlot && createPortal(liveBadge, liveSlot)}
      {filterToggleSlot && createPortal(filterToggle, filterToggleSlot)}

      <FeedDetailSheet item={detailItem} onClose={() => setDetailItem(null)} />

      {fullscreenBranch && (
        <BranchFeedFullscreen
          branch={fullscreenBranch}
          items={itemsByBranch.get(fullscreenBranch.id) ?? []}
          newItemIds={newItemIds}
          accentIndex={fullscreenAccentIndex >= 0 ? fullscreenAccentIndex : 0}
          pulse={pulseBranchIds.has(fullscreenBranch.id)}
          onItemSelect={setDetailItem}
          onClose={() => setFullscreenBranchId(null)}
        />
      )}

      <div className="feed-scroll-body feed-scroll-body--columns">
        {error && <p className="error banner-error">{error}</p>}
        {!isToday(feedDate) && (
          <div className="feed-date-hint" role="status">
            <span className="feed-date-hint-text">
              Showing activity for <strong>{formatFeedDate(feedDate)}</strong> ({feedDate}) — synced or
              dated this day.
            </span>
            <button type="button" className="feed-date-hint-btn" onClick={goLiveToday}>
              <span className="feed-date-hint-btn-dot" aria-hidden />
              Live
            </button>
          </div>
        )}
        {loading && (
          <div className="feed-loading">
            <span className="loader" />
            Loading activity…
          </div>
        )}

        <div className="feed-branch-grid">
          {displayBranches.map((branch, index) => (
            <BranchFeedColumn
              key={branch.id}
              branch={branch}
              items={itemsByBranch.get(branch.id) ?? []}
              newItemIds={newItemIds}
              accentIndex={index}
              pulse={pulseBranchIds.has(branch.id)}
              onItemSelect={setDetailItem}
              mobileOpen={mobileExpandedBranchId === branch.id}
              onMobileOpenChange={(open) => {
                if (open) visitBranch(branch);
                else if (mobileExpandedBranchId === branch.id) setMobileExpandedBranchId(null);
              }}
              onExpand={(b) => {
                visitBranch(b);
                setFullscreenBranchId(b.id);
              }}
            />
          ))}
          {!loading && displayBranches.length === 0 && (
            <p className="feed-grid-empty muted">No branches configured.</p>
          )}
        </div>
      </div>
    </>
  );
}
