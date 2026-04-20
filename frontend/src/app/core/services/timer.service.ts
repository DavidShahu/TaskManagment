import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface TimerResponse {
  taskId: string;
  taskTitle: string;
  startedAt: string;
  elapsedHours: number;
  elapsedFormatted: string;
}

@Injectable({ providedIn: 'root' })
export class TimerService {

  private apiUrl = `${environment.apiUrl}/api/tasks`;

  private intervalId: any = null;

  isRunning = signal(false);
  elapsedSeconds = signal(0);
  activeTaskId = signal<string | null>(null);
  activeTaskTitle = signal<string | null>(null);

  constructor(private http: HttpClient) {
     
  }

restoreTimer(): void {
    this.http.get<TimerResponse | null>(`${this.apiUrl}/timer`)
      .subscribe({
        next: (timer) => {
          if (!timer){

            this.clearLocalState();
            return;
          } 

          const elapsed = Math.floor(
            (Date.now() - new Date(timer.startedAt).getTime()) / 1000);

          this.activeTaskId.set(timer.taskId);
          this.activeTaskTitle.set(timer.taskTitle);
          this.elapsedSeconds.set(elapsed);
          this.isRunning.set(true);
          this.startTicking();
        },
        error: () => {}
      });
  }

  clearLocalState(): void {
    clearInterval(this.intervalId);
    this.intervalId = null;
    this.isRunning.set(false);
    this.elapsedSeconds.set(0);
    this.activeTaskId.set(null);
    this.activeTaskTitle.set(null);
  }

  start(taskId: string): void {
    this.http.post<TimerResponse>(
      `${this.apiUrl}/${taskId}/timer/start`, {})
      .subscribe({
        next: (timer) => {
          if (this.intervalId) clearInterval(this.intervalId);

          const elapsed = Math.floor(
            (Date.now() - new Date(timer.startedAt).getTime()) / 1000);

          this.activeTaskId.set(timer.taskId);
          this.activeTaskTitle.set(timer.taskTitle);
          this.elapsedSeconds.set(elapsed);
          this.isRunning.set(true);
          this.startTicking();
        }
      });
  }

  stop(): Promise<number> {
    return new Promise((resolve) => {
      this.http.post<{ hours: number }>(
        `${this.apiUrl}/timer/stop`, {})
        .subscribe({
          next: (result) => {
            clearInterval(this.intervalId);
            this.intervalId = null;
            this.isRunning.set(false);
            this.elapsedSeconds.set(0);
            this.activeTaskId.set(null);
            this.activeTaskTitle.set(null);
            resolve(result.hours);
          },
          error: () => {
            // Task was deleted — clear timer state anyway
            clearInterval(this.intervalId);
            this.intervalId = null;
            this.isRunning.set(false);
            this.elapsedSeconds.set(0);
            this.activeTaskId.set(null);
            this.activeTaskTitle.set(null);
            resolve(0);
          }
        });
    });
  }

  private startTicking(): void {
    this.intervalId = setInterval(() => {
      this.elapsedSeconds.update(s => s + 1);
    }, 1000);
  }

  formatElapsed(): string {
    const total = this.elapsedSeconds();
    const h = Math.floor(total / 3600);
    const m = Math.floor((total % 3600) / 60);
    const s = total % 60;
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${pad(h)}:${pad(m)}:${pad(s)}`;
  }

  isActiveTask(taskId: string): boolean {
    return this.activeTaskId() === taskId;
  }
}