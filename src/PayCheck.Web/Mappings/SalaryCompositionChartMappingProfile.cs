namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class SalaryCompositionChartMappingProfile : Profile
    {
        public SalaryCompositionChartMappingProfile()
        {
            CreateMap<GraficoComposicaoSalarialResponse, GraficoComposicaoSalarialViewModel>().ForMember(
                dest => dest.Competencia,
                opt => opt.MapFrom(
                    src => src.CompetenciaFormatada)).ReverseMap();
        }
    }
}