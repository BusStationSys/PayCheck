namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Requests;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class AccessMappingProfile : Profile
    {
        public AccessMappingProfile()
        {
            CreateMap<LoginRequest, LoginViewModel>().ReverseMap();
        }
    }
}