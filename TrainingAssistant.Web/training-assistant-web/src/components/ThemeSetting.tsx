import { useTheme, type Theme } from '../context/ThemeContext';

const options: { value: Theme; label: string; icon: string }[] = [
  { value: 'light', label: 'Светлая', icon: '☀' },
  { value: 'dark', label: 'Тёмная', icon: '☾' },
];

export function ThemeSetting() {
  const { theme, setTheme } = useTheme();

  return (
    <div className="theme-setting">
      <p className="muted settings-hint">Выберите оформление интерфейса. Настройка сохраняется в браузере.</p>
      <div className="theme-setting-options" role="radiogroup" aria-label="Тема оформления">
        {options.map((opt) => (
          <button
            key={opt.value}
            type="button"
            role="radio"
            aria-checked={theme === opt.value}
            className={`theme-setting-option${theme === opt.value ? ' theme-setting-option--active' : ''}`}
            onClick={() => setTheme(opt.value)}
          >
            <span className="theme-setting-option-icon" aria-hidden="true">
              {opt.icon}
            </span>
            <span>{opt.label}</span>
          </button>
        ))}
      </div>
    </div>
  );
}
