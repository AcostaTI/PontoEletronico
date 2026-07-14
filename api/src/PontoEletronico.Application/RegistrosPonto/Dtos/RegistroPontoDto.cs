using PontoEletronico.Domain.Enums;

namespace PontoEletronico.Application.RegistrosPonto.Dtos;

public class RegistroPontoDto
{
    public Guid Id { get; set; }
    public DateOnly DataRegistro { get; set; }
    public TimeOnly HoraRegistro { get; set; }
    public TipoRegistroEnum TipoRegistro { get; set; }
}
