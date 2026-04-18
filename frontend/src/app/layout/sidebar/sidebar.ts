import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Auth} from '../../core/auth/auth';
import { Router } from '@angular/router';
import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  user: { email: string, fullName: string, role: string } | null = null;
 

  constructor(
    private authService: Auth,
    private router: Router,
    public layoutService: LayoutService
  ) {

    this.user = this.authService.getUser();
  }


  menuItems = [
    { label: 'Dashboard', icon: '🏠', route: '/dashboard' },
    { label: 'My Tasks', icon: '✅', route: '/tasks' },
    { label: 'Projects', icon: '📁', route: '/projects' },
  ];

  adminItems = [
    { label: 'All Tasks', icon: '📋', route: '/admin/tasks' },
    { label: 'Users', icon: '👥', route: '/admin/users' },
  ];

  

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  // logout(): void {
  //   this.authService.logout();
  //   this.router.navigate(['/login']);
  // }

 @HostListener('window:resize')
  onResize(): void {
    this.layoutService.setCollapsed(window.innerWidth < 768);
  }

  toggle(): void {
    this.layoutService.toggle();
  }

  get isCollapsed(): boolean {
    return this.layoutService.isCollapsed();
  }

  ngOnInit(): void {
    this.layoutService.setCollapsed(window.innerWidth < 768);
  }

}
