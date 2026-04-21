import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { Auth } from '../auth/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(Auth);
  const router = inject(Router);
  const token = authService.getToken();

  // Check if token is expired before sending request
  if (token && isTokenExpired(token)) {
    authService.logout();
    router.navigate(['/login'], {
      queryParams: { reason: 'session_expired' }
    });
    return throwError(() => new Error('Session expired'));
  }

  // Add token to request
  const authReq = token
    ? req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 from server (token invalid/expired)
      if (error.status === 401) {
        authService.logout();
        router.navigate(['/login'], {
          queryParams: { reason: 'session_expired' }
        });
      }
      return throwError(() => error);
    })
  );
};

function isTokenExpired(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const expiry = payload.exp * 1000; // convert to ms
    return Date.now() > expiry;
  } catch {
    return true; // if can't parse, treat as expired
  }
}