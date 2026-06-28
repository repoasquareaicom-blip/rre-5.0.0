import { formatMoney, type FeedDocumentSummary, type FeedItem } from '../api/client';
import type { QuotationSummary } from '../types/quotation';
import { formatBranchDateTime } from './date';
import { feedItemSynconIso } from './feedSort';

export function formatFeedDateTime(value: string | null | undefined): string {
  return formatBranchDateTime(value);
}

export function formatPaymentMode(mode?: string | null): string | null {
  if (!mode?.trim()) return null;
  const raw = mode.trim();
  const key = raw.toLowerCase();
  if (key.includes('cash')) return 'Cash';
  if (key.includes('credit')) return 'Credit';
  if (key.includes('part')) return 'Partial';
  return raw.charAt(0).toUpperCase() + raw.slice(1);
}

export function feedTypeLabel(entityType: string): string {
  switch (entityType) {
    case 'quotation':
      return 'Quotation';
    case 'estimation':
      return 'Estimation';
    case 'sale':
    case 'sales-pipe':
    case 'sales-trader':
      return 'Sales';
    default:
      return entityType;
  }
}

function documentFromQuotation(summary: QuotationSummary, fallbackId: string): FeedDocumentSummary {
  return {
    document_id: summary.quotation_id || fallbackId,
    customer_name: summary.customer_name,
    document_datetime:
      summary.document_datetime ?? summary.document_date ?? summary.synced_at,
    line_count: summary.line_count,
    total_amount: summary.total_amount,
  };
}

export function feedDocumentSummary(item: FeedItem): FeedDocumentSummary {
  if (item.document) {
    return item.document;
  }

  if (item.entity_type === 'quotation' && item.quotation) {
    return documentFromQuotation(item.quotation, item.external_id);
  }

  const amount =
    item.amount != null && item.amount !== '' ? Number(item.amount) : 0;

  return {
    document_id: item.external_id,
    customer_name: item.subtitle,
    document_datetime: item.activity_date ?? feedItemSynconIso(item),
    line_count: 0,
    total_amount: Number.isNaN(amount) ? 0 : amount,
    quotation_id: null,
  };
}

export function feedCustomerLabel(doc: FeedDocumentSummary, item: FeedItem): string {
  const name = doc.customer_name?.trim();
  if (name) return name;
  if (item.subtitle?.trim()) return item.subtitle.trim();
  if (item.entity_type === 'estimation' && doc.quotation_id?.trim()) {
    return `Estimation for Quotation ${doc.quotation_id.trim()}`;
  }
  return feedTypeLabel(item.entity_type);
}

export function feedRecordDateTime(item: FeedItem, doc: FeedDocumentSummary): string {
  return formatFeedDateTime(
    doc.document_datetime ?? feedItemSynconIso(item) ?? item.occurred_at
  );
}

export function feedProductLineLabel(line: {
  product_name?: string | null;
  product_id?: string | null;
}): { name: string; productId: string | null; isMissingName: boolean } {
  const productName = line.product_name?.trim() || '';
  const productId = line.product_id?.trim() || '';

  if (productName) {
    return { name: productName, productId: productId || null, isMissingName: false };
  }

  if (productId) {
    return { name: 'Name not synced', productId, isMissingName: true };
  }

  return { name: '—', productId: null, isMissingName: false };
}

export function feedItemsLabel(count: number): string {
  if (count === 1) return '1 item';
  return `${count} items`;
}

export function feedAmountLabel(doc: FeedDocumentSummary, item: FeedItem): string | null {
  const amount =
    doc.total_amount > 0
      ? doc.total_amount
      : item.amount != null && item.amount !== ''
        ? Number(item.amount)
        : null;
  if (amount == null || Number.isNaN(amount) || amount <= 0) return null;
  return formatMoney(amount);
}
