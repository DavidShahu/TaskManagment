import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ErrorHandlerService } from './error-handler.service';

export interface TaskType {
  id: string;
  name: string;
  icon: string;
  color: string;
  isActive: boolean;
}

export interface CreateTaskTypeRequest {
  name: string;
  icon: string;
  color: string;
}

export interface UpdateTaskTypeRequest {
  name: string;
  icon: string;
  color: string;
}

@Injectable({ providedIn: 'root' })
export class TaskTypeService {
  private apiUrl = 'https://localhost:7145/api/tasktypes';

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  getAll(): Observable<TaskType[]> {
    return this.http.get<TaskType[]>(this.apiUrl)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  create(request: CreateTaskTypeRequest): Observable<TaskType> {
    return this.http.post<TaskType>(this.apiUrl, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  update(id: string, request: UpdateTaskTypeRequest): Observable<TaskType> {
    return this.http.put<TaskType>(`${this.apiUrl}/${id}`, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  activate(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/activate`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }
}