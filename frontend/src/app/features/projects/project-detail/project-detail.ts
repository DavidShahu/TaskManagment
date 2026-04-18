import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProjectService, Project } from '../../../core/services/project.service'; 
import { Auth } from '../../../core/auth/auth';
import { FormsModule } from '@angular/forms';
import { User, UserService } from '../../../core/services/user.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import { ToastService } from '../../../core/services/toast.service';


@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterModule,FormsModule],
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private userService: UserService,
    private authService: Auth,
    private toast: ToastService,
    private swal: SweetAlertService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadProject(id);
  }

  loadProject(id: string): void {
    this.isLoading = true;
    this.projectService.getById(id).subscribe({
      next: (project) => {
        this.project = project;
        this.isLoading = false;
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
}
