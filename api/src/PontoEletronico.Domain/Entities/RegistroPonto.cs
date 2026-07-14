using PontoEletronico.Domain.Enums;
using PontoEletronico.Domain.Exceptions;

namespace PontoEletronico.Domain.Entities;

public class RegistroPonto
{
    private RegistroPonto() { }

    private RegistroPonto(string usuarioId, DateOnly dataRegistro, TimeOnly horaRegistro, TipoRegistroEnum tipoRegistro)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        DataRegistro = dataRegistro;
        HoraRegistro = horaRegistro;
        TipoRegistro = tipoRegistro;
    }

    public Guid Id { get; private set; }
    public DateOnly DataRegistro { get; private set; }
    public TimeOnly HoraRegistro { get; private set; }
    public TipoRegistroEnum TipoRegistro { get; private set; }
    public string UsuarioId { get; private set; } = null!;

    public static RegistroPonto Registrar(string usuarioId, DateTime momento, RegistroPonto? ultimoRegistroDoDia)
    {
        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new DomainException("Usuário é obrigatório para registrar o ponto.");

        var tipoRegistro = ProximoTipoRegistro(ultimoRegistroDoDia);

        return new RegistroPonto(
            usuarioId,
            DateOnly.FromDateTime(momento),
            TimeOnly.FromDateTime(momento),
            tipoRegistro);
    }

    private static TipoRegistroEnum ProximoTipoRegistro(RegistroPonto? ultimoRegistroDoDia)
    {
        if (ultimoRegistroDoDia is null)
            return TipoRegistroEnum.ENTRADA;

        return ultimoRegistroDoDia.TipoRegistro == TipoRegistroEnum.ENTRADA
            ? TipoRegistroEnum.SAIDA
            : TipoRegistroEnum.ENTRADA;
    }
}
