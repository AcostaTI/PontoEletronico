using PontoEletronico.Application.RegistrosPonto.Dtos;

namespace PontoEletronico.Application.RegistrosPonto.Interfaces;

public interface IRegistroPontoService
{
    Task<RegistroPontoDto> RegistrarPontoAsync(string usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroPontoDto>> ObterRegistrosDoDiaAsync(string usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroPontoDto>> ObterRegistrosDoMesAsync(string usuarioId, int ano, int mes, CancellationToken cancellationToken = default);
}
