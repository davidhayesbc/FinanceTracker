// Based on the provided api.json and common financial application patterns.
// Note: Some schemas in the provided api.json (Transaction, RecurringTransaction, etc.)
// had empty properties. Common fields have been assumed for these.

export interface Account {
  id: number;
  name: string | null;
  openingBalance: number; // double
  openDate: string; // date
  accountTypeId: number;
  currencyId: number;
  accountType?: AccountType;
  currency?: Currency;
  transactions?: Transaction[] | null;
  recurringTransactions?: RecurringTransaction[] | null;
  accountPeriods?: AccountPeriod[] | null;
}

export interface AccountType {
  id: number;
  type: string | null;
  accounts?: Account[] | null;
}

export interface Currency {
  id: number;
  name: string | null;
  symbol: string | null;
  displaySymbol: string | null;
  accounts?: Account[] | null;
  securities?: Security[] | null;
  baseCurrencyRates?: FxRate[] | null;
  counterCurrencyRates?: FxRate[] | null;
}

export interface Transaction {
  id: number;
  description: string | null;
  amount: number;
  transactionDate: string; // date
  accountId: number;
  transactionTypeId: number;
  transactionCategoryId: number | null;
  notes: string | null;
  isSplit: boolean; // Assumed: to indicate if it has splits
  account?: Account;
  transactionType?: TransactionType;
  transactionCategory?: TransactionCategory;
  transactionSplits?: TransactionSplit[] | null;
}

export interface RecurringTransaction {
  id: number;
  accountId: number;
  description: string | null;
  amount: number;
  transactionTypeId: number;
  transactionCategoryId: number | null;
  frequency: string; // e.g., "Monthly", "Weekly", "Yearly"
  startDate: string; // date
  endDate: string | null; // date
  nextDueDate: string | null; // date
  notes: string | null;
  isActive: boolean;
  account?: Account;
  transactionType?: TransactionType;
  transactionCategory?: TransactionCategory;
}

export interface TransactionCategory {
  id: number;
  name: string | null;
  parentCategoryId: number | null;
  parentCategory?: TransactionCategory;
  childCategories?: TransactionCategory[] | null;
  transactions?: Transaction[] | null;
  recurringTransactions?: RecurringTransaction[] | null;
  transactionSplits?: TransactionSplit[] | null;
}

export interface TransactionSplit {
  id: number;
  transactionId: number;
  transactionCategoryId: number;
  amount: number;
  description: string | null;
  transaction?: Transaction;
  transactionCategory?: TransactionCategory;
}

export interface TransactionType { // e.g., Income, Expense, Transfer
  id: number;
  name: string | null;
  transactions?: Transaction[] | null;
  recurringTransactions?: RecurringTransaction[] | null;
}

export interface AccountPeriod {
  id: number;
  accountId: number;
  openingBalance: number; // double
  closingBalance: number; // double
  periodStart: string; // date
  periodEnd: string; // date
  periodCloseDate: string; // date
  account?: Account;
}

export interface FxRate {
  id: number;
  fromCurrencyId: number;
  toCurrencyId: number;
  rate: number; // double
  date: string; // date
  fromCurrencyNavigation?: Currency;
  toCurrencyNavigation?: Currency;
}

export interface Security {
  id: number;
  name: string | null;
  tickerSymbol: string | null;
  currencyId: number;
  securityTypeId: number | null; // Assumed: e.g., Stock, Bond, Crypto
  currency?: Currency;
  prices?: Price[] | null;
}

export interface Price {
  id: number;
  securityId: number;
  date: string; // date
  priceValue: number; // double
  security?: Security;
}

// --- DTOs for Create/Update operations (examples) ---

export interface CreateAccountDto {
  name: string | null;
  openingBalance: number;
  openDate: string;
  accountTypeId: number;
  currencyId: number;
}

export interface CreateTransactionDto {
  description: string | null;
  amount: number;
  transactionDate: string;
  accountId: number;
  transactionTypeId: number;
  transactionCategoryId?: number | null;
  notes?: string | null;
  isSplit?: boolean;
  splits?: CreateTransactionSplitDto[] | null; // If creating splits along with transaction
}

export interface CreateTransactionSplitDto {
  transactionCategoryId: number;
  amount: number;
  description?: string | null;
}

export interface CreateRecurringTransactionDto {
  accountId: number;
  description: string | null;
  amount: number;
  transactionTypeId: number;
  transactionCategoryId?: number | null;
  frequency: string;
  startDate: string;
  endDate?: string | null;
  notes?: string | null;
  isActive?: boolean;
}

// Add other DTOs as needed for AccountType, TransactionCategory, etc.
// For example:
export interface CreateAccountTypeDto {
  type: string;
}

export interface CreateTransactionCategoryDto {
  name: string;
  parentCategoryId?: number | null;
}

export interface CreateTransactionTypeDto {
  name: string;
}

// ProblemDetails might be used for error responses, but its structure is empty in api.json
export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  // Extensions would go here as additional properties
  [key: string]: any;
}
