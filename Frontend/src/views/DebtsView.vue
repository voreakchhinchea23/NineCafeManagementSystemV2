<script setup>
import CrudPage from '../components/CrudPage.vue'
import { useAuth } from '../stores/auth'
import { debtsApi } from '../api/endpoints'
import { riel, todayStr } from '../utils/format'

const auth = useAuth()

const columns = [
  { key: 'customerName', label: 'Customer' },
  { key: 'amount', label: 'Amount', format: (v) => riel(v) },
  { key: 'date', label: 'Date' },
  { key: 'isPaid', label: 'Status', badge: (v) => (v ? { text: 'Paid', cls: 'ok' } : { text: 'Unpaid', cls: 'off' }) },
]
const fields = [
  { key: 'customerName', label: 'Customer name', required: true },
  { key: 'amount', label: 'Amount (Riel)', type: 'number', required: true, min: 1 },
  { key: 'date', label: 'Date', type: 'date', default: todayStr(), required: true },
  { key: 'isPaid', label: 'Already paid', type: 'checkbox' },
]

async function pay(row, reload) {
  try {
    await debtsApi.pay(row.id)
    await reload()
  } catch (e) {
    alert(e.message)
  }
}
</script>

<template>
  <CrudPage title="Debts" singular="debt" perm="debt" :api="debtsApi" :columns="columns" :fields="fields">
    <template #actions="{ row, reload }">
      <button v-if="!row.isPaid && auth.can('debt.update')" class="btn small" @click="pay(row, reload)">Mark paid</button>
    </template>
  </CrudPage>
</template>
