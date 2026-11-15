<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from '../stores/auth'
import { navItems } from '../nav'

const auth = useAuth()
const route = useRoute()
const router = useRouter()
const open = ref(false)

const items = computed(() => navItems.filter((n) => auth.can(n.perm)))
watch(() => route.fullPath, () => (open.value = false))

async function logout() {
  await auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="shell">
    <aside class="sidebar" :class="{ open }">
      <div class="brand"><img src="/favicon.png" alt="" /> Nine Cafe</div>
      <nav class="nav">
        <RouterLink v-for="n in items" :key="n.to" :to="n.to"><span>{{ n.icon }}</span>{{ n.label }}</RouterLink>
      </nav>
    </aside>

    <div class="main">
      <header class="topbar">
        <div>
          <button class="btn light small menu-btn" @click="open = !open">☰</button>
          <strong style="color: var(--heading); font-size: 16px">{{ route.meta.title }}</strong>
        </div>
        <div class="who">
          <div style="text-align: right">
            <div style="font-weight: 700; color: var(--heading)">{{ auth.user?.fullName || auth.user?.username }}</div>
            <div class="muted" style="font-size: 12px">{{ auth.user?.role }}</div>
          </div>
          <button class="btn light small" @click="logout">Log out</button>
        </div>
      </header>
      <main class="content"><RouterView /></main>
    </div>
  </div>
</template>
