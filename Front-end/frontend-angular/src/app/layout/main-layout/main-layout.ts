import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../components/header/header';
import { ServiceData } from '../../service/Data/service.data';
import { AuthService } from '../../service/Authentication/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayoutComponent implements OnInit {

  private router = inject(Router);
  private serviceData = inject(ServiceData);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);

  Name: string = '';
  IsActive: boolean = false;
  IsAdmin: boolean = false;
  isDarkMode: boolean = false;

  ngOnInit(): void {
    // Esta requisição agora SÓ roda quando o usuário entra no Layout (área logada)
    this.ObterInformacoesUsuario();
  }

  ObterInformacoesUsuario(): void {
    this.serviceData.obterInformacoesUsuario().subscribe({
      next: (resposta: any) => {
        this.Name = resposta.Name || resposta.name || '';
        this.IsActive = resposta.IsActive ?? resposta.isActive ?? false;
        this.IsAdmin = resposta.IsAdmin ?? resposta.isAdmin ?? false;
        this.isDarkMode = resposta.isDarkMode ?? resposta.isdarkmode ?? false;

        this.cdr.detectChanges();
      },
      error: (erro) => {
        console.error('Erro ao obter informações do usuário:', erro);
      }
    });
  }

  onThemeChange(value: boolean): void {
    this.isDarkMode = value;

    this.serviceData.atualizarTemaUsuario(value).subscribe({
      error: (erro) => {
        console.error('Erro ao atualizar o tema no Banco de Dados:', erro);
        this.isDarkMode = !value;
        this.cdr.detectChanges();
      }
    });
  }

  deslogar(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}