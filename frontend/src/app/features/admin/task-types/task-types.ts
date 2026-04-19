import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskTypeService, TaskType } from '../../../core/services/task-type.service';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-task-types',
  imports: [CommonModule, FormsModule],
  templateUrl: './task-types.html',
  styleUrl: './task-types.scss',
})
export class TaskTypes implements OnInit {
  taskTypes: TaskType[] = [];
  isLoading = true;
  showForm = false;
  selectedType: TaskType | null = null;

  form = {
    name: '', 
    color: '#667eea'
  };

  

  constructor(
    private taskTypeService: TaskTypeService,
    private toast: ToastService,
    private swal: SweetAlertService
  ) {}

  ngOnInit(): void {
    this.loadTaskTypes();
  }

  loadTaskTypes(): void {
    this.isLoading = true;
    this.taskTypeService.getAll().subscribe({
      next: (types) => {
        this.taskTypes = types;
        this.isLoading = false;
      },
      error: (err) => {
        this.toast.error(err.message);
        this.isLoading = false;
      }
    });
  }

  openCreate(): void {
    this.selectedType = null;
     this.showForm = true;
  }

  openEdit(type: TaskType): void {
    this.selectedType = type;
    this.form = {
      name: type.name, 
      color: type.color
    };
    this.showForm = true;
  }

 

  onSubmit(): void {
    if (!this.form.name ) {
      this.toast.warning('Name and icon are required');
      return;
    }

    const request$ = this.selectedType
      ? this.taskTypeService.update(this.selectedType.id, this.form)
      : this.taskTypeService.create(this.form);

    request$.subscribe({
      next: () => {
        this.toast.success(
          this.selectedType
            ? 'Task type updated'
            : 'Task type created');
        this.showForm = false;
        this.loadTaskTypes();
      },
      error: (err) => this.toast.error(err.message)
    });
  }

  toggleStatus(type: TaskType): void {
    const action = type.isActive ? 'deactivate' : 'activate';

    this.swal.confirm(
      `${type.isActive ? 'Deactivate' : 'Activate'} Task Type`,
      `Are you sure you want to ${action} "${type.name}"?`,
      type.isActive ? 'Deactivate' : 'Activate',
      type.isActive ? 'warning' : 'question'
    ).then(result => {
      if (result.isConfirmed) {
        const request$ = type.isActive
          ? this.taskTypeService.delete(type.id)
          : this.taskTypeService.activate(type.id);

        request$.subscribe({
          next: () => {
            this.toast.success(`Task type ${action}d successfully`);
            this.loadTaskTypes();
          },
          error: (err) => this.toast.error(err.message)
        });
      }
    });
  }

  get isEdit(): boolean {
    return !!this.selectedType;
  }
}