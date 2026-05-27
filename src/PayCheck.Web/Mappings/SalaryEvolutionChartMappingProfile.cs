namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class SalaryEvolutionChartMappingProfile : Profile
    {
        public SalaryEvolutionChartMappingProfile()
        {
            CreateMap<GraficoEvolucaoSalarialResponseDto, GraficoEvolucaoSalarialViewModel>().ForMember(
                        dest => dest.Competencia,
                        opt => opt.MapFrom(
                            src => src.CompetenciaFormatada)).ReverseMap();
        }
    }
}