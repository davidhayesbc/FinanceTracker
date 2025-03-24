export const accountSummary = {
  totalBalance: 24500.75,
  accounts: [
    { id: 1, name: 'Checking Account', balance: 3250.50, type: 'Bank' },
    { id: 2, name: 'Savings Account', balance: 12750.25, type: 'Bank' },
    { id: 3, name: 'Investment Portfolio', balance: 8500.00, type: 'Investment' }
  ]
};

export const recentTransactions = [
  { id: 1, date: '2023-05-15', description: 'Grocery Store', amount: -125.50, category: 'Food' },
  { id: 2, date: '2023-05-14', description: 'Salary Deposit', amount: 2800.00, category: 'Income' },
  { id: 3, date: '2023-05-13', description: 'Electric Bill', amount: -95.40, category: 'Utilities' },
  { id: 4, date: '2023-05-10', description: 'Restaurant', amount: -65.30, category: 'Dining' },
  { id: 5, date: '2023-05-08', description: 'Gas Station', amount: -45.00, category: 'Transportation' }
];

export const spendingByCategory = [
  { category: 'Food', amount: 450 },
  { category: 'Transportation', amount: 200 },
  { category: 'Utilities', amount: 350 },
  { category: 'Entertainment', amount: 150 },
  { category: 'Shopping', amount: 275 }
];
