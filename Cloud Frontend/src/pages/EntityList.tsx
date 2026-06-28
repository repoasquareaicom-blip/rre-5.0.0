import { useEffect, useState } from 'react';
import { fetchDashboard, fetchList, formatMoney, type DocumentRow, type MonitorEntity } from '../api/client';
import { Filters } from '../components/Filters';
import { useFilters } from '../hooks/useFilters';

const titles: Record<MonitorEntity, string> = {
  quotations: 'Quotations',
  estimations: 'Estimations',
  sales: 'Sales',
  'sales-pipes': 'Sales (Pipes)',
  'sales-traders': 'Sales (Traders)',
  vouchers: 'Vouchers',
};

const subtitles: Record<MonitorEntity, string> = {
  quotations: 'QuotationHeader — synced from branch MS SQL',
  estimations: 'QuotationEstimation',
  sales: 'Sales',
  'sales-pipes': 'SalesPipes',
  'sales-traders': 'SalesTraders',
  vouchers: 'Vouchers',
};

const channelLabel: Record<string, string> = {
  sales: 'Sales',
  pipes: 'Pipes',
  traders: 'Traders',
};

export function EntityList({ entity }: { entity: MonitorEntity }) {
  const f = useFilters();
  const [branches, setBranches] = useState<{ id: number; code: string; name: string }[]>([]);
  const [rows, setRows] = useState<DocumentRow[]>([]);
  const [loading, setLoading] = useState(true);
  const isQuotations = entity === 'quotations';
  const isEstimations = entity === 'estimations';
  const isSalesFamily = entity === 'sales' || entity === 'sales-pipes' || entity === 'sales-traders';
  const isVouchers = entity === 'vouchers';

  useEffect(() => {
    fetchDashboard({ from: f.from, to: f.to }).then((d) => setBranches(d.branches));
  }, [f.from, f.to]);

  useEffect(() => {
    setLoading(true);
    fetchList(entity, f.params)
      .then((page) => setRows(page.data))
      .finally(() => setLoading(false));
  }, [entity, f.params]);

  const idLabel =
    entity === 'quotations'
      ? 'Quotation'
      : entity === 'estimations'
        ? 'Estimation'
        : isSalesFamily
          ? 'Sales ID'
          : 'ID';

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <h1>{titles[entity]}</h1>
          <p>{subtitles[entity]}</p>
        </div>
      </header>

      <Filters
        branches={branches}
        from={f.from}
        to={f.to}
        branchId={f.branchId}
        search={f.search}
        isSingleDay={f.isSingleDay}
        preset={f.preset}
        dateLabel={f.dateLabel}
        onFromChange={f.setFrom}
        onToChange={f.setTo}
        onDayChange={f.setDay}
        onPreviousDay={f.goPreviousDay}
        onNextDay={f.goNextDay}
        canGoNextDay={f.canGoNextDay}
        onPreset={f.applyPreset}
        onBranchChange={f.setBranchId}
        onSearchChange={f.setSearch}
      />

      <section className="panel table-panel">
        {loading ? (
          <p className="muted">Loading…</p>
        ) : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>{idLabel}</th>
                  <th>Branch</th>
                  <th>Date</th>
                  {isQuotations || isEstimations || isSalesFamily ? (
                    <>
                      <th>Customer</th>
                      <th>City</th>
                      {isEstimations && <th>Quotation</th>}
                      {isSalesFamily && <th>Payment</th>}
                      {!isSalesFamily && <th>Ref</th>}
                      {(isQuotations || isEstimations) && <th>Lines</th>}
                    </>
                  ) : (
                    <th>{isVouchers ? 'Party' : 'Customer'}</th>
                  )}
                  <th>Status</th>
                  <th className="num">Amount</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={`${row.branch?.id ?? 0}-${row.external_id}-${row.id}`}>
                    <td className="mono">{row.external_id}</td>
                    <td>{row.branch?.name}</td>
                    <td>{row.document_date || '—'}</td>
                    {isQuotations || isEstimations || isSalesFamily ? (
                      <>
                        <td>{row.customer_name?.trim() || '—'}</td>
                        <td>{row.city?.trim() || '—'}</td>
                        {isEstimations && (
                          <td className="mono">{row.quotation_external_id?.trim() || '—'}</td>
                        )}
                        {isSalesFamily && (
                          <td>{row.payment_mode?.trim() || '—'}</td>
                        )}
                        {!isSalesFamily && (
                          <td className="mono">{row.reference_id?.trim() || '—'}</td>
                        )}
                        {(isQuotations || isEstimations) && (
                          <td className="num">{row.line_count ?? 0}</td>
                        )}
                      </>
                    ) : (
                      <td>{row.customer_name || row.party_name || '—'}</td>
                    )}
                    <td>
                      {isEstimations && row.is_billed != null ? (
                        <span className={`badge ${row.is_billed ? 'badge-ok' : ''}`}>
                          {row.is_billed ? 'Billed' : row.status || 'Open'}
                        </span>
                      ) : isSalesFamily && row.sales_channel ? (
                        <span className="badge badge-channel">
                          {channelLabel[row.sales_channel] ?? row.sales_channel}
                        </span>
                      ) : (
                        <span className="badge">{row.status || '—'}</span>
                      )}
                    </td>
                    <td className="num">
                      {formatMoney(row.total_amount ?? row.amount ?? 0)}
                    </td>
                  </tr>
                ))}
                {!rows.length && (
                  <tr>
                    <td
                      colSpan={
                        isQuotations ? 9 : isEstimations ? 10 : isSalesFamily ? 8 : isVouchers ? 6 : 6
                      }
                      className="muted center"
                    >
                      No records for this period. Run branch sync for this table.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}
