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
    // Empty path + children: the Shell wraps every route listed below it.
    // The guard now sits ONCE here instead of on every individual route.
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell').then((m) => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.DashboardComponent),
      },
      {
        path: 'my-tasks',
        loadComponent: () => import('./features/tasks/my-tasks/my-tasks').then((m) => m.MyTasksComponent),
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/projects/projects-list/projects-list').then((m) => m.ProjectsListComponent),
      },
      {
        path: 'projects/new',
        loadComponent: () =>
          import('./features/projects/project-create/project-create').then((m) => m.ProjectCreateComponent),
      },
      {
  path: 'projects/:id/edit',
  loadComponent: () =>
    import('./features/projects/project-edit/project-edit').then((m) => m.ProjectEditComponent),
},
      {
        path: 'projects/:projectId/tasks',
        loadComponent: () =>
          import('./features/tasks/tasks-list/tasks-list').then((m) => m.TasksListComponent),
      },
      {
  path: 'projects/:projectId/tasks/:taskId/edit',
  loadComponent: () => import('./features/tasks/task-edit/task-edit').then((m) => m.TaskEditComponent),
},
      {
        path: 'projects/:projectId/tasks/new',
        loadComponent: () =>
          import('./features/tasks/task-create/task-create').then((m) => m.TaskCreateComponent),
      },
      {
        path: 'projects/:projectId/tasks/:taskId',
        loadComponent: () =>
          import('./features/tasks/task-details/task-details').then((m) => m.TaskDetailsComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];