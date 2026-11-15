<script setup>
import { onMounted, reactive, ref } from 'vue'
import { modulesApi, permissionsApi } from '../api/endpoints'
import { useAuth } from '../stores/auth'
import BaseModal from '../components/BaseModal.vue'

const auth = useAuth()
const items = ref([])
const modules = ref([])
const loading = ref(false)
const error = ref('')

const form = reactive({ id: null, name: '', description: '', moduleId: '' })
const modalOpen = ref(false)
const saving = ref(false)
const formError = ref('')

const moduleName = (id) => modules.value.find((m) => String(m.id) === String(id))?.name

async function load() {
  loading.value = true
  error.value = ''
  try {
    const [permissions, moduleList] = await Promise.all([
      permissionsApi.list().catch(() => []),
      modulesApi.list().catch(() => []),
    ])
    items.value = permissions || []
    modules.value = moduleList || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function openCreate() {
  Object.assign(form, { id: null, name: '', description: '', moduleId: modules.value[0]?.id ?? '' })
  formError.value = ''
  modalOpen.value = true
}

function openEdit(permission) {
  Object.assign(form, {
    id: permission.id,
    name: permission.name || '',
    description: permission.description || '',
    moduleId: permission.moduleId ?? '',
  })
  formError.value = ''
  modalOpen.value = true
}

async function save() {
  formError.value = ''
  if (!form.name.trim()) {
    formError.value = 'Permission name is required.'
    return
  }
  if (!form.moduleId) {
    formError.value = 'Please choose a module.'
    return
  }

  saving.value = true
  const dto = {
    name: form.name.trim(),
    description: form.description.trim(),
    moduleId: Number(form.moduleId),
  }

  try {
    if (form.id) await permissionsApi.update(form.id, dto)
    else await permissionsApi.create(dto)

    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e.message
  } finally {
    saving.value = false
  }
}

async function remove(permission) {
  if (!confirm(`Delete permission "${permission.name}"?`)) return
  try {
    await permissionsApi.remove(permission.id)
    await load()
  } catch (e) {
    error.value = e.message
  }
}

onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Permissions</h2>
    <button v-if="auth.can('permission.create')" class="btn" @click="openCreate">+ New permission</button>
  </div>

  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!items.length" class="empty">No permissions yet.</div>
    <div v-else class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Module</th>
            <th>Description</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="permission in items" :key="permission.id">
            <td><strong>{{ permission.name }}</strong></td>
            <td><span class="badge">{{ permission.moduleName || moduleName(permission.moduleId) || '-' }}</span></td>
            <td class="muted">{{ permission.description || '-' }}</td>
            <td class="actions">
              <button v-if="auth.can('permission.update')" class="btn light small" @click="openEdit(permission)">Edit</button>
              <button v-if="auth.can('permission.delete')" class="btn danger small" @click="remove(permission)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <BaseModal v-if="modalOpen" :title="form.id ? 'Edit permission' : 'New permission'" @close="modalOpen = false">
    <form @submit.prevent="save">
      <div v-if="formError" class="alert error" style="margin-top: 10px">{{ formError }}</div>

      <label for="permission-name">Name</label>
      <input id="permission-name" v-model="form.name" required />

      <label for="permission-module">Module</label>
      <select id="permission-module" v-model="form.moduleId" required>
        <option value="" disabled>Select a module</option>
        <option v-for="module in modules" :key="module.id" :value="module.id">{{ module.name }}</option>
      </select>

      <label for="permission-description">Description</label>
      <textarea id="permission-description" v-model="form.description" rows="3" />

      <div class="modal-actions">
        <button type="button" class="btn light" @click="modalOpen = false">Cancel</button>
        <button class="btn" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </form>
  </BaseModal>
</template>
