import { useState } from 'react';

const TYPE_FILTERS = [
  { key: '', label: 'All items' },
  { key: 'quotation', label: 'Quotations' },
  { key: 'estimation', label: 'Estimations' },
  { key: 'sale', label: 'Sales' },
  { key: 'voucher', label: 'Vouchers' },
  { key: 'receipt', label: 'Receipts' },
  { key: 'settlement', label: 'Settlement' },
] as const;

type Props = {
  selectedBranchCount: number;
  totalBranches: number;
  typeFilter: string;
  live: boolean;
  soundOn: boolean;
  notifyOn: boolean;
  onTypeChange: (v: string) => void;
  onLiveToggle: () => void;
  onSoundToggle: () => void;
  onNotifyToggle: () => void;
  onEnableSound: () => void;
  onEnableNotify: () => void;
};

export function FeedFilters({
  selectedBranchCount,
  totalBranches,
  typeFilter,
  live,
  soundOn,
  notifyOn,
  onTypeChange,
  onLiveToggle,
  onSoundToggle,
  onNotifyToggle,
  onEnableSound,
  onEnableNotify,
}: Props) {
  const [open, setOpen] = useState(false);

  const branchSummary =
    totalBranches === 0
      ? 'All branches'
      : selectedBranchCount >= totalBranches
        ? 'All branches'
        : `${selectedBranchCount} of ${totalBranches} branches`;
  const typeLabel = TYPE_FILTERS.find((t) => t.key === typeFilter)?.label ?? 'All items';

  return (
    <section className={`feed-filters-panel ${open ? 'is-open' : ''}`}>
      <button
        type="button"
        className="filters-toggle"
        onClick={() => setOpen((v) => !v)}
        aria-expanded={open}
      >
        <span className="filters-toggle-icon" aria-hidden>
          {open ? '▾' : '▸'}
        </span>
        <span className="filters-toggle-text">
          <span className="filters-summary">{branchSummary}</span>
          <span className="filters-dot">·</span>
          <span className="filters-summary">{typeLabel}</span>
        </span>
        <span className="filters-toggle-hint">{open ? 'Hide' : 'Filter'}</span>
      </button>

      <div className="filters-body">
        <div className="filter-block">
          <span className="filter-label">Activity type</span>
          <div className="type-chips">
            {TYPE_FILTERS.map((t) => (
              <button
                key={t.key || 'all'}
                type="button"
                className={typeFilter === t.key ? 'chip active' : 'chip'}
                onClick={() => onTypeChange(t.key)}
              >
                {t.label}
              </button>
            ))}
          </div>
        </div>

        <div className="filter-block filter-actions-row">
          <span className="filter-label">Controls</span>
          <div className="feed-actions">
            <button type="button" className={`btn-pill ${live ? 'on' : ''}`} onClick={onLiveToggle}>
              {live ? '● Live' : '○ Paused'}
            </button>
            <button
              type="button"
              className={`btn-pill ${soundOn ? 'on' : ''}`}
              onClick={soundOn ? onSoundToggle : onEnableSound}
            >
              Sound
            </button>
            <button
              type="button"
              className={`btn-pill ${notifyOn ? 'on' : ''}`}
              onClick={notifyOn ? onNotifyToggle : onEnableNotify}
            >
              Alerts
            </button>
          </div>
        </div>
      </div>
    </section>
  );
}
