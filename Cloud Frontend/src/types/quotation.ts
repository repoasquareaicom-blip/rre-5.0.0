export type QuotationSummary = {
  quotation_id: string;
  customer_name?: string | null;
  customer_id?: string | null;
  status?: string | null;
  document_date?: string | null;
  document_datetime?: string | null;
  /** When this quotation was last synced to cloud (server time). */
  synced_at?: string | null;
  city?: string | null;
  reference_id?: string | null;
  assist_name?: string | null;
  line_count: number;
  total_amount: number;
  is_estimate?: string | null;
  is_cancelled?: boolean;
};

export type QuotationListRow = {
  id: number;
  external_id: string;
  customer_name?: string | null;
  document_date?: string | null;
  status?: string | null;
  city?: string | null;
  reference_id?: string | null;
  assist_name?: string | null;
  line_count?: number;
  total_amount?: number;
  amount?: number | string;
  branch: { id: number; code: string; name: string };
};
