import { FormsModule } from '@angular/forms';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../../../service/Data/service.data';

@Component({
  selector: 'app-user-table',
  imports: [CommonModule, FormsModule],
  templateUrl: './user-table.html',
  styleUrl: './user-table.css',
})
export class UserTable {
  
  // Recebe a lista de usuários do JSON 
  //@Input() links: User[] = [];
  @Input() users: User[] = [];

  // Mapeamento direto do seu JSON
  @Input() page: number = 1;
  @Input() limit: number = 10;
  @Input() totalCount: number = 0; // Nome correto da propriedade do JSON!

// Eventos de saída do componente
  @Output() alternarStatus = new EventEmitter<string>();
  @Output() excluirUsuario = new EventEmitter<string>(); 
  @Output() paginaAlterada = new EventEmitter<number>();

  @Output() salvarQtdMax = new EventEmitter<{ userId: string, novaQtd: string }>();

  // O cálculo agora usa as variáveis do seu JSON
  get totalPaginas(): number {
    if (!this.limit || this.limit <= 0) return 1;
    return Math.ceil(this.totalCount / this.limit) || 1;
  }

  mudarPagina(novaPagina: number): void {
    if (novaPagina >= 1 && novaPagina <= this.totalPaginas) {
      this.paginaAlterada.emit(novaPagina);
    }
  }


  
}
