using Microsoft.AspNetCore.Identity;

namespace PontoEletronico.Infrastructure.Identity;

/// <summary>
/// Usuário do ASP.NET Identity. É um detalhe de infraestrutura e por isso não vive no Domain.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public ApplicationUser() : base() { }
}
