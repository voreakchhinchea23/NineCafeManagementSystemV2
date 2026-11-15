<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { reportsApi } from '../api/endpoints'
import { riel, usd } from '../utils/format'

const now = new Date()
const sel = reactive({ year: now.getFullYear(), month: now.getMonth() + 1 })
const report = ref(null)
const loading = ref(false)
const error = ref('')
const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

const sales = computed(() => report.value?.dailySales || [])
const expenses = computed(() => report.value?.dailyExpenses || [])
const maxAmount = computed(() => Math.max(1, ...sales.value.map((d) => d.amount)))

async function load() {
  loading.value = true
  error.value = ''
  try {
    report.value = await reportsApi.monthly(sel.year, sel.month)
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
onMounted(load)
</script>

<template>
  <div class="page-head">
    <h2>Monthly report</h2>
    <form style="display: flex; gap: 8px" @submit.prevent="load">
      <select v-model.number="sel.month" style="width: auto">
        <option v-for="(m, i) in months" :key="m" :value="i + 1">{{ m }}</option>
      </select>
      <input v-model.number="sel.year" type="number" min="2020" max="2100" style="width: 100px" />
      <button class="btn" :disabled="loading">Show</button>
    </form>
  </div>
  <div v-if="error" class="alert error">{{ error }}</div>

  <template v-if="report">
    <div class="grid-stats">
      <div class="card stat"><div class="label">Cups sold</div><div class="value">{{ report.totalCupsSold }}</div></div>
      <div class="card stat"><div class="label">Income</div><div class="value">{{ riel(report.totalIncome) }}</div><div class="sub muted">{{ usd(report.totalIncome) }}</div></div>
      <div class="card stat"><div class="label">Expenses</div><div class="value">{{ riel(report.totalExpense) }}</div><div class="sub muted">{{ usd(report.totalExpense) }}</div></div>
      <div class="card stat">
        <div class="label">Profit</div>
        <div class="value" :style="{ color: report.profit < 0 ? '#c8553a' : '#0b9e82' }">{{ riel(report.profit) }}</div>
        <div class="sub muted">{{ usd(report.profit) }}</div>
      </div>
    </div>

    <div class="card" style="margin-bottom: 18px">
      <h3>Daily sales</h3>
      <div v-if="!sales.length" class="empty">No sales this month.</div>
      <div v-else class="bars">
        <div v-for="d in sales" :key="d.date" class="col" :title="`${d.date}: ${d.cupsSold} cups, ${riel(d.amount)}`">
          <div class="bar" :style="{ height: Math.max(4, (d.amount / maxAmount) * 100) + '%' }"></div>
          <span>{{ d.day }}</span>
        </div>
      </div>
    </div>

    <div class="card">
      <h3 style="margin-bottom: 10px">Daily expenses</h3>
      <div v-if="!expenses.length" class="empty">No expenses this month.</div>
      <div v-else class="table-wrap">
        <table>
          <thead><tr><th>Date</th><th>Amount</th></tr></thead>
          <tbody><tr v-for="d in expenses" :key="d.date"><td>{{ d.date }}</td><td>{{ riel(d.amount) }}</td></tr></tbody>
        </table>
      </div>
    </div>
  </template>
</template>

<style scoped>
.bars { display: flex; align-items: flex-end; gap: 4px; height: 190px; margin-top: 14px; overflow-x: auto; }
.col { flex: 1; min-width: 14px; height: 100%; display: flex; flex-direction: column; justify-content: flex-end; align-items: center; font-size: 11px; }
.bar { width: 100%; background: var(--primary); border-radius: 4px 4px 0 0; }
</style>
