import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // Importa o provedor HTTP
import { routes } from './app.routes';
import { authInterceptor } from './service/authGuard/auth.interceptor'; // Importe o interceptor que criamos

export const appConfig: ApplicationConfig = {
  providers: [

    // 1. Ativa o modo Zoneless estável nativo do seu projeto
    provideZonelessChangeDetection(),

    provideBrowserGlobalErrorListeners(),




    // Sistema de rotas
    provideRouter(routes),

    // 3. Cliente HTTP com o interceptor de segurança injetado
    provideHttpClient(
      withInterceptors([authInterceptor])
    )

    
  ]
  
};
