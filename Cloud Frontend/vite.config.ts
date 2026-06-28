import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const appBuildId = Date.now().toString(36)

export default defineConfig({
  plugins: [
    react(),
    {
      name: 'inject-app-build-id',
      transformIndexHtml(html) {
        return html.replace(
          '<head>',
          `<head>\n    <meta name="app-build" content="${appBuildId}" />`
        )
      },
    },
  ],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://127.0.0.1:8000',
        changeOrigin: true,
      },
    },
  },
})
