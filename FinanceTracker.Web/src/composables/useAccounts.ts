import { ref, onMounted } from 'vue';
import { AccountService } from '../services/accountService';

interface AccountSummary {
  totalBalance: number;
  accounts: { id: number; name: string; balance: number; type: string }[];
}

interface Transaction {
  id: number;
  date: string;
  description: string;
  amount: number;
  category: string;
}

interface CategorySpending {
  category: string;
  amount: number;
}

interface NetWorthHistory {
  labels: string[];
  values: number[];
}

export function useAccounts() {
  const accountSummary = ref<AccountSummary | null>(null);
  const recentTransactions = ref<Transaction[]>([]);
  const spendingByCategory = ref<CategorySpending[]>([]);
  const netWorthHistory = ref<NetWorthHistory | null>(null);
  const loading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAccounts() {
    loading.value = true;
    try {
      // Add Promise.all to fetch data in parallel for better performance
      const [accounts, transactions, spending, netWorth] = await Promise.all([
        AccountService.getAccountSummary(),
        AccountService.getRecentTransactions(),
        AccountService.getSpendingByCategory(),
        AccountService.getNetWorthHistory(),
      ]);

      accountSummary.value = accounts;
      recentTransactions.value = transactions;
      spendingByCategory.value = spending;
      netWorthHistory.value = netWorth;
    } catch (err) {
      error.value = err instanceof Error ? err : new Error(String(err));
    } finally {
      loading.value = false;
    }
  }

  onMounted(fetchAccounts);

  return {
    accountSummary,
    recentTransactions,
    spendingByCategory,
    netWorthHistory,
    loading,
    error,
    fetchAccounts,
  };
}
