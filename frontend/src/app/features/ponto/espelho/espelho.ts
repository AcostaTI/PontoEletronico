import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';

import { RegistroPonto, TipoRegistro } from '../../../core/models/registro-ponto.model';
import {
  calcularMinutosTrabalhados,
  formatarDuracao,
  ordenarPorHora,
} from '../../../core/services/jornada.util';
import { RegistroPontoService } from '../../../core/services/registro-ponto.service';

interface DiaTrabalhado {
  data: Date;
  registros: RegistroPonto[];
  minutos: number;
  total: string;
}

/** "yyyy-MM" — formato do <input type="month">. */
function competenciaAtual(): string {
  const hoje = new Date();
  return `${hoje.getFullYear()}-${String(hoje.getMonth() + 1).padStart(2, '0')}`;
}

@Component({
  selector: 'app-espelho',
  imports: [DatePipe],
  templateUrl: './espelho.html',
  styleUrl: './espelho.scss',
})
export class Espelho {
  private readonly registroPontoService = inject(RegistroPontoService);

  protected readonly TipoRegistro = TipoRegistro;

  protected readonly competencia = signal(competenciaAtual());
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  private readonly registros = signal<RegistroPonto[]>([]);

  /** Agrupa os registros do mês por dia, do mais recente para o mais antigo. */
  protected readonly dias = computed<DiaTrabalhado[]>(() => {
    const porData = new Map<string, RegistroPonto[]>();

    for (const registro of this.registros()) {
      const doDia = porData.get(registro.dataRegistro) ?? [];
      doDia.push(registro);
      porData.set(registro.dataRegistro, doDia);
    }

    return [...porData.entries()]
      .sort(([a], [b]) => b.localeCompare(a))
      .map(([data, registrosDoDia]) => {
        const minutos = calcularMinutosTrabalhados(registrosDoDia);

        return {
          data: paraData(data),
          registros: ordenarPorHora(registrosDoDia),
          minutos,
          total: formatarDuracao(minutos),
        };
      });
  });

  protected readonly totalDoMes = computed(() =>
    formatarDuracao(this.dias().reduce((soma, dia) => soma + dia.minutos, 0)),
  );

  protected readonly diasTrabalhados = computed(() => this.dias().length);

  constructor() {
    this.carregar();
  }

  protected aoTrocarCompetencia(valor: string): void {
    if (!valor) return;

    this.competencia.set(valor);
    this.carregar();
  }

  private carregar(): void {
    const [ano, mes] = this.competencia().split('-').map(Number);

    this.carregando.set(true);
    this.erro.set(null);

    this.registroPontoService.obterRegistrosDoMes(ano, mes).subscribe({
      next: (registros) => {
        this.registros.set(registros);
        this.carregando.set(false);
      },
      error: (erro: Error) => {
        this.registros.set([]);
        this.erro.set(erro.message);
        this.carregando.set(false);
      },
    });
  }
}

/** Converte "yyyy-MM-dd" em Date local (evita o deslocamento de fuso do parse ISO). */
function paraData(dataIso: string): Date {
  const [ano, mes, dia] = dataIso.split('-').map(Number);
  return new Date(ano, mes - 1, dia);
}
