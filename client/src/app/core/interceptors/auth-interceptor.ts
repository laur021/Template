import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthApiService } from '../services/auth-api-service';
import { AuthStateService } from '../services/auth-state-service';

export function AuthInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
  const authState = inject(AuthStateService);
  const authApi = inject(AuthApiService);

  const token = authState.accessToken();

  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      })
    : req;

  return next(authReq).pipe(
    catchError((error) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // Attempt token refresh
      return authApi.refresh().pipe(
        switchMap((auth) => {
          authState.setAuth(auth);

          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${auth.accessToken}`,
            },
          });

          return next(retryReq);
        }),
        catchError((refreshError) => {
          authState.clear();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
}
