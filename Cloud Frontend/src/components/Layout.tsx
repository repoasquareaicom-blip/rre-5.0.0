import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { metaForPath } from '../config/routes';
import { useScrollLock } from '../hooks/useScrollLock';
import { ThemeToggle } from './ThemeToggle';
import { CloudEyeBrand } from './CloudEyeBrand';

const primaryLinks = [
  { to: '/', label: 'Feed', icon: '◎' },
  { to: '/dashboard', label: 'Stats', icon: '◫' },
];

const moreLinks = [
  { to: '/quotations', label: 'Quotations' },
  { to: '/estimations', label: 'Estimations' },
  { to: '/sales', label: 'Sales' },
  { to: '/sales-pipes', label: 'Sales (Pipes)' },
  { to: '/sales-traders', label: 'Sales (Traders)' },
  { to: '/vouchers', label: 'Vouchers' },
  { to: '/settlements', label: 'Settlement' },
];

export function Layout() {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const meta = metaForPath(pathname);
  const [menuOpen, setMenuOpen] = useState(false);
  const shellRef = useRef<HTMLDivElement>(null);
  const isFeed = pathname === '/';

  useScrollLock(menuOpen);

  useEffect(() => {
    setMenuOpen(false);
  }, [pathname]);

  useEffect(() => {
    const shell = shellRef.current;
    if (!shell) return;
    if (menuOpen) {
      shell.setAttribute('inert', '');
      shell.setAttribute('aria-hidden', 'true');
    } else {
      shell.removeAttribute('inert');
      shell.removeAttribute('aria-hidden');
    }
  }, [menuOpen]);

  const closeMenu = () => setMenuOpen(false);

  const handleLogout = async () => {
    closeMenu();
    await logout();
    navigate('/login', { replace: true });
  };

  const moreMenu =
    menuOpen && typeof document !== 'undefined'
      ? createPortal(
          <div
            className="menu-overlay"
            role="dialog"
            aria-modal="true"
            aria-label="More navigation"
          >
            <button
              type="button"
              className="menu-backdrop"
              aria-label="Close menu"
              onClick={closeMenu}
            />
            <div className="menu-sheet">
              <div className="menu-sheet-head">
                <div className="menu-sheet-head-text">
                  <strong>Navigate</strong>
                  {user && <span className="menu-sheet-user">{user.username}</span>}
                </div>
                <button type="button" className="menu-close" onClick={closeMenu}>
                  ✕
                </button>
              </div>
              <nav className="menu-sheet-nav">
                {moreLinks.map((link) => (
                  <NavLink
                    key={link.to}
                    to={link.to}
                    className={({ isActive }) => (isActive ? 'menu-link active' : 'menu-link')}
                    onClick={closeMenu}
                  >
                    {link.label}
                  </NavLink>
                ))}
                <button type="button" className="menu-link menu-link--logout" onClick={handleLogout}>
                  Sign out
                </button>
              </nav>
            </div>
          </div>,
          document.body
        )
      : null;

  return (
    <>
      <div ref={shellRef} className={`app-shell ${isFeed ? 'shell-feed' : ''}`}>
        <div className="app-ambient" aria-hidden>
          <span className="app-orb app-orb-1" />
          <span className="app-orb app-orb-2" />
          <span className="app-orb app-orb-3" />
        </div>
        <header className={`app-topbar ${isFeed ? 'feed-topbar' : ''}`}>
          {isFeed ? (
            <>
              <CloudEyeBrand className="topbar-brand-slot" compact />
              <div className="topbar-feed-end">
                <ThemeToggle compact />
                <button
                  type="button"
                  className="topbar-logout-btn"
                  onClick={handleLogout}
                  title="Sign out"
                  aria-label="Sign out"
                >
                  ⎋
                </button>
                <div id="feed-live-slot" className="topbar-live" aria-live="polite" />
                <div id="feed-filter-toggle-slot" className="topbar-filter-slot" />
              </div>
            </>
          ) : (
            <>
              <div className="topbar-brand">
                <span className="brand-mark" aria-hidden>
                  RR
                </span>
                <CloudEyeBrand pageTitle={meta.title} />
              </div>
              <div className="topbar-actions">
                {user && <span className="topbar-user-name">{user.username}</span>}
                <ThemeToggle compact />
                <button
                  type="button"
                  className="topbar-logout-btn"
                  onClick={handleLogout}
                  title="Sign out"
                  aria-label="Sign out"
                >
                  Sign out
                </button>
              </div>
            </>
          )}
        </header>

        <aside className="sidebar desktop-only">
          <nav className="sidebar-nav">
            {[...primaryLinks, ...moreLinks.map((l) => ({ ...l, icon: '·' }))].map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.to === '/'}
                className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
          <div className="sidebar-foot">
            {user && <p className="sidebar-user">{user.name}</p>}
            <ThemeToggle />
            <button type="button" className="sidebar-logout" onClick={handleLogout}>
              Sign out
            </button>
          </div>
        </aside>

        {isFeed && <div id="feed-header-slot" className="feed-header-slot" />}

        <main className={`main ${isFeed ? 'main-feed' : ''}`}>
          <Outlet />
        </main>
      </div>

      <nav className={`bottom-nav mobile-only ${menuOpen ? 'bottom-nav--menu-open' : ''}`} aria-label="Main">
        {primaryLinks.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            end={link.to === '/'}
            className={({ isActive }) => (isActive ? 'bottom-link active' : 'bottom-link')}
          >
            <span className="bottom-icon">{link.icon}</span>
            {link.label}
          </NavLink>
        ))}
        <button
          type="button"
          className={`bottom-link bottom-link-more ${menuOpen ? 'active' : ''}`}
          aria-expanded={menuOpen}
          aria-haspopup="dialog"
          onPointerDown={(e) => e.preventDefault()}
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            setMenuOpen((open) => !open);
          }}
        >
          <span className="bottom-icon">⋯</span>
          More
        </button>
      </nav>

      {moreMenu}
    </>
  );
}
