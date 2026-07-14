using AutoMapper;
using FluentAssertions;
using PontoEletronico.Application.Common.Mappings;
using PontoEletronico.Application.RegistrosPonto.Dtos;
using PontoEletronico.Domain.Entities;
using PontoEletronico.Domain.Enums;
using PontoEletronico.UnitTests.Common;
using Xunit;

namespace PontoEletronico.UnitTests.Application;

public class RegistroPontoProfileTests
{
    [Fact]
    public void Configuracao_DeveSerValida()
    {
        var configuracao = new MapperConfiguration(cfg => cfg.AddProfile<RegistroPontoProfile>());

        configuracao.Invoking(c => c.AssertConfigurationIsValid()).Should().NotThrow();
    }

    [Fact]
    public void Map_DeveCopiarTodosOsCamposDaEntidadeParaODto()
    {
        var registro = RegistroPonto.Registrar("usuario-1", new DateTime(2026, 7, 13, 8, 5, 10), null);

        var dto = MapperFactory.Criar().Map<RegistroPontoDto>(registro);

        dto.Id.Should().Be(registro.Id);
        dto.DataRegistro.Should().Be(new DateOnly(2026, 7, 13));
        dto.HoraRegistro.Should().Be(new TimeOnly(8, 5, 10));
        dto.TipoRegistro.Should().Be(TipoRegistroEnum.ENTRADA);
    }
}
