import { Client, Account, AccountType, Transaction, RecurringTransaction, TransactionCategory, TransactionSplit, TransactionType, ProblemDetails } from '../models/api-client';

// Base URL for the API, provided by Vite environment variables
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5200'; // Fallback if not set

// Flag to determine if mock API should be used
const USE_MOCK_API = import.meta.env.VITE_USE_MOCK_API === 'true';

let apiClientInstance: Client | any; // Ideally, this would be an IApiClient interface

if (USE_MOCK_API) {
  console.warn('[ApiService] Using MOCK API. Ensure all necessary methods are mocked.');

  // Define a comprehensive mock client
  // This should implement the same interface as the real Client
  apiClientInstance = {
    // AccountTypes
    getAllAccountTypes: async (): Promise<AccountType[]> => {
      console.log('MOCK: getAllAccountTypes');
      await new Promise(resolve => setTimeout(resolve, 300)); // Simulate network delay
      return [
        AccountType.fromJS({ id: 1, type: 'Mock Savings' }),
        AccountType.fromJS({ id: 2, type: 'Mock Current' }),
        AccountType.fromJS({ id: 3, type: 'Mock Investment' }),
      ];
    },
    createAccountType: async (body: AccountType): Promise<AccountType> => {
      console.log('MOCK: createAccountType', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return AccountType.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 100 });
    },

    // Accounts
    getAllAccounts: async (): Promise<Account[]> => {
      console.log('MOCK: getAllAccounts');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [
        Account.fromJS({ id: 1, name: 'Mock Checking Plus', openingBalance: 1500.75, accountTypeId: 2, openDate: new Date('2023-01-15') }),
        Account.fromJS({ id: 2, name: 'Mock Savings Pro', openingBalance: 10250.00, accountTypeId: 1, openDate: new Date('2022-11-20') }),
      ];
    },
    createAccount: async (body: Account): Promise<Account> => {
      console.log('MOCK: createAccount', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return Account.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 200 });
    },
    getAccountById: async (id: number): Promise<Account> => {
      console.log(`MOCK: getAccountById for id: ${id}`);
      await new Promise(resolve => setTimeout(resolve, 300));
      if (id === 1) {
        return Account.fromJS({ id: 1, name: 'Mock Checking Plus', openingBalance: 1500.75, accountTypeId: 2, openDate: new Date('2023-01-15') });
      }
      // Simulate Not Found
      const errorDetails = ProblemDetails.fromJS({ title: 'Not Found', status: 404, detail: `Mock Account with ID ${id} not found.` });
      throw new ApiException('Not Found', 404, `Mock Account with ID ${id} not found.`, {}, errorDetails);
    },
    getAccountTransactions: async (id: number): Promise<Transaction[]> => {
      console.log(`MOCK: getAccountTransactions for accountId: ${id}`);
      await new Promise(resolve => setTimeout(resolve, 300));
      // Return some mock transactions or an empty array
      return [];
    },
    getAccountRecurringTransactions: async (id: number): Promise<RecurringTransaction[]> => {
      console.log(`MOCK: getAccountRecurringTransactions for accountId: ${id}`);
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },

    // RecurringTransactions
    getAllRecurringTransactions: async (): Promise<RecurringTransaction[]> => {
      console.log('MOCK: getAllRecurringTransactions');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },
    createRecurringTransaction: async (body: RecurringTransaction): Promise<RecurringTransaction> => {
      console.log('MOCK: createRecurringTransaction', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return RecurringTransaction.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 300 });
    },

    // TransactionCategories
    getAllTransactionCategories: async (): Promise<TransactionCategory[]> => {
      console.log('MOCK: getAllTransactionCategories');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },
    createTransactionCategory: async (body: TransactionCategory): Promise<TransactionCategory> => {
      console.log('MOCK: createTransactionCategory', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return TransactionCategory.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 400 });
    },
    
    // TransactionSplits
    getAllTransactionSplits: async (): Promise<TransactionSplit[]> => {
      console.log('MOCK: getAllTransactionSplits');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },
    createTransactionSplit: async (body: TransactionSplit): Promise<TransactionSplit> => {
      console.log('MOCK: createTransactionSplit', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return TransactionSplit.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 500 });
    },

    // TransactionTypes
    getAllTransactionTypes: async (): Promise<TransactionType[]> => {
      console.log('MOCK: getAllTransactionTypes');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },
    createTransactionType: async (body: TransactionType): Promise<TransactionType> => {
      console.log('MOCK: createTransactionType', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return TransactionType.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 600 });
    },

    // Transactions
    getAllTransactions: async (): Promise<Transaction[]> => {
      console.log('MOCK: getAllTransactions');
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    },
    createTransaction: async (body: Transaction): Promise<Transaction> => {
      console.log('MOCK: createTransaction', body);
      await new Promise(resolve => setTimeout(resolve, 300));
      return Transaction.fromJS({ ...body, id: Math.floor(Math.random() * 1000) + 700 });
    },
    getTransactionById: async (id: number): Promise<Transaction> => {
      console.log(`MOCK: getTransactionById for id: ${id}`);
      await new Promise(resolve => setTimeout(resolve, 300));
      const errorDetails = ProblemDetails.fromJS({ title: 'Not Found', status: 404, detail: `Mock Transaction with ID ${id} not found.` });
      throw new ApiException('Not Found', 404, `Mock Transaction with ID ${id} not found.`, {}, errorDetails);
    },
    getTransactionSplits: async (id: number): Promise<TransactionSplit[]> => {
      console.log(`MOCK: getTransactionSplits for transactionId: ${id}`);
      await new Promise(resolve => setTimeout(resolve, 300));
      return [];
    }
    // ... Add mock implementations for ALL methods defined in the Client class
    // Ensure the mock methods return data that matches the expected interfaces/classes (e.g., IAccount, ITransaction)
    // For methods that create data (POST), simulate ID generation.
    // For methods that fetch by ID, consider returning a specific mock item or a 404-like error.
  };
} else {
  console.log(`[ApiService] Using REAL API at: ${API_BASE_URL}`);
  apiClientInstance = new Client(API_BASE_URL);
}

// Helper class for API exceptions if not already part of your api-client.ts
// Ensure this matches or is compatible with the ApiException class used by the generated client.
class ApiException extends Error {
  message: string;
  status: number;
  response: string;
  headers: { [key: string]: any };
  result: any;
  isApiException = true;

  constructor(message: string, status: number, response: string, headers: { [key: string]: any }, result: any) {
    super(message);
    this.message = message;
    this.status = status;
    this.response = response;
    this.headers = headers;
    this.result = result;
  }

  static isApiException(obj: any): obj is ApiException {
    return obj.isApiException === true;
  }
}


// Export the configured client instance
// It's best practice to define an interface (e.g., IApiClient) that both the real Client
// and your mock client implement, then type apiClientInstance to IApiClient.
// For now, we cast to Client, assuming the mock largely matches its public interface.
export const apiClient = apiClientInstance as Client;
