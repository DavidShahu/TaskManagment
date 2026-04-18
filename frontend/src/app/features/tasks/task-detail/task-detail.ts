import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TaskService, Task, LogTimeRequest } from '../../../core/services/task.service';
import { Auth } from '../../../core/auth/auth';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import { TaskForm } from '../task-form/task-form';
import { TimerService } from '../../../core/services/timer.service';


@Component({
  selector: 'app-task-detail',
  imports: [CommonModule, RouterModule, FormsModule, TaskForm],
  templateUrl: './task-detail.html',
  styleUrl: './task-detail.scss',
})
export class TaskDetail {

  task: Task | null = null;
  isLoading = true;
  isAdmin = false;
  showEditForm = false;
  errorMessage = '';

  // Time logging
  logTimeForm: LogTimeRequest = { hours: 0, note: '' };
  isLoggingTime = false;
  currentUserId: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService,
    public timerService: TimerService 
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.currentUserId = this.authService.getUser()?.id ?? '';
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadTask(id);
  }

  loadTask(id: string): void {
    this.isLoading = true;
    this.taskService.getById(id).subscribe({
      next: (task) => {
        this.task = task;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isLoading = false;
      }
    });
  }

  toggleStatus(): void {
    if (!this.task) return;

    const action$ = this.task.status === 'Open'
      ? this.taskService.markAsDone(this.task.id)
      : this.taskService.markAsOpen(this.task.id);

    action$.subscribe({
      next: () => {
        this.toast.success(
          `Task marked as ${this.task?.status === 'Open' ? 'Done' : 'Open'}`);
        this.loadTask(this.task!.id);
      },
      error: (err) => this.toast.error(err.message)
    });
  }

  delete(): void {
    if (!this.task) return;

    this.swal.confirm(
      'Delete Task',
      `Are you sure you want to delete "${this.task.title}"?`,
      'Delete'
    ).then(result => {
      if (result.isConfirmed) {
        this.taskService.delete(this.task!.id).subscribe({
          next: () => {
            this.toast.success('Task deleted successfully');
            this.router.navigate(['/tasks']);
          },
          error: (err) => this.toast.error(err.message, 'Failed to delete')
        });
      }
    });
  }

  onEditClose(saved: boolean): void {
    this.showEditForm = false;
    if (saved) {
      this.toast.success('Task updated successfully');
      this.loadTask(this.task!.id);
    }
  }

  logTime(): void {
    if (!this.task || !this.logTimeForm.hours) return;

    this.isLoggingTime = true;
    this.taskService.logTime(this.task.id, this.logTimeForm).subscribe({
      next: () => {
        this.toast.success(`${this.logTimeForm.hours}h logged successfully`);
        this.logTimeForm = { hours: 0, note: '' };
        this.isLoggingTime = false;
        this.loadTask(this.task!.id);
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to log time');
        this.isLoggingTime = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tasks']);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  formatDateTime(date: string): string {
    return new Date(date).toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  get progressWidth(): string {
    return Math.min(this.task?.progressPercentage ?? 0, 100) + '%';
  }

  get progressColor(): string {
    const pct = this.task?.progressPercentage ?? 0;
    if (pct >= 100) return 'bg-success';
    if (pct >= 75) return 'bg-warning';
    return 'bg-primary';
  }



  startTimer(): void {
    this.timerService.start(this.task!.id);
    this.toast.info('Timer started! Come back when you\'re done.');
  }

  async stopTimer(): Promise<void> {
    const hours = await this.timerService.stop();

    if (hours < 0.01) {
      this.toast.warning('Less than 1 minute — nothing logged');
      return;
    }

    this.logTimeForm.hours = hours;
    this.toast.success(`Timer stopped! ${hours}h added to log form`);
  }


  stopOtherTimer(): void {
    this.swal.confirm(
      'Stop Timer',
      'This will stop the timer running on another task. Continue?',
      'Stop Timer',
      'warning'
    ).then(result => {
      if (result.isConfirmed) {
        this.timerService.stop();
        this.toast.info('Timer stopped');
      }
    });

  }
}
