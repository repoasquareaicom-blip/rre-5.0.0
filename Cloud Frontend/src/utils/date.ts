const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

export function localDateString(date = new Date()): string {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, '0');
  const d = String(date.getDate()).padStart(2, '0');
  return `${y}-${m}-${d}`;
}

export function isToday(dateStr: string): boolean {
  return dateStr === localDateString();
}

export function shiftDate(dateStr: string, days: number): string {
  const [y, m, d] = dateStr.split('-').map(Number);
  const dt = new Date(y, m - 1, d);
  dt.setDate(dt.getDate() + days);
  return localDateString(dt);
}

/** Format DB wall-clock datetime as-is (no timezone conversion). */
export function formatBranchDateTime(value: string | null | undefined): string {
  if (!value?.trim()) return '—';

  const raw = value.trim().replace('T', ' ');
  const match = raw.match(
    /^(\d{4})-(\d{2})-(\d{2})(?: (\d{2}):(\d{2})(?::(\d{2}))?)?/
  );
  if (!match) return value;

  const [, year, month, day, hour, minute] = match;
  const monthLabel = MONTHS[Number(month) - 1] ?? month;
  const dayNum = Number(day);

  if (hour === undefined) {
    return `${dayNum} ${monthLabel} ${year}`;
  }

  const h = Number(hour);
  const m = Number(minute);
  const h12 = h % 12 || 12;
  const ampm = h < 12 ? 'AM' : 'PM';
  const clock = `${h12}:${String(m).padStart(2, '0')} ${ampm}`;

  return `${dayNum} ${monthLabel} ${year}, ${clock}`;
}

export function formatFeedDate(dateStr: string): string {
  const [y, m, d] = dateStr.split('-').map(Number);
  const dt = new Date(y, m - 1, d);
  const today = localDateString();
  const yesterday = shiftDate(today, -1);
  if (dateStr === today) return 'Today';
  if (dateStr === yesterday) return 'Yesterday';
  return dt.toLocaleDateString(undefined, { weekday: 'short', day: 'numeric', month: 'short' });
}

export function canGoNext(dateStr: string): boolean {
  return dateStr < localDateString();
}

export function daysBetween(from: string, to: string): number {
  const [y1, m1, d1] = from.split('-').map(Number);
  const [y2, m2, d2] = to.split('-').map(Number);
  const start = new Date(y1, m1 - 1, d1);
  const end = new Date(y2, m2 - 1, d2);
  return Math.round((end.getTime() - start.getTime()) / 86400000);
}

export function formatDateRangeLabel(from: string, to: string): string {
  if (from === to) return formatFeedDate(from);
  const [y1, m1, d1] = from.split('-').map(Number);
  const [y2, m2, d2] = to.split('-').map(Number);
  const start = new Date(y1, m1 - 1, d1);
  const end = new Date(y2, m2 - 1, d2);
  const fmt = (dt: Date) =>
    dt.toLocaleDateString(undefined, { day: 'numeric', month: 'short', year: 'numeric' });
  if (from.slice(0, 4) === to.slice(0, 4)) {
    return `${start.toLocaleDateString(undefined, { day: 'numeric', month: 'short' })} – ${end.toLocaleDateString(undefined, { day: 'numeric', month: 'short', year: 'numeric' })}`;
  }
  return `${fmt(start)} – ${fmt(end)}`;
}

export type DatePreset = 'today' | 'yesterday' | 'week' | 'month';

export function presetRange(preset: DatePreset): { from: string; to: string } {
  const today = localDateString();
  switch (preset) {
    case 'today':
      return { from: today, to: today };
    case 'yesterday': {
      const y = shiftDate(today, -1);
      return { from: y, to: y };
    }
    case 'week':
      return { from: shiftDate(today, -6), to: today };
    case 'month':
      return { from: shiftDate(today, -29), to: today };
  }
}

export function activePreset(from: string, to: string): DatePreset | null {
  for (const preset of ['today', 'yesterday', 'week', 'month'] as const) {
    const r = presetRange(preset);
    if (r.from === from && r.to === to) return preset;
  }
  return null;
}
