using PontoEletronico.Domain.Entities;

namespace PontoEletronico.Application.Common.Interfaces;

public interface IRegistroPontoRepository
{
    Task AdicionarAsync(RegistroPonto registroPonto, CancellationToken cancellationToken = default);
    Task<RegistroPonto?> ObterUltimoRegistroDoDiaAsync(string usuarioId, DateOnly data, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroPonto>> ObterRegistrosDoDiaAsync(string usuarioId, DateOnly data, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroPonto>> ObterRegistrosDoMesAsync(string usuarioId, int ano, int mes, CancellationToken cancellationToken = default);
}
