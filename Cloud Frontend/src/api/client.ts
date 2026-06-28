import axios from 'axios';
import type { AuthUser } from '../auth/types';
import type { QuotationSummary } from '../types/quotation';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: { Accept: 'application/json' },
});

export function setAuthToken(token: string | null) {
  if (token) {
    api.defaults.headers.common.Authorization = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common.Authorization;
  }
}

export async function login(username: string, password: string, remember: boolean) {
  const { data } = await api.post<{
    token: string;
    user: AuthUser;
    expires_at: string | null;
  }>('/auth/login', { username, password, remember });
  return data;
}

export async function fetchMe() {
  const { data } = await api.get<{ user: AuthUser }>('/auth/me');
  return data;
}

export async function logout() {
  await api.post('/auth/logout');
}

export type Branch = {
  id: number;
  code: string;
  short_code?: string;
  name: string;
  /** Branch sync/service reachable (from API; later: branch service heartbeat). */
  server_online?: boolean;
  /** Client app / branch service on/off — use for card indicator. */
  client_online?: boolean;
  last_sync_at?: string | null;
  /** Users with inventory login_flag = 1 (is_logged_in). */
  users_logged_in?: number;
  /** @deprecated use users_logged_in — kept for API compatibility */
  users_online?: number;
};

/** Logged-in inventory users at this branch (login_flag 1). */
export function branchLoggedInUserCount(branch: Branch): number {
  return branch.users_logged_in ?? branch.users_online ?? 0;
}

export type SummaryKpi = { count: number; amount: number };

export type DashboardResponse = {
  period: { from: string; to: string };
  summary: {
    quotations: SummaryKpi;
    estimations: SummaryKpi;
    sales: SummaryKpi;
    sales_pipes: SummaryKpi;
    sales_traders: SummaryKpi;
    vouchers: SummaryKpi;
  };
  sales_by_branch: Array<{
    branch_id: number;
    count: number;
    total: number;
    branch: Branch;
  }>;
  branches: Branch[];
};

export type Paginated<T> = {
  data: T[];
  current_page: number;
  last_page: number;
  per_page: number;
  total: number;
};

export type DocumentRow = {
  id: number;
  external_id: string;
  customer_name?: string | null;
  party_name?: string | null;
  document_date?: string | null;
  status?: string | null;
  payment_mode?: string | null;
  quotation_external_id?: string | null;
  city?: string | null;
  reference_id?: string | null;
  is_billed?: boolean;
  line_count?: number;
  amount: number;
  total_amount?: number;
  sales_channel?: 'sales' | 'pipes' | 'traders';
  branch: Branch;
};

export type FeedDocumentSummary = {
  document_id: string;
  customer_name?: string | null;
  document_datetime?: string | null;
  line_count: number;
  total_amount: number;
  quotation_id?: string | null;
  payment_mode?: string | null;
};

export type FeedItem = {
  id: number;
  branch_id: number;
  entity_type: string;
  external_id: string;
  event_kind: string;
  title: string;
  subtitle: string | null;
  amount: string | number | null;
  /** Quotation document date (and other entities’ business date) for feed day filter. */
  activity_date?: string | null;
  /** Cloud receive time — used for feed order and “time ago”. */
  syncon?: string | null;
  /** Milliseconds since epoch (from syncon); preferred for sorting. */
  syncon_ts?: number;
  occurred_at: string;
  branch: Branch;
  /** Unified card fields for all mirror entity types. */
  document?: FeedDocumentSummary;
  /** Populated for quotation feed cards from QuotationHeader + QuotationDetails. */
  quotation?: QuotationSummary;
};

export type FeedResponse = {
  latest_id: number;
  items: FeedItem[];
  branches: Branch[];
};

export type FeedDetailLine = {
  line_no: number;
  product_id?: string | null;
  product_name?: string | null;
  rate: number;
  quantity?: string | null;
  amount: number;
  gst?: number;
};

export type FeedDetailResponse = {
  entity_type: string;
  document_id: string;
  header: {
    customer_name?: string | null;
    document_datetime?: string | null;
    status?: string | null;
    city?: string | null;
    reference_id?: string | null;
    payment_mode?: string | null;
    quotation_id?: string | null;
    line_count: number;
    total_amount: number;
    sales_channel?: string;
    branch?: { id: number; code: string; name: string };
  };
  lines: FeedDetailLine[];
};

export async function fetchFeedDetail(params: {
  entity_type: string;
  branch_id: number;
  external_id: string;
}) {
  const { data } = await api.get<FeedDetailResponse>('/monitor/feed/detail', { params });
  return data;
}

export async function fetchFeed(params: {
  since_id?: number;
  branch_id?: number;
  branch_ids?: number[];
  types?: string[];
  date?: string;
  limit?: number;
}) {
  const { branch_ids, types, ...rest } = params;
  const query = {
    ...rest,
    ...(branch_ids && branch_ids.length > 0 ? { branch_ids: branch_ids.join(',') } : {}),
    ...(types && types.length > 0 ? { types: types.join(',') } : {}),
  };
  const { data } = await api.get<FeedResponse>('/monitor/feed', { params: query });
  return data;
}

export async function fetchDashboard(params: {
  from?: string;
  to?: string;
  branch_id?: number;
}) {
  const { data } = await api.get<DashboardResponse>('/monitor/dashboard', { params });
  return data;
}

export type MonitorEntity =
  | 'quotations'
  | 'estimations'
  | 'sales'
  | 'sales-pipes'
  | 'sales-traders'
  | 'vouchers';

export async function fetchList(
  entity: MonitorEntity,
  params: Record<string, string | number | undefined>
) {
  const { data } = await api.get<Paginated<DocumentRow>>(`/monitor/${entity}`, { params });
  return data;
}

export async function fetchSettlements(params: Record<string, string | number | undefined>) {
  const { data } = await api.get('/monitor/settlements/summary', { params });
  return data;
}

export function formatMoney(value: number | string) {
  const n = typeof value === 'string' ? parseFloat(value) : value;
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 0,
  }).format(n || 0);
}
