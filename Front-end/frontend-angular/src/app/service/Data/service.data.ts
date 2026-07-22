import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 
import { Observable } from 'rxjs';



// 1. Mapeia como é um Link individual dentro do array
// INTERFACE: Define a estrutura de um objeto de link
// 1. O formato real de cada link vindo da API
export interface UrlItem {
  idOfuscado: string;
  originalUrl: string;
  url: string;
  isActive: boolean;
  clickCount: number;
  expiresAt: string;
}

// 2. A resposta real da sua API
export interface ApiResponse {
  urls: UrlItem[];     // Mudamos de 'data' para 'urls'
  page: number;
  limit: number;
  totalCount: number;  // Mudamos de 'total' para 'totalCount'
}

// strutura para criar uma nova URL
export interface CriarUrlRequest {
  url: string;
}



export interface UserInfoResponse {
  Name : string;
  IsActive: boolean;
  IsAdmin: boolean;
  isDarkMode : boolean;
}

@Injectable({
  providedIn: 'root' 
})


export class ServiceData {

    constructor() {}
    
    // Injeta a ferramenta HttpClient do Angular para fazer requisições web
    private http = inject(HttpClient); 

    // URL da sua API de paginação de links
    private apiUrlsListUrl = 'http://localhost:5000/api/v1/urls';
    private apiUserInfoUrl = 'http://localhost:5000/api/v1/user';

    // MÉTODO: Busca os links paginados
    // Ele promete retornar um Observable contendo a nossa estrutura 'ApiResponse'
    obterTodosLinks(page: number, limit: number ): Observable<ApiResponse> {
      // Montamos a URL juntando os parâmetros de página e limite
      const urlCompleta = `${this.apiUrlsListUrl}?page=${page}&limit=${limit}`;
      
      // O http.get faz a requisição web e avisa o TS que o resultado terá o formato de 'ApiResponse'
      return this.http.get<ApiResponse>(urlCompleta);
    }

    CriarUrl(originalUrl: string): Observable<void> {
      // Montamos a URL completa para criar um novo link
      const urlCompleta = `${this.apiUrlsListUrl}`;

      // Criamos o corpo da requisição com a URL original que queremos encurtar
      const body: CriarUrlRequest = {
          url: originalUrl
      };

      return this.http.post<void>(urlCompleta, body);      
    }

    /**
     * Remove permanentemente um link pelo seu ID Ofuscado
     * DELETE -> http://localhost:5000/api/v1/urls/{idOfuscado}
     */
    excluirLink(idOfuscado: string): Observable<void> {
      const urlCompleta = `${this.apiUrlsListUrl}/${idOfuscado}`;
      return this.http.delete<void>(urlCompleta);
    }

    /**
     * Alterna o estado (Ativo/Inativo) de um link no servidor
     * PATCH -> http://localhost:5000/api/v1/urls/{idOfuscado}
     * Não envia parâmetros no corpo, o backend inverte o estado atual automaticamente.
     */
    alternarStatusLink(idOfuscado: string): Observable<UrlItem> {
      const urlCompleta = `${this.apiUrlsListUrl}/${idOfuscado}`;
      
      // Enviamos um objeto vazio {} no corpo do PATCH já que ele não exige dados
      return this.http.patch<UrlItem>(urlCompleta, {});
    }

    /**
     * Busca informações do usuário logado
     * GET -> http://localhost:5000/api/v1/user
     */
    obterInformacoesUsuario(): Observable<UserInfoResponse> {
      const urlCompleta = `${this.apiUserInfoUrl}`;
      return this.http.get<UserInfoResponse>(urlCompleta);
    }

    // Adicione este método dentro da classe ServiceData
    atualizarTemaUsuario(isDarkMode: boolean): Observable<void> {
      const _isDarkMode = isDarkMode.toString();
      const urlCompleta = `${this.apiUserInfoUrl}/theme/${_isDarkMode}`;

      // Enviamos um objeto vazio {} como body, já que o valor vai no path da URL
      return this.http.patch<void>(urlCompleta, {});
    }
}
