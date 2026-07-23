import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout';

export const routes: Routes = [
  // Rota do Login isolada (Sem Header)
  { 
    path: 'login', 
    loadComponent: () => import('./pages/login/login').then(m => m.Login) // Ou o caminho correto do seu login
  },

  // Área Autenticada com Layout (Com Header + Busca de Usuário)
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { 
        path: 'Home', 
        loadComponent: () => import('./pages/home/home').then(m => m.Home) 
      },
      { 
        path: 'User', 
        loadComponent: () => import('./pages/user/user').then(m => m.UsersPage) 
      },
      { 
        path: '', 
        redirectTo: 'Home', 
        pathMatch: 'full' 
      }
    ]
  },

  { 
    path: '**', 
    redirectTo: 'login' 
  }
];