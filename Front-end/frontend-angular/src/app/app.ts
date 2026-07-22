
import { Component, OnInit, inject, ChangeDetectorRef} from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../app/service/Authentication/auth.service';



// Importa o serviço ServiceLab para enviar dados ao servidor
import { HeaderComponent } from '../app/components/header/header';
import { ServiceData } from './service/Data/service.data';



// O decorador @Component é usado para definir um componente Angular. Ele fornece metadados sobre o componente, como seu seletor, modelo e estilos.
// agent que esta fazendo so comentarios ? 
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, HeaderComponent], // adiciona o componente FirstComponent aos imports do AppComponent
  templateUrl: './app.html',
  styleUrl: './app.css'
})


export class App {


// Tornar public permite que o app.html acesse o router diretamente
  public router = inject(Router);

    // Injeta o serviço de dados reais
  private serviceData = inject(ServiceData);
  
  // Injete a ferramenta de detecção de mudanças aqui
  private cdr = inject(ChangeDetectorRef);

  // Injeta o serviço de autenticação para gerenciar a sessão
  private authService = inject(AuthService);
  

    ngOnInit(): void {
    this.ObterInformacoesUsuario();
  }



  // Método que o botão "Sair do Sistema" vai disparar no HTML
  deslogar(): void {
    console.log("click");
    // 1. Limpa o token do localStorage e zera os Signals reativos
    this.authService.logout();
    
    // 2. Redireciona o usuário de volta para o login de forma segura
    this.router.navigate(['/login']);
  }



// ------------------

    Name: string = '';
    IsActive: boolean = false;
    IsAdmin: boolean = false;
    isDarkMode: boolean = false;

    // Método para obter informações do usuário logado
    ObterInformacoesUsuario(): void {
      this.serviceData.obterInformacoesUsuario().subscribe({      
        
        next: (resposta: any) => {
          console.log("ObterInformacoesUsuario()");
          // Aceita tanto PascalCase quanto camelCase
          this.Name = resposta.Name || resposta.name || '';
          this.IsActive = resposta.IsActive ?? resposta.isActive ?? false;
          this.IsAdmin = resposta.IsAdmin ?? resposta.isAdmin ?? false;
          this.isDarkMode = resposta.isDarkMode ?? resposta.isdarkmode ?? false;

          this.cdr.detectChanges(); // Força a atualização da interface
        },
        error: (erro) => {
          console.error('Erro ao obter informações do usuário:', erro);
        }
      });
    }
  
    onThemeChange(value: boolean): void {
    // 1. Atualiza o estado local do componente
    this.isDarkMode = value;
   
    // 2. Envia a alteração para o banco de dados através do serviço
    this.serviceData.atualizarTemaUsuario(value).subscribe({

      error: (erro) => {
        console.error('Erro ao atualizar o tema no Banco de Dados:', erro);
        // Opcional: reverte o valor local caso ocorra erro no backend
        this.isDarkMode = !value;
        this.cdr.detectChanges();
      }
    });
  }








}
