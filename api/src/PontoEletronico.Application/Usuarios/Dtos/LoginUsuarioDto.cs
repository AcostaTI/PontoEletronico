using System.ComponentModel.DataAnnotations;

namespace PontoEletronico.Application.Usuarios.Dtos;

public class LoginUsuarioDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
