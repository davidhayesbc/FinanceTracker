import type { AccountType, TransactionCategory, TransactionType } from '../models/financeModels';

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

export const LookupService = {
  async getAllAccountTypes(): Promise<AccountType[]> {
    return handleApiRequest<AccountType[]>(`${API_BASE_URL}/accountTypes`);
  },

  async createAccountType(accountTypeData: Omit<AccountType, 'id'>): Promise<AccountType> {
    return handleApiRequest<AccountType>(`${API_BASE_URL}/accountTypes`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(accountTypeData),
    });
  },

  async getAllTransactionCategories(): Promise<TransactionCategory[]> {
    return handleApiRequest<TransactionCategory[]>(`${API_BASE_URL}/transactionCategories`);
  },

  async createTransactionCategory(categoryData: Omit<TransactionCategory, 'id'>): Promise<TransactionCategory> {
    return handleApiRequest<TransactionCategory>(`${API_BASE_URL}/transactionCategories`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(categoryData),
    });
  },

  async getAllTransactionTypes(): Promise<TransactionType[]> {
    return handleApiRequest<TransactionType[]>(`${API_BASE_URL}/transactionTypes`);
  },

  async createTransactionType(transactionTypeData: Omit<TransactionType, 'id'>): Promise<TransactionType> {
    return handleApiRequest<TransactionType>(`${API_BASE_URL}/transactionTypes`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(transactionTypeData),
    });
  }

  // Add other lookup/reference data methods here based on api.json
};
