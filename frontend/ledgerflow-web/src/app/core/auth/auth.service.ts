import {
  HttpClient,
  HttpErrorResponse
} from '@angular/common/http';

import {
  computed,
  inject,
  Injectable,
  signal
} from '@angular/core';

import {
  catchError,
  finalize,
  map,
  Observable,
  of,
  switchMap,
  tap
} from 'rxjs';

import {
  AuthUser,
  LoginRequest
} from './auth.models';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http =
    inject(HttpClient);


  private readonly currentUser =
    signal<AuthUser | null>(null);

  private readonly initialized =
    signal(false);

  readonly user =
    this.currentUser.asReadonly();

  readonly isInitialized =
    this.initialized.asReadonly();

  readonly isAuthenticated =
    computed(
      () =>
        this.currentUser() !== null
    );


  initialize(): Observable<void> {
    return this.refreshCsrf()
      .pipe(
        switchMap(() =>
          this.http.get<AuthUser>(
            '/api/auth/me'
          )
        ),

        tap(user =>
          this.currentUser.set(user)
        ),

        catchError(
          (
            error:
              HttpErrorResponse
          ) => {
            /*
             * 401 simply means there is
             * no existing login session.
             */
            if (
              error.status !== 401
            ) {
              console.error(
                'Authentication initialization failed:',
                error
              );
            }

            this.currentUser.set(null);

            return of(null);
          }
        ),

        map(() => void 0),

        finalize(() => {
          this.initialized.set(true);
        })
      );
  }


  login(
    request: LoginRequest
  ): Observable<AuthUser> {
    /*
     * Get an anonymous XSRF token first.
     */
    return this.refreshCsrf()
      .pipe(
        switchMap(() =>
          this.http.post<AuthUser>(
            '/api/auth/login',
            request
          )
        ),

        tap(user => {
          this.currentUser.set(
            user
          );
        }),

        /*
         * The identity changed from
         * anonymous -> authenticated,
         * so get a new XSRF token.
         */
        switchMap(user =>
          this.refreshCsrf()
            .pipe(
              map(() => user)
            )
        )
      );
  }


  logout(): Observable<void> {
    return this.http
      .post<void>(
        '/api/auth/logout',
        {}
      )
      .pipe(
        tap(() => {
          this.currentUser.set(null);
        }),

        /*
         * Establish a fresh anonymous
         * XSRF token after logout.
         */
        switchMap(() =>
          this.refreshCsrf()
        ),

        catchError(error => {
          /*
           * Local UI should never stay
           * authenticated if logout
           * reports that the session
           * no longer exists.
           */
          if (
            error instanceof
              HttpErrorResponse &&
            error.status === 401
          ) {
            this.currentUser.set(null);

            return of(void 0);
          }

          throw error;
        })
      );
  }


  hasRole(
    role: string
  ): boolean {
    return (
      this.currentUser()
        ?.roles
        .includes(role) ??
      false
    );
  }


  private refreshCsrf():
    Observable<void> {
    return this.http
      .get<void>(
        '/api/auth/csrf'
      );
  }
}