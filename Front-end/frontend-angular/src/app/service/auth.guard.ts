import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router'; 
import { AuthService } from './Authentication/auth.service';

// O authGuard é uma função que implementa a interface CanActivateFn do Angular.
// Ele é usado para proteger rotas, permitindo o acesso apenas a usuários autenticados.
// Se o usuário não estiver autenticado, ele será redirecionado para a página de login.
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Se o usuário estiver autenticado, permite o acesso à rota
  if (authService.estaAutenticado()) {
    return true;
  }

  // Se o usuário não estiver autenticado, redireciona para a página de login
  router.navigate(['/login']);
  return false;
};