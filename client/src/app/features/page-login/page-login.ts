import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { themes } from '../../layout/theme';
import { Button } from '../../shared/components/button/button';
import { Inputfield } from '../../shared/components/inputfield/inputfield';
import { AuthApiService } from '../../core/services/auth-api-service';
import { AuthStateService } from '../../core/services/auth-state-service';

@Component({
  selector: 'app-page-login',
  imports: [Button, Inputfield],
  templateUrl: './page-login.html',
})
export class PageLogin {
  /* ================= theme ================= */

  protected selectedTheme = signal<string | null>(
    localStorage.getItem('theme')
  );
  protected themes = themes;

  /* ================= auth state ================= */

  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  private readonly authApi = inject(AuthApiService);
  private readonly authState = inject(AuthStateService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const storedTheme = this.selectedTheme();
    if (storedTheme) {
      document.documentElement.setAttribute('data-theme', storedTheme);
    }
  }

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    (document.activeElement as HTMLElement | null)?.blur();
  }

  /* ================= login ================= */

  login() {
    if (this.loading()) return;

    this.loading.set(true);
    this.error.set(null);

    this.authApi.login(this.email(), this.password()).subscribe({
      next: (auth) => {
        this.authState.setAuth(auth);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(
          err?.error?.message ?? 'Invalid email or password'
        );
        this.loading.set(false);
      },
    });
  }
}
