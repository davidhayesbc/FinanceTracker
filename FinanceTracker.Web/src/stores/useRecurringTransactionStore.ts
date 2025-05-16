import { defineStore } from 'pinia';
import { ref } from 'vue';
import { RecurringTransactionService } from '../services/recurringTransactionService';
import type { RecurringTransaction, CreateRecurringTransactionDto } from '../models/financeModels';

export const useRecurringTransactionStore = defineStore('recurringTransactions', () => {
  const recurringTransactions = ref<RecurringTransaction[]>([]);
  const isLoading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAllRecurringTransactions() {
    isLoading.value = true;
    error.value = null;
    try {
      recurringTransactions.value = await RecurringTransactionService.getAllRecurringTransactions();
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to fetch recurring transactions');
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchRecurringTransactionsForAccount(accountId: number) {
    isLoading.value = true;
    error.value = null;
    try {
      recurringTransactions.value = await RecurringTransactionService.getRecurringTransactionsByAccountId(accountId);
    } catch (err) {
      error.value = err instanceof Error ? err : new Error(`Failed to fetch recurring transactions for account ${accountId}`);
    } finally {
      isLoading.value = false;
    }
  }

  async function createRecurringTransaction(transactionData: CreateRecurringTransactionDto) {
    isLoading.value = true;
    error.value = null;
    try {
      const newTransaction = await RecurringTransactionService.createRecurringTransaction(transactionData);
      recurringTransactions.value.push(newTransaction);
      return newTransaction;
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to create recurring transaction');
      throw error.value; // Re-throw to allow component to handle
    } finally {
      isLoading.value = false;
    }
  }

  // Upcoming bills could be derived from recurringTransactions based on nextDueDate
  // This is a simplified example; more sophisticated logic might be needed
  const upcomingBills = ref<RecurringTransaction[]>([]);
  function calculateUpcomingBills(daysAhead: number = 30) {
    const today = new Date();
    const limitDate = new Date(today);
    limitDate.setDate(today.getDate() + daysAhead);

    upcomingBills.value = recurringTransactions.value.filter(rt => {
      if (!rt.nextDueDate || !rt.isActive) return false;
      const dueDate = new Date(rt.nextDueDate);
      return dueDate >= today && dueDate <= limitDate;
    }).sort((a, b) => new Date(a.nextDueDate!).getTime() - new Date(b.nextDueDate!).getTime());
  }

  // Call this after fetching recurring transactions
  // Example: fetchAllRecurringTransactions().then(() => calculateUpcomingBills());

  return {
    recurringTransactions,
    upcomingBills,
    isLoading,
    error,
    fetchAllRecurringTransactions,
    fetchRecurringTransactionsForAccount,
    createRecurringTransaction,
    calculateUpcomingBills,
  };
});
