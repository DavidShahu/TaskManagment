import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService {
  handle(error: HttpErrorResponse) {
    let message = 'An unexpected error occurred';

    if (error.status === 0) {
      // Network error — API not reachable
      message = 'Cannot connect to server. Please check your connection.';
    } else if (error.status === 400) {
      message = error.error?.message || 'Invalid request';
    } else if (error.status === 401) {
      message = 'You are not authorized. Please login again.';
    } else if (error.status === 403) {
      message = 'You do not have permission to perform this action.';
    } else if (error.status === 404) {
      message = error.error?.message || 'Resource not found.';
    } else if (error.status === 409) {
      message = error.error?.message || 'Conflict error.';
    } else if (error.status === 429) {
      message = 'Too many requests. Please slow down.';
    } else if (error.status >= 500) {
      message = 'Server error. Please try again later.';
    }

    console.error(`[${error.status}] ${message}`, error);
    return throwError(() => new Error(message));
  }
}