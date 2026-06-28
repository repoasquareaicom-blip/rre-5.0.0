import type { FeedItem } from '../api/client';

export async function requestNotificationPermission(): Promise<NotificationPermission> {
  if (!('Notification' in window)) {
    return 'denied';
  }
  if (Notification.permission === 'granted') {
    return 'granted';
  }
  if (Notification.permission !== 'denied') {
    return await Notification.requestPermission();
  }
  return Notification.permission;
}

export function showActivityNotification(item: FeedItem) {
  if (!('Notification' in window) || Notification.permission !== 'granted') {
    return;
  }
  if (document.visibilityState === 'visible') {
    return;
  }

  const body =
    item.entity_type === 'quotation' && item.quotation
      ? [
          item.quotation.customer_name,
          item.quotation.status,
          item.quotation.line_count > 0 ? `${item.quotation.line_count} lines` : null,
          item.quotation.total_amount > 0 ? `₹${item.quotation.total_amount}` : null,
          item.quotation.quotation_id,
        ]
          .filter(Boolean)
          .join(' · ')
      : [item.subtitle, item.external_id].filter(Boolean).join(' · ');
  const n = new Notification(item.title, {
    body,
    tag: `rr-${item.id}`,
    silent: false,
  });
  n.onclick = () => {
    window.focus();
    n.close();
  };
}
