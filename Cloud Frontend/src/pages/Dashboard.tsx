import { useEffect, useState, type CSSProperties } from 'react';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import { fetchDashboard, formatMoney, type DashboardResponse } from '../api/client';
import { Filters } from '../components/Filters';
import { useFilters } from '../hooks/useFilters';

const cards = [
  { key: 'quotations', label: 'Quotations', color: '#6366f1' },
  { key: 'estimations', label: 'Estimations', color: '#8b5cf6' },
  { key: 'sales', label: 'Sales', color: '#10b981' },
  { key: 'sales_pipes', label: 'Sales (Pipes)', color: '#14b8a6' },
  { key: 'sales_traders', label: 'Sales (Traders)', color: '#059669' },
  { key: 'vouchers', label: 'Vouchers', color: '#f59e0b' },
] as const;

export function Dashboard() {
  const f = useFilters();
  const [data, setData] = useState<DashboardResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    setLoading(true);
    setError('');
    fetchDashboard(f.params)
      .then(setData)
      .catch(() => setError('Could not load dashboard. Is the API running?'))
      .finally(() => setLoading(false));
  }, [f.params]);

  const chartData =
    data?.sales_by_branch.map((row) => ({
      name: row.branch?.name ?? `Branch ${row.branch_id}`,
      total: Number(row.total),
      count: row.count,
    })) ?? [];

  const salesTotal =
    data == null
      ? 0
      : data.summary.sales.amount +
        data.summary.sales_pipes.amount +
        data.summary.sales_traders.amount;

  const salesCount =
    data == null
      ? 0
      : data.summary.sales.count +
        data.summary.sales_pipes.count +
        data.summary.sales_traders.count;

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <h1>Dashboard</h1>
          <p>Live totals from synced branch header tables (quotations, estimations, sales)</p>
        </div>
      </header>

      <Filters
        branches={data?.branches ?? []}
        from={f.from}
        to={f.to}
        branchId={f.branchId}
        search=""
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
        onSearchChange={() => {}}
        showSearch={false}
      />

      {loading && <p className="muted">Loading…</p>}
      {error && <p className="error">{error}</p>}

      {data && (
        <>
          <div className="kpi-grid kpi-grid--six">
            {cards.map((card) => {
              const item = data.summary[card.key];
              return (
                <article
                  key={card.key}
                  className="kpi-card"
                  style={{ '--accent': card.color } as CSSProperties}
                >
                  <span className="kpi-label">{card.label}</span>
                  <strong className="kpi-value">{item.count}</strong>
                  <span className="kpi-amount">{formatMoney(item.amount)}</span>
                </article>
              );
            })}
          </div>

          <section className="panel chart-panel">
            <div className="chart-panel-head">
              <h2>All sales by branch</h2>
              <p className="muted chart-sub">
                {salesCount} invoices · {formatMoney(salesTotal)} combined (Sales + Pipes + Traders)
              </p>
            </div>
            <div className="chart-wrap">
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                  <XAxis dataKey="name" tick={{ fontSize: 11 }} interval={0} angle={-12} textAnchor="end" height={56} />
                  <YAxis tick={{ fontSize: 12 }} />
                  <Tooltip
                    formatter={(v, name) =>
                      name === 'count' ? [v, 'Invoices'] : [formatMoney(Number(v)), 'Amount']
                    }
                  />
                  <Legend />
                  <Bar dataKey="total" name="Amount" fill="#10b981" radius={[6, 6, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </section>
        </>
      )}
    </div>
  );
}
