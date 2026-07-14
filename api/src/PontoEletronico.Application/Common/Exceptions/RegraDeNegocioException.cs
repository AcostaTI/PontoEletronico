namespace PontoEletronico.Application.Common.Exceptions;

public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem, IEnumerable<string>? erros = null) : base(mensagem)
    {
        Erros = erros?.ToList() ?? new List<string>();
    }

    public IReadOnlyList<string> Erros { get; }
}
