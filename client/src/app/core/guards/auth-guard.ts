import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '../services/auth-state-service';

export const AuthGuard: CanActivateFn = async () => {
  const auth = inject(AuthStateService);
  const router = inject(Router);

  // If not already authenticated, attempt session restore (single-flight inside the service)
  if (!auth.isAuthenticated()) {
    try {
      await auth.tryRestoreSession();
    } catch {
      // If restore fails unexpectedly, continue to login redirect below
    }
  }

  if (!auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  return true;
};
