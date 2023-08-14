namespace PayCheck.Application.Interfaces.Repository
{
    using System;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Application.Interfaces.Actions;

    /// <summary>
    /// 
    /// </summary>
    public interface IMatriculaDemonstrativoPagamentoRepository : ICreateRepository<MatriculaDemonstrativoPagamentoEntity, MatriculaDemonstrativoPagamentoEntity>, IReadRepository<MatriculaDemonstrativoPagamentoEntity, Guid>, IUpdateRepository<MatriculaDemonstrativoPagamentoEntity, Guid, MatriculaDemonstrativoPagamentoEntity>, IDeleteRepository<Guid>
    {
        void DeleteEventosAndTotalizadoresByCompetenciaAndGuidMatricula(string competencia, Guid guidMatricula);

        IEnumerable<MatriculaDemonstrativoPagamentoEntity> Get(string competencia, string matricula);

        IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetByGuidColaborador(Guid guidColaborador);

        IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetByCompetencia(string competencia);

        IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetByMatricula(string matricula);
    }
}