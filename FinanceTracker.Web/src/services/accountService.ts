import { accountSummary, recentTransactions, spendingByCategory } from '../mocks/accountData';

export const AccountService = {
  getAccountSummary() {
    return Promise.resolve(accountSummary);
  },
  getRecentTransactions() {
    return Promise.resolve(recentTransactions);
  },
  getSpendingByCategory() {
    return Promise.resolve(spendingByCategory);
  }
};
