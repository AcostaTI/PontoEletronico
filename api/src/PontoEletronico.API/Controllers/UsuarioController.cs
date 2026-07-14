using Microsoft.AspNetCore.Mvc;
using PontoEletronico.Application.Usuarios.Dtos;
using PontoEletronico.Application.Usuarios.Interfaces;

namespace PontoEletronico.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("cadastro")]
    public async Task<IActionResult> CriarUsuario([FromBody] CreateUsuarioDto usuarioDto)
    {
        await _usuarioService.CadastrarUsuarioAsync(usuarioDto);

        return Ok(new { message = "Usuário cadastrado!" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginUsuarioDto loginUsuarioDto)
    {
        var resposta = await _usuarioService.LoginAsync(loginUsuarioDto);

        return Ok(resposta);
    }
}
