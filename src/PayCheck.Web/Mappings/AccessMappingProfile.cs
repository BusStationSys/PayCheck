namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class AccessMappingProfile : Profile
    {
        public AccessMappingProfile()
        {
            CreateMap<LoginRequestDto, LoginViewModel>().ReverseMap();
        }
    }
}