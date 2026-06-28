import type { Branch } from '../api/client';

/** Shared monitor list row from mirror header tables. */
export type MirrorDocumentRow = {
  id: number;
  external_id: string;
  customer_name?: string | null;
  party_name?: string | null;
  document_date?: string | null;
  status?: string | null;
  city?: string | null;
  reference_id?: string | null;
  payment_mode?: string | null;
  quotation_external_id?: string | null;
  is_billed?: boolean;
  line_count?: number;
  amount: number;
  total_amount?: number;
  sales_channel?: 'sales' | 'pipes' | 'traders';
  synced_at?: string | null;
  branch: Branch;
};

export type QuotationListRow = MirrorDocumentRow;

export type EstimationListRow = MirrorDocumentRow & {
  quotation_external_id?: string | null;
  is_billed?: boolean;
};

export type SaleListRow = MirrorDocumentRow & {
  sales_channel: 'sales' | 'pipes' | 'traders';
};
