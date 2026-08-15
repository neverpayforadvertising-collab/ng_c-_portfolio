import {
  HttpErrorResponse
} from '@angular/common/http';

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

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatCardModule
} from '@angular/material/card';

import {
  MatCheckboxModule
} from '@angular/material/checkbox';

import {
  MatFormFieldModule
} from '@angular/material/form-field';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  MatInputModule
} from '@angular/material/input';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  finalize
} from 'rxjs';

import {
  AuthService
} from '../../../../core/auth/auth.service';


@Component({
  selector: 'app-login',

  standalone: true,

  imports: [
    ReactiveFormsModule,

    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],

  templateUrl:
    './login.html',

  styleUrl:
    './login.scss'
})
export class Login {
  private readonly formBuilder =
    inject(FormBuilder);

  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  private readonly route =
    inject(ActivatedRoute);


  readonly submitting =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly hidePassword =
    signal(true);


  readonly form =
    this.formBuilder
      .nonNullable
      .group({
        email: [
          '',
          [
            Validators.required,
            Validators.email
          ]
        ],

        password: [
          '',
          [
            Validators.required
          ]
        ],

        rememberMe: [
          false
        ]
      });


  submit(): void {
    if (
      this.form.invalid ||
      this.submitting()
    ) {
      this.form.markAllAsTouched();

      return;
    }


    this.submitting.set(true);
    this.errorMessage.set('');


    const value =
      this.form.getRawValue();


    this.authService
      .login({
        email:
          value.email
            .trim()
            .toLowerCase(),

        password:
          value.password,

        rememberMe:
          value.rememberMe
      })
      .pipe(
        finalize(() => {
          this.submitting.set(
            false
          );
        })
      )
      .subscribe({
        next: () => {
          this.router.navigateByUrl(
            this.getReturnUrl()
          );
        },

        error: (
          error: unknown
        ) => {
          this.errorMessage.set(
            this.getErrorMessage(
              error
            )
          );
        }
      });
  }


  togglePassword(): void {
    this.hidePassword.update(
      current => !current
    );
  }


  private getReturnUrl():
    string {
    const candidate =
      this.route.snapshot
        .queryParamMap
        .get('returnUrl');


    if (
      candidate &&
      candidate.startsWith('/') &&
      !candidate.startsWith('//')
    ) {
      return candidate;
    }


    return '/dashboard';
  }


  private getErrorMessage(
    error: unknown
  ): string {
    if (
      error instanceof
      HttpErrorResponse
    ) {
      if (error.status === 401) {
        return (
          'Invalid email or password.'
        );
      }

      if (error.status === 423) {
        return (
          'Too many failed attempts. Your account is temporarily locked.'
        );
      }

      if (error.status === 0) {
        return (
          'Unable to connect to LedgerFlow. Make sure the API is running.'
        );
      }
    }


    return (
      'Unable to sign in. Please try again.'
    );
  }
}