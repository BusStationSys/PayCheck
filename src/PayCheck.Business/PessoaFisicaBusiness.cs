namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;
    using PayCheck.Business.Interfaces;

    public class PessoaFisicaBusiness : BaseBusiness, IPessoaFisicaBusiness
    {
        // To detect redundant calls.
        private bool _disposedValue = false;

        public PessoaFisicaBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaRequestCreateDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaRequestUpdateDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaResponseDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaResponseDto, PessoaFisicaEntity>().ReverseMap();
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

                connection.RepositoriesUniPayCheck.PessoaFisicaRepository.Delete(
                    guid);

                connection.CommitTransaction();
            }
            catch
            {
                if (connection.Transaction != null)
                    connection.Rollback();

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
        public PessoaFisicaResponseDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.PessoaFisicaRepository.Get(
                        guid);

                    return this._mapper.Map<PessoaFisicaResponseDto>(entity);
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
        public IEnumerable<PessoaFisicaResponseDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.PessoaFisicaRepository.GetAll();

                    return this._mapper.Map<IEnumerable<PessoaFisicaResponseDto>>(entity);
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
        /// <param name="createDto"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        public PessoaFisicaResponseDto SaveData(PessoaFisicaRequestCreateDto? createDto = null, PessoaFisicaRequestUpdateDto? updateDto = null)
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
                    PessoaFisicaEntity);

                connection.BeginTransaction();

                if (updateDto != null)
                {
                    entity = this._mapper.Map<PessoaFisicaEntity>(
                        updateDto);

                    entity = connection.RepositoriesUniPayCheck.PessoaFisicaRepository.Update(
                        entity.Guid,
                        entity);
                }
                else if (createDto != null)
                {
                    entity = this._mapper.Map<PessoaFisicaEntity>(
                        createDto);

                    entity = connection.RepositoriesUniPayCheck.PessoaFisicaRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<PessoaFisicaResponseDto>(
                    entity);
            }
            catch
            {
                if (connection.Transaction != null)
                    connection.Rollback();

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