import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateUsuario,
  LoginResponse,
  LoginUsuario,
  UsuarioAutenticado,
} from '../models/usuario.model';
import { lerUsuarioDoToken } from './jwt.util';

const CHAVE_TOKEN = 'ponto-eletronico.token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/Usuario`;

  private readonly usuarioAtual = signal<UsuarioAutenticado | null>(null);
  private expiracaoTimer?: ReturnType<typeof setTimeout>;

  readonly usuario = this.usuarioAtual.asReadonly();
  readonly autenticado = computed(() => this.usuarioAtual() !== null);

  constructor() {
    this.restaurarSessao();
  }

  cadastrar(dados: CreateUsuario): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/cadastro`, dados);
  }

  login(credenciais: LoginUsuario): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/login`, credenciais)
      .pipe(tap((resposta) => this.registrarSessao(resposta.token)));
  }

  logout(): void {
    this.limparSessao();
    this.router.navigate(['/login']);
  }

  obterToken(): string | null {
    return localStorage.getItem(CHAVE_TOKEN);
  }

  private registrarSessao(token: string): void {
    const usuario = lerUsuarioDoToken(token);
    if (!usuario) {
      this.limparSessao();
      return;
    }

    localStorage.setItem(CHAVE_TOKEN, token);
    this.usuarioAtual.set(usuario);
    this.agendarExpiracao(usuario.expiraEm);
  }

  /** Recupera a sessão do localStorage ao recarregar a página. */
  private restaurarSessao(): void {
    const token = this.obterToken();
    if (!token) return;

    const usuario = lerUsuarioDoToken(token);
    if (!usuario) {
      this.limparSessao();
      return;
    }

    this.usuarioAtual.set(usuario);
    this.agendarExpiracao(usuario.expiraEm);
  }

  /** Desloga automaticamente quando o token expira, sem esperar por um 401. */
  private agendarExpiracao(expiraEm: number): void {
    clearTimeout(this.expiracaoTimer);
    this.expiracaoTimer = setTimeout(() => this.logout(), expiraEm - Date.now());
  }

  private limparSessao(): void {
    clearTimeout(this.expiracaoTimer);
    localStorage.removeItem(CHAVE_TOKEN);
    this.usuarioAtual.set(null);
  }
}
