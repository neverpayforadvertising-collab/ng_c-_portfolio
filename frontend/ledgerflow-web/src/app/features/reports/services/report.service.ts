import {
  HttpClient,
  HttpParams
} from '@angular/common/http';

import {
  inject,
  Injectable
} from '@angular/core';

import {
  Observable
} from 'rxjs';

import {
  ExpenseReport
} from '../../../shared/models/expense-report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly http =
    inject(HttpClient);

  getExpenseReport(
    from: string,
    to: string
  ): Observable<ExpenseReport> {
    const params =
      new HttpParams()
        .set('from', from)
        .set('to', to);

    return this.http.get<ExpenseReport>(
      '/api/reports/expenses',
      { params }
    );
  }
}