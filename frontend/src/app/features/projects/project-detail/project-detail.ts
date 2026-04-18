import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProjectService, Project } from '../../../core/services/project.service'; 
import { Auth } from '../../../core/auth/auth';
import { FormsModule } from '@angular/forms';
import { User, UserService } from '../../../core/services/user.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import { ToastService } from '../../../core/services/toast.service';
import { TaskForm} from '../../tasks/task-form/task-form';
import { TaskService, Task } from '../../../core/services/task.service';


@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterModule,FormsModule, TaskForm],
  templateUrl: './project-detail.html',
  styleUrl: './project-detail.scss',
})


export class ProjectDetail  implements OnInit {
  project: Project | null = null;
  availableUsers: User[] = [];
  isLoading = true;
  isAdmin = false;
  errorMessage = '';
  showAddMember = false;
  selectedUserId = '';
  isLoadingTasks = false; 
  tasks: Task[] = [];
  currentUserId = ''; 
  showTaskForm = false;
  selectedTask: Task | null = null;
  statusFilter = 'all'; 
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private userService: UserService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService,
    private taskService: TaskService,
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.currentUserId = this.authService.getUser()?.id ?? '';

    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadProject(id);
  }

  loadProject(id: string): void {
    this.isLoading = true;
    this.projectService.getById(id).subscribe({
      next: (project) => {
        this.project = project;
        this.isLoading = false;
        this.loadTasks();
        if (this.isAdmin) this.loadAvailableUsers();
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isLoading = false;
      }
    });
  }

  loadAvailableUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        // Filter out users already in project
        const memberIds = this.project?.members
          .map(m => m.userId) ?? [];
        this.availableUsers = users.filter(
          u => !memberIds.includes(u.id));
      }
    });
  }

  addMember(): void {
    if (!this.selectedUserId || !this.project) return;

    this.projectService
      .addMember(this.project.id, this.selectedUserId)
      .subscribe({
        next: () => {
          this.selectedUserId = '';
          this.showAddMember = false;
          this.loadProject(this.project!.id);
        },
        error: (err) => this.errorMessage = err.message
      });
  }

  removeMember(userId: string, memberName: string): void {
    this.swal.confirm(
      'Remove Member',
      `Are you sure you want to remove ${memberName} from this project?`,
      'Remove'
    ).then(result => {
      if (result.isConfirmed) {
        this.projectService
          .removeMember(this.project!.id, userId)
          .subscribe({
            next: () => {
              this.toast.success('Member removed successfully');
              this.loadProject(this.project!.id);
            },
            error: (err) => this.toast.error(err.message, 'Failed to remove')
          });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }


  loadTasks(): void {
    if (!this.project) return;
    this.isLoadingTasks = true;
    this.taskService.getByProject(this.project.id).subscribe({
      next: (tasks) => {
        this.tasks = this.isAdmin
        ? tasks
        : tasks.filter(t => t.ownerId === this.currentUserId);
        this.isLoadingTasks = false;
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to load tasks');
        this.isLoadingTasks = false;
      }
    });
  }

    get filteredTasks(): Task[] {
    if (this.statusFilter === 'all') return this.tasks;
    return this.tasks.filter(
      t => t.status.toLowerCase() === this.statusFilter);
  }

  get openCount(): number {
    return this.tasks.filter(t => t.status === 'Open').length;
  }

  get doneCount(): number {
    return this.tasks.filter(t => t.status === 'Done').length;
  }

  openCreateTask(): void {
    this.selectedTask = null;
    this.showTaskForm = true;
  }

  openEditTask(task: Task, event: Event): void {
    event.stopPropagation();
    this.selectedTask = task;
    this.showTaskForm = true;
  }

  onTaskFormClose(saved: boolean): void {
    this.showTaskForm = false;
    if (saved) {
      this.toast.success(
        this.selectedTask
          ? 'Task updated successfully'
          : 'Task created successfully');
      this.loadTasks();
    }
  }

  toggleTaskStatus(task: Task, event: Event): void {
    event.stopPropagation();
    const action$ = task.status === 'Open'
      ? this.taskService.markAsDone(task.id)
      : this.taskService.markAsOpen(task.id);

    action$.subscribe({
      next: () => {
        this.toast.success(
          `Task marked as ${task.status === 'Open' ? 'Done' : 'Open'}`);
        this.loadTasks();
      },
      error: (err) => this.toast.error(err.message)
    });
  }

  deleteTask(task: Task, event: Event): void {
    event.stopPropagation();
    this.swal.confirm(
      'Delete Task',
      `Are you sure you want to delete "${task.title}"?`,
      'Delete'
    ).then(result => {
      if (result.isConfirmed) {
        this.taskService.delete(task.id).subscribe({
          next: () => {
            this.toast.success('Task deleted');
            this.loadTasks();
          },
          error: (err) => this.toast.error(err.message)
        });
      }
    });
  }

    navigateToTask(taskId: string): void {
    this.router.navigate(['/tasks', taskId]);
  }

}
