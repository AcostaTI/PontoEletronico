import { DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';

import { RegistroPonto, TipoRegistro } from '../../../core/models/registro-ponto.model';
import {
  calcularMinutosTrabalhados,
  formatarDuracao,
  ordenarPorHora,
  proximoTipo,
} from '../../../core/services/jornada.util';
import { RegistroPontoService } from '../../../core/services/registro-ponto.service';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly registroPontoService = inject(RegistroPontoService);

  protected readonly TipoRegistro = TipoRegistro;

  private readonly registros = signal<RegistroPonto[]>([]);
  protected readonly carregando = signal(true);
  protected readonly registrando = signal(false);
  protected readonly erro = signal<string | null>(null);
  protected readonly agora = signal(new Date());

  /** Registros do dia em ordem cronológica. */
  protected readonly linhaDoTempo = computed(() => ordenarPorHora(this.registros()));

  /** ENTRADA ou SAÍDA que o próximo clique vai gerar — mesma regra do domínio. */
  protected readonly proximoRegistro = computed(() => proximoTipo(this.registros()));

  /** Está dentro de um turno aberto (última batida foi ENTRADA). */
  protected readonly turnoAberto = computed(
    () => this.proximoRegistro() === TipoRegistro.SAIDA,
  );

  /** Total trabalhado hoje; o turno em aberto conta até o instante atual. */
  protected readonly totalTrabalhado = computed(() => {
    const agora = this.agora();
    const minutosAgora = agora.getHours() * 60 + agora.getMinutes();

    return formatarDuracao(calcularMinutosTrabalhados(this.registros(), minutosAgora));
  });

  constructor() {
    const relogio = setInterval(() => this.agora.set(new Date()), 1000);
    inject(DestroyRef).onDestroy(() => clearInterval(relogio));

    this.carregarRegistrosDoDia();
  }

  protected registrarPonto(): void {
    this.registrando.set(true);
    this.erro.set(null);

    this.registroPontoService.registrarPonto().subscribe({
      next: (registro) => {
        this.registros.update((atuais) => [...atuais, registro]);
        this.registrando.set(false);
      },
      error: (erro: Error) => {
        this.erro.set(erro.message);
        this.registrando.set(false);
      },
    });
  }

  private carregarRegistrosDoDia(): void {
    this.carregando.set(true);

    this.registroPontoService.obterRegistrosDoDia().subscribe({
      next: (registros) => {
        this.registros.set(registros);
        this.carregando.set(false);
      },
      error: (erro: Error) => {
        this.erro.set(erro.message);
        this.carregando.set(false);
      },
    });
  }
}
