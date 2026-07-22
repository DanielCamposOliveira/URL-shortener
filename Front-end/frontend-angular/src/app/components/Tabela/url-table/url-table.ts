import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UrlItem } from '../../../service/Data/service.data';

@Component({
  selector: 'app-url-table',
  imports: [CommonModule],
  templateUrl: './url-table.html',
  styleUrl: './url-table.css',
})
export class UrlTableComponent {
  @Input() links: UrlItem[] = [];

  // Mapeamento direto do seu JSON
  @Input() page: number = 1;
  @Input() limit: number = 10;
  @Input() totalCount: number = 0; // Nome correto da propriedade do JSON!

  @Output() alternarStatus = new EventEmitter<string>();
  @Output() excluirLink = new EventEmitter<string>();
  @Output() criarUrl = new EventEmitter<string>();
  @Output() paginaAlterada = new EventEmitter<number>();

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

  // Método para copiar a URL para a área de transferência
  copyUrl(url: string): void {
  navigator.clipboard.writeText(url).then(() => {
  
  }).catch(err => {
    console.error('Erro ao copiar:', err);
  });
}
}