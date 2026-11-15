<script setup>
// One reusable list + create/edit/delete page. A view only describes columns and fields.
import { onMounted, reactive, ref } from 'vue'
import { useAuth } from '../stores/auth'
import BaseModal from './BaseModal.vue'

const props = defineProps({
  title: String,
  singular: { type: String, default: 'item' },
  api: Object, // { list, create, update, remove }
  perm: String, // permission prefix, e.g. "price" -> price.create / price.update / price.delete
  columns: Array, // { key, label, format?(value,row), badge?(value,row) -> { text, cls } }
  fields: Array, // { key, label, type?, required?, default?, step?, options? (array | async fn) }
  extraPayload: { type: Function, default: () => ({}) }, // extra values merged into the request body
})

const auth = useAuth()
const rows = ref([])
const loading = ref(false)
const error = ref('')
const options = reactive({})
const form = reactive({})
const editing = ref(null)
const modalOpen = ref(false)
const saving = ref(false)
const formError = ref('')

const blank = () => Object.fromEntries(props.fields.map((f) => [f.key, f.default ?? (f.type === 'checkbox' ? false : '')]))
const can = (action) => auth.can(`${props.perm}.${action}`)

async function load() {
  loading.value = true
  error.value = ''
  try {
    rows.value = (await props.api.list()) || []
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function loadOptions() {
  for (const f of props.fields) {
    if (!f.options) continue
    options[f.key] = typeof f.options === 'function' ? await f.options().catch(() => []) : f.options
  }
}

function openCreate() {
  editing.value = null
  Object.assign(form, blank())
  formError.value = ''
  modalOpen.value = true
}
function openEdit(row) {
  editing.value = row
  const base = blank()
  Object.assign(form, base, Object.fromEntries(props.fields.map((f) => [f.key, row[f.key] ?? base[f.key]])))
  formError.value = ''
  modalOpen.value = true
}

async function save() {
  formError.value = ''
  const body = {}
  for (const f of props.fields) {
    let v = form[f.key]
    if (typeof v === 'string') v = v.trim()
    if (f.type === 'number' || f.numeric) v = v === '' ? null : Number(v)
    else if (v === '' && !f.required) v = null
    body[f.key] = v
  }
  Object.assign(body, props.extraPayload(editing.value))
  saving.value = true
  try {
    if (editing.value) await props.api.update(editing.value.id, body)
    else await props.api.create(body)
    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e.message
  } finally {
    saving.value = false
  }
}

async function remove(row) {
  if (!confirm(`Delete this ${props.singular}?`)) return
  try {
    await props.api.remove(row.id)
    await load()
  } catch (e) {
    error.value = e.message
  }
}

const cell = (c, row) => (c.format ? c.format(row[c.key], row) : (row[c.key] ?? '-'))

onMounted(() => {
  load()
  loadOptions()
})
defineExpose({ reload: load })
</script>

<template>
  <div class="page-head">
    <h2>{{ title }}</h2>
    <button v-if="can('create')" class="btn" @click="openCreate">+ New {{ singular }}</button>
  </div>

  <div v-if="error" class="alert error">{{ error }}</div>

  <div class="card">
    <div v-if="loading" class="empty">Loading…</div>
    <div v-else-if="!rows.length" class="empty">Nothing here yet.</div>
    <div v-else class="table-wrap">
      <table>
        <thead>
          <tr><th v-for="c in columns" :key="c.key">{{ c.label }}</th><th></th></tr>
        </thead>
        <tbody>
          <tr v-for="r in rows" :key="r.id">
            <td v-for="c in columns" :key="c.key">
              <span v-if="c.badge" class="badge" :class="c.badge(r[c.key], r).cls">{{ c.badge(r[c.key], r).text }}</span>
              <template v-else>{{ cell(c, r) }}</template>
            </td>
            <td class="actions">
              <slot name="actions" :row="r" :reload="load" />
              <button v-if="can('update')" class="btn light small" @click="openEdit(r)">Edit</button>
              <button v-if="can('delete')" class="btn danger small" @click="remove(r)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <BaseModal v-if="modalOpen" :title="editing ? `Edit ${singular}` : `New ${singular}`" @close="modalOpen = false">
    <form @submit.prevent="save">
      <div v-if="formError" class="alert error" style="margin-top: 10px">{{ formError }}</div>
      <template v-for="f in fields" :key="f.key">
        <label v-if="f.type === 'checkbox'" class="check"><input v-model="form[f.key]" type="checkbox" /> {{ f.label }}</label>
        <template v-else>
          <label :for="`f-${f.key}`">{{ f.label }}</label>
          <select v-if="f.options" :id="`f-${f.key}`" v-model="form[f.key]" :required="f.required">
            <option value="" disabled>Select…</option>
            <option v-for="o in options[f.key] || []" :key="o.value" :value="o.value">{{ o.label }}</option>
          </select>
          <textarea v-else-if="f.type === 'textarea'" :id="`f-${f.key}`" v-model="form[f.key]" rows="3" :required="f.required" />
          <input v-else :id="`f-${f.key}`" v-model="form[f.key]" :type="f.type || 'text'" :step="f.step" :min="f.min" :required="f.required" />
        </template>
      </template>
      <div class="modal-actions">
        <button type="button" class="btn light" @click="modalOpen = false">Cancel</button>
        <button class="btn" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </form>
  </BaseModal>
</template>
