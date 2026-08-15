import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CreateExpenseRequest } from '../../../shared/models/create-expense-request.model';
import { Expense } from '../../../shared/models/expense.model';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private readonly http =
    inject(HttpClient);

  private readonly baseUrl =
    '/api/expenses';

  getAll(): Observable<Expense[]> {
    return this.http.get<Expense[]>(
      this.baseUrl
    );
  }

  getById(
    id: string
  ): Observable<Expense> {
    return this.http.get<Expense>(
      `${this.baseUrl}/${id}`
    );
  }

  create(
    request: CreateExpenseRequest
  ): Observable<Expense> {
    return this.http.post<Expense>(
      this.baseUrl,
      request
    );
  }
}