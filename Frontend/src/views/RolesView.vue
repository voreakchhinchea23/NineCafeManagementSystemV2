<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { rolesApi, permissionsApi } from '../api/endpoints'
import { useAuth } from '../stores/auth'
import BaseModal from '../components/BaseModal.vue'

const auth = useAuth()
const roles = ref([])
const allPerms = ref([])
const loading = ref(false)
const error = ref('')

// create / edit role
const form = reactive({ id: null, name: '', description: '' })
const roleModal = ref(false)
const saving = ref(false)
const formError = ref('')

// permission picker
const permRole = ref(null)
const selected = ref(new Set())
const permError = ref('')

// permissions grouped by module, for the checkbox list
const groups = computed(() => {
  const map = {}
  for (const p of allPerms.value) (map[p.moduleName || 'Other'] ||= []).push(p)
  return Object.entries(map).map(([module, perms]) => ({ module, perms }))
})

async function load() {
  loading.value = true
  error.value = ''
  try {
    const [r, p] = await Promise.all([rolesApi.list(), permissionsApi.list().catch(() => [])])
    roles.value = r || []
    allPerms.value = p || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function openRole(r) {
  Object.assign(form, { id: r?.id ?? null, name: r?.name ?? '', description: r?.description ?? '' })
  formError.value = ''
  roleModal.value = true
}
async function saveRole() {
  saving.value = true
  formError.value = ''
  const dto = { name: form.name.trim(), description: form.description.trim() || null }
  try {
    if (form.id) await rolesApi.update(form.id, dto)
    else await rolesApi.create(dto)
    roleModal.value = false
    await load()
  } catch (e) {
    formError.value = e.message
  } finally {
    saving.value = false
  }
}
async function removeRole(r) {
  if (!confirm(`Delete role "${r.name}"?`)) return
  try {
    await rolesApi.remove(r.id)
    await load()
  } catch (e) {
    error.value = e.message
  }
}

function openPerms(r) {
  permRole.value = r
  selected.value = new Set((r.permissions || []).map((p) => p.id))
  permError.value = ''
}
function toggle(id) {
  const s = new Set(selected.value)
  s.has(id) ? s.delete(id) : s.add(id)
  selected.value = s
}
const moduleAll = (g) => g.perms.every((p) => selected.value.has(p.id))
function toggleModule(g) {
  const s = new Set(selected.value)
  const on = !moduleAll(g)
  g.perms.forEach((p) => (on ? s.add(p.id) : s.delete(p.id)))
  selected.value = s
}
async function savePerms() {
  saving.value = true
  permError.value = ''
  try {
    await rolesApi.assignPermissions(permRole.value.id, [...selected.value])
    permRole.value = null
    await load()
  } catch (e) {
    permError.value = e.message
  } finally {
    saving.value = false
  }
}
onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Roles</h2>
    <button v-if="auth.can('role.create')" class="btn" @click="openRole(null)">+ New role</button>
  </div>
  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!roles.length" class="empty">No roles.</div>
    <div v-else class="table-wrap">
      <table>
        <thead><tr><th>Role</th><th>Description</th><th>Permissions</th><th></th></tr></thead>
        <tbody>
          <tr v-for="r in roles" :key="r.id">
            <td><strong>{{ r.name }}</strong></td>
            <td class="muted">{{ r.description || '-' }}</td>
            <td><span class="badge">{{ r.permissions?.length || 0 }}</span></td>
            <td class="actions">
              <button v-if="auth.can('role.assign_permission')" class="btn small" @click="openPerms(r)">Permissions</button>
              <button v-if="auth.can('role.update')" class="btn light small" @click="openRole(r)">Edit</button>
              <button v-if="auth.can('role.delete')" class="btn danger small" @click="removeRole(r)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <BaseModal v-if="roleModal" :title="form.id ? 'Edit role' : 'New role'" @close="roleModal = false">
    <form @submit.prevent="saveRole">
      <div v-if="formError" class="alert error" style="margin-top: 10px">{{ formError }}</div>
      <label for="rn">Name</label>
      <input id="rn" v-model="form.name" required />
      <label for="rd">Description</label>
      <textarea id="rd" v-model="form.description" rows="3" />
      <div class="modal-actions">
        <button type="button" class="btn light" @click="roleModal = false">Cancel</button>
        <button class="btn" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </form>
  </BaseModal>

  <BaseModal v-if="permRole" :title="`Permissions: ${permRole.name}`" subtitle="Tick what this role is allowed to do." @close="permRole = null">
    <div v-if="permError" class="alert error" style="margin-top: 10px">{{ permError }}</div>
    <div v-for="g in groups" :key="g.module" style="margin-top: 14px">
      <label class="check" style="margin: 0"><input type="checkbox" :checked="moduleAll(g)" @change="toggleModule(g)" /> <strong>{{ g.module }}</strong></label>
      <label v-for="p in g.perms" :key="p.id" class="check" style="margin: 6px 0 0 24px; font-weight: 400">
        <input type="checkbox" :checked="selected.has(p.id)" @change="toggle(p.id)" /> {{ p.name }}
      </label>
    </div>
    <div class="modal-actions">
      <button class="btn light" @click="permRole = null">Cancel</button>
      <button class="btn" :disabled="saving" @click="savePerms">{{ saving ? 'Saving…' : 'Save permissions' }}</button>
    </div>
  </BaseModal>
</template>
