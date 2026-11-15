<script setup>
defineProps({ title: String, subtitle: String })
defineEmits(['close'])
</script>

<template>
  <div class="backdrop" @mousedown.self="$emit('close')">
    <div class="modal" role="dialog" aria-modal="true">
      <h3>{{ title }}</h3>
      <p v-if="subtitle" class="muted" style="margin: 0">{{ subtitle }}</p>
      <slot />
    </div>
  </div>
</template>
