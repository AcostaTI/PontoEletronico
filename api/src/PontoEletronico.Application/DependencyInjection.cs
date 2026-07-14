using Microsoft.Extensions.DependencyInjection;
using PontoEletronico.Application.RegistrosPonto.Interfaces;
using PontoEletronico.Application.RegistrosPonto.Services;
using PontoEletronico.Application.Usuarios.Interfaces;
using PontoEletronico.Application.Usuarios.Services;
using System.Reflection;

namespace PontoEletronico.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRegistroPontoService, RegistroPontoService>();

        return services;
    }
}
