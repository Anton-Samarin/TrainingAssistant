import { FormEvent, useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import { getProfile } from '../api/profile';
import {
  addProgressNote,
  addStrengthRecord,
  addWeightLog,
  deleteProgressNote,
  deleteStrengthRecord,
  deleteWeightLog,
  getProgressOverview,
  notifyProfileWeightUpdated,
} from '../api/progress';
import type { ProgressOverviewDto } from '../api/types';
import { Field, TextInput } from '../components/Field';
import { NumberInput } from '../components/NumberInput';
import { WeightLineChart } from '../components/WeightLineChart';
import { formatRuDate, toIsoDate } from '../utils/date';

export function ProgressPage() {
  const [overview, setOverview] = useState<ProgressOverviewDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [weightSyncMsg, setWeightSyncMsg] = useState('');
  const [today] = useState(toIsoDate(new Date()));

  const [weightDate, setWeightDate] = useState(today);
  const [weightKg, setWeightKg] = useState(70);
  const [weightNote, setWeightNote] = useState('');

  const [strDate, setStrDate] = useState(today);
  const [strName, setStrName] = useState('');
  const [strWeight, setStrWeight] = useState<number | ''>('');
  const [strReps, setStrReps] = useState<number | ''>('');
  const [strSets, setStrSets] = useState<number | ''>('');

  const [noteDate, setNoteDate] = useState(today);
  const [noteText, setNoteText] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      setOverview(await getProgressOverview());
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось загрузить прогресс');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
    getProfile()
      .then((p) => setWeightKg(p.weightKg))
      .catch(() => {});
  }, [load]);

  async function handleWeight(e: FormEvent) {
    e.preventDefault();
    try {
      const sync = await addWeightLog({ logDate: weightDate, weightKg, note: weightNote || undefined });
      setWeightNote('');
      setWeightSyncMsg(
        `Профиль обновлён: ${sync.profileWeightKg} кг. ИМТ ${sync.bmi} (${sync.bmiCategory}). На вкладке «Неделя» данные обновятся автоматически.`
      );
      notifyProfileWeightUpdated();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка сохранения');
    }
  }

  async function handleDeleteWeight(id: string) {
    try {
      const sync = await deleteWeightLog(id);
      setWeightSyncMsg(
        `Профиль: ${sync.profileWeightKg} кг, ИМТ ${sync.bmi} (${sync.bmiCategory}).`
      );
      notifyProfileWeightUpdated();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось удалить');
    }
  }

  async function handleStrength(e: FormEvent) {
    e.preventDefault();
    if (!strName.trim()) return;
    try {
      await addStrengthRecord({
        recordDate: strDate,
        exerciseName: strName.trim(),
        weightKg: strWeight === '' ? undefined : Number(strWeight),
        reps: strReps === '' ? undefined : Number(strReps),
        sets: strSets === '' ? undefined : Number(strSets),
      });
      setStrName('');
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка сохранения');
    }
  }

  async function handleNote(e: FormEvent) {
    e.preventDefault();
    if (!noteText.trim()) return;
    try {
      await addProgressNote({ noteDate, text: noteText.trim() });
      setNoteText('');
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка сохранения');
    }
  }

  const weights = overview?.weightLogs ?? [];

  return (
    <div className="progress-page">
      <p className="eyebrow">Прогресс</p>
      <h1>Ваши показатели</h1>
      <p className="lede">Вес, силовые и заметки — отдельно от недельного плана.</p>

      {error ? <p className="banner banner-error">{error}</p> : null}
      {weightSyncMsg ? <p className="banner banner-ok">{weightSyncMsg}</p> : null}
      {loading ? <p className="muted">Загрузка…</p> : null}

      {!loading && overview ? (
        <div className="progress-grid">
          <section className="panel panel-card">
            <h2>Вес</h2>
            <form className="form form-inline" onSubmit={handleWeight}>
              <Field label="Дата">
                <input
                  type="date"
                  className="input"
                  value={weightDate}
                  onChange={(e) => setWeightDate(e.target.value)}
                />
              </Field>
              <Field label="кг">
                <NumberInput min={30} max={250} value={weightKg} onChange={setWeightKg} required />
              </Field>
              <Field label="Заметка">
                <TextInput value={weightNote} onChange={(e) => setWeightNote(e.target.value)} />
              </Field>
              <button type="submit" className="btn btn-primary btn-sm">
                Добавить
              </button>
            </form>

            {weights.length > 0 ? <WeightLineChart logs={weights} /> : null}

            <ul className="log-list">
              {[...weights].reverse().map((w) => (
                <li key={w.id}>
                  <span>
                    {formatRuDate(w.logDate)} — <strong>{w.weightKg} кг</strong>
                    {w.note ? ` · ${w.note}` : ''}
                  </span>
                  <button type="button" className="btn-text" onClick={() => handleDeleteWeight(w.id)}>
                    Удалить
                  </button>
                </li>
              ))}
            </ul>
          </section>

          <section className="panel panel-card">
            <h2>Силовые</h2>
            <form className="form form-stack" onSubmit={handleStrength}>
              <div className="row-2">
                <Field label="Дата">
                  <input type="date" className="input" value={strDate} onChange={(e) => setStrDate(e.target.value)} />
                </Field>
                <Field label="Упражнение">
                  <TextInput value={strName} onChange={(e) => setStrName(e.target.value)} required />
                </Field>
              </div>
              <div className="row-3">
                <Field label="Вес, кг">
                  <NumberInput min={0} value={strWeight === '' ? 0 : strWeight} onChange={(v) => setStrWeight(v)} />
                </Field>
                <Field label="Повторы">
                  <NumberInput integer min={1} value={strReps === '' ? 0 : strReps} onChange={(v) => setStrReps(v)} />
                </Field>
                <Field label="Подходы">
                  <NumberInput integer min={1} value={strSets === '' ? 0 : strSets} onChange={(v) => setStrSets(v)} />
                </Field>
              </div>
              <button type="submit" className="btn btn-primary btn-sm">
                Записать
              </button>
            </form>
            <ul className="log-list">
              {overview.strengthRecords.map((r) => (
                <li key={r.id}>
                  <span>
                    {formatRuDate(r.recordDate)} — <strong>{r.exerciseName}</strong>
                    {r.weightKg != null ? ` ${r.weightKg} кг` : ''}
                    {r.sets && r.reps ? ` · ${r.sets}×${r.reps}` : ''}
                  </span>
                  <button type="button" className="btn-text" onClick={() => deleteStrengthRecord(r.id).then(load)}>
                    Удалить
                  </button>
                </li>
              ))}
            </ul>
          </section>

          <section className="panel panel-card">
            <h2>Заметки</h2>
            <form className="form form-stack" onSubmit={handleNote}>
              <Field label="Дата">
                <input type="date" className="input" value={noteDate} onChange={(e) => setNoteDate(e.target.value)} />
              </Field>
              <Field label="Текст">
                <textarea
                  className="input"
                  rows={3}
                  value={noteText}
                  onChange={(e) => setNoteText(e.target.value)}
                  required
                />
              </Field>
              <button type="submit" className="btn btn-primary btn-sm">
                Сохранить
              </button>
            </form>
            <ul className="log-list">
              {overview.notes.map((n) => (
                <li key={n.id}>
                  <div>
                    <strong>{formatRuDate(n.noteDate)}</strong>
                    <p>{n.text}</p>
                  </div>
                  <button type="button" className="btn-text" onClick={() => deleteProgressNote(n.id).then(load)}>
                    Удалить
                  </button>
                </li>
              ))}
            </ul>
          </section>
        </div>
      ) : null}
    </div>
  );
}
