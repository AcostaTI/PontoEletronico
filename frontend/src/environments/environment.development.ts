// Em desenvolvimento as chamadas passam pelo proxy do dev-server (proxy.conf.json),
// que encaminha /api para https://localhost:7239 ignorando o certificado autoassinado.
export const environment = {
  production: false,
  apiUrl: '/api',
};
