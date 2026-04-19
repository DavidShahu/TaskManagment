import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ErrorHandlerService } from './error-handler.service';
import { SignalRService, NotificationMessage } from './signalr.service';
import { ToastService } from './toast.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  //private apiUrl = 'https://localhost:7145/api/notifications';
  private apiUrl = `${environment.apiUrl}/api/notifications`;

  notifications = signal<NotificationMessage[]>([]);
  unreadCount = signal(0);

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService,
    private signalRService: SignalRService,
    private toast: ToastService,
    private router: Router
  ) {}

  private initialized = false;



    initialize(token: string): void {
      if (this.initialized) return; // ← prevent double subscription
      this.initialized = true;

      this.signalRService.startConnection(token);
      this.loadNotifications();
      this.loadUnreadCount();

      this.signalRService.notification$.subscribe(notification => {
        this.notifications.update(n => [notification, ...n]);
        this.unreadCount.update(c => c + 1);
       });
    }


  loadNotifications(): void {
    this.http.get<NotificationMessage[]>(this.apiUrl)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)))
      .subscribe({
        next: (notifications) => {
          this.notifications.set(notifications);
        }
      });
  }

  loadUnreadCount(): void {
    this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)))
      .subscribe({
        next: (result) => this.unreadCount.set(result.count)
      });
  }

  markAllAsRead(): void {
    this.http.patch(`${this.apiUrl}/mark-all-read`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)))
      .subscribe({
        next: () => {
          this.unreadCount.set(0);
          this.notifications.update(notifications =>
            notifications.map(n => ({ ...n, isRead: true })));
        }
      });
  }

  markAsRead(id: string): void {
    this.http.patch(`${this.apiUrl}/${id}/read`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)))
      .subscribe({
        next: () => {
          this.notifications.update(notifications =>
            notifications.map(n =>
              n.id === id ? { ...n, isRead: true } : n));
          this.unreadCount.update(c => Math.max(0, c - 1));
        }
      });
  }

  navigateToRelated(notification: NotificationMessage): void {
    if (!notification.relatedEntityId) return;  

      switch (notification.type) {
        case 'TaskAssigned':
        case 'TaskCompleted':
          this.router.navigate(['/tasks', notification.relatedEntityId], {
            onSameUrlNavigation: 'reload'
          });
          break;
        case 'AddedToProject':
          this.router.navigate(['/projects', notification.relatedEntityId], {
            onSameUrlNavigation: 'reload'
          });
          break;
      }
  }

  destroy(): void {
    this.signalRService.stopConnection();
    this.initialized = false;
  }
}
