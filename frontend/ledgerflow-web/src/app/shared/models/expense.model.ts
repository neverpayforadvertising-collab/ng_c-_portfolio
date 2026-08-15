export interface Expense {
  id: string;
  vendor: string;
  description: string;
  category: string;
  amount: number;
  expenseDate: string;
  reference?: string;
  notes?: string;
  createdAtUtc: string;
}