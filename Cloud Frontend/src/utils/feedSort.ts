import type { FeedItem } from '../api/client';

/** Sort key: cloud syncon (ms). Rows without syncon sink to the bottom. */
export function feedItemSortTime(item: FeedItem): number {
  if (item.syncon_ts != null && item.syncon_ts > 0) {
    return item.syncon_ts;
  }
  if (item.syncon) {
    const t = Date.parse(item.syncon);
    if (!Number.isNaN(t)) return t;
  }
  if (item.entity_type === 'quotation' && item.quotation?.synced_at) {
    const t = Date.parse(item.quotation.synced_at);
    if (!Number.isNaN(t)) return t;
  }
  return 0;
}

/** Newest syncon at the top of each branch column. */
export function sortFeedNewestFirst(items: FeedItem[]): FeedItem[] {
  return [...items].sort((a, b) => feedItemSortTime(b) - feedItemSortTime(a));
}

export function feedItemSynconIso(item: FeedItem): string | null {
  return item.syncon ?? item.quotation?.synced_at ?? null;
}
