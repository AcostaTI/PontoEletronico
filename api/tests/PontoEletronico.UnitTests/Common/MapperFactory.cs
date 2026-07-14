using AutoMapper;
using PontoEletronico.Application.Common.Mappings;

namespace PontoEletronico.UnitTests.Common;

/// <summary>
/// Constrói um IMapper real a partir dos profiles de produção, para que os testes
/// exercitem o mapeamento de verdade em vez de um mock.
/// </summary>
public static class MapperFactory
{
    public static IMapper Criar()
    {
        var configuracao = new MapperConfiguration(cfg => cfg.AddProfile<RegistroPontoProfile>());
        return configuracao.CreateMapper();
    }
}
