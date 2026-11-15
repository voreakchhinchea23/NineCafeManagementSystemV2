<script setup>
import { onMounted, reactive, ref } from 'vue'
import { usersApi, rolesApi } from '../api/endpoints'
import { useAuth } from '../stores/auth'
import BaseModal from '../components/BaseModal.vue'

const auth = useAuth()
const users = ref([])
const roles = ref([])
const loading = ref(false)
const error = ref('')

const blank = () => ({ id: null, username: '', fullName: '', email: '', phoneNumber: '', roleId: '', password: '', isActive: true })
const form = reactive(blank())
const modalOpen = ref(false)
const saving = ref(false)
const formError = ref('')

const roleName = (id) => roles.value.find((r) => r.id === id)?.name

async function load() {
  loading.value = true
  error.value = ''
  try {
    const [u, r] = await Promise.all([usersApi.list(), rolesApi.list().catch(() => [])])
    users.value = u || []
    roles.value = r || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function openCreate() {
  Object.assign(form, blank())
  formError.value = ''
  modalOpen.value = true
}
function openEdit(u) {
  Object.assign(form, blank(), {
    id: u.id ?? u.userId ?? u.publicId,
    username: u.username,
    fullName: u.fullName || '',
    email: u.email || '',
    phoneNumber: u.phoneNumber || u.phone || '',
    roleId: u.roleId ?? '',
    isActive: u.isActive !== false,
  })
  formError.value = ''
  modalOpen.value = true
}

async function save() {
  formError.value = ''
  saving.value = true
  const base = {
    username: form.username.trim(),
    fullName: form.fullName.trim(),
    email: form.email.trim(),
    phoneNumber: form.phoneNumber.trim(),
    roleId: Number(form.roleId),
  }
  try {
    if (form.id) await usersApi.update(form.id, { ...base, isActive: form.isActive })
    else await usersApi.create({ ...base, password: form.password })
    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e.message
  } finally {
    saving.value = false
  }
}

async function remove(u) {
  if (!confirm(`Delete user "${u.username}"?`)) return
  try {
    await usersApi.remove(u.id ?? u.userId ?? u.publicId)
    await load()
  } catch (e) {
    error.value = e.message
  }
}
onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Users</h2>
    <button v-if="auth.can('user.create')" class="btn" @click="openCreate">+ New user</button>
  </div>

  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!users.length" class="empty">No users.</div>
    <div v-else class="table-wrap">
      <table>
        <thead><tr><th>Username</th><th>Full name</th><th>Email</th><th>Role</th><th>Status</th><th></th></tr></thead>
        <tbody>
          <tr v-for="u in users" :key="u.id ?? u.userId ?? u.username">
            <td><strong>{{ u.username }}</strong></td>
            <td>{{ u.fullName || '-' }}</td>
            <td class="muted">{{ u.email || '-' }}</td>
            <td><span class="badge">{{ u.roleName || u.role || roleName(u.roleId) || '-' }}</span></td>
            <td><span class="badge" :class="u.isActive !== false ? 'ok' : 'off'">{{ u.isActive !== false ? 'Active' : 'Inactive' }}</span></td>
            <td class="actions">
              <button v-if="auth.can('user.update')" class="btn light small" @click="openEdit(u)">Edit</button>
              <button v-if="auth.can('user.delete')" class="btn danger small" @click="remove(u)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <BaseModal v-if="modalOpen" :title="form.id ? 'Edit user' : 'New user'" @close="modalOpen = false">
    <form @submit.prevent="save">
      <div v-if="formError" class="alert error" style="margin-top: 10px">{{ formError }}</div>
      <label for="uu">Username</label>
      <input id="uu" v-model="form.username" required />
      <label for="uf">Full name</label>
      <input id="uf" v-model="form.fullName" required />
      <label for="ue">Email</label>
      <input id="ue" v-model="form.email" type="email" />
      <label for="up">Phone</label>
      <input id="up" v-model="form.phoneNumber" />
      <label for="ur">Role</label>
      <select id="ur" v-model="form.roleId" required>
        <option value="" disabled>Select a role</option>
        <option v-for="r in roles" :key="r.id" :value="r.id">{{ r.name }}</option>
      </select>
      <template v-if="!form.id">
        <label for="upw">Password</label>
        <input id="upw" v-model="form.password" type="password" autocomplete="new-password" required />
      </template>
      <label v-else class="check"><input v-model="form.isActive" type="checkbox" /> Active</label>
      <div class="modal-actions">
        <button type="button" class="btn light" @click="modalOpen = false">Cancel</button>
        <button class="btn" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </form>
  </BaseModal>
</template>
