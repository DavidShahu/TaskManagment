import { Component, HostListener ,ElementRef} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Auth } from '../../core/auth/auth';

import { Router } from '@angular/router';
import { LayoutService } from '../../core/services/layout.service';
import { TimerService } from '../../core/services/timer.service';
import { RouterModule } from '@angular/router';

import { NotificationService } from '../../core/services/notification.service';
@Component({
  selector: 'app-topbar',
  standalone:true,
  imports: [CommonModule, RouterModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss',
})
export class Topbar {
  user: { email: string, fullName: string, role: string } | null = null;

  constructor(private authService: Auth,
      private elementRef : ElementRef,
      private router : Router,
      public layoutService: LayoutService,
      public timerService: TimerService ,
      public notificationService : NotificationService
  ) {
    this.user = this.authService.getUser();
  }


  dropdownOpen = false;
  notificationPanelOpen = false;


  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }
 

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }


  get isCollapsed(): boolean {
    return this.layoutService.isCollapsed();
  }


  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
      this.notificationPanelOpen = false;
    }
  }

   toggleNotifications(): void {
    this.notificationPanelOpen = !this.notificationPanelOpen;
    if (this.notificationPanelOpen) {
      this.dropdownOpen = false;
      this.notificationService.markAllAsRead();
    }
  }


  formatTime(date: string): string {
    const d = new Date(date);
    const now = new Date();
    const diff = now.getTime() - d.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes}m ago`;
    if (hours < 24) return `${hours}h ago`;
    return `${days}d ago`;
  }
}
