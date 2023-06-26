namespace UniPayCheck.CadastroApi.Models;

using System.ComponentModel.DataAnnotations;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    public int IdPessoaFisica { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }

    public string UserName { get; set; }
}

