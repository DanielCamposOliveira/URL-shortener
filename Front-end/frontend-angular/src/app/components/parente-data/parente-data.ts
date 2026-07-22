import { Component, Input } from '@angular/core';
import { Identificador } from '../../service/Service Teste/service.lab'; // Importa a interface para tipar o Input

@Component({
  selector: 'app-parente-data',
  imports: [],
  templateUrl: './parente-data.html',
  styleUrl: './parente-data.css',
})
export class ParenteData {
  @Input() name: string = ''; // Define a propriedade de entrada 'name' com um valor padrão vazio
  @Input() sobrenome: string = ''; // Define a propriedade de entrada 'sobrenome' com um valor padrão vazio


  // RECEBE O IDENTIFICADOR DA API
  // Ele receberá o objeto identificador completo ou null (enquanto o botão não for clicado)
  @Input() identificador: Identificador | null = null;
}
