namespace PayCheck.Business
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;

    public class PessoaFisicaBusiness : BaseBusiness
    {
        // To detect redundant calls.
        private bool _disposedValue = false;

        public PessoaFisicaBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaDto, PessoaFisicaEntity>().ReverseMap();
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

                connection.Repositories.PessoaFisicaRepository.Delete(
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
        public PessoaFisicaDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaFisicaRepository.Get(
                        guid);

                    return this._mapper.Map<PessoaFisicaDto>(entity);
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
        public IEnumerable<PessoaFisicaDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaFisicaRepository.GetAll();

                    return this._mapper.Map<IEnumerable<PessoaFisicaDto>>(entity);
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
        /// <param name="nome"></param>
        /// <returns></returns>
        public PessoaFisicaDto GetByNome(string nome)
        {
            try
            {
                if (string.IsNullOrEmpty(nome))
                    throw new ArgumentNullException(
                        nameof(
                            nome));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaFisicaRepository.GetByNome(
                        nome);

                    return this._mapper.Map<PessoaFisicaDto>(entity);
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
        /// <param name="nome"></param>
        /// <param name="numeroCtps"></param>
        /// <param name="serieCtps"></param>
        /// <param name="ufCtps"></param>
        /// <returns></returns>
        public PessoaFisicaDto GetByNomeNumeroCtpsSerieCtpsAndUfCtps(string nome, string numeroCtps, string serieCtps, string ufCtps)
        {
            try
            {
                if (string.IsNullOrEmpty(nome))
                    throw new ArgumentNullException(
                        nameof(
                            nome));
                else if (string.IsNullOrEmpty(numeroCtps))
                    throw new ArgumentNullException(
                        nameof(
                            numeroCtps));
                else if (string.IsNullOrEmpty(serieCtps))
                    throw new ArgumentNullException(
                        nameof(
                            serieCtps));
                else if (string.IsNullOrEmpty(ufCtps))
                    throw new ArgumentNullException(
                        nameof(
                            ufCtps));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.PessoaFisicaRepository.GetByNomeNumeroCtpsSerieCtpsAndUfCtps(
                        nome,
                        numeroCtps,
                        serieCtps,
                        ufCtps);

                    return this._mapper.Map<PessoaFisicaDto>(entity);
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
        public PessoaFisicaDto SaveData(PessoaFisicaDto dto)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                var entity = this._mapper.Map<PessoaFisicaEntity>(dto);

                connection.BeginTransaction();

                if (dto.Guid != null && dto.Guid != Guid.Empty)
                {
                    entity = connection.Repositories.PessoaFisicaRepository.Update(
                        entity);
                }
                else
                {
                    entity.Pessoa = connection.Repositories.PessoaRepository.Create(
                        entity.Pessoa);

                    entity.GuidPessoa = entity.Pessoa.Guid;

                    entity = connection.Repositories.PessoaFisicaRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<PessoaFisicaDto>(
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