namespace PayCheck.Web.Models
{
    public class UsuarioNotificacaoViewModel
    {
        public string Tipo { get; set; }

        public Guid Guid { get; set; }

        public Guid GuidUsuario { get; set; }

        public Guid? GuidMatriculaDemonstrativoPagamento { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataEnvio { get; set; }

        public DateTime? DataLeitura { get; set; }
    }
}