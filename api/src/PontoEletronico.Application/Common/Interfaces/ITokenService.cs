using PontoEletronico.Application.Common.Models;

namespace PontoEletronico.Application.Common.Interfaces;

public interface ITokenService
{
    string GerarToken(UsuarioAutenticado usuario);
}
