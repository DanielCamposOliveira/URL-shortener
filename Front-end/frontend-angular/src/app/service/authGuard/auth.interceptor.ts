import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../Authentication/auth.service'; // Ajuste o caminho do seu serviço

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Injeta as ferramentas necessárias dentro do interceptor funcional
  const router = inject(Router);
  const authService = inject(AuthService);

  // Busca o token salvo no navegador
  const token = localStorage.getItem('jwt_token');

  // Criamos uma cópia modificada da requisição original
  let requisicaoModificada = req;

  // Se o token existir, nós o injetamos no cabeçalho (Header) Authorization de forma automática
  if (token) {
    requisicaoModificada = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}` // O padrão JWT exige o prefixo 'Bearer '
      }
    });
  }

  // Envia a requisição adiante e fica "escutando" a resposta ou possíveis erros da API
  return next(requisicaoModificada).pipe(
    catchError((erro: any) => {
      // Verifica se o erro veio da resposta HTTP da sua API .NET
      if (erro instanceof HttpErrorResponse) {
        // Se a API retornar 401 (Token Expirado, Inválido ou Manipulado)
        if (erro.status === 401) {
          console.warn('Sessão expirada ou token inválido. Redirecionando para o login...');
          
          // Executa o método de logout para limpar o localStorage e zerar os Signals
          authService.logout();
          
          // Expulsa o usuário para a tela de login
          router.navigate(['/login']);
        }
      }
      
      // Repassa o erro para que os componentes (como a tela de login) também possam tratá-lo se quiserem
      return throwError(() => erro);
    })
  );
};