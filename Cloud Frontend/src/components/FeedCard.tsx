import type { FeedItem } from '../api/client';
import {
  feedAmountLabel,
  feedCustomerLabel,
  feedDocumentSummary,
  feedItemsLabel,
  feedRecordDateTime,
  formatPaymentMode,
} from '../utils/feedDisplay';
import { feedItemSynconIso } from '../utils/feedSort';

const TYPE_ICON: Record<string, string> = {
  quotation: 'Q',
  estimation: 'E',
  sale: 'S',
  'sales-pipe': 'S',
  'sales-trader': 'S',
};

type Props = {
  item: FeedItem;
  isNew?: boolean;
  onSelect?: (item: FeedItem) => void;
};

export function FeedCard({ item, isNew = false, onSelect }: Props) {
  const doc = feedDocumentSummary(item);
  const customer = feedCustomerLabel(doc, item);
  const recordTime = feedRecordDateTime(item, doc);
  const itemsLabel = feedItemsLabel(doc.line_count);
  const amountLabel = feedAmountLabel(doc, item);
  const paymentMode = formatPaymentMode(doc.payment_mode);
  const dateTimeIso =
    doc.document_datetime ?? feedItemSynconIso(item) ?? item.occurred_at;

  return (
    <li className={`feed-card-wrap ${isNew ? 'feed-card-wrap--new' : ''}`}>
      <button
        type="button"
        className={`feed-card feed-${item.entity_type}`}
        onClick={() => onSelect?.(item)}
        aria-label={`View ${item.external_id} details`}
      >
      <div className={`feed-icon type-${item.entity_type}`}>
        {TYPE_ICON[item.entity_type] ?? '·'}
      </div>
      <div className="feed-body">
        <div className="feed-title-row">
          <strong className="feed-doc-id mono">{doc.document_id}</strong>
        </div>

        <time className="feed-datetime" dateTime={dateTimeIso}>
          {recordTime}
        </time>

        <p className="feed-customer-line">{customer}</p>

        {item.entity_type === 'estimation' && doc.quotation_id?.trim() && (
          <p className="feed-related-line">
            From quotation <span className="mono">{doc.quotation_id.trim()}</span>
          </p>
        )}

        {item.entity_type === 'estimation' && paymentMode && (
          <p className="feed-payment-line">
            <span className="feed-payment-label">Payment</span>
            <span className="feed-payment-badge">{paymentMode}</span>
          </p>
        )}

        <div className="feed-stats-row">
          <span className="feed-stat">
            <span className="feed-stat-label">Items</span>
            <span className="feed-stat-value">{itemsLabel}</span>
          </span>
          {amountLabel && (
            <span className="feed-stat feed-stat--amount">
              <span className="feed-stat-label">Amount</span>
              <span className="feed-stat-value">{amountLabel}</span>
            </span>
          )}
        </div>
      </div>
      </button>
    </li>
  );
}
