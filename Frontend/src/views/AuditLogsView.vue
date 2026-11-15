<script setup>
import { onMounted, reactive, ref } from 'vue'
import { auditApi } from '../api/endpoints'
import { dateTime } from '../utils/format'

const filters = reactive({ limit: 100, entityType: '', action: '' })
const logs = ref([])
const loading = ref(false)
const error = ref('')
const openId = ref(null)

async function load() {
  loading.value = true
  error.value = ''
  try {
    logs.value = (await auditApi.list({ limit: filters.limit, entityType: filters.entityType.trim(), action: filters.action })) || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
const pretty = (s) => {
  if (!s) return '-'
  try { return JSON.stringify(JSON.parse(s), null, 2) } catch { return s }
}
const cls = (a) => ({ create: 'ok', delete: 'off' })[String(a).toLowerCase()] || ''
onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Audit logs</h2>
    <form style="display: flex; gap: 8px; flex-wrap: wrap" @submit.prevent="load">
      <input v-model="filters.entityType" placeholder="Entity, e.g. DailySale" style="width: 190px" />
      <select v-model="filters.action" style="width: auto">
        <option value="">All actions</option>
        <option value="create">create</option>
        <option value="update">update</option>
        <option value="delete">delete</option>
      </select>
      <select v-model.number="filters.limit" style="width: auto">
        <option :value="50">Last 50</option><option :value="100">Last 100</option><option :value="300">Last 300</option>
      </select>
      <button class="btn">Apply</button>
    </form>
  </div>
  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!logs.length" class="empty">No audit entries.</div>
    <div v-else class="table-wrap">
      <table>
        <thead><tr><th>When</th><th>User</th><th>Action</th><th>Entity</th><th></th></tr></thead>
        <tbody>
          <template v-for="l in logs" :key="l.id">
            <tr>
              <td>{{ dateTime(l.createdAt) }}</td>
              <td><strong>{{ l.userName || '-' }}</strong> <span class="muted">{{ l.roleName }}</span></td>
              <td><span class="badge" :class="cls(l.action)">{{ l.action }}</span></td>
              <td>{{ l.entityType }} <span class="muted">#{{ l.entityId }}</span></td>
              <td class="actions"><button class="btn light small" @click="openId = openId === l.id ? null : l.id">{{ openId === l.id ? 'Hide' : 'Details' }}</button></td>
            </tr>
            <tr v-if="openId === l.id">
              <td colspan="5" style="background: var(--bg)">
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 12px">
                  <div><strong>Before</strong><pre>{{ pretty(l.beforeData) }}</pre></div>
                  <div><strong>After</strong><pre>{{ pretty(l.afterData) }}</pre></div>
                </div>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
pre { margin: 6px 0 0; white-space: pre-wrap; word-break: break-word; font-size: 12px; }
</style>
