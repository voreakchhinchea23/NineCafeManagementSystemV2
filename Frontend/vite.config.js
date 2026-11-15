import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

// In dev, the browser calls /api on the Vite server and Vite forwards it to the
// .NET API. That avoids CORS and the "localhost" problem inside Codespaces.
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  return {
    plugins: [vue()],
    server: {
      host: true,
      port: 5173,
      allowedHosts: ['.app.github.dev', 'localhost'],
      proxy: {
        '/api': { target: env.VITE_API_PROXY || 'http://localhost:5256', changeOrigin: true },
      },
    },
  }
})
