import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.RegisterComponent),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.DashboardComponent),
  },
  {
    path: 'projects',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/projects-list/projects-list').then((m) => m.ProjectsListComponent),
  },
  {
    path: 'projects/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/project-create/project-create').then((m) => m.ProjectCreateComponent),
  },
  {
    path: 'projects/:projectId/tasks',
    canActivate: [authGuard],
    loadComponent: () => import('./features/tasks/tasks-list/tasks-list').then((m) => m.TasksListComponent),
  },
  {
    path: 'projects/:projectId/tasks/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/tasks/task-create/task-create').then((m) => m.TaskCreateComponent),
  },
  { path: '**', redirectTo: 'login' },
];