import { CommonModule } from '@angular/common';

import {
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';

import {
  AuthService
} from '../../../../core/auth/auth.service';

import {
  AppRoles
} from '../../../../core/auth/app-roles';

import {
  MatButtonModule
} from '@angular/material/button';

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

import { finalize } from 'rxjs';

import {
  Customer
} from '../../../../shared/models/customer.model';

import {
  CustomerFormDialog
} from '../../components/customer-form-dialog/customer-form-dialog';

import {
  CustomerService
} from '../../services/customer.service';

@Component({
  selector: 'app-customer-list',

  standalone: true,

  imports: [
    CommonModule,

    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule
  ],

  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList
  implements OnInit {

  private readonly authService =
  inject(AuthService);

  readonly canManageCustomers =
    computed(() =>
      this.authService.hasAnyRole(
        AppRoles.admin,
        AppRoles.accountant
      )
  );

  private readonly customerService =
    inject(CustomerService);

  private readonly dialog =
    inject(MatDialog);

  private readonly snackBar =
    inject(MatSnackBar);


  readonly customers =
    signal<Customer[]>([]);

  readonly loading =
    signal(false);

  readonly errorMessage =
    signal('');


  readonly displayedColumns = [
    'companyName',
    'contactName',
    'email',
    'location',
    'status',
    'actions'
  ];


  ngOnInit(): void {
    this.loadCustomers();
  }


  loadCustomers(): void {
    this.loading.set(true);

    this.errorMessage.set('');

    this.customerService
      .getAll()
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({
        next: customers => {
          this.customers.set(
            customers
          );
        },

        error: error => {
          console.error(
            'Unable to load customers:',
            error
          );

          this.errorMessage.set(
            'Unable to load customers. Please check the API connection.'
          );
        }
      });
  }


  openAddCustomer(): void {

    if (!this.canManageCustomers()) {
      return;
    }
    const dialogRef =
      this.dialog.open(
        CustomerFormDialog,
        {
          width: '720px',

          maxWidth: '95vw',

          maxHeight: '90vh',

          autoFocus:
            'first-tabbable',

          restoreFocus: true
        }
      );


    dialogRef
      .afterClosed()
      .subscribe({
        next: (customers: Customer[]) => {
          this.customers.set(customers);
        },

        error: (error: unknown) => {
          console.error(
            'Unable to load customers:',
            error
          );

          this.errorMessage.set(
            'Unable to load customers. Please check the API connection.'
          );
        }
  });
  }
}