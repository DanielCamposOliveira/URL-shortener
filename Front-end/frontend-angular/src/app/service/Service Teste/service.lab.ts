import { Injectable, inject } from '@angular/core'; // Importa o decorador Injectable e a função inject do Angular para criar serviços injetáveis
import { HttpClient } from '@angular/common/http'; // Importa o HttpClient para fazer requisições HTTP
import { Observable } from 'rxjs'; // Importa o Observable do RxJS para lidar com fluxos de dados assíncronos


// Criamos uma interface para o TypeScript entender exatamente o formato do JSON da sua API
export interface Identificador {
  idNumerico: number;
  idOfuscado: string;
  maquinaOrigem: number;
}


@Injectable({
  providedIn: 'root' // Torna o serviço disponível em toda a aplicação
})

// A classe ServiceLab é um serviço Angular que simula o envio de dados para um servidor.
// o nome da classe é ServiceLab não precisa ter o mesmo nome do arquivo, mas é uma boa prática manter a consistência.
export class ServiceLab {

        // O construtor da classe é vazio, mas é necessário para a criação de instâncias do serviço.
    constructor() {}

    
    // Injeta a ferramenta HttpClient do Angular para fazer requisições web
    private http = inject(HttpClient); 

    // URL da sua API local
    private apiUrl = 'http://localhost:4848/api/v1/identificadores';

    // Método que vai buscar os dados. Ele promete retornar um "Fluxo" (Observable) contendo o Identificador
    obterDados(): Observable<Identificador> {
    return this.http.get<Identificador>(this.apiUrl);
    }




    // Este método simula o envio de informações para um servidor. 
    // Ele recebe uma string como parâmetro e imprime essa informação no console.
    EnviarDados(dados: string): void 
    {
        console.log('Log serviceLab:', dados);
    }

    

}