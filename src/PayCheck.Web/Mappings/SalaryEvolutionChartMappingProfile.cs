namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class SalaryEvolutionChartMappingProfile : Profile
    {
        public SalaryEvolutionChartMappingProfile()
        {
            CreateMap<GraficoEvolucaoSalarialResponse, GraficoEvolucaoSalarialViewModel>().ForMember(
                        dest => dest.Competencia,
                        opt => opt.MapFrom(
                            src => src.CompetenciaFormatada)).ReverseMap();
        }
    }
}