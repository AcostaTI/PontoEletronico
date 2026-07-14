using Microsoft.AspNetCore.Identity;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Application.Common.Models;
using PontoEletronico.Application.Usuarios.Dtos;

namespace PontoEletronico.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ResultadoOperacao> CadastrarUsuarioAsync(CreateUsuarioDto usuarioDto)
    {
        var usuario = new ApplicationUser
        {
            UserName = usuarioDto.Username,
            Email = usuarioDto.Email
        };

        var resultado = await _userManager.CreateAsync(usuario, usuarioDto.Password);

        return resultado.Succeeded
            ? ResultadoOperacao.Ok()
            : ResultadoOperacao.Falha(resultado.Errors.Select(erro => erro.Description));
    }

    public async Task<UsuarioAutenticado?> AutenticarAsync(LoginUsuarioDto loginDto)
    {
        var usuario = await _userManager.FindByNameAsync(loginDto.Username);

        if (usuario is null)
            return null;

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, loginDto.Password, lockoutOnFailure: false);

        if (!resultado.Succeeded)
            return null;

        return new UsuarioAutenticado(usuario.Id, usuario.UserName!, usuario.Email!);
    }
}
