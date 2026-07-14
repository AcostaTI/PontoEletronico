using FluentAssertions;
using Moq;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Application.RegistrosPonto.Services;
using PontoEletronico.Domain.Entities;
using PontoEletronico.Domain.Enums;
using PontoEletronico.UnitTests.Common;
using Xunit;

namespace PontoEletronico.UnitTests.Application;

public class RegistroPontoServiceTests
{
    private const string UsuarioId = "usuario-1";

    private static readonly DateTime Agora = new(2026, 7, 13, 9, 15, 0);
    private static readonly DateOnly Hoje = new(2026, 7, 13);

    private readonly Mock<IRegistroPontoRepository> _repositorio = new(MockBehavior.Strict);
    private readonly FakeDateTimeProvider _relogio = new(Agora);
    private readonly RegistroPontoService _servico;

    public RegistroPontoServiceTests()
    {
        _servico = new RegistroPontoService(_repositorio.Object, _relogio, MapperFactory.Criar());
    }

    private static RegistroPonto CriarEntrada(DateTime momento) =>
        RegistroPonto.Registrar(UsuarioId, momento, ultimoRegistroDoDia: null);

    [Fact]
    public async Task RegistrarPontoAsync_QuandoPrimeiroPontoDoDia_DevePersistirUmaEntrada()
    {
        RegistroPonto? persistido = null;

        _repositorio
            .Setup(r => r.ObterUltimoRegistroDoDiaAsync(UsuarioId, Hoje, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegistroPonto?)null);
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<RegistroPonto>(), It.IsAny<CancellationToken>()))
            .Callback<RegistroPonto, CancellationToken>((registro, _) => persistido = registro)
            .Returns(Task.CompletedTask);

        var dto = await _servico.RegistrarPontoAsync(UsuarioId);

        persistido.Should().NotBeNull();
        persistido!.TipoRegistro.Should().Be(TipoRegistroEnum.ENTRADA);
        persistido.UsuarioId.Should().Be(UsuarioId);
        _repositorio.VerifyAll();

        // O DTO retornado precisa refletir a entidade que foi persistida.
        dto.Id.Should().Be(persistido.Id);
        dto.TipoRegistro.Should().Be(TipoRegistroEnum.ENTRADA);
        dto.DataRegistro.Should().Be(Hoje);
        dto.HoraRegistro.Should().Be(new TimeOnly(9, 15, 0));
    }

    [Fact]
    public async Task RegistrarPontoAsync_QuandoUltimoRegistroEhEntrada_DevePersistirUmaSaida()
    {
        _repositorio
            .Setup(r => r.ObterUltimoRegistroDoDiaAsync(UsuarioId, Hoje, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarEntrada(new DateTime(2026, 7, 13, 8, 0, 0)));
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<RegistroPonto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = await _servico.RegistrarPontoAsync(UsuarioId);

        dto.TipoRegistro.Should().Be(TipoRegistroEnum.SAIDA);
        _repositorio.Verify(
            r => r.AdicionarAsync(
                It.Is<RegistroPonto>(p => p.TipoRegistro == TipoRegistroEnum.SAIDA),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarPontoAsync_DeveUsarADataDoRelogioAoConsultarOUltimoRegistro()
    {
        _relogio.Now = new DateTime(2026, 12, 31, 23, 59, 0);

        _repositorio
            .Setup(r => r.ObterUltimoRegistroDoDiaAsync(UsuarioId, new DateOnly(2026, 12, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegistroPonto?)null);
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<RegistroPonto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = await _servico.RegistrarPontoAsync(UsuarioId);

        dto.DataRegistro.Should().Be(new DateOnly(2026, 12, 31));
        _repositorio.VerifyAll();
    }

    [Fact]
    public async Task RegistrarPontoAsync_DevePropagarOCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        _repositorio
            .Setup(r => r.ObterUltimoRegistroDoDiaAsync(UsuarioId, Hoje, cts.Token))
            .ReturnsAsync((RegistroPonto?)null);
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<RegistroPonto>(), cts.Token))
            .Returns(Task.CompletedTask);

        await _servico.RegistrarPontoAsync(UsuarioId, cts.Token);

        _repositorio.VerifyAll();
    }

    [Fact]
    public async Task ObterRegistrosDoDiaAsync_DeveMapearOsRegistrosDoDiaCorrente()
    {
        var entrada = CriarEntrada(new DateTime(2026, 7, 13, 8, 0, 0));
        var saida = RegistroPonto.Registrar(UsuarioId, new DateTime(2026, 7, 13, 12, 0, 0), entrada);

        _repositorio
            .Setup(r => r.ObterRegistrosDoDiaAsync(UsuarioId, Hoje, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RegistroPonto> { entrada, saida });

        var dtos = await _servico.ObterRegistrosDoDiaAsync(UsuarioId);

        dtos.Should().HaveCount(2);
        dtos.Select(d => d.TipoRegistro).Should().Equal(TipoRegistroEnum.ENTRADA, TipoRegistroEnum.SAIDA);
        dtos.Select(d => d.HoraRegistro).Should().Equal(new TimeOnly(8, 0, 0), new TimeOnly(12, 0, 0));
        _repositorio.VerifyAll();
    }

    [Fact]
    public async Task ObterRegistrosDoDiaAsync_QuandoNaoHaRegistros_DeveRetornarListaVazia()
    {
        _repositorio
            .Setup(r => r.ObterRegistrosDoDiaAsync(UsuarioId, Hoje, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<RegistroPonto>());

        var dtos = await _servico.ObterRegistrosDoDiaAsync(UsuarioId);

        dtos.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterRegistrosDoMesAsync_DeveRepassarAnoEMesParaORepositorio()
    {
        var entrada = CriarEntrada(new DateTime(2026, 6, 2, 8, 0, 0));

        _repositorio
            .Setup(r => r.ObterRegistrosDoMesAsync(UsuarioId, 2026, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RegistroPonto> { entrada });

        var dtos = await _servico.ObterRegistrosDoMesAsync(UsuarioId, 2026, 6);

        dtos.Should().ContainSingle()
            .Which.DataRegistro.Should().Be(new DateOnly(2026, 6, 2));
        _repositorio.VerifyAll();
    }
}
