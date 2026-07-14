/** Espelha PontoEletronico.Domain.Enums.TipoRegistroEnum. */
export enum TipoRegistro {
  ENTRADA = 1,
  SAIDA = 2,
}

/** Espelha PontoEletronico.Application.RegistrosPonto.Dtos.RegistroPontoDto. */
export interface RegistroPonto {
  id: string;
  /** Formato "yyyy-MM-dd" (DateOnly). */
  dataRegistro: string;
  /** Formato "HH:mm:ss" (TimeOnly). */
  horaRegistro: string;
  tipoRegistro: TipoRegistro;
}
