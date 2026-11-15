// Sidebar + route table source. `perm` is the *.read permission from the API.
export const navItems = [
  { label: 'Dashboard',   to: '/dashboard',   perm: 'dashboard.read',  icon: '📊' },
  { label: 'Debts',       to: '/debts',       perm: 'debt.read',       icon: '🧾', legacy: 'debts.html' },
  { label: 'Prices',      to: '/prices',      perm: 'price.read',      icon: '🏷️', legacy: 'prices.html' },
  { label: 'Expenses',    to: '/expenses',    perm: 'expense.read',    icon: '💸', legacy: 'expenses.html' },
  { label: 'Suppliers',   to: '/suppliers',   perm: 'supplier.read',   icon: '🚚', legacy: 'suppliers.html' },
  { label: 'Reports',     to: '/reports',     perm: 'report.read',     icon: '📈', legacy: 'reports.html' },
  { label: 'Users',       to: '/users',       perm: 'user.read',       icon: '👤' },
  { label: 'Roles',       to: '/roles',       perm: 'role.read',       icon: '🛡️', legacy: 'roles.html' },
  { label: 'Permissions', to: '/permissions', perm: 'permission.read', icon: '🔑', legacy: 'permissions.html' },
  { label: 'Modules',     to: '/modules',     perm: 'module.read',     icon: '🧩' },
  { label: 'Audit logs',  to: '/audit-logs',  perm: 'audit.read',      icon: '📜', legacy: 'audit-logs.html' },
]
