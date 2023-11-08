namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;
    using PayCheck.Business.Interfaces;

    public class MatriculaDemonstrativoPagamentoBusiness : BaseBusiness, IMatriculaDemonstrativoPagamentoBusiness
    {
        public MatriculaDemonstrativoPagamentoBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventoRequestDto, EventoEntity>().ReverseMap();
                cfg.CreateMap<EventoResponseDto, EventoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoRequestCreateDto, MatriculaDemonstrativoPagamentoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoRequestUpdateDto, MatriculaDemonstrativoPagamentoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoResponseDto, MatriculaDemonstrativoPagamentoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoEventoRequestDto, MatriculaDemonstrativoPagamentoEventoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoEventoResponseDto, MatriculaDemonstrativoPagamentoEventoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoTotalizadorRequestDto, MatriculaDemonstrativoPagamentoTotalizadorEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoTotalizadorResponseDto, MatriculaDemonstrativoPagamentoTotalizadorEntity>().ReverseMap();
                cfg.CreateMap<MatriculaRequestDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<MatriculaResponseDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaResponseDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaResponseDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaRequestCreateDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaRequestUpdateDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<PessoaResponseDto, PessoaEntity>().ReverseMap();
                cfg.CreateMap<TotalizadorRequestDto, TotalizadorEntity>().ReverseMap();
                cfg.CreateMap<TotalizadorResponseDto, TotalizadorEntity>().ReverseMap();
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

                connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.Delete(
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
        /// <param name="competencia"></param>
        /// <param name="guidMatricula"></param>
        public void Delete(string competencia, Guid guidMatricula)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (string.IsNullOrEmpty(competencia))
                    throw new ArgumentNullException(
                        nameof(competencia));
                else if (guidMatricula == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guidMatricula));

                connection.BeginTransaction();

                connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.DeleteEventosAndTotalizadoresByCompetenciaAndGuidMatricula(
                    competencia,
                    guidMatricula);

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
        public MatriculaDemonstrativoPagamentoResponseDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.Get(
                        guid);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoResponseDto>(
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
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns></returns>
        public IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> Get(string competencia, string matricula)
        {
            try
            {
                if (string.IsNullOrEmpty(competencia))
                    throw new ArgumentNullException(
                        nameof(
                            competencia));
                else if (string.IsNullOrEmpty(matricula))
                    throw new ArgumentNullException(
                        nameof(
                            matricula));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.Get(
                        competencia,
                        matricula);

                    return this._mapper.Map<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>(entity);
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
        public IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.GetAll();

                    return this._mapper.Map<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>(entity);
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
        public IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> GetByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.GetByGuidColaborador(
                        guidColaborador);

                    return this._mapper.Map<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>(entity);
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
        public MatriculaDemonstrativoPagamentoResponseDto SaveData(MatriculaDemonstrativoPagamentoRequestCreateDto? createDto = null, MatriculaDemonstrativoPagamentoRequestUpdateDto? updateDto = null)
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
                    MatriculaDemonstrativoPagamentoEntity);

                connection.BeginTransaction();

                if (updateDto != null)
                {
                    entity = this._mapper.Map<MatriculaDemonstrativoPagamentoEntity>(
                        updateDto);

                    entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.Update(
                        entity.Guid,
                        entity);
                }
                else if (createDto != null)
                {
                    entity = this._mapper.Map<MatriculaDemonstrativoPagamentoEntity>(
                        createDto);

                    entity = connection.RepositoriesUniPayCheck.MatriculaDemonstrativoPagamentoRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<MatriculaDemonstrativoPagamentoResponseDto>(
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