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

            CreateMap<PessoaFisicaResponseDto, PessoaFisicaModel>().AfterMap(
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