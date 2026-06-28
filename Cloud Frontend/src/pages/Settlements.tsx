import { useEffect, useState } from 'react';
import { fetchDashboard, fetchSettlements, formatMoney } from '../api/client';
import { Filters } from '../components/Filters';
import { useFilters } from '../hooks/useFilters';

type SettlementRow = {
  id: number;
  external_id: string;
  settlement_date: string;
  beginning_cash: string;
  cash_in: string;
  cash_out: string;
  net_cash: string;
  actual_cash: string;
  over_short: string;
  status?: string;
  branch: { id: number; code: string; name: string };
};

export function Settlements() {
  const f = useFilters();
  const [branches, setBranches] = useState<{ id: number; code: string; name: string }[]>([]);
  const [totals, setTotals] = useState<Record<string, number> | null>(null);
  const [rows, setRows] = useState<SettlementRow[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboard({ from: f.from, to: f.to }).then((d) => setBranches(d.branches));
  }, [f.from, f.to]);

  useEffect(() => {
    setLoading(true);
    fetchSettlements(f.params)
      .then((data) => {
        setTotals(data.totals);
        setRows(data.rows);
      })
      .finally(() => setLoading(false));
  }, [f.params]);

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <h1>Settlement summary</h1>
          <p>Day-end cash close totals across branches</p>
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

      {totals && (
        <div className="kpi-grid settlement-totals">
          <article className="kpi-card">
            <span className="kpi-label">Settlements</span>
            <strong className="kpi-value">{totals.count}</strong>
          </article>
          <article className="kpi-card">
            <span className="kpi-label">Cash in</span>
            <span className="kpi-amount">{formatMoney(totals.cash_in)}</span>
          </article>
          <article className="kpi-card">
            <span className="kpi-label">Cash out</span>
            <span className="kpi-amount">{formatMoney(totals.cash_out)}</span>
          </article>
          <article className="kpi-card">
            <span className="kpi-label">Over / short</span>
            <span className="kpi-amount">{formatMoney(totals.over_short)}</span>
          </article>
        </div>
      )}

      <section className="panel table-panel">
        {loading ? (
          <p className="muted">Loading…</p>
        ) : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Branch</th>
                  <th className="num">Beginning</th>
                  <th className="num">Cash in</th>
                  <th className="num">Cash out</th>
                  <th className="num">Net</th>
                  <th className="num">Actual</th>
                  <th className="num">Over/short</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={row.id}>
                    <td>{row.settlement_date}</td>
                    <td>{row.branch?.name}</td>
                    <td className="num">{formatMoney(row.beginning_cash)}</td>
                    <td className="num">{formatMoney(row.cash_in)}</td>
                    <td className="num">{formatMoney(row.cash_out)}</td>
                    <td className="num">{formatMoney(row.net_cash)}</td>
                    <td className="num">{formatMoney(row.actual_cash)}</td>
                    <td className="num">{formatMoney(row.over_short)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}
