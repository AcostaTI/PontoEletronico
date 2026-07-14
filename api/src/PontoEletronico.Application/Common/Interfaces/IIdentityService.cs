using PontoEletronico.Application.Common.Models;
using PontoEletronico.Application.Usuarios.Dtos;

namespace PontoEletronico.Application.Common.Interfaces;

/// <summary>
/// Porta para o provedor de identidade. A implementação (ASP.NET Identity) vive na Infrastructure.
/// </summary>
public interface IIdentityService
{
    Task<ResultadoOperacao> CadastrarUsuarioAsync(CreateUsuarioDto usuarioDto);

    Task<UsuarioAutenticado?> AutenticarAsync(LoginUsuarioDto loginDto);
}
