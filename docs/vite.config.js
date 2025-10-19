import { defineConfig } from 'vite'

export default defineConfig({
  server: {
      host: true, // Listen on all addresses
      port: 4444, // Default port
      strictPort: true, // Fail if port is already in use
      allowedHosts: ["involvex.involvex.myfritz.link", "http://localhost:4444"],
      secure: true,
      https: {
key: './ssl/key.pem',
        cert: './ssl/cert.pem'
      },
    },
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    rollupOptions: {
      input: {
        main: 'index.html'
      }
    }
  }
})
