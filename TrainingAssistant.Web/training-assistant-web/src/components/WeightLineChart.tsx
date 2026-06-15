import type { BodyMetricLogDto } from '../api/types';
import { formatRuDate } from '../utils/date';

const WIDTH = 420;
const HEIGHT = 220;
const PAD = { top: 20, right: 16, bottom: 40, left: 48 };

function formatShortDate(iso: string, spanDays: number): string {
  const d = new Date(iso + 'T12:00:00');
  if (spanDays > 365) {
    return d.toLocaleDateString('ru-RU', { month: 'short', year: '2-digit' });
  }
  return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'short' });
}

function pickXLabels(points: BodyMetricLogDto[], spanDays: number): BodyMetricLogDto[] {
  if (points.length <= 4) return points;
  const mid = Math.floor(points.length / 2);
  return [points[0], points[mid], points[points.length - 1]];
}

function buildYTicks(yMin: number, yMax: number): number[] {
  const range = yMax - yMin;
  const step = range <= 3 ? 0.5 : range <= 8 ? 1 : range <= 20 ? 2 : 5;
  const ticks: number[] = [];
  let v = Math.ceil(yMin / step) * step;
  while (v <= yMax + step * 0.01) {
    ticks.push(Number(v.toFixed(1)));
    v += step;
  }
  return ticks;
}

function formatDelta(delta: number): string {
  if (Math.abs(delta) < 0.05) return 'без изменений';
  const sign = delta > 0 ? '+' : '';
  return `${sign}${delta.toFixed(1)} кг`;
}

export function WeightLineChart({ logs }: { logs: BodyMetricLogDto[] }) {
  const points = [...logs].sort((a, b) => a.logDate.localeCompare(b.logDate));
  if (points.length === 0) return null;

  const weights = points.map((p) => p.weightKg);
  const minW = Math.min(...weights);
  const maxW = Math.max(...weights);
  const padding = Math.max(0.5, (maxW - minW) * 0.2, 1);
  const yMin = Math.floor((minW - padding) * 2) / 2;
  const yMax = Math.ceil((maxW + padding) * 2) / 2;
  const yRange = yMax - yMin || 1;

  const plotW = WIDTH - PAD.left - PAD.right;
  const plotH = HEIGHT - PAD.top - PAD.bottom;

  const firstMs = new Date(points[0].logDate + 'T12:00:00').getTime();
  const lastMs = new Date(points[points.length - 1].logDate + 'T12:00:00').getTime();
  const spanMs = Math.max(lastMs - firstMs, 1);
  const spanDays = spanMs / 86_400_000;

  const coords = points.map((pt, i) => {
    const useTimeScale = points.length > 1 && lastMs > firstMs;
    const x = useTimeScale
      ? PAD.left + ((new Date(pt.logDate + 'T12:00:00').getTime() - firstMs) / spanMs) * plotW
      : PAD.left + (i / Math.max(1, points.length - 1)) * plotW;
    const y = PAD.top + plotH - ((pt.weightKg - yMin) / yRange) * plotH;
    return { x, y, pt };
  });

  const linePath =
    coords.length >= 2
      ? coords.map((c, i) => `${i === 0 ? 'M' : 'L'}${c.x.toFixed(1)},${c.y.toFixed(1)}`).join(' ')
      : '';

  const yTicks = buildYTicks(yMin, yMax);
  const xLabels = pickXLabels(points, spanDays);
  const delta = points.length >= 2 ? points[points.length - 1].weightKg - points[0].weightKg : 0;

  const ariaLabel =
    points.length === 1
      ? `Вес ${points[0].weightKg} кг на ${formatRuDate(points[0].logDate)}`
      : `Динамика веса от ${points[0].weightKg} до ${points[points.length - 1].weightKg} кг, ${formatDelta(delta)}`;

  return (
    <div className="weight-line-chart">
      <div className="weight-line-chart-summary">
        <span className="weight-line-chart-delta">
          {points.length >= 2 ? formatDelta(delta) : `${points[0].weightKg} кг`}
        </span>
        {points.length >= 2 ? (
          <span className="weight-line-chart-period">
            {formatRuDate(points[0].logDate)} — {formatRuDate(points[points.length - 1].logDate)}
          </span>
        ) : (
          <span className="weight-line-chart-period">{formatRuDate(points[0].logDate)}</span>
        )}
      </div>

      <svg
        viewBox={`0 0 ${WIDTH} ${HEIGHT}`}
        className="weight-line-chart-svg"
        role="img"
        aria-label={ariaLabel}
      >
        {yTicks.map((v) => {
          const y = PAD.top + plotH - ((v - yMin) / yRange) * plotH;
          return (
            <g key={v}>
              <line
                x1={PAD.left}
                y1={y}
                x2={PAD.left + plotW}
                y2={y}
                className="weight-line-chart-grid"
              />
              <text x={PAD.left - 8} y={y + 4} textAnchor="end" className="weight-line-chart-axis-y">
                {Number.isInteger(v) ? v : v.toFixed(1)}
              </text>
            </g>
          );
        })}

        <line
          x1={PAD.left}
          y1={PAD.top + plotH}
          x2={PAD.left + plotW}
          y2={PAD.top + plotH}
          className="weight-line-chart-axis"
        />

        {linePath ? <path d={linePath} className="weight-line-chart-line" fill="none" /> : null}

        {coords.map(({ x, y, pt }) => (
          <g key={pt.id}>
            <circle cx={x} cy={y} r={5} className="weight-line-chart-dot" />
            <title>
              {formatRuDate(pt.logDate)} — {pt.weightKg} кг
              {pt.note ? `: ${pt.note}` : ''}
            </title>
          </g>
        ))}

        {xLabels.map((pt) => {
          const c = coords.find((item) => item.pt.id === pt.id);
          if (!c) return null;
          return (
            <text
              key={`x-${pt.id}`}
              x={c.x}
              y={HEIGHT - 10}
              textAnchor="middle"
              className="weight-line-chart-axis-x"
            >
              {formatShortDate(pt.logDate, spanDays)}
            </text>
          );
        })}

        <text x={PAD.left - 8} y={PAD.top - 4} textAnchor="end" className="weight-line-chart-unit">
          кг
        </text>
      </svg>
    </div>
  );
}
