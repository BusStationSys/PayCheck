namespace PayCheck.Infrastructure.UnitOfWork.SqlServer
{
    using System;
    using Microsoft.Extensions.Configuration;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;

    public class UnitOfWorkSqlServer : IUnitOfWork
    {
        private readonly IConfiguration _configuration;

        public UnitOfWorkSqlServer(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IUnitOfWorkAdapter Create()
        {
            try
            {
                if (this._configuration == null)
                {
                    throw new Exception("String de Conexão não encontrada.");
                }

                //  string connectionString = this._configuration.GetValue<string>("ConnectionStrings:SqlServer");

                //  return new UnitOfWorkSqlServerAdapter(
                //        connectionString);

                return new UnitOfWorkSqlServerAdapter(
                    this._configuration);
            }
            catch
            {
                throw;
            }
        }
    }
}