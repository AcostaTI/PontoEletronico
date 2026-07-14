export interface CreateUsuario {
  username: string;
  email: string;
  password: string;
  rePassword: string;
}

export interface LoginUsuario {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

/** Dados extraídos do payload do JWT emitido pelo TokenService da API. */
export interface UsuarioAutenticado {
  id: string;
  username: string;
  email: string;
  /** Expiração do token em milissegundos (epoch). */
  expiraEm: number;
}
