namespace PayCheck.Infrastructure.UnitOfWork.Interfaces
{
    using System;
    using System.Data;

    public interface IUnitOfWorkAdapter : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        IUnitOfWorkRepository Repositories { get; }

        string ConnectionString { get; }

        void BeginTransaction();

        void CommitTransaction();

        void Rollback();
    }
}
