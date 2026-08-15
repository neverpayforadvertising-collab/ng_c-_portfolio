import {
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { finalize } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../../customers/services/customer.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly customerService =
    inject(CustomerService);

  readonly customers =
    signal<Customer[]>([]);

  readonly loading =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly activeCustomers =
    computed(() =>
      this.customers()
        .filter(customer => customer.isActive)
        .length
    );

  ngOnInit(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {
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
        next: (customers: Customer[]) => {
          this.customers.set(customers);
        },

        error: (error: unknown) => {
          console.error(
            'Dashboard load failed:',
            error
          );

          this.errorMessage.set(
            'Unable to load dashboard data.'
          );
        }
      });
  }
}
