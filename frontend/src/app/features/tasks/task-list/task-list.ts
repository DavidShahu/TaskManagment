import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { TaskService, Task } from '../../../core/services/task.service';
import { Auth } from '../../../core/auth/auth';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import { TaskForm } from '../task-form/task-form';

@Component({
  selector: 'app-task-list',
  imports: [CommonModule, RouterModule, TaskForm],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
})
export class TaskList implements OnInit {
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  isLoading = true;
  isAdmin = false;
  showForm = false;
  selectedTask: Task | null = null;
  statusFilter: string = 'all';
  isMyTasksView = false;
  currentUserId = '';
  constructor(
    private taskService: TaskService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.currentUserId = this.authService.getUser()?.id ?? '';

    debugger 
    this.isMyTasksView = this.route.snapshot.routeConfig?.path === 'tasks/my';

    this.loadTasks();
  }

  get pageTitle(): string {
    if (this.isMyTasksView) return 'My Tasks';
    if (this.isAdmin) return 'All Tasks';
    return 'My Tasks';
  }


  loadTasks(): void {
    this.isLoading = true;

     
    const tasks$ = this.isMyTasksView || !this.isAdmin
      ? this.taskService.getMyTasks()
      : this.taskService.getAll();

    tasks$.subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to load tasks');
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    this.filteredTasks = this.statusFilter === 'all'
      ? this.tasks
      : this.tasks.filter(t =>
          t.status.toLowerCase() === this.statusFilter);
  }

  setFilter(filter: string): void {
    this.statusFilter = filter;
    this.applyFilter();
  }

  openCreate(): void {
    this.selectedTask = null;
    this.showForm = true;
  }

  openEdit(task: Task): void {
    this.selectedTask = task;
    this.showForm = true;
  }

  onFormClose(saved: boolean): void {
    this.showForm = false;
    if (saved) {
      this.toast.success(
        this.selectedTask
          ? 'Task updated successfully'
          : 'Task created successfully');
      this.loadTasks();
    }
  }

  toggleStatus(task: Task): void {
    const action$ = task.status === 'Open'
      ? this.taskService.markAsDone(task.id)
      : this.taskService.markAsOpen(task.id);

    action$.subscribe({
      next: () => {
        this.toast.success(`Task marked as ${task.status === 'Open' ? 'Done' : 'Open'}`);
        this.loadTasks();
      },
      error: (err) => this.toast.error(err.message)
    });
  }

  delete(task: Task): void {
    this.swal.confirm(
      'Delete Task',
      `Are you sure you want to delete "${task.title}"?`,
      'Delete'
    ).then(result => {
      if (result.isConfirmed) {
        this.taskService.delete(task.id).subscribe({
          next: () => {
            this.toast.success('Task deleted successfully');
            this.loadTasks();
          },
          error: (err) => this.toast.error(err.message, 'Failed to delete')
        });
      }
    });
  }

  getOpenCount(): number {
    return this.tasks.filter(t => t.status === 'Open').length;
  }

  getDoneCount(): number {
    return this.tasks.filter(t => t.status === 'Done').length;
  }

  getOverdueCount(): number {
    return this.tasks.filter(t => t.isOverdue).length;
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }
}
