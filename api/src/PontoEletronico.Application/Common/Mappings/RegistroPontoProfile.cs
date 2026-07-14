using AutoMapper;
using PontoEletronico.Application.RegistrosPonto.Dtos;
using PontoEletronico.Domain.Entities;

namespace PontoEletronico.Application.Common.Mappings;

public class RegistroPontoProfile : Profile
{
    public RegistroPontoProfile()
    {
        CreateMap<RegistroPonto, RegistroPontoDto>();
    }
}
