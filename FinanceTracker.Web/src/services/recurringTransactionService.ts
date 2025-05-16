import type { RecurringTransaction, CreateRecurringTransactionDto } from '../models/financeModels';

const API_BASE_URL = import.meta.env.VITE_API_SERVICE_URL + '/api/v1'; // Adjust if your API base URL is different

// Error handling helper (can be shared in a common utility file)
const handleApiRequest = async <T>(url: string, options?: RequestInit): Promise<T> => {
  try {
    const response = await fetch(url, options);
    if (!response.ok) {
      throw new Error(`API request failed with status ${response.status}`);
    }
    if (response.status === 204) { // No Content
      return undefined as T;
    }
    return await response.json();
  } catch (error) {
    console.error('API request failed:', error);
    throw new Error('Failed to fetch data from server');
  }
};

export const RecurringTransactionService = {
  async getAllRecurringTransactions(): Promise<RecurringTransaction[]> {
    return handleApiRequest<RecurringTransaction[]>(`${API_BASE_URL}/recurringTransactions`);
  },

  async createRecurringTransaction(recurringTransactionData: CreateRecurringTransactionDto): Promise<RecurringTransaction> {
    return handleApiRequest<RecurringTransaction>(`${API_BASE_URL}/recurringTransactions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(recurringTransactionData),
    });
  },

  async getRecurringTransactionsByAccountId(accountId: number): Promise<RecurringTransaction[]> {
    return handleApiRequest<RecurringTransaction[]>(`${API_BASE_URL}/accounts/${accountId}/recurringTransactions`);
  },

  // Add other recurring transaction related methods here based on api.json
  // e.g., getRecurringTransactionById, updateRecurringTransaction, deleteRecurringTransaction
};
