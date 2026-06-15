import { activityLevels } from '../labels';

interface ActivityLevelSelectProps {
  value: number;
  onChange: (value: number) => void;
}

export function ActivityLevelSelect({ value, onChange }: ActivityLevelSelectProps) {
  const selected = activityLevels.find((a) => a.level === value) ?? activityLevels[2];

  return (
    <div className="activity-select">
      <div className="activity-scale" role="radiogroup" aria-label="Уровень повседневной активности">
        {activityLevels.map((opt) => (
          <button
            key={opt.level}
            type="button"
            role="radio"
            aria-checked={value === opt.level}
            className={`activity-scale-btn${value === opt.level ? ' activity-scale-btn--on' : ''}`}
            onClick={() => onChange(opt.level)}
            title={opt.description}
          >
            {opt.level}
          </button>
        ))}
      </div>
      <div className="activity-detail">
        <strong>{selected.title}</strong>
        <p>{selected.description}</p>
        <p className="activity-hint">{selected.hint}</p>
      </div>
    </div>
  );
}
