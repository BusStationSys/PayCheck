namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Create;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class EmpregadorMappingProfile : Profile
    {
        public EmpregadorMappingProfile()
        {
            CreateMap<PessoaJuridicaCreateRequest, PessoaJuridicaResponse>().ReverseMap();
            CreateMap<PessoaJuridicaUpdateRequest, PessoaJuridicaResponse>().ReverseMap();
            CreateMap<PessoaJuridicaCreateRequest, PessoaJuridicaViewModel>().ReverseMap();
            CreateMap<PessoaJuridicaUpdateRequest, PessoaJuridicaViewModel>().ReverseMap();

            CreateMap<MatriculaDemonstrativoPagamentoResponse, DemonstrativoPagamentoViewModel>()
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

            CreateMap<PessoaJuridicaResponse, PessoaJuridicaViewModel>()
                .ForMember(
                    dest => dest.DescricaoUnidadeNegocio,
                    opt => opt.MapFrom(
                        src => src.UnidadeNegocio.Descricao))
                .AfterMap((src, dest) =>
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
                })
                .ReverseMap();
        }
    }
}