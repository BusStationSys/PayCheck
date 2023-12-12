namespace PayCheck.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;

public class EspelhoPontoViewModel
{
    [Key]
    public Guid? Guid { get; set; }

    [Display(Name = "Competência")]
    public string Competencia { get; set; }

    [Display(Name = "Colaborador")]
    public string NomeColaborador { get; set; }

    [Display(Name = "Matrícula")]
    public string NumeroMatricula { get; set; }

    [Display(Name = "Empregador")]
    public string RazaoSocialEmpregador { get; set; }
}