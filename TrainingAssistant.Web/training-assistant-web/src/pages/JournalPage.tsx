import { useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import { getJournalDay, listJournalWeeks } from '../api/journal';
import type { DayJournalDto, WeekSummaryDto } from '../api/types';
import { mealLabels } from '../labels';
import { addDays, formatRuDate, isDateInWeek, isoDateOnly, toIsoDate } from '../utils/date';

export function JournalPage() {
  const [date, setDate] = useState(toIsoDate(new Date()));
  const [day, setDay] = useState<DayJournalDto | null>(null);
  const [weeks, setWeeks] = useState<WeekSummaryDto[]>([]);
  const [selectedPlanId, setSelectedPlanId] = useState<string | null>(null);
  const [selectedWeekStart, setSelectedWeekStart] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadDay = useCallback(async (iso: string, planId?: string | null) => {
    setLoading(true);
    setError('');
    try {
      setDay(await getJournalDay(iso, planId ?? undefined));
    } catch (err) {
      setDay(null);
      if (err instanceof ApiError && err.status === 404) {
        setError(
          planId
            ? 'За эту дату в выбранной неделе нет данных. Выберите другой день недели.'
            : 'За этот день нет сохранённого плана. Сформируйте неделю или выберите архивную неделю ниже.',
        );
      } else {
        setError(err instanceof ApiError ? err.message : 'Не удалось загрузить день');
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    listJournalWeeks()
      .then(setWeeks)
      .catch(() => setWeeks([]));
  }, []);

  useEffect(() => {
    loadDay(date, selectedPlanId);
  }, [date, selectedPlanId, loadDay]);

  const openArchiveWeek = (w: WeekSummaryDto) => {
    const start = isoDateOnly(w.weekStart);
    setSelectedPlanId(w.id);
    setSelectedWeekStart(start);
    setDate(start);
  };

  const clearArchiveSelection = () => {
    setSelectedPlanId(null);
    setSelectedWeekStart(null);
  };

  const handleDateChange = (iso: string) => {
    if (selectedPlanId && selectedWeekStart && !isDateInWeek(iso, selectedWeekStart)) {
      clearArchiveSelection();
    }
    setDate(iso);
  };

  const weekDayDates =
    selectedWeekStart != null
      ? Array.from({ length: 7 }, (_, i) => addDays(selectedWeekStart, i))
      : [];

  return (
    <div className="journal-page">
      <p className="eyebrow">Дневник</p>
      <h1>Что было в плане</h1>
      <p className="lede">Питание и тренировки по дням из сохранённых недель (включая отмеченное выполнение).</p>

      {selectedWeekStart ? (
        <div className="journal-archive-banner">
          <span>
            Архивная неделя с {formatRuDate(selectedWeekStart)}
            {weeks.find((w) => w.id === selectedPlanId)?.isActive ? ' · текущая' : ''}
          </span>
          <button type="button" className="btn btn-ghost btn-sm" onClick={clearArchiveSelection}>
            Выйти из архива
          </button>
        </div>
      ) : null}

      {weekDayDates.length > 0 ? (
        <div className="journal-week-days" role="tablist" aria-label="Дни недели">
          {weekDayDates.map((d, i) => (
            <button
              key={d}
              type="button"
              role="tab"
              aria-selected={date === d}
              className={`btn btn-ghost btn-sm journal-week-day${date === d ? ' journal-week-day-active' : ''}`}
              onClick={() => setDate(d)}
            >
              {i + 1}
              <span className="journal-week-day-label">{formatRuDate(d).split(',')[0]}</span>
            </button>
          ))}
        </div>
      ) : null}

      <div className="journal-toolbar">
        <button
          type="button"
          className="btn btn-ghost btn-sm"
          onClick={() => handleDateChange(addDays(date, -1))}
          disabled={selectedWeekStart != null && date <= selectedWeekStart}
        >
          ← Вчера
        </button>
        <input
          type="date"
          className="input journal-date-input"
          value={date}
          onChange={(e) => handleDateChange(e.target.value)}
        />
        <button
          type="button"
          className="btn btn-ghost btn-sm"
          onClick={() => handleDateChange(addDays(date, 1))}
          disabled={date >= toIsoDate(new Date())}
        >
          Завтра →
        </button>
      </div>

      {error ? <p className="banner banner-error">{error}</p> : null}
      {loading ? <p className="muted">Загрузка…</p> : null}

      {!loading && day ? (
        <>
          <p className="journal-day-title">
            {formatRuDate(day.date)} · {day.dayName}
            <span className="muted journal-week-hint">
              {' '}
              (неделя с {formatRuDate(day.weekStart)})
            </span>
          </p>

          <div className="columns">
            <section className="panel panel-card">
              <h2>Питание</h2>
              {day.nutrition ? (
                <>
                  <p className="macro">
                    {day.nutrition.targetCalories} ккал · Б {day.nutrition.targetProteinG} · Ж{' '}
                    {day.nutrition.targetFatG} · У {day.nutrition.targetCarbsG}
                  </p>
                  <ul className="meal-list">
                    {day.nutrition.meals.map((meal) => (
                      <li key={meal.mealType} className="meal-block">
                        <h3>{mealLabels[meal.mealType] ?? meal.mealType}</h3>
                        <ul>
                          {meal.items.map((item) => (
                            <li key={item.id} className={item.isCompleted ? 'meal-done' : ''}>
                              {item.isCompleted ? '✓ ' : ''}
                              {item.foodName} — {item.grams} г
                            </li>
                          ))}
                        </ul>
                      </li>
                    ))}
                  </ul>
                </>
              ) : (
                <p className="muted">Нет данных</p>
              )}
            </section>

            <section className="panel panel-card">
              <h2>Тренировка</h2>
              {day.training?.isRestDay ? (
                <p className="rest">День отдыха</p>
              ) : day.training ? (
                <>
                  {day.training.focus ? <p className="macro">{day.training.focus}</p> : null}
                  <ol className="exercise-list journal-readonly">
                    {day.training.exercises.map((ex) => (
                      <li key={ex.id} className={ex.isCompleted ? 'exercise-done' : ''}>
                        <div className="ex-body">
                          <div className="ex-name">
                            {ex.isCompleted ? '✓ ' : ''}
                            {ex.name}
                          </div>
                          <div className="ex-meta">
                            {ex.sets}×{ex.reps}
                          </div>
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

      {weeks.length > 0 ? (
        <section className="panel panel-card journal-weeks-list">
          <h2>Архив недель</h2>
          <ul>
            {weeks.map((w) => (
              <li key={w.id} className={selectedPlanId === w.id ? 'journal-week-selected' : ''}>
                <button type="button" className="btn-link" onClick={() => openArchiveWeek(w)}>
                  с {formatRuDate(w.weekStart)}
                  {w.isActive ? ' · текущая' : ''}
                </button>
                <span className="muted">
                  {' '}
                  — тренировки {w.completedExercises}/{w.totalExercises}, питание{' '}
                  {w.completedMealItems}/{w.totalMealItems}
                </span>
              </li>
            ))}
          </ul>
        </section>
      ) : null}
    </div>
  );
}
