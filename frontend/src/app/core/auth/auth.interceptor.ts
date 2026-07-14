import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { AuthService } from './auth.service';

/** Anexa o JWT em toda chamada à API, exceto login e cadastro. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).obterToken();

  const rotaPublica = req.url.includes('/Usuario/login') || req.url.includes('/Usuario/cadastro');

  if (!token || rotaPublica) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
