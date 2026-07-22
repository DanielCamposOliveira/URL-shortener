import { Component, OnInit, inject, ChangeDetectorRef} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiResponse, ServiceData, UrlItem } from '../../service/Data/service.data';
import { UrlTableComponent } from '../../components/Tabela/url-table/url-table';



@Component({
  selector: 'app-home',
  imports: [UrlTableComponent, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})



export class Home implements OnInit {

  // 3. Injeta o serviço de dados reais
  private serviceData = inject(ServiceData);
  
  // 2. Injete a ferramenta de detecção de mudanças aqui
  private cdr = inject(ChangeDetectorRef);

  // 4. Cria a variável que vai guardar a lista de links que virá da API
  listaDeLinks: UrlItem[] = [];

  // O ngOnInit é um método do Angular que roda automaticamente assim que a tela abre
  ngOnInit(): void {
    this.carregarLinks();
  }

paginaAtual: number = 1;
limitePorPagina: number = 7; // define o limite de itens por página
respostaApi?: ApiResponse;   // Guarda a resposta inteira (urls, page, limit, totalCount)
  
// 1. Recebe o número da página por parâmetro (padrão é 1)
carregarLinks(pagina: number = 1): void {
  this.paginaAtual = pagina; // Atualiza a página atual
 
  // 2. Passa a página atual e o limite fixo de 5 itens para o serviço
  this.serviceData.obterTodosLinks(this.paginaAtual, this.limitePorPagina).subscribe({
    next: (resposta) => {
      // Guarda a resposta completa da API para passar ao componente filho
      this.respostaApi = resposta;
      this.listaDeLinks = resposta.urls;

      this.cdr.detectChanges(); // Força a atualização da tela para refletir as mudanças na lista de links
    },
    error: (erro) => {
      console.error('Erro ao buscar os links da API:', erro);
    }
  }); 
}

// Cria os métodos que vai ser chamados pelos botões da tabela
  onAlternarStatus(idOfuscado: string): void {
    
      this.serviceData.alternarStatusLink(idOfuscado).subscribe({
        next: () => {
          // Recarrega a lista do servidor para atualizar o status e a cor da linha na hora
          this.carregarLinks();
        },
        error: (erro) => {
        // Se der erro 429 aqui, capturamos o erro amigavelmente
        if (erro.status === 429) {
          alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
        } else {
          console.error('Erro ao alternar o status do link:', erro);
          alert('Ocorreu um erro ao alterar o status do link. Tente novamente. Se o problema persistir, entre em contato com o suporte.');
        }
        }
      });
    
  }

  // Cria os métodos que vai ser chamados pelos botões da tabela
  onExcluirLink(idOfuscado: string): void {    

      this.serviceData.excluirLink(idOfuscado).subscribe({
        next: () => {
          // Removeu do banco? Recarrega a lista para sumir com a linha da tabela
          this.carregarLinks();
        },
        error: (erro) => {
          // Se der erro 429 aqui, capturamos o erro amigavelmente
        if (erro.status === 429) {
          alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
        } else {
          console.error('Erro ao alternar o status do link:', erro);
          alert('Ocorreu um erro ao alterar o status do link. Tente novamente. Se o problema persistir, entre em contato com o suporte.');
        }
        }
      });
    
  }

  novaUrl = '';

  onCriarUrl(): void {
    // .trim() evita o envio de strings com apenas espaços em branco
    if (!this.novaUrl || !this.novaUrl.trim()) {
      alert('Por favor, insira uma URL válida antes de criar um link encurtado.');
      return;
    }

    this.serviceData.CriarUrl(this.novaUrl.trim()).subscribe({
      next: () => {
        console.log('Link criado com sucesso!');
      // Recarrega a lista do servidor para atualizar a tabela com o novo link
      this.carregarLinks();
      // Limpa o campo de entrada após criar o link
      this.novaUrl = '';
    },
    error: (erro) => {
      // Trata o erro 429 (Rate Limit) de forma amigável
      if (erro.status === 429) {
        alert('Limite de solicitações excedido. Aguarde alguns instantes antes de realizar uma nova tentativa.');
      } else {
        console.error('Erro ao criar o link:', erro);
        alert('Ocorreu um erro ao criar o link. Tente novamente. Se o problema persistir, entre em contato com o suporte.');
      }
    }
  });
}





  









}
