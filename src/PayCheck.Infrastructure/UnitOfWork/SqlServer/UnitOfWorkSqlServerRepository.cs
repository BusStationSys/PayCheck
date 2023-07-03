namespace PayCheck.Infrastructure.UnitOfWork.SqlServer
{
    using System.Data.SqlClient;
    using PayCheck.Application.Interfaces.Repository;
    using PayCheck.Infrastructure.Repository.SqlServer;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;

    public class UnitOfWorkSqlServerRepository : IUnitOfWorkRepository
    {
        public IMatriculaDemonstrativoPagamentoRepository MatriculaDemonstrativoPagamentoRepository { get; }

        public UnitOfWorkSqlServerRepository(SqlConnection connection)
        {
            this.MatriculaDemonstrativoPagamentoRepository = new MatriculaDemonstrativoPagamentoRepository(
                connection);
        }

        public UnitOfWorkSqlServerRepository(SqlConnection connection, SqlTransaction transaction)
        {
            this.MatriculaDemonstrativoPagamentoRepository = new MatriculaDemonstrativoPagamentoRepository(
                connection,
                transaction);
        }
    }
}