import {
  CommonModule
} from '@angular/common';

import {
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule
} from '@angular/forms';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatCardModule
} from '@angular/material/card';

import {
  MatFormFieldModule
} from '@angular/material/form-field';

import {
  MatInputModule
} from '@angular/material/input';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  MatTableModule
} from '@angular/material/table';

import {
  finalize
} from 'rxjs';

import {
  ExpenseReport
} from '../../../../shared/models/expense-report.model';

import {
  ReportService
} from '../../services/report.service';

@Component({
  selector: 'app-report-overview',
  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTableModule
  ],

  templateUrl:
    './report-overview.html',

  styleUrl:
    './report-overview.scss'
})
export class ReportOverview
    implements OnInit {

  private readonly reportService =
    inject(ReportService);

  private readonly formBuilder =
    inject(FormBuilder);

  readonly report =
    signal<ExpenseReport | null>(
      null
    );

  readonly loading =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly maxMonthlyExpense =
    computed(() => {
      const values =
        this.report()
          ?.monthlyTrend
          .map(x => x.amount)
        ?? [];

      return values.length
        ? Math.max(...values)
        : 0;
    });

  readonly filterForm =
    this.formBuilder.nonNullable.group({
      from: [
        this.monthsAgo(5)
      ],

      to: [
        this.today()
      ]
    });

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    const value =
      this.filterForm.getRawValue();

    this.loading.set(true);
    this.errorMessage.set('');

    this.reportService
      .getExpenseReport(
        value.from,
        value.to
      )
      .pipe(
        finalize(() =>
          this.loading.set(false)
        )
      )
      .subscribe({
        next: (
          report: ExpenseReport
        ) =>
          this.report.set(report),

        error: (error: unknown) => {
          console.error(error);

          this.errorMessage.set(
            'Unable to generate the report.'
          );
        }
      });
  }

  monthLabel(
    year: number,
    month: number
  ): string {
    return new Intl.DateTimeFormat(
      'en-US',
      {
        month: 'short',
        year: 'numeric'
      }
    ).format(
      new Date(
        year,
        month - 1,
        1
      )
    );
  }

  barWidth(
    amount: number
  ): number {
    const max =
      this.maxMonthlyExpense();

    return max === 0
      ? 0
      : (amount / max) * 100;
  }

  private today(): string {
    return this.formatDate(
      new Date()
    );
  }

  private monthsAgo(
    months: number
  ): string {
    const date =
      new Date();

    date.setMonth(
      date.getMonth() - months
    );

    date.setDate(1);

    return this.formatDate(date);
  }

  private formatDate(
    date: Date
  ): string {
    const offset =
      date.getTimezoneOffset();

    return new Date(
      date.getTime() -
      offset * 60_000
    )
      .toISOString()
      .slice(0, 10);
  }
}