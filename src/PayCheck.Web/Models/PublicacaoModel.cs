namespace PayCheck.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;
using ARVTech.DataAccess.Enums;

public class PublicacaoModel
{
    [Key]
    public int? Id { get; set; }

    [Display(Name = "Título")]
    [Required(ErrorMessage = "É necessário o preenchimento do Título.")]
    [StringLength(150, ErrorMessage = "O Título não pode exceder 150 caracteres.", MinimumLength = 0)]
    public string Titulo { get; set; }

    [DataType(DataType.MultilineText)]
    public string Resumo { get; set; }

    [DataType(DataType.MultilineText)]
    public string Texto { get; set; }

    //[Display(Name = "Extensão da Imagem")]
    //[EnumDataType(typeof(ExtensaoImagemEnum))]
    //public ExtensaoImagemEnum ExtensaoImagem { get; set; }

    [Display(Name = "Caminho da Imagem")]
    public string? NomeImagem { get; set; }

    public byte[]? ConteudoImagem { get; set; }

    //[Display(Name = "Extensão do Arquivo")]
    //[EnumDataType(typeof(ExtensaoArquivoEnum))]
    //public ExtensaoArquivoEnum ExtensaoArquivo { get; set; }

    [Display(Name = "Caminho do Arquivo")]
    public string? NomeArquivo { get; set; }

    public byte[]? ConteudoArquivo { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Apresentação")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    [Required(ErrorMessage = "É necessário o preenchimento da Data de Apresentação.")]
    public DateTime? DataApresentacao { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Validade")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? DataValidade { get; set; }

    [Display(Name = "Ocultar Publicação?")]
    public bool OcultarPublicacao { get; set; }

    public override string ToString()
    {
        return $"Publicação ID: {this.Id}; Título: {this.Titulo}.";
    }
}