import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ErrorHandlerService } from './error-handler.service';
import { environment } from '../../../environments/environment';

export interface TimeLog {
  id: string;
  hours: number;
  note?: string;
  loggedAt: string;
  userId: string;
  userName: string;
}

export interface Task {
  id: string;
  title: string;
  description?: string;
  dueDate?: string;
  status: string;
  createdAt: string;
  ownerId: string;
  ownerName: string;
  projectId?: string;
  projectName?: string;
  estimatedHours?: number;
  loggedHours: number;
  remainingHours?: number;
  progressPercentage?: number;
  isOverdue: boolean;
  timeLogs: TimeLog[];
  createdByUserId: string;
  isAssignedByAdmin: boolean;
  taskTypeId?: string;
  taskTypeName?: string;
  taskTypeIcon?: string;
  taskTypeColor?: string;
  createdByName: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
  projectId?: string;
  estimatedHours?: number;
  assignedToUserId?: string;
  taskTypeId?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
  estimatedHours?: number;
  taskTypeId?: string;
}

export interface LogTimeRequest {
  hours: number;
  note?: string;
}

@Injectable({ providedIn: 'root' })
export class TaskService {
  
  private apiUrl = `${environment.apiUrl}/api/tasks`;
  

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  getMyTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/my`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  getById(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  getByProject(projectId: string): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/project/${projectId}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  create(request: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  update(id: string, request: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  markAsDone(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/done`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  markAsOpen(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/open`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  logTime(id: string, request: LogTimeRequest): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/${id}/log-time`, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }
}
