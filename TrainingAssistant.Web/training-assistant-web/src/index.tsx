import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';

(function initTheme() {
  try {
    const saved = localStorage.getItem('ta-theme');
    const theme =
      saved === 'light' || saved === 'dark'
        ? saved
        : window.matchMedia('(prefers-color-scheme: dark)').matches
          ? 'dark'
          : 'light';
    document.documentElement.setAttribute('data-theme', theme);
  } catch {
    /* ignore */
  }
})();

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
