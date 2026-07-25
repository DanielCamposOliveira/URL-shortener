import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class HeaderComponent {
  @Input() titulo: string = 'Sistema de Encurtador de URLs';

  @Output() logout = new EventEmitter<void>();

  @Input() Name: string = 'Name';
  @Input() IsActive: boolean = false;
  @Input() IsAdmin: boolean = false;

  // Variável interna para controlar o estado do tema
  private _isDarkMode = true;

  // Getter e Setter para o tema escuro
  @Input()
  set isDarkMode(value: boolean) {
    this._isDarkMode = value;
    // Aplica/Remove a classe no body automaticamente quando o dado chega
   // document.body.classList.toggle('dark-theme', value);

   if (value) {
      document.body.classList.remove('light-theme');
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
      document.body.classList.add('light-theme');
    }    
  }
  
  get isDarkMode(): boolean {
    return this._isDarkMode;
  }

  // Emite o evento de mudança de tema para o componente pai
  @Output() isDarkModeChange = new EventEmitter<boolean>();


  // Método chamado quando o usuário alterna o tema
  toggleTheme(): void {
    this.isDarkModeChange.emit(!this._isDarkMode);
  }

  onSair(): void {
    this.logout.emit();
  }
}