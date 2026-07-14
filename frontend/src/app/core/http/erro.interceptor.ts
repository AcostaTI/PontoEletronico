import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../auth/auth.service';

/** ProblemDetails devolvido pelo ExceptionHandlingMiddleware da API. */
interface ProblemDetails {
  title?: string;
  detail?: string;
  erros?: string[];
  errors?: Record<string, string[]>;
}

/**
 * Traduz o corpo do erro da API em uma única mensagem legível e desloga o
 * usuário quando o token é rejeitado. O erro segue propagando para que o
 * componente decida como exibi-lo.
 */
export const erroInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((erro: HttpErrorResponse) => {
      if (erro.status === 401 && authService.autenticado()) {
        authService.logout();
      }

      return throwError(() => new Error(extrairMensagem(erro)));
    }),
  );
};

function extrairMensagem(erro: HttpErrorResponse): string {
  if (erro.status === 0) {
    return 'Não foi possível conectar à API. Verifique se ela está em execução.';
  }

  if (erro.status === 401) {
    return 'Sessão expirada. Faça login novamente.';
  }

  const corpo = erro.error as ProblemDetails | string | null;

  if (typeof corpo === 'string' && corpo.trim()) {
    return corpo;
  }

  if (corpo && typeof corpo === 'object') {
    // Erros de regra de negócio (RegraDeNegocioException) vêm em "erros".
    if (corpo.erros?.length) {
      return corpo.erros.join(' ');
    }

    // Erros de validação do ModelState vêm em "errors".
    if (corpo.errors) {
      const mensagens = Object.values(corpo.errors).flat();
      if (mensagens.length) return mensagens.join(' ');
    }

    if (corpo.title) return corpo.title;
    if (corpo.detail) return corpo.detail;
  }

  return 'Ocorreu um erro inesperado. Tente novamente.';
}
