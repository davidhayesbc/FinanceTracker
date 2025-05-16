import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import vueDevTools from 'vite-plugin-vue-devtools';

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  // Aspire provides environment variables for referenced services.
  // Format: services__<serviceName>__<bindingName>__<scheme|host|port|url>
  // We expect 'apiservice' with 'http' or 'https' bindings. Aspire might append an index like '__0'.
  const apiServiceHttpsUrl = process.env.services__apiservice__https__0 || process.env.services__apiservice__https__url;
  const apiServiceHttpUrl = process.env.services__apiservice__http__0 || process.env.services__apiservice__http__url;

  // Use HTTPS if available, otherwise HTTP.
  // Fallback for running frontend standalone (e.g., `npm run dev` directly in FinanceTracker.Web)
  // Assuming the ApiService might run on http://localhost:5201 if started independently.
  const resolvedApiServiceUrl = apiServiceHttpsUrl || apiServiceHttpUrl || (mode === 'development' ? 'http://localhost:5201' : '/');


  return {
    plugins: [vue(), vueDevTools()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    define: {
      'import.meta.env.VITE_API_SERVICE_URL': JSON.stringify(resolvedApiServiceUrl),
    },
    server: {
      port: 5174,
      open: true,
      // If you were to proxy (alternative to Aspire's env var injection for local dev):
      // proxy: {
      //   '/api': {
      //     target: 'http://localhost:5201', // Target your local API service
      //     changeOrigin: true,
      //     rewrite: (path) => path.replace(/^\\/api/, '/api') // Ensure /api/v1 prefix is maintained if needed
      //   }
      // }
    },
    esbuild: {
      sourcemap: true,
    },
    css: {
      devSourcemap: true,
    },
  };
});
