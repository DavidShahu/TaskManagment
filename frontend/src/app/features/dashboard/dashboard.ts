import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TaskService, Task } from '../../core/services/task.service';
import { ProjectService, Project } from '../../core/services/project.service';
import { Auth } from '../../core/auth/auth';
import { ToastService } from '../../core/services/toast.service';
import { TaskForm } from '../tasks/task-form/task-form';
@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterModule, TaskForm],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  tasks: Task[] = [];
  projects: Project[] = [];
  isLoading = true;
  isAdmin = false;
  showTaskForm = false;

  constructor(
    private taskService: TaskService,
    private projectService: ProjectService,
    private authService: Auth,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;

    // Load tasks
    const tasks$ = this.isAdmin
      ? this.taskService.getAll()
      : this.taskService.getMyTasks();

    tasks$.subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.checkLoading();
      },
      error: (err) => {
        this.toast.error(err.message);
        this.checkLoading();
      }
    });

    // Load projects
    this.projectService.getAll().subscribe({
      next: (projects) => {
        this.projects = projects.filter(p => p.isActive);
        this.checkLoading();
      },
      error: () => this.checkLoading()
    });
  }

  private loadedCount = 0;
  checkLoading(): void {
    this.loadedCount++;
    if (this.loadedCount >= 2) {
      this.isLoading = false;
      this.loadedCount = 0;
    }
  }

  // Stats
  get myTasksCount(): number {
    return this.isAdmin
      ? this.tasks.length
      : this.tasks.filter(t => t.status === 'Open').length;
  }

  get inProgressCount(): number {
    return this.tasks.filter(t =>
      t.status === 'Open' && t.loggedHours > 0).length;
  }

  get projectsCount(): number {
    return this.projects.length;
  }

  get dueSoonCount(): number {
    const threeDays = new Date();
    threeDays.setDate(threeDays.getDate() + 3);
    return this.tasks.filter(t =>
      t.status === 'Open' &&
      t.dueDate &&
      new Date(t.dueDate) <= threeDays &&
      !t.isOverdue
    ).length;
  }

  get overdueCount(): number {
    return this.tasks.filter(t => t.isOverdue).length;
  }

  get recentTasks(): Task[] {
    return [...this.tasks]
      .sort((a, b) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 5);
  }

  get recentProjects(): Project[] {
    return [...this.projects]
      .sort((a, b) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 3);
  }

  onTaskFormClose(saved: boolean): void {
    this.showTaskForm = false;
    if (saved) {
      this.toast.success('Task created successfully');
      this.loadData();
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric'
    });
  }

}
