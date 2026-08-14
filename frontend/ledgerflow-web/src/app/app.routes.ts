import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'customers',
    pathMatch: 'full'
  },
  {
    path: 'customers',
    loadComponent: () =>
      import(
        './features/customers/pages/customer-list/customer-list'
      ).then(m => m.CustomerList)
  },
  {
    path: '**',
    redirectTo: 'customers'
  }
];