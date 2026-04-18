import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService, Task, CreateTaskRequest } from '../../../core/services/task.service';
import { ProjectService, Project } from '../../../core/services/project.service';
import { UserService, User } from '../../../core/services/user.service';
import { Auth } from '../../../core/auth/auth';
import { ToastService } from '../../../core/services/toast.service';
import { TaskTypeService, TaskType } from '../../../core/services/task-type.service';


@Component({
  selector: 'app-task-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss',
})
export class TaskForm implements OnInit {
  @Input() task: Task | null = null;
  @Output() close = new EventEmitter<boolean>();

  form: CreateTaskRequest = {
    title: '',
    description: '',
    dueDate: undefined,
    projectId: undefined,
    estimatedHours: undefined,
    assignedToUserId: undefined,
    taskTypeId: undefined
  };

  projects: Project[] = [];
  allUsers: User[] = [];
  filteredUsers: User[] = [];
  isLoading = false;
  isAdmin = false;
  taskTypes: TaskType[] = [];

  constructor(
    private taskService: TaskService,
    private projectService: ProjectService,
    private userService: UserService,
    private authService: Auth,
    private toast: ToastService,
    private taskTypeService: TaskTypeService,
  ) {}


  @Input() preselectedProjectId: string | undefined = undefined;


  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();

    if (this.task) {
      this.form = {
        title: this.task.title,
        description: this.task.description,
        dueDate: this.task.dueDate
          ? new Date(this.task.dueDate).toISOString().split('T')[0]
          : undefined,
        estimatedHours: this.task.estimatedHours,
        projectId: this.task.projectId,
        assignedToUserId: this.task.ownerId,
        taskTypeId: this.task.taskTypeId 
      };
    }

    this.loadProjects();
    if (this.isAdmin) this.loadUsers();

    if (this.preselectedProjectId && !this.isEdit) {
      this.form.projectId = this.preselectedProjectId;
    }
    this.loadTaskTypes();

  }

  loadProjects(): void {
    this.projectService.getAll().subscribe({
      next: (projects) => this.projects = projects.filter(p => p.isActive)
    });
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        this.allUsers = users.filter(u => u.isActive);
        // Initialize filtered users based on current project
        this.updateFilteredUsers();
      }
    });
  }

  onProjectChange(): void {
    // Reset assigned user when project changes
    this.form.assignedToUserId = undefined;
    this.updateFilteredUsers();
  }

  updateFilteredUsers(): void {
    debugger
    if (!this.form.projectId) {
      // No project selected — show all users
      this.filteredUsers = this.allUsers;
      return;
    }

    // Find selected project
    const selectedProject = this.projects
      .find(p => p.id === this.form.projectId);

    if (!selectedProject) {
      this.filteredUsers = this.allUsers;
      return;
    }


    // Filter users to only project members
    const memberIds = selectedProject.members.map(m => m.userId);
    this.filteredUsers = this.allUsers
      .filter(u => memberIds.includes(u.id));
  }


  get isEdit(): boolean {
    return !!this.task;
  }

  onSubmit(): void {
    this.isLoading = true;

    const request$ = this.isEdit
      ? this.taskService.update(this.task!.id, {
          title: this.form.title,
          description: this.form.description,
          dueDate: this.form.dueDate,
          estimatedHours: this.form.estimatedHours, 
          taskTypeId: this.form.taskTypeId 

        })
      : this.taskService.create(this.form);

    request$.subscribe({
      next: () => {
        this.isLoading = false;
        this.close.emit(true);
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to save task');
        this.isLoading = false;
      }
    });
  }

  onClose(): void {
    this.close.emit(false);
  }
  loadTaskTypes(): void {
    this.taskTypeService.getAll().subscribe({
      next: (types) => this.taskTypes = types.filter(t => t.isActive)
    });
  }
}
