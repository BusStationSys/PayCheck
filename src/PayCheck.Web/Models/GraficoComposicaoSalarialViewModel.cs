namespace PayCheck.Web.Models
{
    public class GraficoComposicaoSalarialViewModel
    {
        public Guid GuidColaborador { get; set; }

        public string Competencia { get; set; }

        public decimal Valor { get; set; }

        public string Matricula { get; set; }

        public string DescricaoEvento { get; set; }

        public string Tipo { get; set; }

        public string Cor { get; set; }
    }
}