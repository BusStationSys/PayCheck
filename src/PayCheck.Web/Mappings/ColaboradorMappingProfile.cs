namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Create;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class ColaboradorMappingProfile : Profile
    {
        public ColaboradorMappingProfile()
        {
            CreateMap<PessoaFisicaCreateRequest, PessoaFisicaResponse>().ReverseMap();
            CreateMap<PessoaFisicaUpdateRequest, PessoaFisicaResponse>().ReverseMap();
            CreateMap<PessoaFisicaCreateRequest, PessoaFisicaModel>().ReverseMap();
            CreateMap<PessoaFisicaUpdateRequest, PessoaFisicaModel>().ReverseMap();

            CreateMap<PessoaFisicaResponse, PessoaFisicaModel>().AfterMap(
                (src, dest) =>
                {
                    if (src.Pessoa is null)
                        return;

                    dest.Bairro = src.Pessoa.Bairro;
                    dest.Cep = src.Pessoa.Cep;
                    dest.Cidade = src.Pessoa.Cidade;
                    dest.Complemento = src.Pessoa.Complemento;
                    dest.Email = src.Pessoa.Email;
                    dest.Endereco = src.Pessoa.Endereco;
                    dest.Numero = src.Pessoa.Numero;
                    dest.Telefone = src.Pessoa.Telefone;
                    dest.Uf = src.Pessoa.Uf;
                }).ReverseMap();
        }
    }
}