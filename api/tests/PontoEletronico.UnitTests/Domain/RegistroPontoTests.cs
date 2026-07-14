using FluentAssertions;
using PontoEletronico.Domain.Entities;
using PontoEletronico.Domain.Enums;
using PontoEletronico.Domain.Exceptions;
using Xunit;

namespace PontoEletronico.UnitTests.Domain;

public class RegistroPontoTests
{
    private const string UsuarioId = "usuario-1";

    [Fact]
    public void Registrar_QuandoNaoHaRegistroAnteriorNoDia_DeveCriarEntrada()
    {
        var momento = new DateTime(2026, 7, 13, 8, 30, 0);

        var registro = RegistroPonto.Registrar(UsuarioId, momento, ultimoRegistroDoDia: null);

        registro.TipoRegistro.Should().Be(TipoRegistroEnum.ENTRADA);
    }

    [Fact]
    public void Registrar_QuandoUltimoRegistroEhEntrada_DeveCriarSaida()
    {
        var entrada = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 8, 0, 0), null);

        var saida = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 12, 0, 0), entrada);

        saida.TipoRegistro.Should().Be(TipoRegistroEnum.SAIDA);
    }

    [Fact]
    public void Registrar_QuandoUltimoRegistroEhSaida_DeveCriarEntrada()
    {
        var entrada = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 8, 0, 0), null);
        var saida = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 12, 0, 0), entrada);

        var novaEntrada = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 13, 0, 0), saida);

        novaEntrada.TipoRegistro.Should().Be(TipoRegistroEnum.ENTRADA);
    }

    [Fact]
    public void Registrar_AoLongoDeUmDia_DeveAlternarEntradaESaida()
    {
        var momento = new DateTime(2026, 7, 13, 8, 0, 0);
        RegistroPonto? ultimo = null;
        var tipos = new List<TipoRegistroEnum>();

        for (var i = 0; i < 4; i++)
        {
            ultimo = RegistroPonto.Registrar(UsuarioId, momento.AddHours(i), ultimo);
            tipos.Add(ultimo.TipoRegistro);
        }

        tipos.Should().Equal(
            TipoRegistroEnum.ENTRADA,
            TipoRegistroEnum.SAIDA,
            TipoRegistroEnum.ENTRADA,
            TipoRegistroEnum.SAIDA);
    }

    [Fact]
    public void Registrar_DeveSepararDataEHoraDoMomentoInformado()
    {
        var momento = new DateTime(2026, 7, 13, 17, 45, 30);

        var registro = RegistroPonto.Registrar(UsuarioId, momento, null);

        registro.DataRegistro.Should().Be(new DateOnly(2026, 7, 13));
        registro.HoraRegistro.Should().Be(new TimeOnly(17, 45, 30));
        registro.UsuarioId.Should().Be(UsuarioId);
    }

    [Fact]
    public void Registrar_DeveGerarIdUnicoParaCadaRegistro()
    {
        var momento = new DateTime(2026, 7, 13, 8, 0, 0);

        var primeiro = RegistroPonto.Registrar(UsuarioId, momento, null);
        var segundo = RegistroPonto.Registrar(UsuarioId, momento, primeiro);

        primeiro.Id.Should().NotBeEmpty();
        segundo.Id.Should().NotBe(primeiro.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Registrar_QuandoUsuarioIdInvalido_DeveLancarDomainException(string? usuarioIdInvalido)
    {
        var acao = () => RegistroPonto.Registrar(usuarioIdInvalido!, new DateTime(2026, 7, 13, 8, 0, 0), null);

        acao.Should().Throw<DomainException>()
            .WithMessage("Usuário é obrigatório para registrar o ponto.");
    }
}
