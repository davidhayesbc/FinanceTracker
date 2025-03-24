import { ref, onMounted } from 'vue';
import { AccountService } from '../services/accountService';

interface AccountSummary {
  totalBalance: number;
  accounts: { id: number; name: string; balance: number; type: string; }[];
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

export function useAccounts() {
  const accountSummary = ref<AccountSummary | null>(null);
  const recentTransactions = ref<Transaction[]>([]);
  const spendingByCategory = ref<CategorySpending[]>([]);
  const loading = ref(false);
  const error = ref<Error | null>(null);

  async function fetchAccounts() {
    loading.value = true;
    try {
      accountSummary.value = await AccountService.getAccountSummary();
      recentTransactions.value = await AccountService.getRecentTransactions();
      spendingByCategory.value = await AccountService.getSpendingByCategory();
    } catch (err) {
      error.value = err instanceof Error ? err : new Error(String(err));
    } finally {
      loading.value = false;
    }
  }

  onMounted(fetchAccounts);

  return { accountSummary, recentTransactions, spendingByCategory, loading, error, fetchAccounts };
}
