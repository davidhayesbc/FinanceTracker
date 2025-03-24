import { accountSummary, recentTransactions, spendingByCategory } from '../mocks/accountData';
import { netWorthHistory } from '../mocks/netWorthData';

export const AccountService = {
  getAccountSummary() {
    return Promise.resolve(accountSummary);
  },
  getRecentTransactions() {
    return Promise.resolve(recentTransactions);
  },
  getSpendingByCategory() {
    return Promise.resolve(spendingByCategory);
  },
  getNetWorthHistory() {
    return Promise.resolve(netWorthHistory);
  },
};
