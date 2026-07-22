import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { UsersPage } from './pages/user/user';
import { authGuard } from './service/auth.guard';

export const routes: Routes = [
    // 1. Rota de Login: É pública. Se o usuário digitar /login, ele entra direto.
  { 
    path: 'login', 
    component: Login 
  },

  // 2. Rota Home (Página Principal): Protegida pelo Nosso Guard.
  // Colocando o 'canActivate: [authGuard]', o Angular só deixa entrar se o Guard retornar true.
  { 
    path: 'Home', 
    component: Home, 
    canActivate: [authGuard] 
  },

  // 3. Rota Padrão (Vazia): Quando o usuário digita apenas "http://localhost:4200/"
  // Se ele tiver token, o Guard da Home vai validar. Se não tiver, o Guard joga pro login.
  { 
    path: 'User', 
    component: UsersPage, 
    canActivate: [authGuard] 
  },

  // 4. Rota Curinga: Caso o usuário digite qualquer endereço que não existe, manda de volta pra Home
  { 
    path: '**', 
    redirectTo: 'Home' 
  }
];
