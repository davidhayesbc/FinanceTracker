<script setup lang="ts">
import AppLayout from './layouts/AppLayout.vue';
</script>

<template>
  <AppLayout />
</template>

<style>
/* Reset some base styles to ensure full width */
html,
body,
#app {
  margin: 0;
  padding: 0;
  width: 100%;
  height: 100%;
  overflow-x: hidden; /* Prevent horizontal scroll if content is too wide */
}

body {
  font-family: var(--font-family); /* Ensure PrimeVue font family is applied if set up via main.ts */
  background-color: var(--surface-ground); /* Example global background */
  color: var(--text-color); /* Example global text color */
}
</style>
