import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService {
  handle(error: HttpErrorResponse) {
    let message = 'An unexpected error occurred';
    
    if (error.error) {
       if (error.error.Message) {
        message = error.error.Message;
      }
      // Validation errors
      else if (error.error.message) {
        message = error.error.message;
      }
      // String error
      else if (typeof error.error === 'string') {
        message = error.error;
      }
    } else if (error.message) {
      message = error.message;
    }

    return throwError(() => new Error(message));
  }
}