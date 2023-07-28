namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using AutoMapper;
    using PayCheck.Business.Interfaces;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;

    public class UsuarioBusiness : BaseBusiness, IUsuarioBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UsuarioResponse CheckPasswordValid(Guid guid, string password)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));
                else if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException(
                        nameof(password));

                var entity = connection.Repositories.UsuarioRepository.CheckPasswordValid(
                    guid,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpfEmailUsername"></param>
        /// <returns></returns>
        public UsuarioResponse GetByUsername(string cpfEmailUsername)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (string.IsNullOrEmpty(cpfEmailUsername))
                    throw new ArgumentNullException(
                        nameof(cpfEmailUsername));

                var entity = connection.Repositories.UsuarioRepository.GetByUsername(
                    cpfEmailUsername);

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