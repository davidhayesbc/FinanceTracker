export interface Account {
  id: number;
  name: string;
  balance: number;
  type: string;
}

export interface AccountSummary {
  totalBalance: number;
  accounts: Account[];
}

export interface Transaction {
  id: number;
  date: string;
  description: string;
  amount: number;
  category: string;
}

export interface CategorySpending {
  category: string;
  amount: number;
}

export interface NetWorthHistory {
  labels: string[];
  values: number[];
}
