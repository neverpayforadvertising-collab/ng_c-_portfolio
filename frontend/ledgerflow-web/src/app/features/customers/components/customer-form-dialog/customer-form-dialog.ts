import { HttpErrorResponse } from '@angular/common/http';

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

import { MatButtonModule } from '@angular/material/button';

import {
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import { finalize } from 'rxjs';

import { CreateCustomerRequest } from '../../../../shared/models/create-customer-request.model';
import { Customer } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-customer-form-dialog',

  standalone: true,

  imports: [
    ReactiveFormsModule,

    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],

  templateUrl: './customer-form-dialog.html',
  styleUrl: './customer-form-dialog.scss'
})
export class CustomerFormDialog {
  private readonly formBuilder =
    inject(FormBuilder);

  private readonly customerService =
    inject(CustomerService);

  private readonly dialogRef =
    inject<
      MatDialogRef<
        CustomerFormDialog,
        Customer | undefined
      >
    >(MatDialogRef);

  readonly saving = signal(false);

  readonly serverError =
    signal('');

  readonly form =
    this.formBuilder.nonNullable.group({
      companyName: [
        '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ],

      contactName: [
        '',
        [
          Validators.required,
          Validators.maxLength(150)
        ]
      ],

      email: [
        '',
        [
          Validators.required,
          Validators.email,
          Validators.maxLength(255)
        ]
      ],

      phone: [
        '',
        Validators.maxLength(50)
      ],

      addressLine1: [
        '',
        Validators.maxLength(255)
      ],

      addressLine2: [
        '',
        Validators.maxLength(255)
      ],

      city: [
        '',
        Validators.maxLength(100)
      ],

      state: [
        '',
        Validators.maxLength(100)
      ],

      postalCode: [
        '',
        Validators.maxLength(20)
      ],

      country: [
        'US',
        [
          Validators.required,
          Validators.pattern(/^[A-Za-z]{2}$/)
        ]
      ]
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
    this.serverError.set('');

    /*
     * Prevent accidentally closing the
     * dialog while the POST is running.
     */
    this.dialogRef.disableClose = true;

    const value =
      this.form.getRawValue();

    const request: CreateCustomerRequest = {
      companyName:
        value.companyName.trim(),

      contactName:
        value.contactName.trim(),

      email:
        value.email
          .trim()
          .toLowerCase(),

      phone:
        this.optional(value.phone),

      addressLine1:
        this.optional(
          value.addressLine1
        ),

      addressLine2:
        this.optional(
          value.addressLine2
        ),

      city:
        this.optional(value.city),

      state:
        this.optional(value.state),

      postalCode:
        this.optional(
          value.postalCode
        ),

      country:
        value.country
          .trim()
          .toUpperCase()
    };

    this.customerService
      .create(request)
      .pipe(
        finalize(() => {
          this.saving.set(false);

          this.dialogRef.disableClose =
            false;
        })
      )
      .subscribe({
        next: (customer: Customer) => {
          this.dialogRef.close(customer);
        },

        error: (error: unknown) => {
          this.serverError.set(
            this.getErrorMessage(error)
          );
        }
      });
  }


  cancel(): void {
    if (this.saving()) {
      return;
    }

    this.dialogRef.close();
  }


  private optional(
    value: string
  ): string | undefined {
    const cleaned =
      value.trim();

    return cleaned === ''
      ? undefined
      : cleaned;
  }


  private getErrorMessage(
    error: unknown
  ): string {
    if (
      error instanceof
      HttpErrorResponse
    ) {
      if (error.status === 409) {
        return (
          error.error?.detail ??
          'A customer with this email already exists.'
        );
      }

      if (error.status === 400) {
        return (
          'Please review the highlighted fields and correct the invalid information.'
        );
      }

      if (error.status === 0) {
        return (
          'Unable to connect to the server. Make sure the API is running.'
        );
      }
    }

    return (
      'Unable to create the customer. Please try again.'
    );
  }
}