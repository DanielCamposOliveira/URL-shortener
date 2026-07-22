import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
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
  private _isDarkMode = false;

  // Getter e Setter para o tema escuro
  @Input()
  set isDarkMode(value: boolean) {
    this._isDarkMode = value;
    // Aplica/Remove a classe no body automaticamente quando o dado chega
    document.body.classList.toggle('dark-theme', value);
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