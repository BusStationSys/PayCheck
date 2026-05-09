namespace PayCheck.Web.Models
{
    using System;

    public class GraficoEvolucaoSalarialViewModel
    {
        public Guid GuidColaborador { get; set; }

        public string Competencia { get; set; }

        public decimal Valor { get; set; }
    }
}
