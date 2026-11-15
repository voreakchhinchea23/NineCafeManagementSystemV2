<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from '../stores/auth'
import { firstAllowedPath } from '../router'

const auth = useAuth()
const route = useRoute()
const router = useRouter()

const username = ref('')
const password = ref('')
const show = ref(false)
const busy = ref(false)
const error = ref('')

async function submit() {
  error.value = ''
  busy.value = true
  try {
    await auth.login(username.value.trim(), password.value)
    router.replace(route.query.redirect || firstAllowedPath(auth))
  } catch (e) {
    error.value = e.message
  } finally {
    busy.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <form class="card login-card" @submit.prevent="submit">
      <img src="/favicon.png" alt="Nine Cafe" />
      <h2>Nine Cafe</h2>
      <p class="muted">Management System</p>
      <div v-if="error" class="alert error">{{ error }}</div>

      <label for="u">Username</label>
      <input id="u" v-model="username" autocomplete="username" required autofocus />

      <label for="p">Password</label>
      <div style="display: flex; gap: 6px">
        <input id="p" v-model="password" :type="show ? 'text' : 'password'" autocomplete="current-password" required />
        <button type="button" class="btn light" @click="show = !show">{{ show ? 'Hide' : 'Show' }}</button>
      </div>

      <button class="btn" style="width: 100%; margin-top: 18px" :disabled="busy">{{ busy ? 'Signing in…' : 'Sign in' }}</button>
    </form>
  </div>
</template>
