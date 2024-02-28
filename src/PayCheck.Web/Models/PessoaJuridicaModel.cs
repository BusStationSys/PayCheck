namespace PayCheck.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;
using PayCheck.Web.Models.CustomValidationAttribute;

public class PessoaJuridicaModel
{
    [Key]
    public Guid? Guid { get; set; }

    public Guid? GuidPessoa { get; set; }

    [StringLength(40, ErrorMessage = "O Bairro não pode exceder 40 caracteres.", MinimumLength = 0)]
    public string? Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "É necessário o preenchimento da Cidade.")]
    [StringLength(60, ErrorMessage = "A Cidade não pode exceder 60 caracteres.")]
    public string Cidade { get; set; }

    [Display(Name = "CNPJ")]
    [Required(ErrorMessage = "É necessário o preenchimento do CNPJ.")]
    [StringLength(18, MinimumLength = 18, ErrorMessage = "O CNPJ deve conter 18 caracteres.")]
    [CnpjValidation(ErrorMessage = "O CNPJ está inválido.")]
    [DisplayFormat(DataFormatString = "##.###.###/####-##")]
    public string Cnpj { get; set; }

    [Display(Name = "CEP")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#####-###}")]
    [StringLength(9, ErrorMessage = "O Cep não pode exceder 9 caracteres.", MinimumLength = 0)]
    public string? Cep { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fundação")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    [Required(ErrorMessage = "É necessário o preenchimento da Data de Fundação.")]
    public DateTime? DataFundacao { get; set; }

    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-mail")]
    [StringLength(75, ErrorMessage = "O E-Mail não pode exceder 75 caracteres.", MinimumLength = 0)]
    public string? Email { get; set; }

    [Display(Name = "Endereço")]
    [Required(ErrorMessage = "É necessário o preenchimento do Endereco.")]
    [StringLength(100, ErrorMessage = "O Endereço não pode exceder 100 caracteres.")]
    public string Endereco { get; set; }

    [Display(Name = "Número")]
    [StringLength(10, ErrorMessage = "O Número não pode exceder 10 caracteres.", MinimumLength = 0)]
    public string? Numero { get; set; }

    [StringLength(30, ErrorMessage = "O Complemento não pode exceder 30 caracteres.", MinimumLength = 0)]
    public string? Complemento { get; set; }

    [Display(Name = "Razão Social")]
    [Required(ErrorMessage = "É necessário o preenchimento do Razão Social.")]
    [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres.")]
    public string RazaoSocial { get; set; }

    [StringLength(30, ErrorMessage = "O Telefone não pode exceder 30 caracteres.", MinimumLength = 0)]
    public string? Telefone { get; set; }

    [Display(Name = "UF")]
    [Required(ErrorMessage = "É necessário o preenchimento da UF.")]
    [StringLength(2, ErrorMessage = "A UF deve ter 2 caracteres.", MinimumLength = 2)]
    public string Uf { get; set; }
}