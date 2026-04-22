namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class ColaboradorMappingProfile : Profile
    {
        public ColaboradorMappingProfile()
        {
            CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaResponseDto>().ReverseMap();
            CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaResponseDto>().ReverseMap();
            CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaModel>().ReverseMap();
            CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaModel>().ReverseMap();
            CreateMap<PessoaFisicaResponseDto, PessoaFisicaModel>().ReverseMap();
        }
    }
}