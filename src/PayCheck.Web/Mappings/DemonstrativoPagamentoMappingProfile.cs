namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class DemonstrativoPagamentoMappingProfile : Profile
    {
        public DemonstrativoPagamentoMappingProfile()
        {
            CreateMap<MatriculaDemonstrativoPagamentoRequestCreateDto, MatriculaDemonstrativoPagamentoResponseDto>().ReverseMap();
            CreateMap<MatriculaDemonstrativoPagamentoRequestUpdateDto, MatriculaDemonstrativoPagamentoResponseDto>().ReverseMap();
            CreateMap<MatriculaDemonstrativoPagamentoResponseDto, DemonstrativoPagamentoViewModel>().ForMember(
                dest => dest.NumeroMatricula,
                opt => opt.MapFrom(
                    src => src.Matricula.Matricula)).ForMember(
                dest => dest.NomeColaborador,
                opt => opt.MapFrom(
                    src => src.Matricula.Colaborador.Nome)).ForMember(
                dest => dest.RazaoSocialEmpregador,
                opt => opt.MapFrom(
                    src => src.Matricula.Empregador.RazaoSocial)).ReverseMap();
        }
    }
}