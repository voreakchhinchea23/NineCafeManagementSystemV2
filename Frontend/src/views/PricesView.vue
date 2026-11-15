<script setup>
import CrudPage from '../components/CrudPage.vue'
import { pricesApi } from '../api/endpoints'
import { riel, usd } from '../utils/format'

const columns = [
  { key: 'priceRiel', label: 'Price', format: (v) => riel(v) },
  { key: 'priceRiel', label: 'In USD', format: (v) => usd(v) },
]
const fields = [{ key: 'priceRiel', label: 'Price (Riel, min 100)', type: 'number', required: true, min: 100, step: 100 }]
</script>

<template>
  <CrudPage title="Prices" singular="price" perm="price" :api="pricesApi" :columns="columns" :fields="fields" />
</template>
