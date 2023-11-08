namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;
    using PayCheck.Business.Interfaces;

    public class MatriculaEspelhoPontoBusiness : BaseBusiness, IMatriculaEspelhoPontoBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MatriculaEspelhoPontoBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CalculoRequestDto, CalculoEntity>().ReverseMap();
                cfg.CreateMap<CalculoResponseDto, CalculoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoCalculoRequestDto, MatriculaEspelhoPontoCalculoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoCalculoResponseDto, MatriculaEspelhoPontoCalculoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoMarcacaoResponseDto, MatriculaEspelhoPontoMarcacaoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoMarcacaoRequestDto, MatriculaEspelhoPontoMarcacaoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoMarcacaoResponseDto, MatriculaEspelhoPontoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoRequestUpdateDto, MatriculaEspelhoPontoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoResponseDto, MatriculaEspelhoPontoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaRequestDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<MatriculaResponseDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaResponseDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaResponseDto, PessoaJuridicaEntity>().ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);
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

                connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.Delete(
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

                connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.DeleteCalculosAndMarcacoesByCompetenciaAndGuidMatricula(
                    competencia,
                    guidMatricula);

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
        public MatriculaEspelhoPontoResponseDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.Get(
                        guid);

                    return this._mapper.Map<MatriculaEspelhoPontoResponseDto>(
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
        public IEnumerable<MatriculaEspelhoPontoResponseDto> Get(string competencia, string matricula)
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
                    var entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.Get(
                        competencia,
                        matricula);

                    return this._mapper.Map<IEnumerable<MatriculaEspelhoPontoResponseDto>>(entity);
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
        public IEnumerable<MatriculaEspelhoPontoResponseDto> GetAll()
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.GetAll();

                    return this._mapper.Map<IEnumerable<MatriculaEspelhoPontoResponseDto>>(entity);
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
        public IEnumerable<MatriculaEspelhoPontoResponseDto> GetByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.GetByGuidColaborador(
                        guidColaborador);

                    return this._mapper.Map<IEnumerable<MatriculaEspelhoPontoResponseDto>>(entity);
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
        public MatriculaEspelhoPontoResponseDto SaveData(MatriculaEspelhoPontoRequestCreateDto? createDto = null, MatriculaEspelhoPontoRequestUpdateDto? updateDto = null)
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
                    MatriculaEspelhoPontoEntity);

                connection.BeginTransaction();

                if (updateDto != null)
                {

                    entity = this._mapper.Map<MatriculaEspelhoPontoEntity>(
                        updateDto);

                    entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.Update(
                        entity.Guid,
                        entity);
                }
                else if (createDto != null)
                {
                    entity = this._mapper.Map<MatriculaEspelhoPontoEntity>(
                        createDto);

                    entity = connection.RepositoriesUniPayCheck.MatriculaEspelhoPontoRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<MatriculaEspelhoPontoResponseDto>(
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
    }
}