import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import localePt from '@angular/common/locales/pt';
import {
  ApplicationConfig,
  LOCALE_ID,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';
import { erroInterceptor } from './core/http/erro.interceptor';

registerLocaleData(localePt);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    // A ordem importa: o authInterceptor injeta o Bearer, o erroInterceptor
    // traduz a resposta de erro que volta.
    provideHttpClient(withInterceptors([authInterceptor, erroInterceptor])),
    { provide: LOCALE_ID, useValue: 'pt-BR' },
  ],
};
