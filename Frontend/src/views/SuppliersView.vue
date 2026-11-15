<script setup>
import CrudPage from '../components/CrudPage.vue'
import { suppliersApi } from '../api/endpoints'

const columns = [
  { key: 'name', label: 'Name' },
  { key: 'product', label: 'Product' },
  { key: 'contactInfo', label: 'Contact' },
  { key: 'address', label: 'Address' },
  { key: 'note', label: 'Note' },
]
const fields = [
  { key: 'name', label: 'Name', required: true },
  { key: 'product', label: 'Product', required: true },
  { key: 'contactInfo', label: 'Contact info', required: true },
  { key: 'address', label: 'Address' },
  { key: 'note', label: 'Note', type: 'textarea' },
]
</script>

<template>
  <CrudPage title="Suppliers" singular="supplier" perm="supplier" :api="suppliersApi" :columns="columns" :fields="fields" />
</template>
