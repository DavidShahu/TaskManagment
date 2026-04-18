import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { authGuard } from './core/guards/auth-guard';
import { Dashboard } from './features/dashboard/dashboard';
import { MainLayout } from './layout/main-layout/main-layout';


export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'projects', loadComponent: () => import('./features/projects/project-list/project-list').then(m => m.ProjectList) },
      { path: 'projects/:id', loadComponent: () => import('./features/projects/project-detail/project-detail').then(m => m.ProjectDetail) },
     
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
   }
];
