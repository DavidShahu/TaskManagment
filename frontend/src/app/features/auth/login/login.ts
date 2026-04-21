import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Auth, LoginRequest } from '../../../core/auth/auth';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

  user: { email: string, fullName: string, role: string } | null = null;

  request: LoginRequest = { email: '', password: '' };
  isLoading = false;
  errorMessage = '';

  sessionExpired = false;

  constructor(
    private authService: Auth,
    private router: Router, 
    private route: ActivatedRoute
  ) {

    this.user = authService.getUser();

  }

  ngOnInit():void{
    debugger
    this.route.queryParams.subscribe(params => {
      if (params['reason'] === 'session_expired') {
        this.sessionExpired = true;
      }
    });

    
    if(this.user){
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.request).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Invalid email or password or your account has been deactivated';
        this.isLoading = false;
      }
    });
  }

}
