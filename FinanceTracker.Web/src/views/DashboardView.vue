<template>
  <div>
    <!-- Accounts Summary -->
    <div v-if="accountStore.isLoading">
      <p>Loading accounts...</p>
    </div>
    <div v-if="accountStore.error">
      <p style="color: red;">Error loading accounts: {{ accountStore.error.message }}</p>
    </div>
    <div v-if="!accountStore.isLoading && !accountStore.error && accountStore.accounts.length > 0">
      <h2>Accounts Summary</h2>
      <DataTable :value="accountStore.accounts" sortMode="multiple" paginator :rows="10" stripedRows>
        <Column field="institution" header="Bank" sortable></Column>
        <Column field="name" header="Name" sortable></Column>
        <Column field="accountTypeName" header="Type" sortable></Column>
        <Column field="openingBalance" header="Balance" sortable :pt="{ 'bodyCell': { style: 'text-align: right;' } }">
          <template #body="slotProps">
            <span :style="{ color: slotProps.data.openingBalance < 0 ? '#ff8c8c' : '#90ee90' }"> <!-- Updated red/green colors -->
              {{ formatCurrency(slotProps.data.openingBalance, slotProps.data.currencyDisplaySymbol) }}
            </span>
          </template>
        </Column>
        <Column field="currencySymbol" header="Currency" sortable :pt="{ 'bodyCell': { style: 'text-align: center;' } }"></Column>
        <Column field="openedDate" header="Opened Date" sortable>
          <template #body="slotProps">
            {{ formatDate(slotProps.data.openedDate) }}
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-if="!accountStore.isLoading && !accountStore.error && accountStore.accounts.length === 0">
      <p>No accounts found.</p>
    </div>

    <hr />

    <!-- Recent Transactions -->
    <h2>Recent Transactions</h2>
    <div v-if="transactionStore.isLoading && transactionStore.recentTransactions.length === 0">
      <p>Loading recent transactions...</p>
    </div>
    <div v-if="transactionStore.error && transactionStore.recentTransactions.length === 0">
      <p style="color: red;">Error loading recent transactions: {{ transactionStore.error.message }}</p>
    </div>
    <div v-if="!transactionStore.isLoading && transactionStore.recentTransactions.length > 0">
      <ul>
        <li v-for="transaction in transactionStore.recentTransactions" :key="transaction.id">
          {{ transaction.transactionDate }} - {{ transaction.description }}: {{ transaction.amount }}
        </li>
      </ul>
    </div>
    <div v-if="!transactionStore.isLoading && !transactionStore.error && transactionStore.recentTransactions.length === 0">
      <p>No recent transactions found.</p>
    </div>

    <hr />

    <!-- Upcoming Bills -->
    <h2>Upcoming Bills</h2>
    <div v-if="recurringTransactionStore.isLoading && recurringTransactionStore.upcomingBills.length === 0">
      <p>Loading upcoming bills...</p>
    </div>
    <div v-if="recurringTransactionStore.error && recurringTransactionStore.upcomingBills.length === 0">
      <p style="color: red;">Error loading upcoming bills: {{ recurringTransactionStore.error.message }}</p>
    </div>
    <div v-if="!recurringTransactionStore.isLoading && recurringTransactionStore.upcomingBills.length > 0">
      <ul>
        <li v-for="bill in recurringTransactionStore.upcomingBills" :key="bill.id">
          {{ bill.nextDueDate }} - {{ bill.description }}: {{ bill.amount }}
        </li>
      </ul>
    </div>
    <div v-if="!recurringTransactionStore.isLoading && !recurringTransactionStore.error && recurringTransactionStore.upcomingBills.length === 0">
      <p>No upcoming bills found.</p>
    </div>

    <!-- Further dashboard content will go here (e.g., Spending by Category, Budget vs Actual) -->

  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useAccountStore } from '../stores/useAccountStore';
import { useTransactionStore } from '../stores/useTransactionStore';
import { useRecurringTransactionStore } from '../stores/useRecurringTransactionStore';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';

