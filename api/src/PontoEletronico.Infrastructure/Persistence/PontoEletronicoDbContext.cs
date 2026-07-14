using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PontoEletronico.Domain.Entities;
using PontoEletronico.Infrastructure.Identity;
using PontoEletronico.Infrastructure.Persistence.Conversions;
using System.Reflection;

namespace PontoEletronico.Infrastructure.Persistence;

public class PontoEletronicoDbContext : IdentityDbContext<ApplicationUser>
{
    public PontoEletronicoDbContext(DbContextOptions<PontoEletronicoDbContext> options)
        : base(options)
    {
    }

    public DbSet<RegistroPonto> RegistrosPontos => Set<RegistroPonto>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        // Configura o DateOnly para mapear para o tipo 'date' do SQL Server
        builder.Properties<DateOnly>()
               .HaveConversion<DateOnlyConverter>()
               .HaveColumnType("date");

        // Configura o TimeOnly para mapear para o tipo 'time' do SQL Server
        builder.Properties<TimeOnly>()
               .HaveConversion<TimeOnlyConverter>()
               .HaveColumnType("time");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
