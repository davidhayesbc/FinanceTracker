import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import vueDevTools from 'vite-plugin-vue-devtools';

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueDevTools()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5174,
    open: true,
  },
  // build: {
  //   sourcemap: true, // More relevant for production builds
  // },
  esbuild: {
    sourcemap: true, // Changed from 'inline'
  },
  css: {
    devSourcemap: true,
  },
});
