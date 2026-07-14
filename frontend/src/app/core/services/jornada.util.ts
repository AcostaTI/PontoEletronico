import { RegistroPonto, TipoRegistro } from '../models/registro-ponto.model';

/** Converte "HH:mm:ss" em minutos desde a meia-noite. */
export function horaEmMinutos(hora: string): number {
  const [h, m] = hora.split(':').map(Number);
  return h * 60 + m;
}

/** Ordena por horário, sem alterar o array original. */
export function ordenarPorHora(registros: readonly RegistroPonto[]): RegistroPonto[] {
  return [...registros].sort((a, b) => a.horaRegistro.localeCompare(b.horaRegistro));
}

/**
 * Soma os minutos trabalhados pareando cada ENTRADA com a SAÍDA seguinte.
 * Se o dia terminar com uma ENTRADA em aberto, `minutosAgora` (quando
 * informado) fecha o par para exibir o tempo corrido do turno atual.
 */
export function calcularMinutosTrabalhados(
  registros: readonly RegistroPonto[],
  minutosAgora?: number,
): number {
  const ordenados = ordenarPorHora(registros);

  let total = 0;
  let entradaAberta: number | null = null;

  for (const registro of ordenados) {
    if (registro.tipoRegistro === TipoRegistro.ENTRADA) {
      entradaAberta = horaEmMinutos(registro.horaRegistro);
    } else if (entradaAberta !== null) {
      total += horaEmMinutos(registro.horaRegistro) - entradaAberta;
      entradaAberta = null;
    }
  }

  if (entradaAberta !== null && minutosAgora !== undefined && minutosAgora > entradaAberta) {
    total += minutosAgora - entradaAberta;
  }

  return total;
}

/** Formata minutos como "8h 05min". */
export function formatarDuracao(minutos: number): string {
  const horas = Math.floor(minutos / 60);
  const restante = Math.round(minutos % 60);

  return `${horas}h ${String(restante).padStart(2, '0')}min`;
}

/**
 * O próximo registro alterna a partir do último do dia — mesma regra do
 * RegistroPonto.Registrar no domínio da API.
 */
export function proximoTipo(registrosDoDia: readonly RegistroPonto[]): TipoRegistro {
  const ordenados = ordenarPorHora(registrosDoDia);
  const ultimo = ordenados[ordenados.length - 1];

  if (!ultimo) return TipoRegistro.ENTRADA;

  return ultimo.tipoRegistro === TipoRegistro.ENTRADA ? TipoRegistro.SAIDA : TipoRegistro.ENTRADA;
}
