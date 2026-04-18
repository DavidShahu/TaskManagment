import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LayoutService {
  isCollapsed = signal(false);

  toggle(): void {
    this.isCollapsed.set(!this.isCollapsed());
  }

  setCollapsed(value: boolean): void {
    this.isCollapsed.set(value);
  }
}