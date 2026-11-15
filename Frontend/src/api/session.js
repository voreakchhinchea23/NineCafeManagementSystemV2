const KEY = 'ncms_session'

export function loadSession() {
  try { return JSON.parse(localStorage.getItem(KEY)) || null } catch { return null }
}
export function saveSession(s) { localStorage.setItem(KEY, JSON.stringify(s)) }
export function clearSession() { localStorage.removeItem(KEY) }
