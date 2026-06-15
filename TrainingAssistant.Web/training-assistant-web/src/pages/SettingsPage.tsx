import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ApiError } from '../api/client';
import { clearAllWeeks, deleteAccount } from '../api/settings';
import { ThemeSetting } from '../components/ThemeSetting';
import { useAuth } from '../context/AuthContext';

export function SettingsPage() {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [clearing, setClearing] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [password, setPassword] = useState('');
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [showClearConfirm, setShowClearConfirm] = useState(false);

  async function handleClearWeeks() {
    setClearing(true);
    setError('');
    setSuccess('');
    try {
      const result = await clearAllWeeks();
      setShowClearConfirm(false);
      setSuccess(
        result.deletedCount > 0
          ? `Удалено недель: ${result.deletedCount}. Можно сформировать новый план на главной.`
          : 'Сохранённых недель не было.',
      );
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось очистить недели');
    } finally {
      setClearing(false);
    }
  }

  async function handleDeleteAccount(e: FormEvent) {
    e.preventDefault();
    if (!confirmDelete) {
      setError('Подтвердите удаление аккаунта');
      return;
    }
    setDeleting(true);
    setError('');
    setSuccess('');
    try {
      await deleteAccount(password);
      logout();
      navigate('/login', { replace: true, state: { accountDeleted: true } });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Не удалось удалить аккаунт');
    } finally {
      setDeleting(false);
    }
  }

  return (
    <div className="settings-page">
      <p className="eyebrow">Настройки</p>
      <h1>Параметры приложения</h1>
      <p className="lede">Оформление, управление данными недель и аккаунтом.</p>

      {error ? <p className="banner banner-error">{error}</p> : null}
      {success ? <p className="banner banner-ok">{success}</p> : null}

      <div className="settings-sections">
        <section className="panel-card settings-section">
          <h2>Оформление</h2>
          <ThemeSetting />
        </section>

        <section className="panel-card settings-section">
          <h2>Недельные планы</h2>
          <p className="muted settings-hint">
            Удаляет все сохранённые недели — текущую и архивные записи в дневнике. Профиль и прогресс не
            затрагиваются.
          </p>
          {!showClearConfirm ? (
            <button type="button" className="btn" onClick={() => setShowClearConfirm(true)} disabled={clearing}>
              Очистить все недели
            </button>
          ) : (
            <div className="settings-confirm">
              <p className="settings-confirm-text">Удалить все недельные планы без возможности восстановления?</p>
              <div className="settings-confirm-actions">
                <button type="button" className="btn btn-primary" onClick={handleClearWeeks} disabled={clearing}>
                  {clearing ? 'Удаляем…' : 'Да, удалить'}
                </button>
                <button
                  type="button"
                  className="btn btn-ghost"
                  onClick={() => setShowClearConfirm(false)}
                  disabled={clearing}
                >
                  Отмена
                </button>
              </div>
            </div>
          )}
        </section>

        <section className="panel-card settings-section settings-section--danger">
          <h2>Удаление аккаунта</h2>
          <p className="muted settings-hint">
            Безвозвратно удалит профиль, все недели, дневник, прогресс и загруженные фото. Восстановление
            невозможно.
          </p>
          <form className="settings-danger-form" onSubmit={handleDeleteAccount}>
            <label className="field">
              <span className="field-label">Пароль для подтверждения</span>
              <input
                className="input"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                disabled={deleting}
              />
            </label>
            <label className="settings-check">
              <input
                type="checkbox"
                checked={confirmDelete}
                onChange={(e) => setConfirmDelete(e.target.checked)}
                disabled={deleting}
              />
              <span>Я понимаю, что все данные будут удалены без возможности восстановления</span>
            </label>
            <button type="submit" className="btn settings-danger-btn" disabled={deleting || !password || !confirmDelete}>
              {deleting ? 'Удаляем…' : 'Удалить аккаунт'}
            </button>
          </form>
        </section>
      </div>
    </div>
  );
}
