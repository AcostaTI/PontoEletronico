using PontoEletronico.Application.Common.Exceptions;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Application.Usuarios.Dtos;
using PontoEletronico.Application.Usuarios.Interfaces;

namespace PontoEletronico.Application.Usuarios.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public UsuarioService(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task CadastrarUsuarioAsync(CreateUsuarioDto usuarioDto)
    {
        var resultado = await _identityService.CadastrarUsuarioAsync(usuarioDto);

        if (!resultado.Sucesso)
            throw new RegraDeNegocioException("Falha ao cadastrar usuário.", resultado.Erros);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUsuarioDto loginUsuarioDto)
    {
        var usuario = await _identityService.AutenticarAsync(loginUsuarioDto);

        if (usuario is null)
            throw new RegraDeNegocioException("Falha ao realizar login!");

        return new LoginResponseDto(_tokenService.GerarToken(usuario));
    }
}
