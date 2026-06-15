import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { getProfile } from '../api/profile';

interface AuthState {
  token: string | null;
  userId: string | null;
  email: string | null;
  name: string | null;
}

interface AuthContextValue extends AuthState {
  isAuthenticated: boolean;
  setSession: (token: string, userId: string, email: string, name: string) => void;
  setName: (name: string) => void;
  logout: () => void;
}

const STORAGE_KEY = 'ta_session';

const AuthContext = createContext<AuthContextValue | null>(null);

function loadSession(): AuthState {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { token: null, userId: null, email: null, name: null };
    const parsed = JSON.parse(raw) as AuthState;
    return {
      token: parsed.token ?? null,
      userId: parsed.userId ?? null,
      email: parsed.email ?? null,
      name: parsed.name ?? null,
    };
  } catch {
    return { token: null, userId: null, email: null, name: null };
  }
}

function persistSession(state: AuthState) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>(loadSession);

  const setSession = useCallback((token: string, userId: string, email: string, name: string) => {
    localStorage.setItem('ta_token', token);
    const next = { token, userId, email, name: name.trim() };
    persistSession(next);
    setState(next);
  }, []);

  const setName = useCallback((name: string) => {
    setState((prev) => {
      const next = { ...prev, name: name.trim() };
      persistSession(next);
      return next;
    });
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('ta_token');
    localStorage.removeItem(STORAGE_KEY);
    setState({ token: null, userId: null, email: null, name: null });
  }, []);

  useEffect(() => {
    if (!state.token || state.name) return;
    let cancelled = false;
    (async () => {
      try {
        const profile = await getProfile();
        if (!cancelled && profile.name) {
          setState((prev) => {
            const next = { ...prev, name: profile.name };
            persistSession(next);
            return next;
          });
        }
      } catch {
        /* ignore */
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [state.token, state.name]);

  const value = useMemo(
    () => ({
      ...state,
      isAuthenticated: Boolean(state.token),
      setSession,
      setName,
      logout,
    }),
    [state, setSession, setName, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth outside AuthProvider');
  return ctx;
}
