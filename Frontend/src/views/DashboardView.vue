<script setup>
import { computed, onMounted, ref } from 'vue'
import { dashboardApi } from '../api/endpoints'

const today = new Date().toISOString().slice(0, 10)
const date = ref(today)
const data = ref(null)
const loading = ref(false)
const error = ref('')

const riel = (n) => Math.round(Number(n) || 0).toLocaleString('en-US')
const usd = (n) => '$' + ((Number(n) || 0) / 4000).toFixed(2)

const sales = computed(() => data.value?.todaySales || data.value?.sales || [])
const withdrawals = computed(() => data.value?.withdrawals || [])
const unit = (s) => s.unitPrice || s.currentPriceRiel || s.price?.priceRiel || 0
const total = (s) => s.total ?? s.totalPrice ?? unit(s) * s.quantity

async function load() {
  loading.value = true
  error.value = ''
  try {
    data.value = await dashboardApi.get(date.value)
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
    <h2>Daily overview</h2>
    <div style="display: flex; gap: 8px; align-items: center">
      <input v-model="date" type="date" :max="today" style="width: auto" @change="load" />
      <button class="btn light" :disabled="loading" @click="load">Refresh</button>
    </div>
  </div>

  <div v-if="error" class="alert error">{{ error }}</div>
  <div v-if="loading && !data" class="card empty">Loading…</div>

  <template v-if="data">
    <div class="grid-stats">
      <div class="card stat">
        <div class="label">Cups sold</div>
        <div class="value">{{ data.totalCupSold || 0 }}</div>
      </div>
      <div class="card stat">
        <div class="label">Income</div>
        <div class="value">{{ riel(data.dailyIncome) }} ៛</div>
        <div class="sub muted">{{ usd(data.dailyIncome) }}</div>
      </div>
      <div class="card stat">
        <div class="label">Withdrawn</div>
        <div class="value">{{ riel(data.totalWithdrawnToday) }} ៛</div>
        <div class="sub muted">{{ usd(data.totalWithdrawnToday) }}</div>
      </div>
    </div>

    <div class="card" style="margin-bottom: 18px">
      <h3 style="margin-bottom: 10px">Sales</h3>
      <div v-if="!sales.length" class="empty">No sales recorded.</div>
      <div v-else class="table-wrap">
        <table>
          <thead><tr><th>Unit price</th><th>Qty</th><th>Total</th></tr></thead>
          <tbody>
            <tr v-for="(s, i) in sales" :key="s.id ?? i">
              <td>{{ riel(unit(s)) }} ៛</td>
              <td>{{ s.quantity }}</td>
              <td><strong>{{ riel(total(s)) }} ៛</strong></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="card">
      <h3 style="margin-bottom: 10px">Withdrawals</h3>
      <div v-if="!withdrawals.length" class="empty">No withdrawals.</div>
      <div v-else class="table-wrap">
        <table>
          <thead><tr><th>Amount</th><th>Reason</th></tr></thead>
          <tbody>
            <tr v-for="(w, i) in withdrawals" :key="w.id ?? i">
              <td style="color: #c8553a; font-weight: 700">{{ riel(w.amount) }} ៛</td>
              <td>{{ w.reason }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </template>
</template>
