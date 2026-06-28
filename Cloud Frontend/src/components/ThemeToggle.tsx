import { useTheme } from '../theme/ThemeProvider';

type Props = {
  className?: string;
  compact?: boolean;
};

export function ThemeToggle({ className = '', compact }: Props) {
  const { theme, toggleTheme } = useTheme();
  const isDark = theme === 'dark';

  return (
    <button
      type="button"
      className={`theme-toggle ${compact ? 'theme-toggle--compact' : ''} ${className}`.trim()}
      onClick={toggleTheme}
      aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
      title={isDark ? 'Light mode' : 'Dark mode'}
    >
      <span className="theme-toggle-track" aria-hidden>
        <span className={`theme-toggle-thumb ${isDark ? 'is-dark' : ''}`}>
          {isDark ? (
            <svg viewBox="0 0 24 24" fill="none" className="theme-toggle-icon">
              <path
                d="M21 14.5A8.5 8.5 0 0 1 9.5 3 7 7 0 1 0 21 14.5Z"
                stroke="currentColor"
                strokeWidth="1.75"
                strokeLinejoin="round"
              />
            </svg>
          ) : (
            <svg viewBox="0 0 24 24" fill="none" className="theme-toggle-icon">
              <circle cx="12" cy="12" r="4.25" stroke="currentColor" strokeWidth="1.75" />
              <path
                d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M4.93 19.07l1.41-1.41M17.66 6.34l1.41-1.41"
                stroke="currentColor"
                strokeWidth="1.75"
                strokeLinecap="round"
              />
            </svg>
          )}
        </span>
      </span>
      {!compact && (
        <span className="theme-toggle-label">{isDark ? 'Dark' : 'Light'}</span>
      )}
    </button>
  );
}
