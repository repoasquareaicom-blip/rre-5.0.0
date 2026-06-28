import { canGoNext, formatFeedDate, localDateString, shiftDate } from '../utils/date';

type Props = {
  date: string;
  label?: string;
  onDateChange: (date: string) => void;
  onPrevious?: () => void;
  onNext?: () => void;
  canNext?: boolean;
  variant?: 'feed' | 'panel';
  showToday?: boolean;
  className?: string;
};

export function DateNavigator({
  date,
  label,
  onDateChange,
  onPrevious,
  onNext,
  canNext,
  variant = 'panel',
  showToday = true,
  className = '',
}: Props) {
  const today = localDateString();
  const displayLabel = label ?? formatFeedDate(date);
  const nextAllowed = canNext ?? canGoNext(date);

  const handlePrevious = () => {
    if (onPrevious) onPrevious();
    else onDateChange(shiftDate(date, -1));
  };

  const handleNext = () => {
    if (onNext) onNext();
    else onDateChange(shiftDate(date, 1));
  };

  return (
    <div
      className={`date-navigator ${variant === 'panel' ? 'date-navigator--panel' : ''} ${className}`.trim()}
    >
      <button
        type="button"
        className="date-nav-btn"
        onClick={handlePrevious}
        aria-label="Previous day"
      >
        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" aria-hidden>
          <path
            d="M10 12L6 8l4-4"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>

      <label className="date-display">
        <span className="date-display-label">{displayLabel}</span>
        <span className="date-display-sub">{date}</span>
        <input
          type="date"
          className="date-display-input"
          value={date}
          max={today}
          onChange={(e) => onDateChange(e.target.value)}
          aria-label="Select date"
        />
      </label>

      <button
        type="button"
        className="date-nav-btn"
        disabled={!nextAllowed}
        onClick={handleNext}
        aria-label="Next day"
      >
        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" aria-hidden>
          <path
            d="M6 4l4 4-4 4"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>

      {showToday && (
        <button
          type="button"
          className={`date-today-btn ${date === today ? 'active' : ''}`}
          onClick={() => onDateChange(today)}
        >
          Today
        </button>
      )}
    </div>
  );
}
