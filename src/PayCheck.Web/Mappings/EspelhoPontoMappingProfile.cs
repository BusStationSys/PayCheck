namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Create;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class EspelhoPontoMappingProfile : Profile
    {
        public EspelhoPontoMappingProfile()
        {
            CreateMap<MatriculaEspelhoPontoCreateRequest, MatriculaEspelhoPontoResponse>().ReverseMap();
            CreateMap<MatriculaEspelhoPontoUpdateRequest, MatriculaEspelhoPontoResponse>().ReverseMap();

            CreateMap<MatriculaEspelhoPontoResponse, EspelhoPontoViewModel>()
                .ForMember(
                    dest => dest.NumeroMatricula,
                    opt => opt.MapFrom(
                        src => src.Matricula.Matricula))
                .ForMember(
                    dest => dest.NomeColaborador,
                    opt => opt.MapFrom(
                        src => src.Matricula.Colaborador.Nome))
                .ForMember(
                    dest => dest.RazaoSocialEmpregador,
                    opt => opt.MapFrom(
                        src => src.Matricula.Empregador.RazaoSocial))
                .ReverseMap();
        }
    }
}