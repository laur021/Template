import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthApiService, AuthResponse } from './auth-api-service';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  readonly user = signal<AuthResponse['user'] | null>(null);
  readonly accessToken = signal<string | null>(null);
  readonly isAuthenticated = computed(() => !!this.accessToken());
  readonly restoring = signal(false);

  private readonly authApi = inject(AuthApiService);
  private refreshTimer: number | null = null;

  setAuth(auth: AuthResponse) {
    this.user.set(auth.user);
    this.accessToken.set(auth.accessToken);
    this.scheduleRefresh(auth.accessToken);
  }

  clear() {
    this.user.set(null);
    this.accessToken.set(null);
    this.cancelRefresh();
    // Ensure any restoring UI is cleared when the user is explicitly logged out
    this.restoring.set(false);
  }

  private restoringPromise: Promise<void> | null = null;

  async tryRestoreSession(): Promise<void> {
    if (this.restoringPromise) return this.restoringPromise;

    // If the user explicitly logged out in this browser session, avoid attempting restore
    if (sessionStorage.getItem('auth-logged-out') === '1') {
      // Ensure restoring flag is false and return quickly
      this.restoring.set(false);
      return Promise.resolve();
    }

    this.restoring.set(true);
    this.restoringPromise = (async () => {
      try {
        // Add a guard timeout so we don't hang forever if the refresh request never completes
        const refreshPromise = firstValueFrom(this.authApi.refresh());
        const timeoutPromise = new Promise<never>((_, reject) =>
          setTimeout(() => reject(new Error('refresh timeout')), 5_000),
        );

        const auth = (await Promise.race([refreshPromise, timeoutPromise])) as AuthResponse;
        this.setAuth(auth);
      } catch {
        this.clear();
      } finally {
        this.restoring.set(false);
        this.restoringPromise = null;
      }
    })();

    return this.restoringPromise;
  }

  private scheduleRefresh(token: string) {
    this.cancelRefresh();

    const expTs = this.parseExpFromJwt(token);
    if (!expTs) return;

    const msUntilRefresh = Math.max(0, expTs - Date.now() - 60_000); // refresh 60s before expiry

    this.refreshTimer = window.setTimeout(async () => {
      try {
        const auth = await firstValueFrom(this.authApi.refresh());
        this.setAuth(auth);
      } catch {
        this.clear();
      }
    }, msUntilRefresh);
  }

  private cancelRefresh() {
    if (this.refreshTimer !== null) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  private parseExpFromJwt(token: string): number | null {
    try {
      const parts = token.split('.');
      if (parts.length < 2) return null;
      const payload = JSON.parse(atob(parts[1]));
      const exp = payload.exp;
      return typeof exp === 'number' ? exp * 1000 : null;
    } catch {
      return null;
    }
  }
}
