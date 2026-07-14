using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PontoEletronico.Application.RegistrosPonto.Dtos;
using PontoEletronico.Application.RegistrosPonto.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PontoEletronico.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class RegistroPontoController : ControllerBase
{
    private readonly IRegistroPontoService _registroPontoService;

    public RegistroPontoController(IRegistroPontoService registroPontoService)
    {
        _registroPontoService = registroPontoService;
    }

    [HttpPost("RegistraPonto")]
    public async Task<ActionResult<RegistroPontoDto>> RegistrarPonto(CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized(new { message = "Usuário não identificado no token." });

        var registro = await _registroPontoService.RegistrarPontoAsync(usuarioId, cancellationToken);

        return Ok(registro);
    }

    [HttpGet("ObterPontoDia")]
    public async Task<ActionResult<IReadOnlyList<RegistroPontoDto>>> ObterRegistrosDoDia(CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized(new { message = "Usuário não identificado no token." });

        var registros = await _registroPontoService.ObterRegistrosDoDiaAsync(usuarioId, cancellationToken);

        return Ok(registros);
    }

    [HttpGet("ObterPontoMes")]
    public async Task<ActionResult<IReadOnlyList<RegistroPontoDto>>> ObterRegistrosDoMes(
        [BindRequired, Range(1, 9999)] int ano,
        [BindRequired, Range(1, 12)] int mes,
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized(new { message = "Usuário não identificado no token." });

        var registros = await _registroPontoService.ObterRegistrosDoMesAsync(usuarioId, ano, mes, cancellationToken);

        return Ok(registros);
    }

    private string? ObterUsuarioId() => User.FindFirstValue("id");
}
