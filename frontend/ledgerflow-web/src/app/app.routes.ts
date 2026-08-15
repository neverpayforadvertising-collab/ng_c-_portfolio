import {
  Routes
} from '@angular/router';

import {
  authGuard
} from './core/auth/auth.guard';

import {
  guestGuard
} from './core/auth/guest.guard';


export const routes:
  Routes = [

  /*
   * Public route
   */
  {
    path: 'login',

    canActivate: [
      guestGuard
    ],

    loadComponent: () =>
      import(
        './features/auth/pages/login/login'
      ).then(
        m => m.Login
      )
  },


  /*
   * Authenticated application
   */
  {
    path: '',

    canActivate: [
      authGuard
    ],

    canActivateChild: [
      authGuard
    ],

    loadComponent: () =>
      import(
        './layout/app-shell/app-shell'
      ).then(
        m => m.AppShell
      ),

    children: [
      {
        path: '',

        redirectTo:
          'dashboard',

        pathMatch:
          'full'
      },

      {
        path:
          'dashboard',

        loadComponent: () =>
          import(
            './features/dashboard/pages/dashboard/dashboard'
          ).then(
            m => m.Dashboard
          )
      },

      {
        path:
          'customers',

        loadComponent: () =>
          import(
            './features/customers/pages/customer-list/customer-list'
          ).then(
            m => m.CustomerList
          )
      },

        {
          path: 'expenses',

          loadComponent: () =>
            import(
              './features/expenses/pages/expense-list/expense-list'
            ).then(
              m => m.ExpenseList
            )
        },

        {
          path: 'reports',

          loadComponent: () =>
            import(
              './features/reports/pages/report-overview/report-overview'
            ).then(
              m => m.ReportOverview
            )
        },
    ]
  },

  {
    path: '**',

    redirectTo:
      'dashboard'
  }
];