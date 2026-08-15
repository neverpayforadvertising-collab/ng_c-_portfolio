import {
  HttpErrorResponse
} from '@angular/common/http';

import {
  Component,
  inject,
  signal
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import {
  MatFormFieldModule
} from '@angular/material/form-field';

import {
  MatInputModule
} from '@angular/material/input';

import {
  MatSelectModule
} from '@angular/material/select';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  finalize
} from 'rxjs';

import {
  CreateExpenseRequest
} from '../../../../shared/models/create-expense-request.model';

import {
  Expense
} from '../../../../shared/models/expense.model';

import {
  ExpenseService
} from '../../services/expense.service';

@Component({
  selector: 'app-expense-form-dialog',
  standalone: true,

  imports: [
    ReactiveFormsModule,

    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],

  templateUrl:
    './expense-form-dialog.html',

  styleUrl:
    './expense-form-dialog.scss'
})
export class ExpenseFormDialog {
  private readonly formBuilder =
    inject(FormBuilder);

  private readonly expenseService =
    inject(ExpenseService);

  private readonly dialogRef =
    inject<
      MatDialogRef<
        ExpenseFormDialog,
        Expense | undefined
      >
    >(MatDialogRef);

  readonly saving =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly categories = [
    'Office & Software',
    'Travel',
    'Marketing',
    'Professional Services',
    'Utilities',
    'Equipment',
    'Other'
  ];

  readonly form =
    this.formBuilder.nonNullable.group({
      vendor: [
        '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ],

      description: [
        '',
        [
          Validators.required,
          Validators.maxLength(300)
        ]
      ],

      category: [
        '',
        Validators.required
      ],

      amount: [
        0,
        [
          Validators.required,
          Validators.min(0.01)
        ]
      ],

      expenseDate: [
        this.today(),
        Validators.required
      ],

      reference: [''],

      notes: ['']
    });

  submit(): void {
    if (
      this.form.invalid ||
      this.saving()
    ) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    const value =
      this.form.getRawValue();

    const request:
      CreateExpenseRequest = {
      vendor:
        value.vendor.trim(),

      description:
        value.description.trim(),

      category:
        value.category,

      amount:
        Number(value.amount),

      expenseDate:
        `${value.expenseDate}T00:00:00`,

      reference:
        this.optional(
          value.reference),

      notes:
        this.optional(
          value.notes)
    };

    this.expenseService
      .create(request)
      .pipe(
        finalize(() =>
          this.saving.set(false)
        )
      )
      .subscribe({
        next: (expense: Expense) =>
          this.dialogRef.close(
            expense),

        error: (error: unknown) => {
          console.error(error);

          this.errorMessage.set(
            error instanceof HttpErrorResponse &&
            error.status === 403
              ? 'You do not have permission to add expenses.'
              : 'Unable to save the expense.'
          );
        }
      });
  }

  cancel(): void {
    if (!this.saving()) {
      this.dialogRef.close();
    }
  }

  private optional(
    value: string
  ): string | undefined {
    const cleaned =
      value.trim();

    return cleaned || undefined;
  }

  private today(): string {
    const now = new Date();

    const offset =
      now.getTimezoneOffset();

    return new Date(
      now.getTime() -
      offset * 60_000
    )
      .toISOString()
      .slice(0, 10);
  }
}