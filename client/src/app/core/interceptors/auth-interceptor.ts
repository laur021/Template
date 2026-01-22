import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, finalize, switchMap, take, throwError } from 'rxjs';
import { AuthApiService } from '../services/auth-api-service';
import { AuthStateService } from '../services/auth-state-service';

// Module-level single-flight state so concurrent interceptors coordinate
let refreshInProgress = false;
const refreshSubject = new BehaviorSubject<string | null>(null);

export function AuthInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
  const authState = inject(AuthStateService);
  const authApi = inject(AuthApiService);

  const token = authState.accessToken();

  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(authReq).pipe(
    catchError((error) => {
      if (error.status !== 401) return throwError(() => error);

      // If refresh is not already running, start it
      if (!refreshInProgress) {
        refreshInProgress = true;
        refreshSubject.next(null);

        return authApi.refresh().pipe(
          switchMap((auth) => {
            authState.setAuth(auth);
            refreshSubject.next(auth.accessToken);

            const retry = req.clone({
              setHeaders: { Authorization: `Bearer ${auth.accessToken}` },
            });
            return next(retry);
          }),
          catchError((refreshErr) => {
            authState.clear();
            return throwError(() => refreshErr);
          }),
          finalize(() => {
            refreshInProgress = false;
          }),
        );
      }

      // If refresh already in progress, wait for it to complete and retry
      return refreshSubject.pipe(
        filter((t): t is string => !!t),
        take(1),
        switchMap((newToken) => {
          const retry = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
          return next(retry);
        }),
      );
    }),
  );
}
