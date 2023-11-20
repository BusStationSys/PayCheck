namespace PayCheck.Web.Models;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PayCheck.Web.Models.CustomValidationAttribute;

public class PessoaFisicaViewModel
{
    [Key]
    public Guid? Guid { get; set; }

    public Guid? GuidPessoa { get; set; }

    [StringLength(40, ErrorMessage = "O Bairro não pode exceder 40 caracteres.", MinimumLength = 0)]
    public string? Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "É necessário o preenchimento da Cidade.")]
    [StringLength(60, ErrorMessage = "A Cidade não pode exceder 60 caracteres.")]
    public string Cidade { get; set; }

    [Display(Name = "CPF")]
    [Required(ErrorMessage = "É necessário o preenchimento do CPF.")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "O CPF deve conter 14 caracteres.")]
    [CpfValidation(ErrorMessage = "O CPF está inválido.")]
    [DisplayFormat(DataFormatString = "###.###.###-##")]
    public string Cpf { get; set; }

    [Display(Name = "CEP")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#####-###}")]
    [StringLength(9, ErrorMessage = "O Cep não pode exceder 9 caracteres.", MinimumLength = 0)]
    public string? Cep { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Nascimento")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    [Required(ErrorMessage = "É necessário o preenchimento da Data de Nascimento.")]
    public DateTime? DataNascimento { get; set; }

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

    [Required(ErrorMessage = "É necessário o preenchimento do Nome.")]
    [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Display(Name = "Número da CTPS")]
    [Required(ErrorMessage = "É necessário o preenchimento do Número CTPS.")]
    [StringLength(7, ErrorMessage = "O Número da CTPS não pode exceder 7 caracteres.", MinimumLength = 1)]
    public string NumeroCtps { get; set; }

    [Display(Name = "RG")]
    [StringLength(20, ErrorMessage = "O RG não pode exceder 20 caracteres.", MinimumLength = 0)]
    public string? Rg { get; set; }

    [Display(Name = "Série da CTPS")]
    [Required(ErrorMessage = "É necessário o preenchimento da Série CTPS.")]
    [StringLength(4, ErrorMessage = "A Série da CTPS não pode exceder 4 caracteres.", MinimumLength = 1)]
    public string SerieCtps { get; set; }

    [StringLength(30, ErrorMessage = "O Telefone não pode exceder 30 caracteres.", MinimumLength = 0)]
    public string? Telefone { get; set; }

    [Display(Name = "UF")]
    [Required(ErrorMessage = "É necessário o preenchimento da UF.")]
    [StringLength(2, ErrorMessage = "A UF deve ter 2 caracteres.", MinimumLength = 2)]
    public string Uf { get; set; }

    [Display(Name = "UF da CTPS")]
    [StringLength(2, ErrorMessage = "A UF da CTPS deve ter 2 caracteres.", MinimumLength = 2)]
    public string UfCtps { get; set; }
}