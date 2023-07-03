namespace PayCheck.Infrastructure.UnitOfWork.SqlServer
{
    using Microsoft.Extensions.Configuration;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class UnitOfWorkSqlServerAdapter : IUnitOfWorkAdapter
    {
        private readonly SqlConnection _connection = null;

        private SqlTransaction _transaction = null;

        private readonly string _connectionString;

        public IDbConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        public IUnitOfWorkRepository Repositories { get; set; }

        //public UnitOfWorkSqlServerAdapter(string connectionString = "")
        //{
        //    this._connectionString = connectionString;

        //    this._connection = new SqlConnection(
        //        this._connectionString);

        //    this._connection.Open();

        //    this.Repositories = new UnitOfWorkSqlServerRepository(
        //        this._connection);
        //}

        public UnitOfWorkSqlServerAdapter(IConfiguration configuration)
        {
            this._connectionString = configuration.GetValue<string>("ConnectionStrings:SqlServer");

            this._connection = new SqlConnection(
                this._connectionString);

            this._connection.Open();

            this.Repositories = new UnitOfWorkSqlServerRepository(
                this._connection);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (this._transaction != null)
            {
                this._transaction.Dispose();

                this._transaction = null;
            }

            if (this._connection != null)
            {
                if (this._connection.State == ConnectionState.Open)
                {
                    this._connection.Close();
                }

                this._connection.Dispose();
            }

            this.Repositories = null;
        }

        public void BeginTransaction()
        {
            this._transaction = this._connection.BeginTransaction();

            this.Repositories = new UnitOfWorkSqlServerRepository(
                this._connection,
                this._transaction);
        }

        public void CommitTransaction()
        {
            this._transaction.Commit();

            this._transaction.Dispose();
            this._transaction = null;
        }

        public void Rollback()
        {
            this._transaction.Rollback();

            this._transaction.Dispose();
            this._transaction = null;
        }

        ~UnitOfWorkSqlServerAdapter()
        {
            this.Dispose(false);
        }
    }
}
