namespace UniPayCheck.CadastroApi.Models;

using System.ComponentModel.DataAnnotations;

public class Pessoa
{
    [Key]
    public int IdPessoa { get; set; }

    public string? Bairro { get; set; }

    public string? Cep { get; set; }

    public string? Cidade { get; set; }

    public string? Complemento { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Uf { get; set; }

    public ICollection<PessoaFisica>? PessoasFisicas { get; set; }

    public ICollection<PessoaJuridica>? PessoasJuridicas { get; set; }
}