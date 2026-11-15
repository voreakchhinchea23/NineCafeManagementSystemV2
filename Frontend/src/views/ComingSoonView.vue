<script setup>
import { useRoute } from 'vue-router'
const route = useRoute()
</script>

<template>
  <div class="card empty">
    <h3>{{ route.meta.title }}</h3>
    <p>Not built in Vue yet.</p>
    <p v-if="route.meta.legacy" class="muted">Port it from <code>{{ route.meta.legacy }}</code> in the old frontend.</p>
  </div>
</template>
