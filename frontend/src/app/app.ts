import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Auth } from './core/auth/auth';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
// export class App {
//   protected readonly title = signal('frontend');
// }
export class App implements OnInit {
  constructor(
    private authService: Auth,
    private notificationService: NotificationService
  ) {}
  protected readonly title = signal('TaskManagment');
  ngOnInit(): void { 
    if (this.authService.isLoggedIn()) {
      const token = this.authService.getToken();
    if (token) this.notificationService.initialize(token);
  }
  }
}