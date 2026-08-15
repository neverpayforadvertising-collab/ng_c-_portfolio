export interface CreateExpenseRequest {
  vendor: string;
  description: string;
  category: string;
  amount: number;
  expenseDate: string;
  reference?: string;
  notes?: string;
}