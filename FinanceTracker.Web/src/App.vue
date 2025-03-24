<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router';
import { ref } from 'vue';

// Define menu items for the sidebar
const menuItems = ref([
  {
    label: 'Dashboard',
    icon: 'pi pi-home',
    to: '/',
  },
  {
    label: 'Accounts',
    icon: 'pi pi-wallet',
    to: '/accounts',
  },
  {
    label: 'Transactions',
    icon: 'pi pi-list',
    to: '/transactions',
  },
  {
    label: 'Budget',
    icon: 'pi pi-chart-pie',
    to: '/budget',
  },
  {
    label: 'Reports',
    icon: 'pi pi-chart-bar',
    to: '/reports',
  },
  {
    label: 'Settings',
    icon: 'pi pi-cog',
    to: '/settings',
  },
]);
</script>

<template>
  <div class="layout-wrapper">
    <!-- Top Navigation Bar -->
    <Menubar class="top-bar">
      <template #start>
        <img alt="Finance Tracker Logo" src="@/assets/logo.svg" width="40" height="40" />
        <span class="app-title">Finance Tracker</span>
      </template>
      <template #end>
        <Button icon="pi pi-bell" class="p-button-rounded p-button-text" />
        <Button icon="pi pi-user" class="p-button-rounded p-button-text" />
      </template>
    </Menubar>

    <div class="layout-content">
      <!-- Sidebar Navigation -->
      <div class="layout-sidebar">
        <Menu :model="menuItems" />
      </div>

      <!-- Main Content Area -->
      <div class="layout-main">
        <RouterView />
      </div>
    </div>
  </div>
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
  overflow-x: hidden;
}

/* Global styles */
.layout-wrapper {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 100vw;
  box-sizing: border-box;
}

.top-bar {
  padding: 0.5rem 1rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.app-title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-left: 1rem;
}

.layout-content {
  display: flex;
  flex-grow: 1;
  width: 100%;
  max-width: 100vw;
}

.layout-sidebar {
  width: 250px;
  min-width: 250px;
  flex-shrink: 0;
  border-right: 1px solid var(--surface-border);
  padding: 1rem;
}

.layout-main {
  flex: 1;
  padding: 1.5rem;
  background-color: var(--surface-ground);
  width: calc(100% - 250px);
  max-width: 100%;
  box-sizing: border-box;
  overflow-x: hidden;
}

@media (max-width: 768px) {
  .layout-content {
    flex-direction: column;
  }
  .layout-sidebar {
    width: 100%;
    min-width: 100%;
    border-right: none;
    border-bottom: 1px solid var(--surface-border);
  }
  .layout-main {
    width: 100%;
  }
}
</style>
