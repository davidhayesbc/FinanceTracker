import { defineStore } from 'pinia';
import { ref } from 'vue';
import { TransactionService } from '../services/transactionService';
import type { Transaction, CreateTransactionDto } from '../models/financeModels';

export const useTransactionStore = defineStore('transactions', () => {
  const transactions = ref<Transaction[]>([]);
  const isLoading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAllTransactions() {
    isLoading.value = true;
    error.value = null;
    try {
      transactions.value = await TransactionService.getAllTransactions();
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to fetch transactions');
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchTransactionsForAccount(accountId: number) {
    isLoading.value = true;
    error.value = null;
    try {
      // Assuming TransactionService will have this method based on API spec
      transactions.value = await TransactionService.getTransactionsByAccountId(accountId);
    } catch (err) {
      error.value = err instanceof Error ? err : new Error(`Failed to fetch transactions for account ${accountId}`);
    } finally {
      isLoading.value = false;
    }
  }

  async function createTransaction(transactionData: CreateTransactionDto) {
    isLoading.value = true;
    error.value = null;
    try {
      const newTransaction = await TransactionService.createTransaction(transactionData);
      transactions.value.push(newTransaction);
      return newTransaction;
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to create transaction');
      throw error.value; // Re-throw to allow component to handle
    } finally {
      isLoading.value = false;
    }
  }

  // Placeholder for recent transactions - this might need more specific logic or a dedicated API endpoint
  const recentTransactions = ref<Transaction[]>([]);
  async function fetchRecentTransactions(limit: number = 10) {
    isLoading.value = true;
    error.value = null;
    try {
      // This is a simplified approach. Ideally, the API supports fetching recent transactions directly.
      // Or, if all transactions are fetched, we can sort and slice.
      if (transactions.value.length === 0) {
        await fetchAllTransactions();
      }
      // Sort by date descending (assuming transactionDate is available and sortable)
      const sorted = [...transactions.value].sort((a, b) => 
        new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime()
      );
      recentTransactions.value = sorted.slice(0, limit);
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Failed to fetch recent transactions');
    } finally {
      isLoading.value = false;
    }
  }


  return {
    transactions,
    recentTransactions,
    isLoading,
    error,
    fetchAllTransactions,
    fetchTransactionsForAccount,
    createTransaction,
    fetchRecentTransactions,
  };
});
