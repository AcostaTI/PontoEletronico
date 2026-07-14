import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { RegistroPonto } from '../models/registro-ponto.model';

@Injectable({ providedIn: 'root' })
export class RegistroPontoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/RegistroPonto`;

  /**
   * Bate o ponto. A API decide sozinha se é ENTRADA ou SAÍDA, alternando a
   * partir do último registro do dia — o front não envia o tipo.
   */
  registrarPonto(): Observable<RegistroPonto> {
    return this.http.post<RegistroPonto>(`${this.baseUrl}/RegistraPonto`, {});
  }

  obterRegistrosDoDia(): Observable<RegistroPonto[]> {
    return this.http.get<RegistroPonto[]>(`${this.baseUrl}/ObterPontoDia`);
  }

  obterRegistrosDoMes(ano: number, mes: number): Observable<RegistroPonto[]> {
    const params = new HttpParams().set('ano', ano).set('mes', mes);

    return this.http.get<RegistroPonto[]>(`${this.baseUrl}/ObterPontoMes`, { params });
  }
}
