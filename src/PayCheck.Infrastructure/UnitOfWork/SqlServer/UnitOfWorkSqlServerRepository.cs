namespace PayCheck.Infrastructure.UnitOfWork.SqlServer
{
    using System.Data.SqlClient;
    using PayCheck.Application.Interfaces.Repository;
    using PayCheck.Infrastructure.Repository.SqlServer;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;

    public class UnitOfWorkSqlServerRepository : IUnitOfWorkRepository
    {
        public IMatriculaDemonstrativoPagamentoRepository MatriculaDemonstrativoPagamentoRepository { get; }

        public IPessoaFisicaRepository PessoaFisicaRepository { get; }

        public IUsuarioRepository UsuarioRepository { get; }

        //public UnitOfWorkSqlServerRepository(SqlConnection connection)
        //{
        //    this.MatriculaDemonstrativoPagamentoRepository = new MatriculaDemonstrativoPagamentoRepository(
        //        connection);

        //    this.UsuarioRepository = new UsuarioRepository(
        //        connection);
        //}

        public UnitOfWorkSqlServerRepository(SqlConnection connection, SqlTransaction? transaction = null)
        {
            this.MatriculaDemonstrativoPagamentoRepository = new MatriculaDemonstrativoPagamentoRepository(
                connection,
                transaction);

            this.PessoaFisicaRepository = new PessoaFisicaRepository(
                connection,
                transaction);

            this.UsuarioRepository = new UsuarioRepository(
                connection,
                transaction);
        }
    }
}