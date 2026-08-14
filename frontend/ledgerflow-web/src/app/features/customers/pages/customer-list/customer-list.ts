import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';

import { finalize, timeout } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList implements OnInit {
  private readonly customerService = inject(CustomerService);

  customers: Customer[] = [];
  loading = false;
  errorMessage = '';

  displayedColumns: string[] = [
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
  this.loading = true;
  this.errorMessage = '';

  this.customerService
    .getAll()
    .pipe(
      timeout(5000),
      finalize(() => {
        this.loading = false;
      })
    )
    .subscribe({
      next: (customers: Customer[]) => {
        console.log('Customers:', customers);

        this.customers = customers;
      },

      error: (error: unknown) => {
        console.error(
          'Customer API request failed:',
          error
        );

        this.errorMessage =
          'Unable to load customers.';
      }
    });
}
}
