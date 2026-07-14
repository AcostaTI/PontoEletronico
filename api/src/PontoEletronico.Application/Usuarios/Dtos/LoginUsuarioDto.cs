using System.ComponentModel.DataAnnotations;

namespace PontoEletronico.Application.Usuarios.Dtos;

public class LoginUsuarioDto
{
    [Required(ErrorMessage = "Informe o nome de usuário.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Password { get; set; } = null!;
}
