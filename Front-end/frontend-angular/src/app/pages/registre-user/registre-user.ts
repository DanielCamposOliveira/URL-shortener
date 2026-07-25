import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {AuthService} from  '../../service/Authentication/auth.service'

@Component({
  selector: 'app-registre-user',
  imports: [FormsModule, RouterLink],
  templateUrl: './registre-user.html',
  styleUrl: './registre-user.css',
})
export class RegistreUser   {


  name = '';
  email = '';
  password = '';

  private authService = inject(AuthService);
  private router = inject(Router);

  mensagemErro = signal<string | null>(null);
  estaCarregando = signal<boolean>(false);

RegistarUsuario() {
    if (this.estaCarregando()) return;
    
    this.mensagemErro.set(null);
    this.estaCarregando.set(true);

    this.authService.CreateUser({
      name: this.name,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.estaCarregando.set(false);
        this.router.navigate(['/login']); // Redireciona para o login ao criar com sucesso
      },
      error: (err) => {
        this.estaCarregando.set(false);
        console.error('Erro:', err);
        if (err.status >= 400 && err.status < 500) {
          this.mensagemErro.set('Não foi possível realizar o cadastro. Verifique os dados inseridos.');
        } else {
          this.mensagemErro.set('Erro ao conectar ao servidor. Tente novamente mais tarde.');
        }
      }
    });

  }





}
