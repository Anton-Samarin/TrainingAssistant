import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { register } from '../api/auth';
import { ApiError } from '../api/client';
import { FitnessLevel, TrainingFocus, UserGoal, UserSex } from '../api/types';
import { ChipToggle } from '../components/ChipToggle';
import { Field, SelectInput, TextInput } from '../components/Field';
import { ActivityLevelSelect } from '../components/ActivityLevelSelect';
import { NumberInput } from '../components/NumberInput';
import { useAuth } from '../context/AuthContext';
import { ThemeToggle } from '../components/ThemeToggle';
import {
  equipmentOptions,
  goalLabels,
  injuryOptions,
  levelLabels,
  trainingFocusLabels,
  sexLabels,
} from '../labels';

export function RegisterPage() {
  const navigate = useNavigate();
  const { setSession } = useAuth();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
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
  const [equipment, setEquipment] = useState<string[]>(['mat', 'dumbbells']);
  const [injuries, setInjuries] = useState<string[]>([]);

  function toggle(list: string[], id: string, setter: (v: string[]) => void) {
    setter(list.includes(id) ? list.filter((x) => x !== id) : [...list, id]);
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await register({
        name: name.trim(),
        email,
        password,
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
      setSession(res.token, res.userId, res.email, res.name);
      navigate('/');
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Ошибка регистрации');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth auth--wide">
      <ThemeToggle className="auth-theme-toggle" />
      <div className="auth-panel">
        <p className="eyebrow">Регистрация</p>
        <h1>{name.trim() ? `Привет, ${name.trim().split(/\s+/)[0]}!` : 'Ваш профиль'}</h1>
        <p className="lede">Расскажите о себе — план питания и тренировок соберём под вас.</p>

        <form className="form form-grid" onSubmit={handleSubmit}>
          <section className="form-section">
            <h2>Аккаунт</h2>
            <Field label="Имя" hint="как к вам обращаться в приложении">
              <TextInput
                type="text"
                autoComplete="given-name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                minLength={2}
                maxLength={80}
                placeholder="Алексей"
                required
              />
            </Field>
            <Field label="Email">
              <TextInput type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            </Field>
            <Field label="Пароль" hint="не меньше 6 символов">
              <TextInput
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                minLength={6}
                required
              />
            </Field>
          </section>

          <section className="form-section">
            <h2>Тело и цель</h2>
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
            <Field label="Фокус тренировок" hint="влияет на баланс силовых и кардио в плане">
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
            <Field label="Уровень подготовки">
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
          </section>

          <section className="form-section">
            <h2>Нагрузка</h2>
            <div className="row-2">
              <Field label="Тренировок / нед">
                <NumberInput integer min={2} max={6} value={sessionsPerWeek} onChange={setSessionsPerWeek} required />
              </Field>
              <Field label="Минут на тренировку">
                <NumberInput
                  integer
                  min={20}
                  max={120}
                  value={sessionDurationMin}
                  onChange={setSessionDurationMin}
                  required
                />
              </Field>
            </div>
            <Field
              label="Повседневная активность"
              hint="Насколько вы двигаетесь в обычные дни без учёта тренировок — от этого считаются калории"
            >
              <ActivityLevelSelect value={activityLevel} onChange={setActivityLevel} />
            </Field>
          </section>

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
          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading ? 'Создаём…' : 'Создать аккаунт'}
          </button>
        </form>

        <p className="auth-footer">
          Уже есть аккаунт? <Link to="/login">Войти</Link>
        </p>
      </div>
    </div>
  );
}
