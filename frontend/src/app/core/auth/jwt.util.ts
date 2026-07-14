import { UsuarioAutenticado } from '../models/usuario.model';

/**
 * O TokenService monta o token pelo construtor de JwtSecurityToken, caminho em
 * que o .NET não aplica o mapa de claims de saída. Por isso Name/Email chegam
 * com as URIs completas do schema, e não como "unique_name"/"email". Aceitamos
 * as duas formas para não depender desse detalhe.
 */
const CLAIM_NOME = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const CLAIM_EMAIL = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const CLAIM_ID = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

interface JwtPayload {
  id?: string;
  exp?: number;
  [claim: string]: unknown;
}

function texto(payload: JwtPayload, ...claims: string[]): string | undefined {
  for (const claim of claims) {
    const valor = payload[claim];
    if (typeof valor === 'string' && valor) return valor;
  }
  return undefined;
}

function decodificarPayload(token: string): JwtPayload | null {
  const payload = token.split('.')[1];
  if (!payload) return null;

  try {
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    );
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

/**
 * Lê o usuário a partir do JWT emitido pela API.
 * Retorna null se o token for inválido ou já estiver expirado.
 */
export function lerUsuarioDoToken(token: string): UsuarioAutenticado | null {
  const payload = decodificarPayload(token);
  if (!payload?.exp) return null;

  const expiraEm = payload.exp * 1000;
  if (expiraEm <= Date.now()) return null;

  const id = texto(payload, 'id', CLAIM_ID, 'nameid');
  if (!id) return null;

  return {
    id,
    username: texto(payload, CLAIM_NOME, 'unique_name') ?? '',
    email: texto(payload, CLAIM_EMAIL, 'email') ?? '',
    expiraEm,
  };
}
