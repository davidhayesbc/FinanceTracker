import type { Transaction, TransactionSplit, CreateTransactionDto } from '../models/financeModels';

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

export const TransactionService = {
  async getAllTransactions(): Promise<Transaction[]> {
    return handleApiRequest<Transaction[]>(`${API_BASE_URL}/transactions`);
  },

  async createTransaction(transactionData: CreateTransactionDto): Promise<Transaction> {
    return handleApiRequest<Transaction>(`${API_BASE_URL}/transactions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(transactionData),
    });
  },

  async getTransactionById(id: number): Promise<Transaction> {
    return handleApiRequest<Transaction>(`${API_BASE_URL}/transactions/${id}`);
  },

  async getTransactionsByAccountId(accountId: number): Promise<Transaction[]> {
    return handleApiRequest<Transaction[]>(`${API_BASE_URL}/accounts/${accountId}/transactions`);
  },

  async getTransactionSplits(transactionId: number): Promise<TransactionSplit[]> {
    return handleApiRequest<TransactionSplit[]>(`${API_BASE_URL}/transactions/${transactionId}/transactionSplits`);
  },

  // Add other transaction related methods here based on api.json
  // e.g., updateTransaction, deleteTransaction if needed
};
