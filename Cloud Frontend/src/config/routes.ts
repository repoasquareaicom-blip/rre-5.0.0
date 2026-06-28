export type RouteMeta = {
  title: string;
  subtitle: string;
};

export const ROUTE_META: Record<string, RouteMeta> = {
  '/': {
    title: 'Live feed',
    subtitle: 'Activity by location & date',
  },
  '/dashboard': {
    title: 'Dashboard',
    subtitle: 'Sales overview across branches',
  },
  '/quotations': { title: 'Quotations', subtitle: 'Synced quotation list' },
  '/estimations': { title: 'Estimations', subtitle: 'Synced estimation list' },
  '/sales': { title: 'Sales', subtitle: 'Sales header table' },
  '/sales-pipes': { title: 'Sales (Pipes)', subtitle: 'SalesPipes header table' },
  '/sales-traders': { title: 'Sales (Traders)', subtitle: 'SalesTraders header table' },
  '/vouchers': { title: 'Vouchers', subtitle: 'Payment vouchers' },
  '/settlements': { title: 'Settlement', subtitle: 'Day-end cash summary' },
};

export function metaForPath(pathname: string): RouteMeta {
  return ROUTE_META[pathname] ?? { title: 'RR Monitor', subtitle: 'Cloud admin' };
}
