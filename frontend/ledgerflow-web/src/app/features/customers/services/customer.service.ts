import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CreateCustomerRequest } from '../../../shared/models/create-customer-request.model';
import { Customer } from '../../../shared/models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = '/api/customers';

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(
      this.baseUrl
    );
  }

  getById(id: string): Observable<Customer> {
    return this.http.get<Customer>(
      `${this.baseUrl}/${id}`
    );
  }

  create(
    request: CreateCustomerRequest
  ): Observable<Customer> {
    return this.http.post<Customer>(
      this.baseUrl,
      request
    );
  }
}
