namespace UniPayCheck.CadastroApi.Models;

using System.ComponentModel.DataAnnotations;

public class PessoaFisica
{
    [Key]
    public int IdPessoaFisica { get; set; }

    public int IdPessoa { get; set; }

    public Pessoa? Pessoa { get; set; }

    public string? Cpf { get; set; }

    public string? Rg { get; set; }

    public string? Nome { get; set; }

    public string? Apelido { get; set; }

    public DateTime? DataNascimento { get; set; }

    public Usuario? Usuario { get; set; }
}
