import type { Branch } from '../api/client';
import type { DatePreset } from '../utils/date';
import { DateNavigator } from './DateNavigator';

type Props = {
  branches: Branch[];
  from: string;
  to: string;
  branchId: string;
  search: string;
  isSingleDay?: boolean;
  preset?: DatePreset | null;
  dateLabel?: string;
  onFromChange: (v: string) => void;
  onToChange: (v: string) => void;
  onDayChange?: (v: string) => void;
  onPreviousDay?: () => void;
  onNextDay?: () => void;
  canGoNextDay?: boolean;
  onPreset?: (preset: DatePreset) => void;
  onBranchChange: (v: string) => void;
  onSearchChange: (v: string) => void;
  showSearch?: boolean;
};

const PRESETS: { key: DatePreset; label: string }[] = [
  { key: 'today', label: 'Today' },
  { key: 'yesterday', label: 'Yesterday' },
  { key: 'week', label: '7 days' },
  { key: 'month', label: '30 days' },
];

export function Filters({
  branches,
  from,
  to,
  branchId,
  search,
  isSingleDay = from === to,
  preset = null,
  dateLabel,
  onFromChange,
  onToChange,
  onDayChange,
  onPreviousDay,
  onNextDay,
  canGoNextDay,
  onPreset,
  onBranchChange,
  onSearchChange,
  showSearch = true,
}: Props) {
  const navDate = isSingleDay ? from : to;
  const handleDateChange = onDayChange ?? ((d: string) => {
    onFromChange(d);
    onToChange(d);
  });

  return (
    <div className="filters filters--modern">
      <div className="filters-date-block">
        <DateNavigator
          date={navDate}
          label={dateLabel}
          onDateChange={handleDateChange}
          onPrevious={onPreviousDay}
          onNext={onNextDay}
          canNext={canGoNextDay}
          variant="panel"
          showToday={false}
        />

        {onPreset && (
          <div className="date-preset-row" role="group" aria-label="Date presets">
            {PRESETS.map((p) => (
              <button
                key={p.key}
                type="button"
                className={`date-preset-btn ${preset === p.key ? 'active' : ''}`}
                onClick={() => onPreset(p.key)}
              >
                {p.label}
              </button>
            ))}
          </div>
        )}

        {!isSingleDay && (
          <div className="filters-range-inputs">
            <label>
              From
              <input type="date" value={from} max={to} onChange={(e) => onFromChange(e.target.value)} />
            </label>
            <label>
              To
              <input type="date" value={to} min={from} onChange={(e) => onToChange(e.target.value)} />
            </label>
          </div>
        )}
      </div>

      <div className="filters-divider" aria-hidden />

      <div className="filters-fields">
        <label>
          Branch
          <select value={branchId} onChange={(e) => onBranchChange(e.target.value)}>
            <option value="">All branches</option>
            {branches.map((b) => (
              <option key={b.id} value={b.id}>
                {b.name}
              </option>
            ))}
          </select>
        </label>

        {showSearch && (
          <label className="search-field">
            Search
            <input
              type="search"
              placeholder="ID, customer, status…"
              value={search}
              onChange={(e) => onSearchChange(e.target.value)}
            />
          </label>
        )}
      </div>
    </div>
  );
}
