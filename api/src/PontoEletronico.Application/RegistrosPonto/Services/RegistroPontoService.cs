using AutoMapper;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Application.RegistrosPonto.Dtos;
using PontoEletronico.Application.RegistrosPonto.Interfaces;
using PontoEletronico.Domain.Entities;

namespace PontoEletronico.Application.RegistrosPonto.Services;

public class RegistroPontoService : IRegistroPontoService
{
    private readonly IRegistroPontoRepository _registroPontoRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public RegistroPontoService(
        IRegistroPontoRepository registroPontoRepository,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _registroPontoRepository = registroPontoRepository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<RegistroPontoDto> RegistrarPontoAsync(string usuarioId, CancellationToken cancellationToken = default)
    {
        var agora = _dateTimeProvider.Now;
        var hoje = DateOnly.FromDateTime(agora);

        var ultimoRegistro = await _registroPontoRepository.ObterUltimoRegistroDoDiaAsync(usuarioId, hoje, cancellationToken);

        var registroPonto = RegistroPonto.Registrar(usuarioId, agora, ultimoRegistro);

        await _registroPontoRepository.AdicionarAsync(registroPonto, cancellationToken);

        return _mapper.Map<RegistroPontoDto>(registroPonto);
    }

    public async Task<IReadOnlyList<RegistroPontoDto>> ObterRegistrosDoDiaAsync(string usuarioId, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(_dateTimeProvider.Now);

        var registros = await _registroPontoRepository.ObterRegistrosDoDiaAsync(usuarioId, hoje, cancellationToken);

        return _mapper.Map<IReadOnlyList<RegistroPontoDto>>(registros);
    }

    public async Task<IReadOnlyList<RegistroPontoDto>> ObterRegistrosDoMesAsync(string usuarioId, int ano, int mes, CancellationToken cancellationToken = default)
    {
        var registros = await _registroPontoRepository.ObterRegistrosDoMesAsync(usuarioId, ano, mes, cancellationToken);
        return _mapper.Map<IReadOnlyList<RegistroPontoDto>>(registros);
    }
}
