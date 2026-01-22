import { Injectable, signal } from '@angular/core';
import { AuthResponse } from './auth-api-service';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  readonly user = signal<AuthResponse['user'] | null>(null);
  readonly accessToken = signal<string | null>(null);

  setAuth(auth: AuthResponse) {
    this.user.set(auth.user);
    this.accessToken.set(auth.accessToken);
  }

  clear() {
    this.user.set(null);
    this.accessToken.set(null);
  }

  isAuthenticated() {
    return !!this.accessToken();
  }
}
