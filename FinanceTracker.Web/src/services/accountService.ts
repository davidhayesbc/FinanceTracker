import { accountSummary, recentTransactions, spendingByCategory } from '../mocks/accountData';
import { netWorthHistory } from '../mocks/netWorthData';
import type {
  AccountSummary,
  Transaction,
  CategorySpending,
  NetWorthHistory,
} from '../models/financeModels';

// Simulate network delay for more realistic testing
const simulateNetworkDelay = (ms = 800) => new Promise(resolve => setTimeout(resolve, ms));

// Error handling helper
const handleApiRequest = async <T>(dataProvider: () => T): Promise<T> => {
  try {
    await simulateNetworkDelay();
    return dataProvider();
  } catch (error) {
    console.error('API request failed:', error);
    throw new Error('Failed to fetch data from server');
  }
};

export const AccountService = {
  async getAccountSummary(): Promise<AccountSummary> {
    return handleApiRequest(() => accountSummary);
  },

  async getRecentTransactions(): Promise<Transaction[]> {
    return handleApiRequest(() => recentTransactions);
  },

  async getSpendingByCategory(): Promise<CategorySpending[]> {
    return handleApiRequest(() => spendingByCategory);
  },

  async getNetWorthHistory(): Promise<NetWorthHistory> {
    return handleApiRequest(() => netWorthHistory);
  },
};
