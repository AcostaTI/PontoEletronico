import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

/** Bloqueia rotas privadas para quem não tem um token válido. */
export const authGuard: CanActivateFn = (_rota, estado) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.autenticado()) return true;

  return router.createUrlTree(['/login'], {
    queryParams: { redirect: estado.url },
  });
};

/** Impede que um usuário já autenticado volte para login/cadastro. */
export const visitanteGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.autenticado() ? router.createUrlTree(['/dashboard']) : true;
};
