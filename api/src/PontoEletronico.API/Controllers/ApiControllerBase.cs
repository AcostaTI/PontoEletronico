using Microsoft.AspNetCore.Mvc;
using PontoEletronico.Application.Common.Exceptions;
using System.Security.Claims;

namespace PontoEletronico.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Id do usuário autenticado, extraído do claim "id" do token.
    /// </summary>
    protected string UsuarioId =>
        User.FindFirstValue("id") ?? throw new UsuarioNaoAutenticadoException();
}
