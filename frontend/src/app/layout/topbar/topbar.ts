import { Component, HostListener ,ElementRef} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Auth } from '../../core/auth/auth';

import { Router } from '@angular/router';
import { LayoutService } from '../../core/services/layout.service';


@Component({
  selector: 'app-topbar',
  standalone:true,
  imports: [CommonModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss',
})
export class Topbar {
  user: { email: string, fullName: string, role: string } | null = null;

  constructor(private authService: Auth,
      private elementRef : ElementRef,
      private router : Router,
      public layoutService: LayoutService
  ) {
    this.user = this.authService.getUser();
  }


  dropdownOpen = false;
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
    }
  }
}
