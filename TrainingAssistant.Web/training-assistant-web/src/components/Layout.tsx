import { Link, NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { greetingLine } from '../utils/greeting';

export function Layout() {
  const { email, name, logout } = useAuth();
  const who = greetingLine(name);

  return (
    <div className="shell">
      <header className="top">
        <Link to="/" className="brand">
          Training<span>Assistant</span>
        </Link>
        <nav className="nav">
          <NavLink to="/" end>
            Неделя
          </NavLink>
          <NavLink to="/journal">Дневник</NavLink>
          <NavLink to="/progress">Прогресс</NavLink>
          <NavLink to="/profile">Профиль</NavLink>
          <NavLink to="/settings">Настройки</NavLink>
        </nav>
        <div className="top-meta">
          <div className="user-badge">
            <span className="user-badge-name">Привет, {who}</span>
            {email ? <span className="email">{email}</span> : null}
          </div>
          <button type="button" className="btn-text" onClick={logout}>
            Выйти
          </button>
        </div>
      </header>
      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}

