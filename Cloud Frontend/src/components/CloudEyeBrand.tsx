import { BRAND } from '../config/brand';

type Props = {
  pageTitle?: string;
  className?: string;
  /** Feed / mobile top bar: single-line, smaller mark */
  compact?: boolean;
};

export function CloudEyeBrand({ pageTitle, className = '', compact = false }: Props) {
  return (
    <div
      className={`cloudeye-brand ${compact ? 'cloudeye-brand--compact' : ''} ${className}`.trim()}
    >
      <span className="cloudeye-mark" aria-hidden>
        <span className="cloudeye-iris" />
        <span className="cloudeye-shine" />
      </span>
      <span className="cloudeye-stack">
        <span className="cloudeye-name">{BRAND.productName}</span>
      </span>
      {pageTitle ? <span className="cloudeye-page">{pageTitle}</span> : null}
    </div>
  );
}
