import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout';
import { AuthService } from './service/Authentication/auth.service';

// Guard exclusivo para as ROTAS (Não confunda com o Interceptor HTTP)
const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.estaAutenticado()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

export const routes: Routes = [
  // Rotas Públicas
  { 
    path: 'login', 
    loadComponent: () => import('./pages/login/login').then(m => m.Login) // Ou o caminho correto do seu login
  },
  { 
    path: 'registre', 
    loadComponent: () => import('./pages/registre-user/registre-user').then(m => m.RegistreUser) 
  },

  // 2. Redirecionamento Inicial Explícito
  // Se o usuário acessar só "localhost:4200/", ele vai para a Home (se logado) ou Login (pelo Guard)
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'Home'
  },

  //Rotas Privadas
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { 
        path: 'Home', 
        loadComponent: () => import('./pages/home/home').then(m => m.Home) 
      },
      { 
        path: 'User', 
        loadComponent: () => import('./pages/user/user').then(m => m.UsersPage) 
      },

    ]
  },

  // Rota Coringa (Redireciona URLs desconhecidas para o login)
  { 
    path: '**', 
    redirectTo: 'login' 
  }
];