export interface ExpenseReport {
  fromDate: string;
  toDate: string;

  totalExpenses: number;
  expenseCount: number;
  averageExpense: number;
  largestExpense: number;

  categories: CategoryExpenseSummary[];

  monthlyTrend: MonthlyExpenseSummary[];
}

export interface CategoryExpenseSummary {
  category: string;
  amount: number;
  count: number;
}

export interface MonthlyExpenseSummary {
  year: number;
  month: number;
  amount: number;
  count: number;
}