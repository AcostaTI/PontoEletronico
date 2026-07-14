namespace PontoEletronico.Application.Common.Exceptions;

public class UsuarioNaoAutenticadoException : Exception
{
    public UsuarioNaoAutenticadoException() : base("Usuário não identificado no token.") { }
}
