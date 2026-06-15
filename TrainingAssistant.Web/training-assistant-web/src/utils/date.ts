export function toIsoDate(d: Date): string {
  return d.toISOString().slice(0, 10);
}

export function addDays(iso: string, days: number): string {
  const d = new Date(iso + 'T12:00:00');
  d.setDate(d.getDate() + days);
  return toIsoDate(d);
}

export function isoDateOnly(iso: string): string {
  return iso.slice(0, 10);
}

export function isDateInWeek(date: string, weekStart: string): boolean {
  const start = isoDateOnly(weekStart);
  return date >= start && date <= addDays(start, 6);
}

export function formatRuDate(iso: string): string {
  return new Date(iso + 'T12:00:00').toLocaleDateString('ru-RU', {
    weekday: 'short',
    day: 'numeric',
    month: 'long',
  });
}
