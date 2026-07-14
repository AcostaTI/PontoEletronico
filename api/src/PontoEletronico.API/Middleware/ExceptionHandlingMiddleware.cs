using Microsoft.AspNetCore.Mvc;
using PontoEletronico.Application.Common.Exceptions;
using PontoEletronico.Domain.Exceptions;
using System.Net;

namespace PontoEletronico.API.Middleware;

/// <summary>
/// Traduz exceções das camadas internas em respostas HTTP, mantendo os controllers livres de try/catch.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UsuarioNaoAutenticadoException excecao)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.Unauthorized, excecao.Message);
        }
        catch (RegraDeNegocioException excecao)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, excecao.Message, excecao.Erros);
        }
        catch (DomainException excecao)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, excecao.Message);
        }
        catch (Exception excecao)
        {
            _logger.LogError(excecao, "Erro não tratado ao processar {Path}", context.Request.Path);
            await EscreverRespostaAsync(context, HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado.");
        }
    }

    private static Task EscreverRespostaAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string mensagem,
        IReadOnlyList<string>? erros = null)
    {
        var problema = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = mensagem
        };

        if (erros is { Count: > 0 })
            problema.Extensions["erros"] = erros;

        context.Response.StatusCode = problema.Status.Value;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsJsonAsync(problema);
    }
}
