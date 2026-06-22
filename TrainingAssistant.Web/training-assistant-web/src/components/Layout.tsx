import { Link, NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { greetingLine } from '../utils/greeting';

export function Layout() {
  const { email, name, logout } = useAuth();
  const who = greetingLine(name);

  return (
    <div className="shell">
      <aside className="sidebar">
        <div className="sidebar-head">
          <Link to="/" className="brand">
            Training<span>Assistant</span>
          </Link>
          <div className="top-meta">
            <div className="user-badge">
              <span className="user-badge-name">Привет, {who}</span>
              {email ? <span className="email">{email}</span> : null}
            </div>
          </div>
        </div>
        <nav className="nav" aria-label="Основная навигация">
          <NavLink to="/" end>
            Неделя
          </NavLink>
          <NavLink to="/journal">Дневник</NavLink>
          <NavLink to="/progress">Прогресс</NavLink>
          <NavLink to="/profile">Профиль</NavLink>
          <NavLink to="/settings">Настройки</NavLink>
        </nav>
        <button type="button" className="btn-text sidebar-logout" onClick={logout}>
          Выйти
        </button>
      </aside>
      <div className="content-shell">
        <main className="main">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

