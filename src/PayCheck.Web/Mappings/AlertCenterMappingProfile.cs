namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class AlertCenterMappingProfile : Profile
    {
        public AlertCenterMappingProfile()
        {
            CreateMap<MatriculaDemonstrativoPagamentoResponse, DemonstrativoPagamentoViewModel>().ForMember(
                dest => dest.NumeroMatricula,
                opt => opt.MapFrom(
                    src => src.Matricula.Matricula)).ForMember(
                dest => dest.NomeColaborador,
                opt => opt.MapFrom(
                    src => src.Matricula.Colaborador.Nome)).ForMember(
                dest => dest.RazaoSocialEmpregador,
                opt => opt.MapFrom(
                    src => src.Matricula.Empregador.RazaoSocial)).ReverseMap();

            CreateMap<UsuarioNotificacaoResponse, UsuarioNotificacaoViewModel>().ReverseMap();
        }
    }
}