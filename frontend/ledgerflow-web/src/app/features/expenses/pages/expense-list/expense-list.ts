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
  MatButtonModule
} from '@angular/material/button';

import {
  MatCardModule
} from '@angular/material/card';

import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  MatSnackBar,
  MatSnackBarModule
} from '@angular/material/snack-bar';

import {
  MatTableModule
} from '@angular/material/table';

import {
  finalize
} from 'rxjs';

import {
  AppRoles
} from '../../../../core/auth/app-roles';

import {
  AuthService
} from '../../../../core/auth/auth.service';

import {
  Expense
} from '../../../../shared/models/expense.model';

import {
  ExpenseFormDialog
} from '../../components/expense-form-dialog/expense-form-dialog';

import {
  ExpenseService
} from '../../services/expense.service';

@Component({
  selector: 'app-expense-list',
  standalone: true,

  imports: [
    CommonModule,

    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTableModule
  ],

  templateUrl:
    './expense-list.html',

  styleUrl:
    './expense-list.scss'
})
export class ExpenseList
    implements OnInit {

  private readonly expenseService =
    inject(ExpenseService);

  private readonly authService =
    inject(AuthService);

  private readonly dialog =
    inject(MatDialog);

  private readonly snackBar =
    inject(MatSnackBar);

  readonly expenses =
    signal<Expense[]>([]);

  readonly loading =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly canManageExpenses =
    computed(() =>
      this.authService.hasAnyRole(
        AppRoles.admin,
        AppRoles.accountant
      )
    );

  readonly totalExpenses =
    computed(() =>
      this.expenses()
        .reduce(
          (total, expense) =>
            total + expense.amount,
          0
        )
    );

  readonly displayedColumns = [
    'expenseDate',
    'vendor',
    'description',
    'category',
    'amount'
  ];

  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.expenseService
      .getAll()
      .pipe(
        finalize(() =>
          this.loading.set(false)
        )
      )
      .subscribe({
        next: (expenses: Expense[]) =>
          this.expenses.set(expenses),

        error: (error: unknown) => {
          console.error(error);

          this.errorMessage.set(
            'Unable to load expenses.'
          );
        }
      });
  }

  openAddExpense(): void {
    if (!this.canManageExpenses()) {
      return;
    }

    const ref =
      this.dialog.open(
        ExpenseFormDialog,
        {
          width: '720px',
          maxWidth: '95vw'
        }
      );

    ref.afterClosed()
      .subscribe(
        (
          expense:
            Expense | undefined
        ) => {
          if (!expense) {
            return;
          }

          this.expenses.update(
            expenses => [
              expense,
              ...expenses
            ]
          );

          this.snackBar.open(
            'Expense recorded successfully.',
            'Dismiss',
            {
              duration: 3000
            }
          );
        }
      );
  }
}