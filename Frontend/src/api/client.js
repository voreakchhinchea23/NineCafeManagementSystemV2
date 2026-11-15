import { loadSession, clearSession } from './session'

const BASE = (import.meta.env.VITE_API_BASE || '/api').replace(/\/+$/, '')

export class ApiError extends Error {
  constructor(message, status = 0, data = null) {
    super(message)
    this.status = status
    this.data = data
  }
}

async function request(path, { method = 'GET', body, params } = {}) {
  let url = `${BASE}/${path.replace(/^\/+/, '')}`
  if (params) {
    const q = new URLSearchParams(Object.entries(params).filter(([, v]) => v !== '' && v != null))
    if ([...q].length) url += `?${q}`
  }

  const headers = { Accept: 'application/json' }
  if (body !== undefined) headers['Content-Type'] = 'application/json'
  const token = loadSession()?.token
  if (token) headers.Authorization = `Bearer ${token}`

  let res
  try {
    res = await fetch(url, { method, headers, body: body !== undefined ? JSON.stringify(body) : undefined })
  } catch {
    throw new ApiError(`Cannot reach the API (${url}). Is NineCafeApi running?`)
  }

  if (res.status === 204) return null
  const isJson = (res.headers.get('content-type') || '').includes('json')
  const data = isJson ? await res.json().catch(() => null) : (await res.text().catch(() => '')) || null

  if (!res.ok) {
    let message = (data && (data.message || data.title)) || `Server returned HTTP ${res.status}`
    if (data && typeof data.errors === 'object') {
      const lines = Object.values(data.errors).flat().join('\n')
      if (lines) message += `\n${lines}`
    }
    // An expired/invalid token (not a failed login) ends the session.
    if (res.status === 401 && token) {
      clearSession()
      window.dispatchEvent(new CustomEvent('ncms:unauthorized'))
    }
    if (res.status === 403) message = (data && data.message) || 'Access denied: you do not have permission.'
    throw new ApiError(message, res.status, data)
  }
  return data
}

export const api = {
  get: (path, params) => request(path, { params }),
  post: (path, body) => request(path, { method: 'POST', body }),
  put: (path, body) => request(path, { method: 'PUT', body }),
  patch: (path, body) => request(path, { method: 'PATCH', body }),
  delete: (path) => request(path, { method: 'DELETE' }),
}
