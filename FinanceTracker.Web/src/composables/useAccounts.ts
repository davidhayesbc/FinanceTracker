import { ref, onMounted } from 'vue';
import { AccountService } from '../services/accountService';
import type { Account } from '../models/financeModels'; // Using the new models

export function useAccounts() {
  const accounts = ref<Account[]>([]); // Changed from accountSummary to a list of Accounts
  const loading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAccountsData() { // Renamed function for clarity
    loading.value = true;
    try {
      // Fetch all accounts
      const allAccounts = await AccountService.getAllAccounts();
      accounts.value = allAccounts;

    } catch (err) {
      error.value = err instanceof Error ? err : new Error(String(err));
    } finally {
      loading.value = false;
    }
  }

  onMounted(fetchAccountsData);

  return {
    accounts, // Changed from accountSummary
    loading,
    error,
    fetchAccountsData, // Renamed function
  };
}
