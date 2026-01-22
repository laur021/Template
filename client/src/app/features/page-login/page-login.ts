import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../core/services/auth-api-service';
import { AuthStateService } from '../../core/services/auth-state-service';
import { themes } from '../../layout/theme';
import { Button } from '../../shared/components/button/button';
import { Inputfield } from '../../shared/components/inputfield/inputfield';

interface LoginData {
  email: string;
  password: string;
}

@Component({
  selector: 'app-page-login',
  imports: [Button, Inputfield, ReactiveFormsModule],
  templateUrl: './page-login.html',
})
export class PageLogin {
  /* ================= theme ================= */

  protected selectedTheme = signal<string | null>(localStorage.getItem('theme'));
  protected themes = themes;

  /* ================= auth state ================= */

  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  private readonly authApi = inject(AuthApiService);
  private readonly authState = inject(AuthStateService);
  private readonly router = inject(Router);

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
  });

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
    if (this.loading() || this.loginForm.invalid) return;

    // Mark all controls as touched to show validation errors
    this.loginForm.markAllAsTouched();

    if (this.loginForm.invalid) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const { email, password } = this.loginForm.value;

    this.authApi
      .login(email!, password!)
      .pipe(
        finalize(() => {
          // Always clear loading regardless of success or failure
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (auth) => {
          // Clear explicit logout flag if present (user signed back in)
          sessionStorage.removeItem('auth-logged-out');
          this.authState.setAuth(auth);
          // Navigate after state is set
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.error.set(err?.error?.message ?? 'Invalid email or password');
        },
      });
  }
}
