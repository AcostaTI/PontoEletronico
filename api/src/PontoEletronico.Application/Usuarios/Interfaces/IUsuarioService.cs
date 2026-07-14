using PontoEletronico.Application.Usuarios.Dtos;

namespace PontoEletronico.Application.Usuarios.Interfaces;

public interface IUsuarioService
{
    Task CadastrarUsuarioAsync(CreateUsuarioDto usuarioDto);

    Task<LoginResponseDto> LoginAsync(LoginUsuarioDto loginUsuarioDto);
}
