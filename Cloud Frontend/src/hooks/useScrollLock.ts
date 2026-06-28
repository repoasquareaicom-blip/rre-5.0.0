import { useEffect, useRef } from 'react';

type Snapshot = {
  windowY: number;
  feedScrollTop: number;
  mainScrollTop: number;
};

/**
 * Hard lock while the More menu is open (mobile).
 * Locks document scroll + .feed-scroll-body + .main scrollers.
 */
export function useScrollLock(active: boolean) {
  const snapshotRef = useRef<Snapshot | null>(null);

  useEffect(() => {
    if (!active) return;

    const html = document.documentElement;
    const body = document.body;
    const feed = document.querySelector<HTMLElement>('.feed-scroll-body');
    const main = document.querySelector<HTMLElement>('.main:not(.main-feed)');

    snapshotRef.current = {
      windowY: window.scrollY,
      feedScrollTop: feed?.scrollTop ?? 0,
      mainScrollTop: main?.scrollTop ?? 0,
    };

    html.classList.add('nav-menu-open');
    body.classList.add('nav-menu-open');

    const scrollY = snapshotRef.current.windowY;
    body.dataset.scrollLockY = String(scrollY);
    body.style.position = 'fixed';
    body.style.top = `-${scrollY}px`;
    body.style.left = '0';
    body.style.right = '0';
    body.style.width = '100%';
    body.style.overflow = 'hidden';

    if (feed) {
      feed.dataset.scrollLockTop = String(snapshotRef.current.feedScrollTop);
      feed.style.overflow = 'hidden';
      feed.style.touchAction = 'none';
    }
    if (main) {
      main.dataset.scrollLockTop = String(snapshotRef.current.mainScrollTop);
      main.style.overflow = 'hidden';
      main.style.touchAction = 'none';
    }

    const blockScroll = (e: Event) => {
      e.preventDefault();
    };

    document.addEventListener('touchmove', blockScroll, { passive: false, capture: true });
    document.addEventListener('wheel', blockScroll, { passive: false, capture: true });

    return () => {
      document.removeEventListener('touchmove', blockScroll, { capture: true });
      document.removeEventListener('wheel', blockScroll, { capture: true });

      const snap = snapshotRef.current;

      body.style.position = '';
      body.style.top = '';
      body.style.left = '';
      body.style.right = '';
      body.style.width = '';
      body.style.overflow = '';
      delete body.dataset.scrollLockY;

      html.classList.remove('nav-menu-open');
      body.classList.remove('nav-menu-open');

      if (feed) {
        feed.style.overflow = '';
        feed.style.touchAction = '';
        if (snap) feed.scrollTop = snap.feedScrollTop;
        delete feed.dataset.scrollLockTop;
      }
      if (main) {
        main.style.overflow = '';
        main.style.touchAction = '';
        if (snap) main.scrollTop = snap.mainScrollTop;
        delete main.dataset.scrollLockTop;
      }

      if (snap) {
        window.scrollTo(0, snap.windowY);
      }

      snapshotRef.current = null;
    };
  }, [active]);
}
