import { defineStore } from 'pinia'
import { authApi } from '../api/endpoints'
import { loadSession, saveSession, clearSession } from '../api/session'

export const useAuth = defineStore('auth', {
  state: () => ({ session: loadSession() }),
  getters: {
    isAuthenticated: (s) => Boolean(s.session?.token),
    user: (s) => s.session?.user ?? null,
    isAdmin: (s) => (s.session?.user?.role || '').toLowerCase() === 'admin',
  },
  actions: {
    can(perm) {
      if (!perm || this.isAdmin) return true
      const target = String(perm).toLowerCase()
      return (this.user?.permissions || []).some((p) => String(p).toLowerCase() === target)
    },
    async login(username, password) {
      const d = await authApi.login(username, password)
      const { token, ...user } = d
      this.session = { token, user }
      saveSession(this.session)
    },
    async logout() {
      try { await authApi.logout() } catch { /* token may already be invalid */ }
      this.clear()
    },
    clear() {
      this.session = null
      clearSession()
    },
  },
})
