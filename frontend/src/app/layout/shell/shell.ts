import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly authService = inject(AuthService);

  protected readonly usuario = this.authService.usuario;

  protected sair(): void {
    this.authService.logout();
  }

  protected get iniciais(): string {
    const nome = this.usuario()?.username ?? '';
    return nome.slice(0, 2).toUpperCase() || '??';
  }
}
