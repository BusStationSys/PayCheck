namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class SalaryCompositionChartMappingProfile : Profile
    {
        public SalaryCompositionChartMappingProfile()
        {
            CreateMap<GraficoComposicaoSalarialResponseDto, GraficoComposicaoSalarialViewModel>().ForMember(
                dest => dest.Competencia,
                opt => opt.MapFrom(
                    src => src.CompetenciaFormatada)).ReverseMap();
        }
    }
}