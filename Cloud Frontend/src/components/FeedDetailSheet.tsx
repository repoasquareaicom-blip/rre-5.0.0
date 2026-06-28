import { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import {
  fetchFeedDetail,
  formatMoney,
  type FeedDetailResponse,
  type FeedItem,
} from '../api/client';
import {
  feedProductLineLabel,
  formatFeedDateTime,
  feedTypeLabel,
  formatPaymentMode,
} from '../utils/feedDisplay';

type Props = {
  item: FeedItem | null;
  onClose: () => void;
};

export function FeedDetailSheet({ item, onClose }: Props) {
  const [detail, setDetail] = useState<FeedDetailResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!item) {
      setDetail(null);
      setError('');
      return;
    }

    let cancelled = false;
    setLoading(true);
    setError('');

    fetchFeedDetail({
      entity_type: item.entity_type,
      branch_id: item.branch_id,
      external_id: item.external_id,
    })
      .then((data) => {
        if (!cancelled) setDetail(data);
      })
      .catch(() => {
        if (!cancelled) setError('Could not load bill details.');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [item]);

  useEffect(() => {
    if (!item) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [item, onClose]);

  if (!item || typeof document === 'undefined') return null;

  const showGst = detail?.lines.some((line) => line.gst != null && line.gst > 0) ?? false;
  const header = detail?.header;
  const paymentMode = formatPaymentMode(header?.payment_mode);
  const showPayment =
    item.entity_type === 'estimation' || item.entity_type === 'sale' ||
    item.entity_type === 'sales-pipe' || item.entity_type === 'sales-trader';

  return createPortal(
    <div className="feed-detail-overlay" role="presentation">
      <button
        type="button"
        className="menu-backdrop"
        onClick={onClose}
        aria-label="Close details"
      />
      <div
        className="feed-detail-sheet"
        role="dialog"
        aria-modal="true"
        aria-labelledby="feed-detail-title"
      >
        <div className="feed-detail-head">
          <div>
            <p className="feed-detail-kicker">{feedTypeLabel(item.entity_type)}</p>
            <h2 id="feed-detail-title" className="feed-detail-title mono">
              {item.external_id}
            </h2>
            {header?.branch?.name && (
              <p className="feed-detail-branch">{header.branch.name}</p>
            )}
          </div>
          <button type="button" className="menu-close" onClick={onClose} aria-label="Close">
            ✕
          </button>
        </div>

        {loading && (
          <div className="feed-detail-loading">
            <span className="loader" />
            Loading bill…
          </div>
        )}

        {error && <p className="error feed-detail-error">{error}</p>}

        {detail && !loading && (
          <>
            <div className="feed-detail-meta">
              <div className="feed-detail-meta-row">
                <span className="feed-detail-meta-label">Customer</span>
                <span className="feed-detail-meta-value">
                  {header?.customer_name?.trim() || '—'}
                </span>
              </div>
              <div className="feed-detail-meta-row">
                <span className="feed-detail-meta-label">Date</span>
                <span className="feed-detail-meta-value">
                  {formatFeedDateTime(header?.document_datetime)}
                </span>
              </div>
              {showPayment && (
                <div className="feed-detail-meta-row">
                  <span className="feed-detail-meta-label">Payment</span>
                  <span className="feed-detail-meta-value">
                    {paymentMode ? (
                      <span className="feed-payment-badge feed-payment-badge--detail">
                        {paymentMode}
                      </span>
                    ) : (
                      '—'
                    )}
                  </span>
                </div>
              )}
              {header?.quotation_id?.trim() && (
                <div className="feed-detail-meta-row">
                  <span className="feed-detail-meta-label">Quotation</span>
                  <span className="feed-detail-meta-value mono">{header.quotation_id}</span>
                </div>
              )}
              {header?.city?.trim() && (
                <div className="feed-detail-meta-row">
                  <span className="feed-detail-meta-label">City</span>
                  <span className="feed-detail-meta-value">{header.city}</span>
                </div>
              )}
            </div>

            <div className="feed-bill-wrap">
              <table className="feed-bill-table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Product</th>
                    <th className="num">Qty</th>
                    <th className="num">Rate</th>
                    {showGst && <th className="num">GST</th>}
                    <th className="num">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  {detail.lines.map((line) => {
                    const product = feedProductLineLabel(line);
                    return (
                    <tr key={`${line.line_no}-${line.product_id ?? ''}`}>
                      <td>{line.line_no}</td>
                      <td>
                        <span
                          className={`feed-product-name ${product.isMissingName ? 'feed-product-name--missing' : ''}`}
                        >
                          {product.name}
                        </span>
                        {product.productId && (
                          <span className="feed-product-id mono">{product.productId}</span>
                        )}
                      </td>
                      <td className="num">{line.quantity?.trim() || '—'}</td>
                      <td className="num">
                        {line.rate > 0 ? formatMoney(line.rate) : '—'}
                      </td>
                      {showGst && (
                        <td className="num">
                          {line.gst != null && line.gst > 0 ? formatMoney(line.gst) : '—'}
                        </td>
                      )}
                      <td className="num">{formatMoney(line.amount)}</td>
                    </tr>
                    );
                  })}
                  {!detail.lines.length && (
                    <tr>
                      <td colSpan={showGst ? 6 : 5} className="feed-bill-empty">
                        No line items synced for this document.
                      </td>
                    </tr>
                  )}
                </tbody>
                {detail.lines.length > 0 && (
                  <tfoot>
                    <tr>
                      <td colSpan={showGst ? 4 : 3} className="feed-bill-total-label">
                        Total ({header?.line_count ?? detail.lines.length} items)
                      </td>
                      {showGst && <td />}
                      <td className="num feed-bill-total-value">
                        {formatMoney(header?.total_amount ?? 0)}
                      </td>
                    </tr>
                  </tfoot>
                )}
              </table>
            </div>
          </>
        )}
      </div>
    </div>,
    document.body
  );
}
