namespace UniPayCheck.CadastroApi.Models;

using System.ComponentModel.DataAnnotations;

public class PessoaJuridica
{
    [Key]
    public int IdPessoaJuridica { get; set; }

    public int IdPessoa { get; set; }

    public Pessoa? Pessoa { get; set; }

    public string? Cnpj { get; set; }

    public string? RazaoSocial { get; set; }

    public string? NomeFantasia { get; set; }

    public DateTime? DataFundacao { get; set; }
}
