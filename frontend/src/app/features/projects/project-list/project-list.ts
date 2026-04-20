import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProjectService, Project } from '../../../core/services/project.service';
import { Auth } from '../../../core/auth/auth';
import { ProjectForm } from '../project-form/project-form';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';

@Component({
  selector: 'app-project-list',
  imports: [CommonModule, RouterModule, ProjectForm],
  templateUrl: './project-list.html',
  styleUrl: './project-list.scss',
})
export class ProjectList implements OnInit  {
 projects: Project[] = [];
  isLoading = true;
  isAdmin = false;
  showForm = false;
  selectedProject: Project | null = null;

  constructor(
    private projectService: ProjectService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadProjects();
  }

  loadProjects(): void {
    this.isLoading = true;
    this.projectService.getAll().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  openCreate(): void {
    this.selectedProject = null;
    this.showForm = true;
  }

  openEdit(project: Project): void {
    this.selectedProject = project;
    this.showForm = true;
  }

  onFormClose(saved: boolean): void {
    this.showForm = false;
    if (saved) this.loadProjects();
  }

  delete(id: string): void {
    this.swal.confirm(
      'Delete Project',
      'Are you sure?',
      'Delete'
    ).then(result => {
      if (result.isConfirmed) {
        this.projectService.delete(id).subscribe({
          next: () => {
            this.toast.success('Project deleted successfully');
            this.loadProjects();
          },
          error: (err) => this.toast.error(err.message, 'Failed to delete')
        });
      }
    });
  }
  
  activate(id: string): void {
    this.swal.confirm(
      'Activate Project',
      'Are you sure you want to activate this project?',
      'Activate',
      'question'
    ).then(result => {
      if (result.isConfirmed) {
        this.projectService.activate(id).subscribe({
          next: () => {
            this.toast.success('Project activated successfully');
            this.loadProjects();
          },
          error: (err) => this.toast.error(err.message, 'Failed to activate')
        });
      }
    });
  }

  getActiveCount(): number {
    return this.projects.filter(p => p.isActive).length;
  }



}
