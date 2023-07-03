namespace PayCheck.Business
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;

    public class PessoaJuridicaBusiness : BaseBusiness
    {
        // To detect redundant calls.
        private bool _disposedValue = false;

        public PessoaJuridicaBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaDto, PessoaJuridicaEntity>().ReverseMap();
            });

            this._mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        public void Delete(Guid guid)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                connection.BeginTransaction();

                connection.Repositories.PessoaJuridicaRepository.Delete(
                    guid);

                connection.CommitTransaction();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PessoaJuridicaDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaJuridicaRepository.Get(
                        guid);

                    return this._mapper.Map<PessoaJuridicaDto>(entity);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PessoaJuridicaDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaJuridicaRepository.GetAll();

                    return this._mapper.Map<IEnumerable<PessoaJuridicaDto>>(entity);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="razaoSocial"></param>
        /// <returns></returns>
        public PessoaJuridicaDto GetByRazaoSocial(string razaoSocial)
        {
            try
            {
                if (string.IsNullOrEmpty(razaoSocial))
                    throw new ArgumentNullException(
                        nameof(
                            razaoSocial));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaJuridicaRepository.GetByRazaoSocial(
                        razaoSocial);

                    return this._mapper.Map<PessoaJuridicaDto>(entity);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="razaoSocial"></param>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public PessoaJuridicaDto GetByRazaoSocialAndCnpj(string razaoSocial, string cnpj)
        {
            try
            {
                if (string.IsNullOrEmpty(razaoSocial))
                    throw new ArgumentNullException(
                        nameof(
                            razaoSocial));
                else if (string.IsNullOrEmpty(cnpj))
                    throw new ArgumentNullException(
                        nameof(
                            cnpj));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaJuridicaRepository.GetByRazaoSocialAndCnpj(
                        razaoSocial,
                        cnpj);

                    return this._mapper.Map<PessoaJuridicaDto>(entity);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PessoaJuridicaDto SaveData(PessoaJuridicaDto dto)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                var entity = this._mapper.Map<PessoaJuridicaEntity>(dto);

                connection.BeginTransaction();

                if (dto.Guid != null && dto.Guid != Guid.Empty)
                {
                    entity = connection.Repositories.PessoaJuridicaRepository.Update(
                        entity);
                }
                else
                {
                    entity.Pessoa = connection.Repositories.PessoaRepository.Create(
                        entity.Pessoa);

                    entity.GuidPessoa = entity.Pessoa.Guid;

                    entity = connection.Repositories.PessoaJuridicaRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<PessoaJuridicaDto>(
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

        // Protected implementation of Dispose pattern. https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        protected override void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    //  TODO: dispose managed state (managed objects).
                }

                this._disposedValue = true;
            }

            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}