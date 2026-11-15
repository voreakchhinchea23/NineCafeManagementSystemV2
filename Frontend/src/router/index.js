import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '../stores/auth'
import { navItems } from '../nav'
import AppShell from '../layouts/AppShell.vue'

const built = {
  '/dashboard': () => import('../views/DashboardView.vue'),
  '/debts': () => import('../views/DebtsView.vue'),
  '/prices': () => import('../views/PricesView.vue'),
  '/expenses': () => import('../views/ExpensesView.vue'),
  '/suppliers': () => import('../views/SuppliersView.vue'),
  '/reports': () => import('../views/ReportsView.vue'),
  '/users': () => import('../views/UsersView.vue'),
  '/roles': () => import('../views/RolesView.vue'),
  '/permissions': () => import('../views/PermissionsView.vue'),
  '/modules': () => import('../views/ModulesView.vue'),
  '/audit-logs': () => import('../views/AuditLogsView.vue'),
}

const pages = navItems.map((n) => ({
  path: n.to.slice(1),
  name: n.to.slice(1),
  component: built[n.to] || (() => import('../views/ComingSoonView.vue')),
  meta: { perm: n.perm, title: n.label, legacy: n.legacy },
}))

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: () => import('../views/LoginView.vue'), meta: { public: true } },
    {
      path: '/',
      component: AppShell,
      children: [
        { path: '', redirect: '/dashboard' },
        ...pages,
        { path: 'forbidden', name: 'forbidden', component: () => import('../views/ForbiddenView.vue'), meta: { title: 'No access' } },
      ],
    },
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
})

export function firstAllowedPath(auth) {
  return navItems.find((n) => auth.can(n.perm))?.to || '/forbidden'
}

router.beforeEach((to) => {
  const auth = useAuth()
  if (to.meta.public) return auth.isAuthenticated && to.name === 'login' ? firstAllowedPath(auth) : true
  if (!auth.isAuthenticated) return { name: 'login', query: { redirect: to.fullPath } }
  if (to.meta.perm && !auth.can(to.meta.perm)) return { name: 'forbidden' }
  return true
})

export default router
