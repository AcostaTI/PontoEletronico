using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PontoEletronico.Application.RegistrosPonto.Dtos;
using PontoEletronico.Application.RegistrosPonto.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PontoEletronico.API.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
public class RegistroPontoController : ApiControllerBase
{
    private readonly IRegistroPontoService _registroPontoService;

    public RegistroPontoController(IRegistroPontoService registroPontoService)
    {
        _registroPontoService = registroPontoService;
    }

    [HttpPost("RegistraPonto")]
    public async Task<ActionResult<RegistroPontoDto>> RegistrarPonto(CancellationToken cancellationToken)
    {
        var registro = await _registroPontoService.RegistrarPontoAsync(UsuarioId, cancellationToken);

        return Ok(registro);
    }

    [HttpGet("ObterPontoDia")]
    public async Task<ActionResult<IReadOnlyList<RegistroPontoDto>>> ObterRegistrosDoDia(CancellationToken cancellationToken)
    {
        var registros = await _registroPontoService.ObterRegistrosDoDiaAsync(UsuarioId, cancellationToken);

        return Ok(registros);
    }

    [HttpGet("ObterPontoMes")]
    public async Task<ActionResult<IReadOnlyList<RegistroPontoDto>>> ObterRegistrosDoMes(
        [BindRequired, Range(1, 9999)] int ano,
        [BindRequired, Range(1, 12)] int mes,
        CancellationToken cancellationToken)
    {
        var registros = await _registroPontoService.ObterRegistrosDoMesAsync(UsuarioId, ano, mes, cancellationToken);

        return Ok(registros);
    }
}
