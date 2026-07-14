using Microsoft.EntityFrameworkCore;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Domain.Entities;

namespace PontoEletronico.Infrastructure.Persistence.Repositories;

public class RegistroPontoRepository : IRegistroPontoRepository
{
    private readonly PontoEletronicoDbContext _context;

    public RegistroPontoRepository(PontoEletronicoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(RegistroPonto registroPonto, CancellationToken cancellationToken = default)
    {
        await _context.RegistrosPontos.AddAsync(registroPonto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RegistroPonto?> ObterUltimoRegistroDoDiaAsync(string usuarioId, DateOnly data, CancellationToken cancellationToken = default)
    {
        return await _context.RegistrosPontos
            .AsNoTracking()
            .Where(registro => registro.UsuarioId == usuarioId && registro.DataRegistro == data)
            .OrderByDescending(registro => registro.HoraRegistro)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroPonto>> ObterRegistrosDoDiaAsync(string usuarioId, DateOnly data, CancellationToken cancellationToken = default)
    {
        return await _context.RegistrosPontos
            .AsNoTracking()
            .Where(registro => registro.UsuarioId == usuarioId && registro.DataRegistro == data)
            .OrderBy(registro => registro.HoraRegistro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroPonto>> ObterRegistrosDoMesAsync(string usuarioId, int ano, int mes, CancellationToken cancellationToken = default)
    {
        // Intervalo em vez de DataRegistro.Year/.Month: o EF Core 6 não traduz acesso a membro
        // de DateOnly, que aqui é mapeado por ValueConverter.
        // O limite superior é inclusivo (e não o 1º dia do mês seguinte) para não estourar
        // DateOnly.MaxValue em 9999/12.
        var primeiroDiaDoMes = new DateOnly(ano, mes, 1);
        var ultimoDiaDoMes = new DateOnly(ano, mes, DateTime.DaysInMonth(ano, mes));

        return await _context.RegistrosPontos
            .AsNoTracking()
            .Where(registro => registro.UsuarioId == usuarioId
                            && registro.DataRegistro >= primeiroDiaDoMes
                            && registro.DataRegistro <= ultimoDiaDoMes)
            .OrderBy(registro => registro.DataRegistro)
            .ThenBy(registro => registro.HoraRegistro)
            .ToListAsync(cancellationToken);
    }
}
