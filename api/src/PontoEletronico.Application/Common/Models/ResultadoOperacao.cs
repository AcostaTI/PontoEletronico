namespace PontoEletronico.Application.Common.Models;

public class ResultadoOperacao
{
    private ResultadoOperacao(bool sucesso, IReadOnlyList<string> erros)
    {
        Sucesso = sucesso;
        Erros = erros;
    }

    public bool Sucesso { get; }
    public IReadOnlyList<string> Erros { get; }

    public static ResultadoOperacao Ok() => new(true, Array.Empty<string>());

    public static ResultadoOperacao Falha(IEnumerable<string> erros) => new(false, erros.ToList());
}
