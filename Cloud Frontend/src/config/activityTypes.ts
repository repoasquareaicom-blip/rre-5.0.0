/** UI filter chips — maps to activity_events entity_type values served by /monitor/feed. */
export const ACTIVITY_FILTERS = [
  { key: 'quotation', label: 'Quotation', short: 'QUO', icon: 'Q', apiTypes: ['quotation'] as const },
  { key: 'estimation', label: 'Estimation', short: 'EST', icon: 'E', apiTypes: ['estimation'] as const },
  { key: 'sale', label: 'Sales', short: 'SAL', icon: 'S', apiTypes: ['sale'] as const },
  {
    key: 'sales-pipe',
    label: 'Sales (Pipes)',
    short: 'PIP',
    icon: 'P',
    apiTypes: ['sales-pipe'] as const,
  },
  {
    key: 'sales-trader',
    label: 'Sales (Traders)',
    short: 'TRD',
    icon: 'T',
    apiTypes: ['sales-trader'] as const,
  },
] as const;

export type ActivityFilterKey = (typeof ACTIVITY_FILTERS)[number]['key'];

export const ALL_ACTIVITY_FILTER_KEYS: ActivityFilterKey[] = ACTIVITY_FILTERS.map((t) => t.key);

/** Entity types stored in activity_events and returned by the feed API. */
export const ENTITY_TYPES = [
  'quotation',
  'estimation',
  'sale',
  'sales-pipe',
  'sales-trader',
] as const;

export type EntityType = (typeof ENTITY_TYPES)[number];

const filterByKey = Object.fromEntries(ACTIVITY_FILTERS.map((f) => [f.key, f])) as Record<
  ActivityFilterKey,
  (typeof ACTIVITY_FILTERS)[number]
>;

/** Map selected filter keys to API `types` param (comma-separated entity types). */
export function filterKeysToApiTypes(selected: Set<string>): string[] | undefined {
  if (selected.size === 0 || selected.size >= ALL_ACTIVITY_FILTER_KEYS.length) {
    return undefined;
  }
  const api = new Set<string>();
  for (const key of selected) {
    const group = filterByKey[key as ActivityFilterKey];
    if (group) group.apiTypes.forEach((t) => api.add(t));
  }
  return api.size > 0 ? [...api] : undefined;
}

/** @deprecated use ACTIVITY_FILTERS */
export const ACTIVITY_TYPES = ACTIVITY_FILTERS;

/** @deprecated use ALL_ACTIVITY_FILTER_KEYS */
export const ALL_ACTIVITY_TYPE_KEYS = ALL_ACTIVITY_FILTER_KEYS;
