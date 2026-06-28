import { useCallback, useMemo, useState } from 'react';
import {
  activePreset,
  canGoNext,
  daysBetween,
  formatDateRangeLabel,
  localDateString,
  presetRange,
  shiftDate,
  type DatePreset,
} from '../utils/date';

export function useFilters() {
  const today = useMemo(() => localDateString(), []);
  const [from, setFrom] = useState(today);
  const [to, setTo] = useState(today);
  const [branchId, setBranchId] = useState('');
  const [search, setSearch] = useState('');

  const isSingleDay = from === to;
  const preset = useMemo(() => activePreset(from, to), [from, to]);
  const dateLabel = useMemo(
    () => (isSingleDay ? undefined : formatDateRangeLabel(from, to)),
    [from, to, isSingleDay]
  );

  const params = useMemo(
    () => ({
      from,
      to,
      ...(branchId ? { branch_id: Number(branchId) } : {}),
      ...(search ? { search } : {}),
    }),
    [from, to, branchId, search]
  );

  const setDay = useCallback((date: string) => {
    setFrom(date);
    setTo(date);
  }, []);

  const applyPreset = useCallback((p: DatePreset) => {
    const range = presetRange(p);
    setFrom(range.from);
    setTo(range.to);
  }, []);

  const goPreviousDay = useCallback(() => {
    if (isSingleDay) {
      const d = shiftDate(from, -1);
      setFrom(d);
      setTo(d);
      return;
    }
    const span = daysBetween(from, to);
    const newTo = shiftDate(to, -1);
    setTo(newTo);
    setFrom(shiftDate(newTo, -span));
  }, [from, to, isSingleDay]);

  const goNextDay = useCallback(() => {
    if (!canGoNext(to)) return;
    if (isSingleDay) {
      const d = shiftDate(from, 1);
      setFrom(d);
      setTo(d);
      return;
    }
    const span = daysBetween(from, to);
    const newFrom = shiftDate(from, 1);
    const newTo = shiftDate(newFrom, span);
    const cappedTo = newTo > today ? today : newTo;
    const cappedFrom = shiftDate(cappedTo, -span);
    setFrom(cappedFrom);
    setTo(cappedTo);
  }, [from, to, isSingleDay, today]);

  return {
    from,
    to,
    branchId,
    search,
    params,
    isSingleDay,
    preset,
    dateLabel,
    setFrom,
    setTo,
    setDay,
    setBranchId,
    setSearch,
    applyPreset,
    goPreviousDay,
    goNextDay,
    canGoNextDay: canGoNext(to),
  };
}
