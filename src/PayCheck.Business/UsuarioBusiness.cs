namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
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
                cfg.CreateMap<UsuarioRequestCreateDto, UsuarioEntity>().ReverseMap();
                cfg.CreateMap<UsuarioRequestUpdateDto, UsuarioEntity>().ReverseMap();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDto"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        public UsuarioResponse SaveData(UsuarioRequestCreateDto? createDto = null, UsuarioRequestUpdateDto? updateDto = null)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (createDto != null && updateDto != null)
                    throw new InvalidOperationException($"{nameof(createDto)} e {nameof(updateDto)} não podem estar preenchidos ao mesmo tempo.");
                else if (createDto is null && updateDto is null)
                    throw new InvalidOperationException($"{nameof(createDto)} e {nameof(updateDto)} não podem estar vazios ao mesmo tempo.");
                else if (updateDto != null && updateDto.Guid == Guid.Empty)
                    throw new InvalidOperationException($"É necessário o preenchimento do {nameof(updateDto.Guid)}.");

                var entity = default(
                    UsuarioEntity);

                connection.BeginTransaction();

                if (updateDto != null)
                {
                    updateDto.Password = PasswordCryptography.GetHashMD5(
                        updateDto.Password);

                    entity = this._mapper.Map<UsuarioEntity>(
                        updateDto);

                    entity = connection.Repositories.UsuarioRepository.Update(
                        entity.Guid,
                        entity);
                }
                else if (createDto != null)
                {
                    createDto.Password = PasswordCryptography.GetHashMD5(
                        createDto.Password);

                    entity = this._mapper.Map<UsuarioEntity>(
                        createDto);

                    entity = connection.Repositories.UsuarioRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<UsuarioResponse>(
                    entity);
            }
            catch
            {
                if (connection.Transaction != null)
                {
                    connection.Rollback();
                }

                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}