namespace PayCheck.Application.Interfaces.Repository
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Application.Interfaces.Actions;

    /// <summary>
    /// 
    /// </summary>
    public interface IMatriculaEspelhoPontoRepository : ICreateRepository<MatriculaEspelhoPontoEntity, MatriculaEspelhoPontoEntity>, IReadRepository<MatriculaEspelhoPontoEntity, Guid>, IUpdateRepository<MatriculaEspelhoPontoEntity, Guid, MatriculaEspelhoPontoEntity>, IDeleteRepository<Guid>
    {
        void DeleteEventosAndTotalizadoresByCompetenciaAndGuidMatricula(string competencia, Guid guidMatricula);

        IEnumerable<MatriculaEspelhoPontoEntity> Get(string competencia, string matricula);

        IEnumerable<MatriculaEspelhoPontoEntity> GetByGuidColaborador(Guid guidColaborador);

        IEnumerable<MatriculaEspelhoPontoEntity> GetByCompetencia(string competencia);

        IEnumerable<MatriculaEspelhoPontoEntity> GetByMatricula(string matricula);
    }
}