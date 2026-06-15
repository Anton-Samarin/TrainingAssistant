import { FormEvent, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import { getProfile, updateProfile } from '../api/profile';
import { FitnessLevel, TrainingFocus, UserGoal, UserSex } from '../api/types';
import { ChipToggle } from '../components/ChipToggle';
import { Field, SelectInput, TextInput } from '../components/Field';
import { useAuth } from '../context/AuthContext';
import { ActivityLevelSelect } from '../components/ActivityLevelSelect';
import { NumberInput } from '../components/NumberInput';
import {
  equipmentOptions,
  goalLabels,
  injuryOptions,
  levelLabels,
  sexLabels,
  trainingFocusLabels,
} from '../labels';
import { greetingLine } from '../utils/greeting';

export function ProfilePage() {
  const { setName: setAuthName } = useAuth();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [saved, setSaved] = useState(false);

  const [name, setName] = useState('');
  const [sex, setSex] = useState<UserSex>(UserSex.Male);
  const [age, setAge] = useState(28);
  const [weightKg, setWeightKg] = useState(75);
  const [heightCm, setHeightCm] = useState(175);
  const [goal, setGoal] = useState<UserGoal>(UserGoal.LoseWeight);
  const [trainingFocus, setTrainingFocus] = useState<TrainingFocus>(TrainingFocus.Mixed);
  const [fitnessLevel, setFitnessLevel] = useState<FitnessLevel>(FitnessLevel.Beginner);
  const [sessionsPerWeek, setSessionsPerWeek] = useState(3);
  const [sessionDurationMin, setSessionDurationMin] = useState(45);
  const [activityLevel, setActivityLevel] = useState(3);
  const [equipment, setEquipment] = useState<string[]>([]);
  const [injuries, setInjuries] = useState<string[]>([]);

  useEffect(() => {
    (async () => {
      try {
        const p = await getProfile();
        setName(p.name);
        setAuthName(p.name);
        setSex(p.sex);
        setAge(p.age);
        setWeightKg(p.weightKg);
        setHeightCm(p.heightCm);
        setGoal(p.goal);
        setTrainingFocus(p.trainingFocus ?? TrainingFocus.Mixed);
        setFitnessLevel(p.fitnessLevel);
        setSessionsPerWeek(p.sessionsPerWeek);
        setSessionDurationMin(p.sessionDurationMin);
        setActivityLevel(p.activityLevel);
        setEquipment(p.equipment);
        setInjuries(p.injuries);
      } catch (err) {
        setError(err instanceof ApiError ? err.message : 'Не удалось загрузить профиль');
      } finally {
        setLoading(false);
      }
    })();
  }, [setAuthName]);

  function toggle(list: string[], id: string, setter: (v: string[]) => void) {
    setter(list.includes(id) ? list.filter((x) => x !== id) : [...list, id]);
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError('');
    setSaved(false);
    try {
      await updateProfile({
        name: name.trim(),
        sex,
        age,
        weightKg,
        heightCm,
        goal,
        trainingFocus,
        fitnessLevel,
        sessionsPerWeek,
        sessionDurationMin,
        activityLevel,
        equipment,
        injuries,
      });
      setAuthName(name.trim());
      setSaved(true);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось сохранить');
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <p className="muted">Загрузка профиля…</p>;

  return (
    <div className="profile-page">
      <p className="eyebrow">Профиль</p>
      <h1>{greetingLine(name) ? `Параметры, ${greetingLine(name)}` : 'Параметры'}</h1>
      <p className="lede">После изменений пересоберите неделю на главной.</p>

      <form className="form form-grid" onSubmit={handleSubmit}>
        <Field label="Имя">
          <TextInput
            type="text"
            autoComplete="given-name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            minLength={2}
            maxLength={80}
            required
          />
        </Field>
        <div className="row-2">
          <Field label="Пол">
            <SelectInput value={sex} onChange={(e) => setSex(Number(e.target.value) as UserSex)}>
              {Object.entries(sexLabels).map(([v, l]) => (
                <option key={v} value={v}>
                  {l}
                </option>
              ))}
            </SelectInput>
          </Field>
          <Field label="Возраст">
            <NumberInput integer min={14} max={80} value={age} onChange={setAge} required />
          </Field>
        </div>
        <div className="row-2">
          <Field label="Вес, кг">
            <NumberInput min={30} max={250} value={weightKg} onChange={setWeightKg} required />
          </Field>
          <Field label="Рост, см">
            <NumberInput integer min={130} max={220} value={heightCm} onChange={setHeightCm} required />
          </Field>
        </div>
        <Field label="Цель">
          <SelectInput value={goal} onChange={(e) => setGoal(Number(e.target.value) as UserGoal)}>
            {Object.entries(goalLabels).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </SelectInput>
        </Field>
        <Field label="Фокус тренировок">
          <SelectInput
            value={trainingFocus}
            onChange={(e) => setTrainingFocus(Number(e.target.value) as TrainingFocus)}
          >
            {Object.entries(trainingFocusLabels).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </SelectInput>
        </Field>
        <Field label="Уровень">
          <SelectInput
            value={fitnessLevel}
            onChange={(e) => setFitnessLevel(Number(e.target.value) as FitnessLevel)}
          >
            {Object.entries(levelLabels).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </SelectInput>
        </Field>
        <div className="row-2">
          <Field label="Тренировок / нед">
            <NumberInput integer min={2} max={6} value={sessionsPerWeek} onChange={setSessionsPerWeek} required />
          </Field>
          <Field label="Минут">
            <NumberInput integer min={20} max={120} value={sessionDurationMin} onChange={setSessionDurationMin} required />
          </Field>
        </div>
        <Field
          label="Повседневная активность"
          hint="Движение в обычные дни без тренировок — влияет на калории в плане питания"
        >
          <ActivityLevelSelect value={activityLevel} onChange={setActivityLevel} />
        </Field>

        <section className="form-section">
          <h2>Оборудование</h2>
          <div className="chips">
            {equipmentOptions.map((opt) => (
              <ChipToggle
                key={opt.id}
                label={opt.label}
                selected={equipment.includes(opt.id)}
                onToggle={() => toggle(equipment, opt.id, setEquipment)}
              />
            ))}
          </div>
        </section>

        <section className="form-section">
          <h2>Ограничения</h2>
          <div className="chips">
            {injuryOptions.map((opt) => (
              <ChipToggle
                key={opt.id}
                label={opt.label}
                selected={injuries.includes(opt.id)}
                onToggle={() => toggle(injuries, opt.id, setInjuries)}
              />
            ))}
          </div>
        </section>

        {error ? <p className="form-error">{error}</p> : null}
        {saved ? <p className="form-ok">Сохранено</p> : null}
        <button className="btn btn-primary" type="submit" disabled={saving}>
          {saving ? 'Сохраняем…' : 'Сохранить'}
        </button>
      </form>
    </div>
  );
}
