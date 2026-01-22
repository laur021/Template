import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  user: {
    id: string;
    email: string;
    displayName: string;
    imageUrl?: string;
    roles: string[];
  };
  accessToken: string;
  accessTokenExpiration: string;
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  login(email: string, password: string) {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}/Auth/login`,
      {
        email,
        password,
      },
      {
        withCredentials: true, // 🔴 REQUIRED for refresh cookie
      },
    );
  }

  refresh() {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}/Auth/refresh`,
      {},
      { withCredentials: true }
    );
  }

  logout() {
    return this.http.post(`${this.baseUrl}/Auth/logout`, {}, { withCredentials: true });
  }

  register(payload: {
    email: string;
    password: string;
    displayName?: string;
    confirmationUrlBase: string;
  }) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/Auth/register`, payload, {
      withCredentials: true,
    });
  }
}
