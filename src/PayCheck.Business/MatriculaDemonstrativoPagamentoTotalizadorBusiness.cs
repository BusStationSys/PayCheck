namespace PayCheck.Business
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;

    public class MatriculaDemonstrativoPagamentoTotalizadorBusiness : BaseBusiness
    {
        public MatriculaDemonstrativoPagamentoTotalizadorBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaDemonstrativoPagamentoTotalizadorDto, MatriculaDemonstrativoPagamentoTotalizadorEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoDto, MatriculaDemonstrativoPagamentoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<TotalizadorDto, TotalizadorEntity>().ReverseMap();
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

                connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.Delete(
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
        public MatriculaDemonstrativoPagamentoTotalizadorDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.Get(
                        guid);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoTotalizadorDto>(
                        entity);
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
        public IEnumerable<MatriculaDemonstrativoPagamentoTotalizadorDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.GetAll();

                    return this._mapper.Map<IEnumerable<MatriculaDemonstrativoPagamentoTotalizadorDto>>(
                        entity);
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
        /// <param name="guidMatriculaDemonstrativoPagamento"></param>
        /// <param name="idTotalizador"></param>
        /// <returns></returns>
        public MatriculaDemonstrativoPagamentoTotalizadorDto GetByGuidMatriculaDemonstrativoPagamentoAndIdTotalizador(Guid guidMatriculaDemonstrativoPagamento, int idTotalizador)
        {
            try
            {
                if (guidMatriculaDemonstrativoPagamento == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guidMatriculaDemonstrativoPagamento));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.GetByGuidMatriculaDemonstrativoPagamentoAndIdTotalizador(
                        guidMatriculaDemonstrativoPagamento,
                        idTotalizador);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoTotalizadorDto>(
                        entity);
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
        public MatriculaDemonstrativoPagamentoTotalizadorDto SaveData(MatriculaDemonstrativoPagamentoTotalizadorDto dto)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                var entity = this._mapper.Map<MatriculaDemonstrativoPagamentoTotalizadorEntity>(dto);

                connection.BeginTransaction();

                if (dto.Guid != null && dto.Guid != Guid.Empty)
                {
                    string x = string.Empty;

                    //entity = connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.Update(
                    //    entity);
                }
                else
                {
                    entity = connection.Repositories.MatriculaDemonstrativoPagamentoTotalizadorRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<MatriculaDemonstrativoPagamentoTotalizadorDto>(
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