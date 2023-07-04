namespace PayCheck.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DemonstrativoPagamentoViewModel
{
    [Key]
    public Guid? Guid { get; set; }

    [Required]
    public Guid GuidMatricula { get; set; }

    [Required]
    [Display(Name = "Competência")]
    public string? Competencia { get; set; }

    [NotMapped]
    public string MesCompetencia
    {
        get
        {
            if (this.Competencia is not null)
            {
                return this.Competencia.Substring(4, 2);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    [NotMapped]
    public string AnoCompetencia
    {
        get
        {
            if (this.Competencia is not null)
            {
                return this.Competencia.Substring(0, 4);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    [NotMapped]
    public int OrdemCompetencia
    {
        get
        {
            if (this.Competencia is not null)
            {
                return Convert.ToInt32(
                    this.Competencia.Substring(6, 2));
            }
            else
            {
                return 1;
            }
        }
    }
}