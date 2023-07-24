namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.EquHos;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Business.Interfaces;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;

    public class UsuarioBusiness : BaseBusiness, IUsuarioBusiness
    {
        public UsuarioBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        { 
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsuarioDto, UsuarioEntity>().ReverseMap();
                cfg.CreateMap<UsuarioResponse, UsuarioEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaResponse, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaResponse, PessoaEntity>().ReverseMap();
            });

            this._mapper = new Mapper(mapperConfiguration);
        }

        public UsuarioResponse Authenticate(string cpfEmailUsername, string password)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (string.IsNullOrEmpty(cpfEmailUsername))
                    throw new ArgumentNullException(
                        nameof(cpfEmailUsername));
                else if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException(
                        nameof(password));

                var entity = connection.Repositories.UsuarioRepository.Authenticate(
                    cpfEmailUsername,
                    password);

                return this._mapper.Map<UsuarioResponse>(
                    entity);

            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}