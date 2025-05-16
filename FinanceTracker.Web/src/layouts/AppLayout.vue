<template>
    <div class="app-layout">
        <Button icon="pi pi-bars" @click="sidebarVisible = true" class="p-button-text site-sidebar-toggle" />
        <Sidebar v-model:visible="sidebarVisible">
            <h3>Menu</h3>
            <Menu :model="menuItems" />
        </Sidebar>

        <div class="layout-main-container">
            <div class="layout-content">
                <router-view />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import Sidebar from 'primevue/sidebar';
import Menu from 'primevue/menu';
import type { MenuItem } from 'primevue/menuitem';

const sidebarVisible = ref(false);
const router = useRouter();

const menuItems = ref<MenuItem[]>([
    {
        label: 'Dashboard',
        icon: 'pi pi-th-large',
        command: () => router.push('/')
    },
    {
        label: 'Accounts',
        icon: 'pi pi-wallet',
        command: () => router.push('/accounts')
    },
    {
        label: 'Transactions',
        icon: 'pi pi-list',
        command: () => router.push('/transactions')
    },
    {
        label: 'Budgets',
        icon: 'pi pi-chart-pie',
        command: () => router.push('/budgets')
    },
    {
        label: 'Reports',
        icon: 'pi pi-chart-bar',
        command: () => router.push('/reports')
    },
    {
        label: 'Settings',
        icon: 'pi pi-cog',
        command: () => router.push('/settings')
    }
]);
</script>

<style scoped>
@import 'primeicons/primeicons.css';

.app-layout {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
}

.site-sidebar-toggle {
    position: fixed;
    top: 1rem;
    left: 1rem;
    z-index: 1000; /* Ensure it's above the sidebar overlay */
}

/* Basic styling for main content area */
.layout-main-container {
    flex: 1;
    padding-top: 4rem; /* Adjust if using a fixed top bar later */
    padding-left: 1rem; /* Basic padding */
    padding-right: 1rem;
}

/* Responsive adjustments for the toggle button if needed */
@media (min-width: 992px) { /* Example breakpoint for larger screens */
    /* 
        On larger screens, you might want a persistent sidebar instead of a toggle.
        This example keeps the toggle for simplicity, but a common pattern is to 
        transform the Sidebar into a fixed element and push content to the side.
        PrimeVue's AppLayout component or custom CSS with PrimeFlex can achieve this.
    */
}

/* Add some padding to the content when sidebar might be open or for general layout */
.layout-content {
    padding: 1rem;
}
</style>
