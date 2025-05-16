import type { Account, Transaction } from '../models/financeModels'; // Assuming financeModels.ts will be updated or created based on API spec

const API_BASE_URL = import.meta.env.VITE_API_SERVICE_URL + '/api/v1'; // Adjust if your API base URL is different

// Error handling helper
const handleApiRequest = async <T>(url: string): Promise<T> => {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`API request failed with status ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error('API request failed:', error);
    throw new Error('Failed to fetch data from server');
  }
};

export const AccountService = {
  async getAllAccounts(): Promise<Account[]> {
    return handleApiRequest<Account[]>(`${API_BASE_URL}/accounts`);
  },

  async getAccountById(id: number): Promise<Account> {
    return handleApiRequest<Account>(`${API_BASE_URL}/accounts/${id}`);
  },

  async getAccountTransactions(id: number): Promise<Transaction[]> {
    return handleApiRequest<Transaction[]>(`${API_BASE_URL}/accounts/${id}/transactions`);
  },

  async getAccountRecurringTransactions(id: number): Promise<Transaction[]> { // Assuming RecurringTransaction is similar to Transaction for now
    return handleApiRequest<Transaction[]>(`${API_BASE_URL}/accounts/${id}/recurringTransactions`);
  },

  // Add other account related methods here based on api.json if needed
  // For example, methods for AccountTypes if they are directly related to AccountService
  // async getAllAccountTypes(): Promise<AccountType[]> {
  //   return handleApiRequest<AccountType[]>(`${API_BASE_URL}/accountTypes`);
  // }
};
