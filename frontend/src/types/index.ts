export interface User {
  email: string;
  firstName: string;
  lastName: string;
}

export interface Category {
  id: number;
  name: string;
  type: string;
  icon: string;
  isCustom: boolean;
}

export interface Transaction {
  id: number;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  amount: number;
  description: string;
  transactionDate: string;
  type: string;
  createdAt: string;
}

export interface Budget {
  id: number;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  amount: number;
  month: number;
  year: number;
  spent: number;
  remaining: number;
  percentageUsed: number;
  status: string;
}

export interface PaginatedTransactions{
  transactions: Transaction[];
  totalcount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateTransaction{
  categoryId: number;
  amount: number;
  description: string;
  transactionDate: Date;
}

export interface MonthlySummary{
  month: number;
  year: number;
  totalIncome: number;
  totalExpenses: number;
  balance: number;
  transactionCount: number;
  savingsRate: number;
}

// Add these to your existing types file
export interface SpendingByCategoryData {
  categoryName: string;
  categoryIcon: string;
  amount: number;
  percentage: number;
}

export interface SpendingByCategoryReport {
  startDate: string;
  endDate: string;
  totalSpending: number;
  categories: SpendingByCategoryData[];
}

export interface MonthlyTrendData {
  month: number;
  year: number;
  monthName: string;
  income: number;
  expenses: number;
  netSavings: number;
}

export interface IncomeVsExpenseTrends {
  startDate: string;
  endDate: string;
  trends: MonthlyTrendData[];
}

export interface CreateBudget{
  categoryId: number;
  amount: number;
  month: number;
  year: number;
}

export interface EditBudget{
  amount: number;
}

export interface CreateEditCategory{
  name: string;
  type: string;
  icon: string;
}