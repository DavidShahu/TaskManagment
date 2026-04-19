import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ErrorHandlerService } from './error-handler.service';
import { environment } from '../../../environments/environment';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  
  private apiUrl = `${environment.apiUrl}/api/users`;
  

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  getById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  updateStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { isActive })
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  resetPassword(id: string, newPassword: string): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${id}/reset-password`,
      { newPassword })
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }
}
