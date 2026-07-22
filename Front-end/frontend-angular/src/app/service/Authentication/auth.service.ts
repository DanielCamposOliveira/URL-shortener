// auth.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

// Interface que define o corpo da requisição que a API 2 espera
export interface LoginRequest {
  email: string;
  password: string; // Nota: o backend espera a propriedade como 'password'
}

// Interface que define a resposta bem-sucedida da API (conforme o documento)
export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root' // Torna o serviço global para toda a aplicação
})
export class AuthService {
  // Injeta o cliente HTTP do Angular para realizar requisições web
  private http = inject(HttpClient); 

  // URL base da API 2 para o endpoint de autenticação
  private apiUrl = 'http://localhost:5000/api/v1/auth/login';

 // Um Signal para expor o estado de autenticação (se o usuário possui um token válido)
    tokenUsuario = signal<string | null>(localStorage.getItem('jwt_token'));

  // Método que realiza a tentativa de login enviando as credenciais para o backend
  efetuarLogin(credenciais: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, credenciais).pipe(
      // O operador 'tap' executa um efeito colateral assim que a API responde com sucesso
      tap((resposta) => {
        // Guarda o token JWT retornado no armazenamento local do navegador
        localStorage.setItem('jwt_token', resposta.token);
        
        // Atualiza o Signal reativo com o novo token recebido
        this.tokenUsuario.set(resposta.token);
      })
    );   
}

  
  // Método que verifica se o usuário está autenticado com base na presença de um token JWT
  estaAutenticado(): boolean {
    // Verifica se existe um token armazenado no localStorage. Se existir, o usuário está autenticado.
    const token = localStorage.getItem('jwt_token');
    return !!token;
  }

  // Método auxiliar para deslogar do sistema
  logout(): void {
    localStorage.removeItem('jwt_token');
    this.tokenUsuario.set(null);
  }
}