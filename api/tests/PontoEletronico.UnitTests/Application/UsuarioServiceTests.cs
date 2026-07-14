using FluentAssertions;
using Moq;
using PontoEletronico.Application.Common.Exceptions;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Application.Common.Models;
using PontoEletronico.Application.Usuarios.Dtos;
using PontoEletronico.Application.Usuarios.Services;
using Xunit;

namespace PontoEletronico.UnitTests.Application;

public class UsuarioServiceTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly UsuarioService _servico;

    public UsuarioServiceTests()
    {
        _servico = new UsuarioService(_identityService.Object, _tokenService.Object);
    }

    private static CreateUsuarioDto NovoCadastro() => new()
    {
        Username = "alexandre",
        Email = "alexandre@email.com",
        Password = "Senha@123",
        RePassword = "Senha@123"
    };

    private static LoginUsuarioDto NovoLogin() => new()
    {
        Username = "alexandre",
        Password = "Senha@123"
    };

    [Fact]
    public async Task CadastrarUsuarioAsync_QuandoIdentityRetornaSucesso_DeveConcluirSemErro()
    {
        var cadastro = NovoCadastro();
        _identityService
            .Setup(s => s.CadastrarUsuarioAsync(cadastro))
            .ReturnsAsync(ResultadoOperacao.Ok());

        var acao = async () => await _servico.CadastrarUsuarioAsync(cadastro);

        await acao.Should().NotThrowAsync();
        _identityService.Verify(s => s.CadastrarUsuarioAsync(cadastro), Times.Once);
    }

    [Fact]
    public async Task CadastrarUsuarioAsync_QuandoIdentityFalha_DeveLancarRegraDeNegocioComOsErros()
    {
        var erros = new[] { "Senha muito curta.", "E-mail já utilizado." };
        _identityService
            .Setup(s => s.CadastrarUsuarioAsync(It.IsAny<CreateUsuarioDto>()))
            .ReturnsAsync(ResultadoOperacao.Falha(erros));

        var acao = async () => await _servico.CadastrarUsuarioAsync(NovoCadastro());

        var excecao = await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("Falha ao cadastrar usuário.");
        excecao.Which.Erros.Should().BeEquivalentTo(erros);
    }

    [Fact]
    public async Task LoginAsync_QuandoCredenciaisValidas_DeveRetornarTokenGerado()
    {
        var usuario = new UsuarioAutenticado("id-1", "alexandre", "alexandre@email.com");
        var login = NovoLogin();

        _identityService.Setup(s => s.AutenticarAsync(login)).ReturnsAsync(usuario);
        _tokenService.Setup(s => s.GerarToken(usuario)).Returns("token-jwt");

        var resposta = await _servico.LoginAsync(login);

        resposta.Token.Should().Be("token-jwt");
        _tokenService.Verify(s => s.GerarToken(usuario), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_QuandoCredenciaisInvalidas_DeveLancarRegraDeNegocio()
    {
        _identityService
            .Setup(s => s.AutenticarAsync(It.IsAny<LoginUsuarioDto>()))
            .ReturnsAsync((UsuarioAutenticado?)null);

        var acao = async () => await _servico.LoginAsync(NovoLogin());

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("Falha ao realizar login!");
    }

    [Fact]
    public async Task LoginAsync_QuandoAutenticacaoFalha_NaoDeveGerarToken()
    {
        _identityService
            .Setup(s => s.AutenticarAsync(It.IsAny<LoginUsuarioDto>()))
            .ReturnsAsync((UsuarioAutenticado?)null);

        await Assert.ThrowsAsync<RegraDeNegocioException>(() => _servico.LoginAsync(NovoLogin()));

        _tokenService.Verify(s => s.GerarToken(It.IsAny<UsuarioAutenticado>()), Times.Never);
    }
}
