using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PontoEletronico.Application.Common.Interfaces;
using PontoEletronico.Infrastructure.Common;
using PontoEletronico.Infrastructure.Identity;
using PontoEletronico.Infrastructure.Persistence;
using PontoEletronico.Infrastructure.Persistence.Repositories;

namespace PontoEletronico.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PontoEletronicoConnection")
            ?? throw new InvalidOperationException("Connection string 'PontoEletronicoConnection' não encontrada.");

        services.AddDbContext<PontoEletronicoDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(PontoEletronicoDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<PontoEletronicoDbContext>()
                .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRegistroPontoRepository, RegistroPontoRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
