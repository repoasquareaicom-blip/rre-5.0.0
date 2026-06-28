import { ACTIVITY_FILTERS } from '../config/activityTypes';
import { DateNavigator } from './DateNavigator';

type Props = {
  feedDate: string;
  onDateChange: (date: string) => void;
  selectedTypes: Set<string>;
  onToggleType: (key: string) => void;
  onSelectAllTypes: () => void;
  live: boolean;
  soundOn: boolean;
  notifyOn: boolean;
  onLiveToggle: () => void;
  onSoundToggle: () => void;
  onNotifyToggle: () => void;
  onEnableSound: () => void;
  onEnableNotify: () => void;
  onTestSound: () => void;
  filtersOpen: boolean;
};

export function FeedStickyHeader({
  feedDate,
  onDateChange,
  selectedTypes,
  onToggleType,
  onSelectAllTypes,
  live,
  soundOn,
  notifyOn,
  onLiveToggle,
  onSoundToggle,
  onNotifyToggle,
  onEnableSound,
  onEnableNotify,
  onTestSound,
  filtersOpen,
}: Props) {
  const allTypesSelected = ACTIVITY_FILTERS.every((t) => selectedTypes.has(t.key));

  return (
    <div id="feed-filters" className={`filter-panel ${filtersOpen ? 'is-open' : ''}`}>
      <div className="filter-panel-inner">
        <div className="filter-deck filter-deck--animated">
          <div className="filter-deck-shine" aria-hidden />
          <div className="filter-deck-top">
            <DateNavigator
              date={feedDate}
              onDateChange={onDateChange}
              variant="feed"
              showToday
            />
            <div className="filter-actions">
              <button
                type="button"
                className={`filter-icon-btn ${live ? 'active live' : ''}`}
                onClick={onLiveToggle}
                title={live ? 'Live on' : 'Paused'}
              >
                <span className="filter-icon-dot" />
              </button>
              <button
                type="button"
                className={`filter-icon-btn ${soundOn ? 'active' : ''}`}
                onClick={soundOn ? onSoundToggle : onEnableSound}
                onDoubleClick={(e) => {
                  e.preventDefault();
                  onTestSound();
                }}
                title={soundOn ? 'Click: off · Double-click: test sound' : 'Click: enable sound'}
              >
                ♪
              </button>
              <button
                type="button"
                className={`filter-icon-btn ${notifyOn ? 'active' : ''}`}
                onClick={notifyOn ? onNotifyToggle : onEnableNotify}
                title="Alerts"
              >
                ◉
              </button>
            </div>
          </div>

          <div className="filter-deck-types">
            <div className="filter-types-head">
              <span>Activity</span>
              {!allTypesSelected && (
                <button type="button" className="location-select-all" onClick={onSelectAllTypes}>
                  All types
                </button>
              )}
            </div>
            <div className="type-select-grid" role="group" aria-label="Activity types">
              {ACTIVITY_FILTERS.map((t) => {
                const on = selectedTypes.has(t.key);
                return (
                  <button
                    key={t.key}
                    type="button"
                    className={`type-select-item type-${t.key} ${on ? 'is-on' : ''}`}
                    onClick={() => onToggleType(t.key)}
                    aria-pressed={on}
                    title={t.label}
                  >
                    <span className="type-select-icon">{t.icon}</span>
                    <span className="type-select-text">{t.short}</span>
                  </button>
                );
              })}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
