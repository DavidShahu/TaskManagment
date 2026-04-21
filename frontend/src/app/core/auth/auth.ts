import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { ErrorHandlerService } from '../services/error-handler.service';
import { TimerService } from '../services/timer.service';
import { NotificationService } from '../services/notification.service';
import { environment } from '../../../environments/environment';


export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  fullName: string;
  role: string;
  expiresAt: string;
  id : string,
}



@Injectable({
  providedIn: 'root',
})
export class Auth {


  private apiUrl = `${environment.apiUrl}/api`;


  constructor(private http: HttpClient, private errorHandler : ErrorHandlerService, private timerService: TimerService, private notificationService : NotificationService) {}

  // login(request: LoginRequest): Observable<AuthResponse> {
  //   return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
  //     .pipe(
  //       tap(response => this.saveToken(response)),
  //       catchError((err: HttpErrorResponse) =>
  //         this.errorHandler.handle(err)));
  // }


  login(request: LoginRequest): Observable<AuthResponse> {
    debugger
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/auth/login`, request)
      .pipe(
        tap(response => {
          this.saveToken(response);
          
          
          this.notificationService.initialize(response.token);

          // restore timer for this specific user after login
          this.timerService.restoreTimer();
        }),
        catchError((err: HttpErrorResponse) =>
          this.errorHandler.handle(err)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, request)
      .pipe(
        tap(response => this.saveToken(response)),
        catchError((err: HttpErrorResponse) =>
          this.errorHandler.handle(err)));
  }

  logout(): void {
    this.notificationService.destroy();
    this.timerService.clearLocalState();
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  private saveToken(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify({
      email: response.email,
      fullName: response.fullName,
      role: response.role,
      id: response.id,  
    }));
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUser(): {  id: string,  email: string, fullName: string, role: string } | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
  
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return Date.now() < payload.exp * 1000;
    } catch {
      return false;
    }
  }

  isAdmin(): boolean {
    return this.getUser()?.role === 'Admin';
  }
}
