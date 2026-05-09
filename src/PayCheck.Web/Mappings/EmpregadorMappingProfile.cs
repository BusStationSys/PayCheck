namespace PayCheck.Web.Mappings
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Web.Models;

    public class EmpregadorMappingProfile : Profile
    {
        public EmpregadorMappingProfile()
        {
            CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaResponseDto>().ReverseMap();
            CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaResponseDto>().ReverseMap();
            CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaViewModel>().ReverseMap();
            CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaViewModel>().ReverseMap();

            CreateMap<MatriculaDemonstrativoPagamentoResponseDto, DemonstrativoPagamentoViewModel>()
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

            CreateMap<PessoaJuridicaResponseDto, PessoaJuridicaViewModel>()
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