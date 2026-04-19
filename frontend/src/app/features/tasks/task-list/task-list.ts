import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { TaskService, Task } from '../../../core/services/task.service';
import { Auth } from '../../../core/auth/auth';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import { TaskForm } from '../task-form/task-form';
import { User, UserService } from '../../../core/services/user.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-list',
  imports: [CommonModule, RouterModule, FormsModule, TaskForm],
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

   
  users: User[] = [];
  dateFrom = '';
  dateTo = '';
  selectedUserId = '';
  showFilters = false;

  openCount = 0;
  doneCount = 0;
  overdueCount = 0;
  
  totalCount = 0;
  constructor(
    private taskService: TaskService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService,
    private route: ActivatedRoute,
    private userService: UserService,
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.currentUserId = this.authService.getUser()?.id ?? '';

    debugger 
    this.isMyTasksView = this.route.snapshot.routeConfig?.path === 'tasks/my';

    this.loadTasks();
    
    if (this.isAdmin && !this.isMyTasksView) {
      this.loadUsers();
    }
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
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to load tasks');
        this.isLoading = false;
      }
    });
  }


  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => this.users = users.filter(u => u.isActive)
    });
  }
 

  applyFilters(): void {
    let baseResult = [...this.tasks];
  
    if (this.dateFrom) {
      const from = new Date(this.dateFrom);
      from.setHours(0, 0, 0, 0);
      baseResult = baseResult.filter(t =>
        new Date(t.createdAt) >= from);
    }
  
    if (this.dateTo) {
      const to = new Date(this.dateTo);
      to.setHours(23, 59, 59, 999);
      baseResult = baseResult.filter(t =>
        new Date(t.createdAt) <= to);
    }
  
    if (this.isAdmin && this.selectedUserId) {
      baseResult = baseResult.filter(t =>
        t.ownerId === this.selectedUserId);
    }
  
    // Counts from base result (before status filter)
    this.openCount = baseResult
      .filter(t => t.status === 'Open').length;
    this.doneCount = baseResult
      .filter(t => t.status === 'Done').length;
    this.overdueCount = baseResult
      .filter(t => t.isOverdue).length;
    this.totalCount = baseResult.length; // ← total for this filter set
  
    // Apply status filter for display
    if (this.statusFilter !== 'all') {
      baseResult = baseResult.filter(t =>
        t.status.toLowerCase() === this.statusFilter);
    }
  
    this.filteredTasks = baseResult;
  }

  setStatusFilter(filter: string): void {
    this.statusFilter = filter;
    this.applyFilters();
  }

  clearFilters(): void {
    this.dateFrom = '';
    this.dateTo = '';
    this.selectedUserId = '';
    this.statusFilter = 'all';
    this.applyFilters();
  }

  get hasActiveFilters(): boolean {
    return !!this.dateFrom ||
      !!this.dateTo ||
      !!this.selectedUserId ||
      this.statusFilter !== 'all';
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
 
  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }


 

}
