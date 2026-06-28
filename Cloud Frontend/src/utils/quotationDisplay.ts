import type { QuotationSummary } from '../types/quotation';
import { formatBranchDateTime } from './date';

export function quotationCustomerLabel(summary: QuotationSummary): string {
  const name = summary.customer_name?.trim();
  if (name) return name;
  if (summary.customer_id?.trim()) return `Customer ${summary.customer_id}`;
  if (summary.quotation_id?.trim()) return `Quotation ${summary.quotation_id.trim()}`;
  return 'Quotation';
}

function formatSyncedAt(iso: string): string {
  return formatBranchDateTime(iso);
}

/** Meta chips for feed cards (syncon shown separately in card header). */
export function quotationFeedMetaParts(summary: QuotationSummary): string[] {
  const parts: string[] = [];
  if (summary.status?.trim()) parts.push(summary.status.trim());
  if (summary.document_date) parts.push(`Doc ${summary.document_date}`);
  if (summary.city?.trim()) parts.push(summary.city.trim());
  if (summary.line_count > 0) {
    parts.push(`${summary.line_count} line${summary.line_count === 1 ? '' : 's'}`);
  }
  if (summary.reference_id?.trim()) parts.push(`Ref ${summary.reference_id.trim()}`);
  if (summary.assist_name?.trim()) parts.push(summary.assist_name.trim());
  return parts;
}

export function quotationMetaParts(summary: QuotationSummary): string[] {
  const parts: string[] = [];
  if (summary.synced_at) parts.push(`Synced ${formatSyncedAt(summary.synced_at)}`);
  if (summary.status?.trim()) parts.push(summary.status.trim());
  if (summary.document_date) parts.push(`Doc ${summary.document_date}`);
  if (summary.city?.trim()) parts.push(summary.city.trim());
  if (summary.line_count > 0) {
    parts.push(`${summary.line_count} line${summary.line_count === 1 ? '' : 's'}`);
  }
  if (summary.reference_id?.trim()) parts.push(`Ref ${summary.reference_id.trim()}`);
  if (summary.assist_name?.trim()) parts.push(summary.assist_name.trim());
  return parts;
}

export function quotationFlags(summary: QuotationSummary): string[] {
  const flags: string[] = [];
  if (summary.is_cancelled) flags.push('Cancelled');
  const est = summary.is_estimate?.trim();
  if (est && est !== '0' && est.toLowerCase() !== 'false' && est.toLowerCase() !== 'n') {
    flags.push('Estimate');
  }
  return flags;
}
