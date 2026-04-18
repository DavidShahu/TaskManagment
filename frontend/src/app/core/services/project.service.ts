import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable } from "rxjs";
import { ErrorHandlerService } from './error-handler.service';


export interface ProjectMember{
  userId: string;
  fullName: string;
  email: string;
  joinedAt: string;
}

export interface Project {
  id: string;
  name: string;
  description?: string;
  createdByUserId: string;
  createdByName: string;
  createdAt: string;
  isActive: boolean;
  members: ProjectMember[];
}


export interface CreateProjectRequest{
    name: string;
    description?: string;
}

export interface UpdateProjectRequest{
    name: string;
    description?: string;
}

@Injectable({providedIn: 'root'})
export class ProjectService{
    private apiUrl = 'https://localhost:7145/api/projects';
    constructor(private http: HttpClient, private errorHandler: ErrorHandlerService) {}

  getAll(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  getById(id: string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  create(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, request)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  update(id: string, request: UpdateProjectRequest): Observable<Project> {
    return this.http.put<Project>(`${this.apiUrl}/${id}`, request)
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

  addMember(projectId: string, userId: string): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${projectId}/members/${userId}`, {})
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

  removeMember(projectId: string, userId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${projectId}/members/${userId}`)
      .pipe(catchError((err: HttpErrorResponse) =>
        this.errorHandler.handle(err)));
  }

}
