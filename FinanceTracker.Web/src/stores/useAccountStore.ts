// filepath: d:/source/FinanceTracker/FinanceTracker.Web/src/stores/useAccountStore.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { AccountService } from '../services/accountService';
import type { Account } from '../models/financeModels';

export const useAccountStore = defineStore('accounts', () => {
  const accounts = ref<Account[]>([]);
  const isLoading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAccounts() {
    isLoading.value = true;
    error.value = null;
    try {
      accounts.value = await AccountService.getAllAccounts();
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to fetch accounts');
    } finally {
      isLoading.value = false;
    }
  }

  // Placeholder for current balance calculation if needed
  // function getAccountBalance(accountId: number): number {
  //   const account = accounts.value.find(a => a.id === accountId);
  //   if (!account) return 0;
  //   // This would need to sum transactions or refer to a dedicated balance field
  //   // For now, using openingBalance as a stand-in for simplicity
  //   return account.openingBalance;
  // }

  return {
    accounts,
    isLoading,
    error,
    fetchAccounts,
    // getAccountBalance,
  };
});
