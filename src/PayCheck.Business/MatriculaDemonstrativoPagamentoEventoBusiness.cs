namespace PayCheck.Business
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;

    public class MatriculaDemonstrativoPagamentoEventoBusiness : BaseBusiness
    {
        public MatriculaDemonstrativoPagamentoEventoBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaDemonstrativoPagamentoEventoDto, MatriculaDemonstrativoPagamentoEventoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoDto, MatriculaDemonstrativoPagamentoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<EventoDto, EventoEntity>().ReverseMap();
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

                connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.Delete(
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
        public MatriculaDemonstrativoPagamentoEventoDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.Get(
                        guid);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoEventoDto>(
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
        public IEnumerable<MatriculaDemonstrativoPagamentoEventoDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.GetAll();

                    return this._mapper.Map<IEnumerable<MatriculaDemonstrativoPagamentoEventoDto>>(
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
        /// <param name="idEvento"></param>
        /// <returns></returns>
        public MatriculaDemonstrativoPagamentoEventoDto GetByGuidMatriculaDemonstrativoPagamentoAndIdEvento(Guid guidMatriculaDemonstrativoPagamento, int idEvento)
        {
            try
            {
                if (guidMatriculaDemonstrativoPagamento == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guidMatriculaDemonstrativoPagamento));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.GetByGuidMatriculaDemonstrativoPagamentoAndIdEvento(
                        guidMatriculaDemonstrativoPagamento,
                        idEvento);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoEventoDto>(
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
        public MatriculaDemonstrativoPagamentoEventoDto SaveData(MatriculaDemonstrativoPagamentoEventoDto dto)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                var entity = this._mapper.Map<MatriculaDemonstrativoPagamentoEventoEntity>(dto);

                connection.BeginTransaction();

                if (dto.Guid != null && dto.Guid != Guid.Empty)
                {
                    string x = string.Empty;

                    //entity = connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.Update(
                    //    entity);
                }
                else
                {
                    entity = connection.Repositories.MatriculaDemonstrativoPagamentoEventoRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<MatriculaDemonstrativoPagamentoEventoDto>(
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