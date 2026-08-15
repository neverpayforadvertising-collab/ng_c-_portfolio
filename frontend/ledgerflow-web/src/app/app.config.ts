import {
  ApplicationConfig,
  inject,
  provideAppInitializer
} from '@angular/core';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  provideRouter
} from '@angular/router';

import { routes } from './app.routes';

import {
  AuthService
} from './core/auth/auth.service';


export const appConfig:
  ApplicationConfig = {

  providers: [
    provideRouter(routes),

    provideHttpClient(),

    provideAppInitializer(
      () =>
        inject(AuthService)
          .initialize()
    )
  ]
};