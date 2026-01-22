import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthApiService } from '../../core/services/auth-api-service';
import { AuthStateService } from '../../core/services/auth-state-service';

@Component({
  selector: 'app-side-nav',
  imports: [],
  templateUrl: './side-nav.html',
  styleUrl: './side-nav.css',
})
export class SideNav {
  private readonly authApi = inject(AuthApiService);
  private readonly authState = inject(AuthStateService);
  private readonly router = inject(Router);

  protected readonly loading = signal(false);

  async logout(): Promise<void> {
    if (this.loading()) return;

    // Mark explicit logout so the app does not attempt automatic restore on reload
    sessionStorage.setItem('auth-logged-out', '1');

    this.loading.set(true);
    try {
      await firstValueFrom(this.authApi.logout());
    } catch {
      // ignore network errors — still clear client state
    } finally {
      this.authState.clear();
      this.loading.set(false);
      // navigate to login page
      this.router.navigate(['/login']);
    }
  }
}
