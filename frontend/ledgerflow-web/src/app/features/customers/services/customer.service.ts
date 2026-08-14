import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Customer } from '../../../shared/models/customer.model';
import { CreateCustomerRequest } from '../../../shared/models/create-customer-request.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl =
    'http://localhost:5013/api/customers';

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.baseUrl);
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