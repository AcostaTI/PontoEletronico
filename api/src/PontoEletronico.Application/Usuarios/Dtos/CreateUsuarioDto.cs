using System.ComponentModel.DataAnnotations;

namespace PontoEletronico.Application.Usuarios.Dtos;

public class CreateUsuarioDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [Compare(nameof(Password))]
    public string RePassword { get; set; } = null!;
}
