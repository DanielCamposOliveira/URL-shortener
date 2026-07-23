import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ServiceData, UsersResponse, User } from '../../service/Data/service.data';
import { UserTable } from '../../components/Tabela/user-table/user-table'; // Ajuste o caminho conforme sua estrutura

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [UserTable, FormsModule],
  templateUrl: './user.html',
  styleUrl: './user.css',
})
export class UsersPage implements OnInit {

  // Injeção de dependências
  private serviceData = inject(ServiceData);
  private cdr = inject(ChangeDetectorRef);

  // Armazena a lista de usuários da página atual
  listaDeUsuarios: User[] = [];

  // Variáveis de paginação e resposta
  paginaAtual: number = 1;
  limitePorPagina: number = 7;
  respostaApi?: UsersResponse;

  ngOnInit(): void {
    this.carregarUsuarios();
  }

  // Carrega os usuários da API com suporte a paginação
  carregarUsuarios(pagina: number = 1): void {
    this.paginaAtual = pagina;

    this.serviceData.obterTodosUsuarios(this.paginaAtual, this.limitePorPagina).subscribe({
      next: (resposta) => {
        this.respostaApi = resposta;
        this.listaDeUsuarios = resposta.user;

        this.cdr.detectChanges(); // Força a atualização do DOM
      },
      error: (erro) => {
        console.error('Erro ao buscar usuários da API:', erro);
      }
    });
  }

  // Alterna status de um usuário (Ativo/Inativo)
  onAlternarStatus(userId: string): void {
    this.serviceData.alternarStatusUsuario(userId).subscribe({
      next: () => {
        this.carregarUsuarios(this.paginaAtual);
      },
      error: (erro) => {
        if (erro.status === 429) {
          alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
        } else {
          console.error('Erro ao alternar o status do usuário:', erro);
          alert('Ocorreu um erro ao alterar o status do usuário. Tente novamente.');
        }
      }
    });
  }

  // Exclui um usuário
  onExcluirUsuario(userId: string): void {
    if (!confirm('Tem certeza de que deseja excluir este usuário?')) {
      return;
    }

    this.serviceData.excluirUsuario(userId).subscribe({
      next: () => {
        this.carregarUsuarios(this.paginaAtual);
      },
      error: (erro) => {
        if (erro.status === 429) {
          alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
        } else {
          console.error('Erro ao excluir o usuário:', erro);
          alert('Ocorreu um erro ao excluir o usuário. Tente novamente.');
        }
      }
    });
  }

// Adicione este método dentro da classe UsersPage no seu user.ts
onSalvarQtdMax(event: { userId: string; novaQtd: string }): void {
  this.serviceData.atualizarQtdMaxUrl(event.userId, event.novaQtd).subscribe({
    next: () => {
      // Recarrega a lista para confirmar as alterações
      this.carregarUsuarios(this.paginaAtual);
    },
    error: (erro) => {
      if (erro.status === 429) {
        alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
      } else {
        console.error('Erro ao atualizar limite de URLs:', erro);
        alert('Ocorreu um erro ao atualizar a quantidade máxima de URLs. Tente novamente.');
      }
    }
  });
}














}