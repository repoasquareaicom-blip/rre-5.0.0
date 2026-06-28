import { formatMoney } from '../api/client';
import type { QuotationSummary } from '../types/quotation';
import {
  quotationCustomerLabel,
  quotationFeedMetaParts,
  quotationFlags,
} from '../utils/quotationDisplay';

type Props = {
  summary: QuotationSummary;
  fallbackId: string;
  amount?: string | number | null;
};

export function QuotationFeedSummary({ summary, fallbackId, amount }: Props) {
  const customer = quotationCustomerLabel(summary);
  const meta = quotationFeedMetaParts(summary);
  const flags = quotationFlags(summary);
  const displayAmount =
    summary.total_amount > 0
      ? summary.total_amount
      : amount != null && amount !== ''
        ? Number(amount)
        : null;
  const docId = summary.quotation_id || fallbackId;

  return (
    <div className="feed-card-detail">
      <p className="feed-primary-line">
        <span className="feed-customer">{customer}</span>
        {flags.map((flag) => (
          <span key={flag} className="feed-flag">
            {flag}
          </span>
        ))}
      </p>
      {meta.length > 0 && (
        <ul className="feed-meta-chips" aria-label="Quotation details">
          {meta.map((part) => (
            <li key={part}>{part}</li>
          ))}
        </ul>
      )}
      <div className="feed-card-foot">
        <span className="mono feed-doc-id">{docId}</span>
        {displayAmount != null && !Number.isNaN(displayAmount) && displayAmount > 0 && (
          <span className="feed-amount">{formatMoney(displayAmount)}</span>
        )}
      </div>
    </div>
  );
}
