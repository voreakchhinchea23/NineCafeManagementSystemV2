<script setup>
import CrudPage from '../components/CrudPage.vue'
import { useAuth } from '../stores/auth'
import { expensesApi } from '../api/endpoints'
import { riel, todayStr } from '../utils/format'

const auth = useAuth()

const columns = [
  { key: 'date', label: 'Date' },
  { key: 'description', label: 'Description' },
  { key: 'amount', label: 'Amount', format: (v) => riel(v) },
  { key: 'other', label: 'Other' },
  { key: 'createdBy', label: 'Created by' },
]
const fields = [
  { key: 'date', label: 'Date', type: 'date', default: todayStr(), required: true },
  { key: 'description', label: 'Description', required: true },
  { key: 'amount', label: 'Amount (Riel)', type: 'number', required: true, min: 1 },
  { key: 'other', label: 'Other (optional)', type: 'textarea' },
]
// The API wants who the expense belongs to: the current user on create, the original owner on edit.
const extraPayload = (row) => ({ userPublicId: row?.userPublicId || auth.user?.userId })
</script>

<template>
  <CrudPage title="Expenses" singular="expense" perm="expense" :api="expensesApi" :columns="columns" :fields="fields" :extra-payload="extraPayload" />
</template>
