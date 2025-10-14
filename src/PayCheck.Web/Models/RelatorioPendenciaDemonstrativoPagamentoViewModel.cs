namespace PayCheck.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using ARVTech.Shared.Enums;
    
    public class RelatorioPendenciaDemonstrativoPagamentoViewModel
    {
        [Display(Name = "Competência Inicial")]
        public string CompetenciaInicial { get; set; }

        [Display(Name = "Competência Final")]
        public string CompetenciaFinal { get; set; }

        [Display(Name = "Situação")]
        public SituacaoPendenciaDemonstrativoPagamento Situacao { get; set; }
    }
}