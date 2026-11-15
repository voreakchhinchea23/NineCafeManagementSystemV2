<script setup>
import { onMounted, reactive, ref } from 'vue'
import { modulesApi } from '../api/endpoints'
import { useAuth } from '../stores/auth'
import BaseModal from '../components/BaseModal.vue'

const auth = useAuth()
const items = ref([])
const loading = ref(false)
const error = ref('')

const form = reactive({ id: null, name: '', description: '' })
const modalOpen = ref(false)
const saving = ref(false)
const formError = ref('')

async function load() {
  loading.value = true
  error.value = ''
  try {
    items.value = (await modulesApi.list()) || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function openCreate() {
  Object.assign(form, { id: null, name: '', description: '' })
  formError.value = ''
  modalOpen.value = true
}
function openEdit(m) {
  Object.assign(form, { id: m.id, name: m.name, description: m.description || '' })
  formError.value = ''
  modalOpen.value = true
}

async function save() {
  formError.value = ''
  if (!form.name.trim()) { formError.value = 'Module name is required.'; return }
  saving.value = true
  const dto = { name: form.name.trim(), description: form.description.trim() }
  try {
    if (form.id) await modulesApi.update(form.id, dto)
    else await modulesApi.create(dto)
    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e.message
  } finally {
    saving.value = false
  }
}

async function remove(m) {
  if (!confirm(`Delete module "${m.name}"? Permissions under it may be affected.`)) return
  try {
    await modulesApi.remove(m.id)
    await load()
  } catch (e) {
    error.value = e.message
  }
}
onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Modules</h2>
    <button v-if="auth.can('module.create')" class="btn" @click="openCreate">+ New module</button>
  </div>

  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!items.length" class="empty">No modules yet.</div>
    <div v-else class="table-wrap">
      <table>
        <thead><tr><th>Name</th><th>Description</th><th></th></tr></thead>
        <tbody>
          <tr v-for="m in items" :key="m.id">
            <td><strong>{{ m.name }}</strong></td>
            <td class="muted">{{ m.description || '-' }}</td>
            <td class="actions">
              <button v-if="auth.can('module.update')" class="btn light small" @click="openEdit(m)">Edit</button>
              <button v-if="auth.can('module.delete')" class="btn danger small" @click="remove(m)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <BaseModal v-if="modalOpen" :title="form.id ? 'Edit module' : 'New module'" @close="modalOpen = false">
    <form @submit.prevent="save">
      <div v-if="formError" class="alert error" style="margin-top: 10px">{{ formError }}</div>
      <label for="mn">Name</label>
      <input id="mn" v-model="form.name" required />
      <label for="md">Description</label>
      <textarea id="md" v-model="form.description" rows="3" />
      <div class="modal-actions">
        <button type="button" class="btn light" @click="modalOpen = false">Cancel</button>
        <button class="btn" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </form>
  </BaseModal>
</template>
