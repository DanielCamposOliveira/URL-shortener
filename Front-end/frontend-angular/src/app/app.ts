// esse arquivo antes se chamava app.component.ts, mas foi renomeado para app.ts
import { Component, signal,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';

// Importa o serviço ServiceLab para enviar dados ao servidor
import { ServiceLab, Identificador } from './service/Service Teste/service.lab';



// O decorador @Component é usado para definir um componente Angular. Ele fornece metadados sobre o componente, como seu seletor, modelo e estilos.
// agent que esta fazendo so comentarios ? 
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule], // adiciona o componente FirstComponent aos imports do AppComponent
  templateUrl: './app.html',
  styleUrl: './app.css'
})


export class App {




  // Exemplo de como enviar dados para o servidor usando o serviço ServiceLab
  Dados ='Dados do Formulário Enviado para o Servidor';
  private serviceLab = inject(ServiceLab); // Injeta o serviço ServiceLab para enviar dados ao servidor
  enviarDados() {    
    this.serviceLab.EnviarDados(this.Dados);
  }



  // Exemplo de como buscar dados da API usando o serviço ServiceLab
  // 1. Injeta o serviço ServiceLab para buscar dados da API
  private identificadoresService = inject(ServiceLab);

  // 2. Cria um Signal para armazenar os dados recebidos da API
  dadosApi = signal<Identificador | null>(null);

  // 3. Método que busca os dados da API e atualiza o Signal
  buscarDadosDaApi() {

    // 4. Chama o método obterDados do serviço ServiceLab, que retorna um Observable
    this.identificadoresService.obterDados().subscribe({
      // 5. Quando os dados chegarem com sucesso, nós guardamos no nosso Signal
      next: (resposta) => {
        
        // 6. Atualiza o Signal com os dados recebidos da API
        this.dadosApi.set(resposta);

      },
      error: (erro) => {
        // 7. Em caso de erro, logamos no console para depuração
        console.error('Erro ao buscar dados da API:', erro);
      }
    });
  }
  // 8. O Signal 'dadosApi' agora pode ser usado na view para exibir os dados recebidos da API



}
