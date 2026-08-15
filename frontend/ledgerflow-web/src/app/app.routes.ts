import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',

    loadComponent: () =>
      import(
        './layout/app-shell/app-shell'
      ).then(
        m => m.AppShell
      ),

    children: [
      {
        path: '',

        redirectTo: 'dashboard',

        pathMatch: 'full'
      },

      {
        path: 'dashboard',

        loadComponent: () =>
          import(
            './features/dashboard/pages/dashboard/dashboard'
          ).then(
            m => m.Dashboard
          )
      },

      {
        path: 'customers',

        loadComponent: () =>
          import(
            './features/customers/pages/customer-list/customer-list'
          ).then(
            m => m.CustomerList
          )
      }
    ]
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];