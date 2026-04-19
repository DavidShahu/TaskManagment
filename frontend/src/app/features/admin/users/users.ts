import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, User } from '../../../core/services/user.service';
import { ToastService } from '../../../core/services/toast.service';
import { SweetAlertService } from '../../../core/services/sweet-alert.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-users',
  imports: [CommonModule, FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.scss',
})
export class Users {

  users: User[] = [];
  filteredUsers: User[] = [];
  isLoading = true;
  searchQuery = '';
  roleFilter = 'all';

  constructor(
    private userService: UserService,
    private toast: ToastService,
    private swal: SweetAlertService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        this.toast.error(err.message, 'Failed to load users');
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredUsers = this.users.filter(u => {
      const matchesSearch =
        !this.searchQuery ||
        u.firstName.toLowerCase()
          .includes(this.searchQuery.toLowerCase()) ||
        u.lastName.toLowerCase()
          .includes(this.searchQuery.toLowerCase()) ||
        u.email.toLowerCase()
          .includes(this.searchQuery.toLowerCase());

      const matchesRole =
        this.roleFilter === 'all' ||
        u.role.toLowerCase() === this.roleFilter;

      return matchesSearch && matchesRole;
    });
  }

  toggleStatus(user: User): void {
    const action = user.isActive ? 'deactivate' : 'activate';

    this.swal.confirm(
      `${user.isActive ? 'Deactivate' : 'Activate'} User`,
      `Are you sure you want to ${action} ${user.firstName} ${user.lastName}?`,
      user.isActive ? 'Deactivate' : 'Activate',
      user.isActive ? 'warning' : 'question'
    ).then(result => {
      if (result.isConfirmed) {
        this.userService.updateStatus(user.id, !user.isActive).subscribe({
          next: () => {
            this.toast.success(
              `User ${action}d successfully`);
            this.loadUsers();
          },
          error: (err) => this.toast.error(err.message)
        });
      }
    });
  }

  resetPassword(user: User): void {
    Swal.fire({
      title: 'Reset Password',
      html: `
        <p class="text-muted mb-3">
          Set a new password for <strong>${user.firstName} ${user.lastName}</strong>
        </p>
        <input
          id="new-password"
          type="password"
          class="swal2-input"
          placeholder="New password (min 8 characters)">
        <input
          id="confirm-password"
          type="password"
          class="swal2-input"
          placeholder="Confirm password">
      `,
      showCancelButton: true,
      confirmButtonText: 'Reset Password',
      confirmButtonColor: '#667eea',
      cancelButtonColor: '#718096',
      preConfirm: () => {
        const newPassword = (document.getElementById('new-password') as HTMLInputElement).value;
        const confirmPassword = (document.getElementById('confirm-password') as HTMLInputElement).value;

        if (!newPassword || newPassword.length < 8) {
          Swal.showValidationMessage('Password must be at least 8 characters');
          return false;
        }

        if (newPassword !== confirmPassword) {
          Swal.showValidationMessage('Passwords do not match');
          return false;
        }

        return newPassword;
      }
    }).then(result => {
      if (result.isConfirmed && result.value) {
        this.userService.resetPassword(user.id, result.value)
          .subscribe({
            next: () => this.toast.success(
              `Password reset successfully for ${user.firstName}`),
            error: (err) => this.toast.error(err.message)
          });
      }
    });
  }

  getActiveCount(): number {
    return this.users.filter(u => u.isActive).length;
  }

  getAdminCount(): number {
    return this.users.filter(u => u.role === 'Admin').length;
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

}
