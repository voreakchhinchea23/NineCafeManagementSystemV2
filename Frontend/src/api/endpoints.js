import { api } from './client'

const crud = (base) => ({
  list: () => api.get(base),
  get: (id) => api.get(`${base}/${id}`),
  create: (dto) => api.post(base, dto),
  update: (id, dto) => api.put(`${base}/${id}`, dto),
  remove: (id) => api.delete(`${base}/${id}`),
})

export const authApi = {
  login: (username, password) => api.post('auth/login', { username, password }),
  logout: () => api.post('auth/logout', {}),
}

export const dashboardApi = {
  get: (date) => api.get('dashboard', date ? { date } : null),
  addSale: (dto) => api.post('dashboard/sales', dto),
  updateSale: (id, quantity) => api.put(`dashboard/sales/${id}`, { quantity }),
  deleteSale: (id) => api.delete(`dashboard/sales/${id}`),
  addWithdrawal: (dto) => api.post('dashboard/withdrawals', dto),
  deleteWithdrawal: (id) => api.delete(`dashboard/withdrawals/${id}`),
}

export const usersApi = { ...crud('users'), changePassword: (id, dto) => api.put(`users/${id}/change-password`, dto) }
export const modulesApi = crud('modules')
export const rolesApi = {
  ...crud('roles'),
  // Replaces the role's whole permission list with these ids.
  assignPermissions: (roleId, permissionIds) => api.post('roles/assign-permissions', { roleId, permissionIds }),
}
export const permissionsApi = crud('permissions')
export const debtsApi = { ...crud('debts'), pay: (id) => api.patch(`debts/${id}/pay`) }
export const pricesApi = crud('prices')
export const expensesApi = crud('expenses')
export const suppliersApi = crud('suppliers')
export const reportsApi = { monthly: (year, month) => api.get('reports/monthly', { year, month }) }
export const auditApi = { list: (params) => api.get('audit-logs', params) }
