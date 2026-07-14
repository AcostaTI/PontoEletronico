import { Routes } from '@angular/router';

import { authGuard, visitanteGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [visitanteGuard],
    title: 'Entrar · Ponto Eletrônico',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'cadastro',
    canActivate: [visitanteGuard],
    title: 'Criar conta · Ponto Eletrônico',
    loadComponent: () => import('./features/auth/cadastro/cadastro').then((m) => m.Cadastro),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    children: [
      {
        path: 'dashboard',
        title: 'Hoje · Ponto Eletrônico',
        loadComponent: () =>
          import('./features/ponto/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'espelho',
        title: 'Espelho do mês · Ponto Eletrônico',
        loadComponent: () => import('./features/ponto/espelho/espelho').then((m) => m.Espelho),
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },
  { path: '**', redirectTo: '' },
];
