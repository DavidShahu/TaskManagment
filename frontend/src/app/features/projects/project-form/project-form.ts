import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService, Project, CreateProjectRequest } from '../../../core/services/project.service';

@Component({
  selector: 'app-project-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './project-form.html',
  styleUrl: './project-form.scss',
})
export class ProjectForm implements OnInit {
  @Input() project: Project | null = null;
  @Output() close = new EventEmitter<boolean>();

  form: CreateProjectRequest = { name: '', description: '' };
  isLoading = false;
  errorMessage = '';

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void {
    if (this.project) {
      this.form = {
        name: this.project.name,
        description: this.project.description
      };
    }
  }

  get isEdit(): boolean {
    return !!this.project;
  }

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request$ = this.isEdit
      ? this.projectService.update(this.project!.id, this.form)
      : this.projectService.create(this.form);

    request$.subscribe({
      next: () => {
        this.isLoading = false;
        this.close.emit(true);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Something went wrong';
        this.isLoading = false;
      }
    });
  }

  onClose(): void {
    this.close.emit(false);
  }
}