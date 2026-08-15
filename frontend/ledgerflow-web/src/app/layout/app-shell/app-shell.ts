import {
  Component,
  inject,
  signal
} from '@angular/core';

import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  MatListModule
} from '@angular/material/list';

import {
  MatMenuModule
} from '@angular/material/menu';

import {
  MatSidenavModule
} from '@angular/material/sidenav';

import {
  MatToolbarModule
} from '@angular/material/toolbar';

import {
  finalize
} from 'rxjs';

import {
  AuthService
} from '../../core/auth/auth.service';


@Component({
  selector: 'app-shell',

  standalone: true,

  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,

    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatMenuModule,
    MatSidenavModule,
    MatToolbarModule
  ],

  templateUrl:
    './app-shell.html',

  styleUrl:
    './app-shell.scss'
})
export class AppShell {
  readonly auth =
    inject(AuthService);

  private readonly router =
    inject(Router);

  readonly loggingOut =
    signal(false);


  logout(): void {
    if (this.loggingOut()) {
      return;
    }


    this.loggingOut.set(true);


    this.auth
      .logout()
      .pipe(
        finalize(() => {
          this.loggingOut.set(
            false
          );
        })
      )
      .subscribe({
        next: () => {
          this.router
            .navigateByUrl(
              '/login'
            );
        },

        error: error => {
          console.error(
            'Logout failed:',
            error
          );
        }
      });
  }
}