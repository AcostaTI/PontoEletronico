using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoEletronico.Domain.Entities;

namespace PontoEletronico.Infrastructure.Persistence.Configurations;

public class RegistroPontoConfiguration : IEntityTypeConfiguration<RegistroPonto>
{
    public void Configure(EntityTypeBuilder<RegistroPonto> builder)
    {
        builder.ToTable("RegistrosPontos");

        builder.HasKey(registro => registro.Id);

        builder.Property(registro => registro.UsuarioId)
               .IsRequired();

        builder.Property(registro => registro.TipoRegistro)
               .IsRequired();

        builder.HasIndex(registro => new { registro.UsuarioId, registro.DataRegistro });
    }
}
