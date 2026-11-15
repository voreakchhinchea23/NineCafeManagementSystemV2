import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import { useAuth } from './stores/auth'
import './style.css'

const app = createApp(App)
app.use(createPinia())
app.use(router)

// The API client fires this when a token is rejected (expired / logged out elsewhere).
window.addEventListener('ncms:unauthorized', () => {
  useAuth().clear()
  router.push({ name: 'login' })
})

app.mount('#app')
