// login.ts
import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'; // Importado para redirecionar após o login bem-sucedido
import { AuthService } from '../../service/Authentication/auth.service'; // Ajuste o caminho conforme sua estrutura

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})

export class Login implements OnInit {
  // Dados de entrada que permanecem vinculados ao formulário (Two-way data binding)
  email = '';
  senha = '';

  // Injeção do serviço de autenticação recém-criado
  private authService = inject(AuthService);
  
  // Injeção do roteador do Angular para trocar de tela após autenticar
  private router = inject(Router);

  // Signals para controlar mensagens de erro e estados de carregamento diretamente na view
  mensagemErro = signal<string | null>(null);
  estaCarregando = signal<boolean>(false);

  // Método acionado pelo evento (ngSubmit) do formulário
  login() {
    // Evita múltiplas requisições se o usuário clicar duas vezes rapidamente
    if (this.estaCarregando()) return;

    // Reseta erros anteriores e ativa o indicador de carregamento
    this.mensagemErro.set(null);
    this.estaCarregando.set(true);

    // Dispara a requisição montando o objeto de acordo com a interface mapeada do backend
    this.authService.efetuarLogin({
      email: this.email,
      password: this.senha // Mapeia o campo local 'senha' para a propriedade 'password' esperada pelo backend
    }).subscribe({
      next: (resposta) => {
        // Executado se a API retornar Status 200 OK
        this.estaCarregando.set(false);
               
        // Redireciona o usuário para a página principal ou tela de listagem de URLs
        this.router.navigate(['/']); 
      },
      error: (erro) => {
        // Executado em caso de falhas de rede, dados incorretos ou erro de servidor
        this.estaCarregando.set(false);
        
      if (erro.status >= 400 && erro.status < 500) {
        this.mensagemErro.set('E-mail ou senha inválidos.');
      } else {
        this.mensagemErro.set('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
      }
        
      }
    });

    
  }

// Este método roda automaticamente assim que a tela de login tenta ser carregada
  ngOnInit(): void {
    // Se o usuário já tiver um token válido salvo, manda ele direto para a Home
    if (this.authService.estaAutenticado()) {
      this.router.navigate(['/home']);
    }
  }


}