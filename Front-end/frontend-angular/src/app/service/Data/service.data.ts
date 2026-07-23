import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 
import { Observable } from 'rxjs';




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



// --- Interfaces para Usuários ---
export interface User {
  id: string;
  name: string;
  email: string;
  isActive: boolean;
  isAdmin: boolean;
  qtdMaxUrl: number;
}

export interface UsersResponse {
  user: User[];
  page: number;
  limit: number;
  totalCount: number;
}




// strutura para criar uma nova URL
export interface CriarUrlRequest {
  url: string;
}

export interface QtdUrl
{
  userId: string,
  qtdMaxUrl: string
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
    private apiUsersUrl = 'http://localhost:5000/api/v1/user';
    private apiUsersList = 'http://localhost:5000/api/v1/users';
    private apiUserMaxUrl = 'http://localhost:5000/api/v1/user/QtdUrl';

    // ==========================================
    // MÉTODOS DE URLS
    // ==========================================
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
      const urlCompleta = `${this.apiUsersUrl}`;
      return this.http.get<UserInfoResponse>(urlCompleta);
    }

    // Adicione este método dentro da classe ServiceData
    atualizarTemaUsuario(isDarkMode: boolean): Observable<void> {
      const _isDarkMode = isDarkMode.toString();
      const urlCompleta = `${this.apiUsersUrl}/theme/${_isDarkMode}`;

      // Enviamos um objeto vazio {} como body, já que o valor vai no path da URL
      return this.http.patch<void>(urlCompleta, {});
    }

    // ==========================================
    // NOVOS MÉTODOS DE USUÁRIOS
    // ==========================================
    obterTodosUsuarios(page: number = 1, limit: number = 10): Observable<UsersResponse> {

        const urlCompleta = `${this.apiUsersList}?page=${page}&limit=${limit}`;

    //const params = new HttpParams()
    //  .set('page', page.toString())
    //  .set('limit', limit.toString());

    //return this.http.get<UsersResponse>(this.apiUsersUrl, { params });
     return this.http.get<UsersResponse>(urlCompleta);
  }

atualizarQtdMaxUrl(userId: string, qtdMaxUrl: string): Observable<void> {
 
        const body: QtdUrl = {
          userId : userId,
          qtdMaxUrl: qtdMaxUrl
      };

  return this.http.patch<void>(`${this.apiUserMaxUrl}`, body);
}

  alternarStatusUsuario(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUsersUrl}/${id}`, {});
  }

  excluirUsuario(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUsersUrl}/${id}`);
  }

}
