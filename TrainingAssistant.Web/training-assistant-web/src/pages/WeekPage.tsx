import { useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import {
  adjustTrainingDay,
  generateWeek,
  getCurrentWeek,
  getExerciseAlternatives,
  replaceExercise,
  setExerciseCompleted,
  setMealItemCompleted,
} from '../api/plans';
import type { MealItemDto, TrainingExerciseDto, WeekPlanDto } from '../api/types';
import { useAuth } from '../context/AuthContext';
import { mealLabels } from '../labels';
import { addDays, isoDateOnly } from '../utils/date';
import { BmiZoneBar } from '../components/BmiZoneBar';
import { greetingLine } from '../utils/greeting';

export function WeekPage() {
  const { name } = useAuth();
  const who = greetingLine(name);
  const [plan, setPlan] = useState<WeekPlanDto | null>(null);
  const [dayIndex, setDayIndex] = useState(1);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState('');
  const [swapTarget, setSwapTarget] = useState<TrainingExerciseDto | null>(null);
  const [alternatives, setAlternatives] = useState<string[]>([]);
  const [swapLoading, setSwapLoading] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);
  const [exporting, setExporting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await getCurrentWeek();
      setPlan(data);
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) {
        setPlan(null);
      } else {
        setError(err instanceof ApiError ? err.message : 'Не удалось загрузить план');
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const reloadQuiet = useCallback(async () => {
    try {
      const data = await getCurrentWeek();
      setPlan(data);
    } catch {
    }
  }, []);

  useEffect(() => {
    const onWeightUpdated = () => void reloadQuiet();
    window.addEventListener('ta-profile-weight-updated', onWeightUpdated);
    return () => window.removeEventListener('ta-profile-weight-updated', onWeightUpdated);
  }, [reloadQuiet]);

  async function handleExportPdf() {
    if (!plan) return;
    setExporting(true);
    setError('');
    try {
      const { exportWeekPlanPdf } = await import('../utils/exportWeekPdf');
      exportWeekPlanPdf(plan, name || undefined);
    } catch {
      setError('Не удалось сформировать PDF');
    } finally {
      setExporting(false);
    }
  }

  async function handleGenerate() {
    setGenerating(true);
    setError('');
    try {
      const data = await generateWeek();
      setPlan(data);
      setDayIndex(1);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось сформировать план');
    } finally {
      setGenerating(false);
    }
  }

  async function openSwap(ex: TrainingExerciseDto) {
    setSwapTarget(ex);
    setSwapLoading(true);
    try {
      setAlternatives(await getExerciseAlternatives(ex.name));
    } catch {
      setAlternatives([]);
    } finally {
      setSwapLoading(false);
    }
  }

  async function confirmSwap(newName: string) {
    if (!swapTarget) return;
    setActionLoading(true);
    setError('');
    try {
      const data = await replaceExercise(swapTarget.id, { newName, rememberForFuture: true });
      setPlan(data);
      setSwapTarget(null);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось заменить упражнение');
    } finally {
      setActionLoading(false);
    }
  }

  async function handleIntensity(mode: 'easier' | 'harder') {
    const training = plan?.trainingDays.find((d) => d.dayIndex === dayIndex);
    if (!training || training.isRestDay) return;
    setActionLoading(true);
    setError('');
    try {
      const data = await adjustTrainingDay(training.id, { mode });
      setPlan(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось изменить день');
    } finally {
      setActionLoading(false);
    }
  }

  async function toggleExercise(ex: TrainingExerciseDto) {
    setActionLoading(true);
    try {
      const data = await setExerciseCompleted(ex.id, { isCompleted: !ex.isCompleted });
      setPlan(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка сохранения');
    } finally {
      setActionLoading(false);
    }
  }

  async function toggleMealItem(item: MealItemDto) {
    if (!item.id) return;
    setActionLoading(true);
    try {
      const data = await setMealItemCompleted(item.id, { isCompleted: !item.isCompleted });
      setPlan(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка сохранения');
    } finally {
      setActionLoading(false);
    }
  }

  const nutrition = plan?.nutritionDays.find((d) => d.dayIndex === dayIndex);
  const training = plan?.trainingDays.find((d) => d.dayIndex === dayIndex);
  const trainPct =
    plan && plan.totalExercises > 0
      ? Math.round((plan.completedExercises / plan.totalExercises) * 100)
      : 0;
  const mealPct =
    plan && plan.totalMealItems > 0
      ? Math.round((plan.completedMealItems / plan.totalMealItems) * 100)
      : 0;

  return (
    <div className="week">
      <header className="week-head">
        <div>
          <p className="eyebrow">Неделя · {who}</p>
          <h1>{plan ? `Ваш план с ${formatDate(plan.weekStart)}` : `${who}, соберём первую неделю?`}</h1>
          {plan ? (
            <p className="lede">
              Период {formatDate(plan.weekStart)} — {formatDate(addDays(isoDateOnly(plan.weekStart), 6))} (7 дней от
              регистрации).
            </p>
          ) : (
            <p className="lede">Нажмите кнопку справа — подберём меню и нагрузку на 7 дней.</p>
          )}
        </div>
        <div className="week-head-actions">
          {plan ? (
            <button
              type="button"
              className="btn"
              onClick={handleExportPdf}
              disabled={exporting || generating || actionLoading}
            >
              {exporting ? 'PDF…' : 'Скачать PDF'}
            </button>
          ) : null}
          <button
            type="button"
            className="btn btn-primary"
            onClick={handleGenerate}
            disabled={generating || actionLoading || exporting}
          >
            {generating ? 'Собираем…' : plan ? 'Пересобрать' : 'Сформировать'}
          </button>
        </div>
      </header>

      {error ? <p className="banner banner-error">{error}</p> : null}

      {plan?.health ? (
        <section className="health-panel panel-card">
          <div className="health-bmi-row">
            <p className="health-bmi">
              ИМТ <strong>{plan.health.bmi}</strong>
              <span className="health-bmi-cat"> — {plan.health.bmiCategory}</span>
            </p>
            <BmiZoneBar bmi={plan.health.bmi} category={plan.health.bmiCategory} />
          </div>
          {plan.health.warnings.map((w) => (
            <p key={w} className="banner banner-warn">
              {w}
            </p>
          ))}
          {plan.health.recommendations.length > 0 ? (
            <ul className="health-recs">
              {plan.health.recommendations.map((r) => (
                <li key={r}>{r}</li>
              ))}
            </ul>
          ) : null}
        </section>
      ) : null}

      {plan ? (
        <div className="progress-strip">
          <span>
            Тренировки: {plan.completedExercises}/{plan.totalExercises} ({trainPct}%)
          </span>
          <span>
            Питание: {plan.completedMealItems}/{plan.totalMealItems} ({mealPct}%)
          </span>
        </div>
      ) : null}

      {loading ? <p className="muted">Загрузка…</p> : null}

      {!loading && !plan ? (
        <p className="empty">Нажмите «Сформировать» — подберём питание и тренировки на 7 дней.</p>
      ) : null}

      {plan ? (
        <>
          <div className="day-strip" role="tablist">
            {Array.from({ length: 7 }, (_, i) => {
              const idx = i + 1;
              const dateIso = addDays(isoDateOnly(plan.weekStart), i);
              const tr = plan.trainingDays.find((d) => d.dayIndex === idx);
              const isRest = tr?.isRestDay;
              return (
                <button
                  key={idx}
                  type="button"
                  role="tab"
                  aria-selected={dayIndex === idx}
                  className={`day-btn${dayIndex === idx ? ' day-btn--active' : ''}${isRest ? ' day-btn--rest' : ''}`}
                  onClick={() => setDayIndex(idx)}
                >
                  <span className="day-btn-label">{formatDayChip(dateIso)}</span>
                  <span className="day-btn-sub">{isRest ? 'отдых' : `день ${idx}`}</span>
                </button>
              );
            })}
          </div>

          <div className="columns">
            <section className="panel panel-card">
              <h2>Питание</h2>
              {nutrition ? (
                <>
                  <p className="nutrition-hint muted">
                    Позиции «<strong>или</strong>» — замены в категории. Отмечайте выполненные приёмы.
                  </p>
                  <p className="macro">
                    {nutrition.targetCalories} ккал · Б {nutrition.targetProteinG} · Ж{' '}
                    {nutrition.targetFatG} · У {nutrition.targetCarbsG}
                  </p>
                  <ul className="meal-list">
                    {nutrition.meals.map((meal) => (
                      <MealBlock
                        key={meal.mealType}
                        mealType={meal.mealType}
                        items={meal.items}
                        onToggle={toggleMealItem}
                        disabled={actionLoading}
                      />
                    ))}
                  </ul>
                </>
              ) : (
                <p className="muted">Нет данных</p>
              )}
            </section>

            <section className="panel panel-card">
              <h2>Тренировка</h2>
              {training?.isRestDay ? (
                <p className="rest">День отдыха — восстановление тоже часть плана.</p>
              ) : training ? (
                <>
                  {training.focus ? <p className="macro">{training.focus}</p> : null}
                  <div className="day-actions">
                    <button
                      type="button"
                      className="btn btn-ghost btn-sm"
                      disabled={actionLoading}
                      onClick={() => handleIntensity('easier')}
                    >
                      Легче сегодня
                    </button>
                    <button
                      type="button"
                      className="btn btn-ghost btn-sm"
                      disabled={actionLoading}
                      onClick={() => handleIntensity('harder')}
                    >
                      Тяжелее
                    </button>
                  </div>
                  <ol className="exercise-list">
                    {training.exercises.map((ex) => (
                      <li key={ex.id} className={ex.isCompleted ? 'exercise-done' : ''}>
                        <label className="ex-check">
                          <input
                            type="checkbox"
                            checked={!!ex.isCompleted}
                            disabled={actionLoading}
                            onChange={() => toggleExercise(ex)}
                          />
                          <span className="ex-check-box" />
                        </label>
                        <div className="ex-body">
                          <div className="ex-name">{ex.name}</div>
                          <div className="ex-meta">
                            {ex.sets}×{ex.reps}
                            {ex.restSec ? ` · ${ex.restSec} с` : ''}
                          </div>
                          {ex.notes ? <p className="ex-note">{ex.notes}</p> : null}
                          <button
                            type="button"
                            className="btn-link"
                            onClick={() => openSwap(ex)}
                            disabled={actionLoading}
                          >
                            Заменить
                          </button>
                        </div>
                      </li>
                    ))}
                  </ol>
                </>
              ) : (
                <p className="muted">Нет данных</p>
              )}
            </section>
          </div>
        </>
      ) : null}

      {swapTarget ? (
        <div className="modal-backdrop" role="dialog" aria-modal="true">
          <div className="modal panel-card">
            <h2>Заменить «{swapTarget.name}»</h2>
            <p className="muted">Выбор сохранится для следующих недель.</p>
            {swapLoading ? <p className="muted">Загрузка…</p> : null}
            <ul className="alt-list">
              {alternatives.map((name) => (
                <li key={name}>
                  <button
                    type="button"
                    className="btn btn-ghost"
                    disabled={actionLoading}
                    onClick={() => confirmSwap(name)}
                  >
                    {name}
                  </button>
                </li>
              ))}
            </ul>
            <button type="button" className="btn btn-ghost" onClick={() => setSwapTarget(null)}>
              Отмена
            </button>
          </div>
        </div>
      ) : null}
    </div>
  );
}

function MealBlock({
  mealType,
  items,
  onToggle,
  disabled,
}: {
  mealType: string;
  items: MealItemDto[];
  onToggle: (item: MealItemDto) => void;
  disabled: boolean;
}) {
  const primary = items.filter((i) => !i.isAlternative);
  const alternatives = items.filter((i) => i.isAlternative);

  return (
    <li className="meal-block">
      <h3>{mealLabels[mealType] ?? mealType}</h3>
      <ul>
        {primary.map((item) => (
          <MealRow key={item.id || item.foodName} item={item} onToggle={onToggle} disabled={disabled} />
        ))}
      </ul>
      {alternatives.length > 0 ? (
        <div className="meal-alts">
          <p className="meal-alts-label">
            <span className="meal-alts-or">или</span> замена:
          </p>
          <ul>
            {alternatives.map((item) => (
              <MealRow key={item.id || item.foodName} item={item} onToggle={onToggle} disabled={disabled} />
            ))}
          </ul>
        </div>
      ) : null}
    </li>
  );
}

function MealRow({
  item,
  onToggle,
  disabled,
}: {
  item: MealItemDto;
  onToggle: (item: MealItemDto) => void;
  disabled: boolean;
}) {
  return (
    <li className={item.isCompleted ? 'meal-done' : ''}>
      <label className="meal-check">
        <input
          type="checkbox"
          checked={!!item.isCompleted}
          disabled={disabled || !item.id}
          onChange={() => onToggle(item)}
        />
      </label>
      <span>{item.foodName}</span>
      <span className="meal-qty">
        {item.grams} г · {item.calories} ккал
      </span>
    </li>
  );
}

function formatDate(iso: string) {
  const d = new Date(iso.includes('T') ? iso : iso + 'T12:00:00');
  return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' });
}

function formatDayChip(iso: string) {
  return new Date(iso + 'T12:00:00').toLocaleDateString('ru-RU', { day: 'numeric', month: 'short' });
}