const accountStore = useAccountStore();
const transactionStore = useTransactionStore();
const recurringTransactionStore = useRecurringTransactionStore();

const formatCurrency = (value: number, currencySymbol: string) => {
  return `${currencySymbol}${value.toFixed(2)}`;
};

const formatDate = (value: string) => {
  if (!value) return '';
  const date = new Date(value);
  // Adjust for timezone offset to display the correct date
  const userTimezoneOffset = date.getTimezoneOffset() * 60000;
  return new Date(date.getTime() + userTimezoneOffset).toLocaleDateString();
};

onMounted(async () => {
  // Fetch accounts if they haven't been loaded yet
  if (accountStore.accounts.length === 0) {
    await accountStore.fetchAccounts();
  }

  // Fetch recent transactions
  if (transactionStore.recentTransactions.length === 0) {
    await transactionStore.fetchRecentTransactions(); // Default limit is 10
  }

  // Fetch recurring transactions and calculate upcoming bills
  if (recurringTransactionStore.recurringTransactions.length === 0) {
    await recurringTransactionStore.fetchAllRecurringTransactions();
    recurringTransactionStore.calculateUpcomingBills(); // Default 30 days ahead
  }
});
</script>

<style scoped>
/* Styles for DashboardView */
ul {
  list-style-type: none;
  padding: 0;
}
li {
  padding: 8px 0;
  border-bottom: 1px solid #eee;
}

/* PrimeVue DataTable custom styling (optional) */
:deep(.p-datatable .p-datatable-thead > tr > th) {
  background-color: #2a2a2a; /* Darker gray for header */
  color: #f0f0f0; /* Light gray text for header */
  font-weight: bold;
  padding: 0.75rem 1rem;
  border-bottom: 2px solid #444; /* Darker border */
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #3a3a3a; /* Darker row border */
  border-right: 1px solid #3a3a3a; /* Darker cell border */
  color: #e0e0e0; /* Light gray text for body cells */
}

:deep(.p-datatable .p-datatable-tbody > tr > td:first-child) {
  border-left: 1px solid #3a3a3a; /* Darker cell border */
}

:deep(.p-datatable .p-datatable-tbody > tr:nth-child(even)) {
  background-color: #2c2c2c; /* Slightly different dark gray for even rows */
}

:deep(.p-datatable .p-datatable-tbody > tr:hover) {
  background-color: #383838; /* Darker hover effect */
}

/* Ensure the last cell in a row doesn't have a right border if you prefer */
/*
:deep(.p-datatable .p-datatable-tbody > tr > td:last-child) {
  border-right: none;
}
*/

/* Style for the paginator */
:deep(.p-paginator) {
  background-color: #2a2a2a; /* Darker gray for paginator */
  border-top: 1px solid #444; /* Darker border */
  padding: 0.5rem 1rem;
  color: #f0f0f0; /* Light gray text for paginator */
}

:deep(.p-paginator .p-paginator-element:not(.p-disabled).p-highlight) {
  background-color: #007bff; /* Keep primary blue for highlight, or choose a dark theme accent */
  color: white;
  border-color: #007bff;
}

:deep(.p-paginator .p-paginator-element:not(.p-disabled):not(.p-highlight):hover) {
  background-color: #383838; /* Darker hover for paginator elements */
  color: #f0f0f0;
}

:deep(.p-paginator .p-dropdown .p-dropdown-label),
:deep(.p-paginator .p-dropdown .p-dropdown-trigger) {
  color: #f0f0f0; /* Ensure dropdown text/icons in paginator are light */
}

:deep(.p-paginator .p-dropdown-panel) {
  background-color: #3a3a3a; /* Dark background for paginator dropdown */
}

:deep(.p-paginator .p-dropdown-item) {
  color: #f0f0f0; /* Light text for paginator dropdown items */
}

:deep(.p-paginator .p-dropdown-item:hover) {
  background-color: #484848; /* Darker hover for paginator dropdown items */
}

</style>
