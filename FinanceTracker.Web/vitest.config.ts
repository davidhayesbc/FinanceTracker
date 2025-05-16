import { fileURLToPath } from 'node:url'
import { mergeConfig, defineConfig, configDefaults, type UserConfig } from 'vitest/config'
import viteConfigCallback from './vite.config' // Renamed to indicate it's a callback

// Vitest typically runs in a 'test' or 'development' like mode.
// We need to call the viteConfig callback with a mode.
// If 'process.env.NODE_ENV' is set by Vitest or your test runner, it could be used.
// Otherwise, defaulting to 'development' for defining VITE_API_SERVICE_URL during tests is a safe bet.
const mode = process.env.NODE_ENV === 'production' ? 'production' : 'development';
const resolvedViteConfig = viteConfigCallback({ mode, command: 'serve' }); // command is 'serve' for dev/test

export default mergeConfig(
  resolvedViteConfig as UserConfig, // Cast to UserConfig as mergeConfig expects plain objects or promises
  defineConfig({
    test: {
      environment: 'jsdom',
      exclude: [...configDefaults.exclude, 'e2e/**'],
      root: fileURLToPath(new URL('./', import.meta.url)),
      // If your tests rely on VITE_API_SERVICE_URL, ensure it's available here
      // For example, by setting it via `define` in the Vitest config part if needed,
      // though usually, the define from viteConfig should carry over.
      // setupFiles: ['./src/setupTests.ts'], // if you have a test setup file
    },
  }),
)
