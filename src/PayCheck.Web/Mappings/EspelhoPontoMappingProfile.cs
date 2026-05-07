namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class EspelhoPontoMappingProfile : Profile
    {
        public EspelhoPontoMappingProfile()
        {
            CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponseDto>().ReverseMap();
            CreateMap<MatriculaEspelhoPontoRequestUpdateDto, MatriculaEspelhoPontoResponseDto>().ReverseMap();

            CreateMap<MatriculaEspelhoPontoResponseDto, EspelhoPontoViewModel>()
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