/** ВОЗ: границы ИМТ и доля на шкале 15–45 */
const BMI_MIN = 15;
const BMI_MAX = 45;
const RANGE = BMI_MAX - BMI_MIN;

/** Границы зон на шкале (кг/м²) */
const BOUNDARIES = [18.5, 25, 30, 35, 40] as const;

const ZONES = [
  { upTo: 18.5, label: '<18,5', title: 'Недостаток', className: 'bmi-zone--low' },
  { upTo: 25, label: '18,5–24,9', title: 'Норма', className: 'bmi-zone--ok' },
  { upTo: 30, label: '25–29,9', title: 'Избыток', className: 'bmi-zone--over' },
  { upTo: 35, label: '30–34,9', title: 'Ожирение I', className: 'bmi-zone--ob1' },
  { upTo: 40, label: '35–39,9', title: 'Ожирение II', className: 'bmi-zone--ob2' },
  { upTo: BMI_MAX, label: '≥40', title: 'Ожирение III', className: 'bmi-zone--ob3' },
] as const;

function zoneWidth(from: number, to: number) {
  return `${((to - from) / RANGE) * 100}%`;
}

function segmentStarts(): number[] {
  const starts = [BMI_MIN];
  for (let i = 0; i < ZONES.length - 1; i++) starts.push(ZONES[i].upTo);
  return starts;
}

function activeZoneIndex(bmi: number): number {
  if (bmi < 18.5) return 0;
  if (bmi < 25) return 1;
  if (bmi < 30) return 2;
  if (bmi < 35) return 3;
  if (bmi < 40) return 4;
  return 5;
}

function markerPercent(bmi: number): number {
  const clamped = Math.min(BMI_MAX, Math.max(BMI_MIN, bmi));
  return ((clamped - BMI_MIN) / RANGE) * 100;
}

export function BmiZoneBar({ bmi, category }: { bmi: number; category: string }) {
  const starts = segmentStarts();
  const active = activeZoneIndex(bmi);
  const marker = markerPercent(bmi);

  return (
    <div className="bmi-zone-bar" role="img" aria-label={`ИМТ ${bmi}, ${category}`}>
      <div className="bmi-zone-scale">
        <div className="bmi-zone-track">
          {ZONES.map((z, i) => (
            <div
              key={z.title}
              className={`bmi-zone-seg ${z.className}${i === active ? ' bmi-zone-seg--active' : ''}${i < ZONES.length - 1 ? ' bmi-zone-seg--bordered' : ''}`}
              style={{ width: zoneWidth(starts[i], z.upTo) }}
              title={`${z.title} (до ${z.upTo === BMI_MAX ? '40+' : z.upTo})`}
            />
          ))}
          <div className="bmi-zone-marker" style={{ left: `${marker}%` }} aria-hidden />
        </div>
        <div className="bmi-zone-ticks" aria-hidden>
          <span className="bmi-zone-tick bmi-zone-tick--edge" style={{ left: '0%' }}>
            {BMI_MIN}
          </span>
          {BOUNDARIES.map((v) => (
            <span key={v} className="bmi-zone-tick" style={{ left: `${markerPercent(v)}%` }}>
              {v}
            </span>
          ))}
          <span className="bmi-zone-tick bmi-zone-tick--edge bmi-zone-tick--end" style={{ left: '100%' }}>
            {BMI_MAX}+
          </span>
        </div>
      </div>
      <ul className="bmi-zone-legend">
        {ZONES.map((z, i) => (
          <li key={z.title} className={i === active ? 'bmi-zone-legend--active' : ''}>
            <span className={`bmi-zone-dot ${z.className}`} />
            <span>{z.title}</span>
          </li>
        ))}
      </ul>
    </div>
  );
}
