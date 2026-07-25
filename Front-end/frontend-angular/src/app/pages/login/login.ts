import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router'; 
import { AuthService } from '../../service/Authentication/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink], // Importado RouterLink aqui
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  email = '';
  senha = '';

  private authService = inject(AuthService);
  private router = inject(Router);

  mensagemErro = signal<string | null>(null);
  estaCarregando = signal<boolean>(false);

  login() {
    if (this.estaCarregando()) return;

    this.mensagemErro.set(null);
    this.estaCarregando.set(true);

    this.authService.Login({
      email: this.email,
      password: this.senha
    }).subscribe({
      next: () => {
        this.estaCarregando.set(false);
        this.router.navigate(['/']); 
      },
      error: (erro) => {
        this.estaCarregando.set(false);
        if (erro.status >= 400 && erro.status < 500) {
          this.mensagemErro.set('E-mail ou senha inválidos.');
        } else {
          this.mensagemErro.set('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
        }
      }
    });
  }


  ngOnInit(): void {
    
    // Remove as classes de tema para que o body volte a respeitar o :root puro
    document.body.classList.remove('light-theme', 'dark-theme');

    if (this.authService.estaAutenticado()) {
      this.router.navigate(['/home']);
    }
  }
}