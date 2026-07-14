using System.ComponentModel.DataAnnotations;

namespace PontoEletronico.Application.Usuarios.Dtos;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "Informe o nome de usuário.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirme a senha.")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não conferem.")]
    public string RePassword { get; set; } = null!;
}
