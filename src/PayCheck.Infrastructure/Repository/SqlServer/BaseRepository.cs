namespace PayCheck.Infrastructure.Repository.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Dapper;

    public abstract class BaseRepository : IDisposable
    {
        protected readonly string TableAliasEventos = "E";
        protected readonly string TableNameEventos = "EVENTOS";

        protected readonly string TableAliasMatriculas = "M";
        protected readonly string TableNameMatriculas = "MATRICULAS";

        protected readonly string TableAliasMatriculasDemonstrativosPagamento = "MDP";
        protected readonly string TableNameMatriculasDemonstrativosPagamento = "MATRICULAS_DEMONSTRATIVOS_PAGAMENTO";

        protected readonly string TableAliasMatriculasDemonstrativosPagamentoEventos = "MDPE";
        protected readonly string TableNameMatriculasDemonstrativosPagamentoEventos = "MATRICULAS_DEMONSTRATIVOS_PAGAMENTO_EVENTOS";

        protected readonly string TableAliasMatriculasDemonstrativosPagamentoTotalizadores = "MDPT";
        protected readonly string TableNameMatriculasDemonstrativosPagamentoTotalizadores = "MATRICULAS_DEMONSTRATIVOS_PAGAMENTO_TOTALIZADORES";

        protected readonly string TableAliasMatriculasEspelhosPonto = "MEP";
        protected readonly string TableNameMatriculasEspelhosPonto = "MATRICULAS_ESPELHOS_PONTO";

        protected readonly string TableAliasPessoas = "P";
        protected readonly string TableNamePessoas = "PESSOAS";

        protected readonly string TableAliasPessoasFisicas = "PF";
        protected readonly string TableNamePessoasFisicas = "PESSOAS_FISICAS";

        protected readonly string TableAliasPessoasJuridicas = "PJ";
        protected readonly string TableNamePessoasJuridicas = "PESSOAS_JURIDICAS";

        protected readonly string TableAliasTotalizadores = "T";
        protected readonly string TableNameTotalizadores = "TOTALIZADORES";

        private bool _disposedValue = false;    //  To detect redundant calls.

        protected readonly string ParameterSymbol = "@";

        protected SqlConnection _connection = null;

        protected SqlTransaction _transaction = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        protected BaseRepository(SqlConnection connection)
        {
            this._connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        protected BaseRepository(SqlConnection connection, SqlTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected DataTable GetDataTableFromDataAdapter(IDbCommand command)
        {
            try
            {
                if (command == null)
                    throw new ArgumentNullException(nameof(command));

                using (SqlCommand sqlCommand = new SqlCommand(
                    command.CommandText.ToString(),
                    this._connection))
                {
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                    {
                        adapter.Fill(dataTable);
                    }

                    return dataTable;
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
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected SqlCommand CreateCommand(string cmdText, CommandType commandType = CommandType.Text, SqlParameter[] parameters = null)
        {
            try
            {
                SqlCommand command = new SqlCommand(
                    cmdText,
                    this._connection,
                    this._transaction)
                {

                    CommandTimeout = 0,
                    CommandType = commandType,
                };

                if (parameters != null &&
                    parameters.Any())
                    foreach (var parameter in parameters)
                        command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);

                return command;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateDataParameter(string parameterName, object value)
        {
            return new SqlParameter()
            {
                ParameterName = parameterName,
                Value = (value ?? DBNull.Value),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected SqlParameter[] GetDataParameters<T>(T entity) where T : class
        {
            IList<SqlParameter> dataParameters = null;

            foreach (var property in entity.GetType().GetProperties())
            {
                if (dataParameters == null)
                    dataParameters = new List<SqlParameter>();

                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                string itemName = property.Name;

                if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                    itemName = columnAttribute.Name.ToUpper();

                string parameterName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    this.ParameterSymbol,
                    itemName);

                object parameterValue = property.GetValue(
                    entity,
                    null);

                SqlParameter item = this.CreateDataParameter(
                    parameterName,
                    parameterValue);

                dataParameters.Add(item);
            }

            return dataParameters.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="alias"></param>
        /// <param name="fieldsToIgnore"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected string GetAllColumnsFromTable(string tableName, string alias = "", string fieldsToIgnore = "")
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(
                    nameof(tableName));

            StringBuilder sbColumns = new StringBuilder();

            string cmdText = $@" SELECT TOP 0 *
                                   FROM [{this._connection.Database}].[dbo].[{tableName}]
                                  WHERE 0 = 1 ";

            using (var reader = this.CreateCommand(
                cmdText).ExecuteReader())
            {
                reader.Read();

                using (var schemaTable = reader.GetSchemaTable())
                {
                    foreach (DataRow column in schemaTable.Rows)
                    {
                        if (sbColumns.Length > 0)
                        {
                            sbColumns.Append(", ");
                        }

                        if (!string.IsNullOrEmpty(alias))
                        {
                            sbColumns.AppendFormat(
                                CultureInfo.InvariantCulture,
                                "{0}.",
                                alias);
                        }

                        sbColumns.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "[{0}]",
                            column["ColumnName"].ToString());
                    }
                }

                reader.Close();
            }

            string columns = sbColumns.ToString();

            if (!string.IsNullOrEmpty(fieldsToIgnore))
            {
                foreach (var fieldToIgnore in fieldsToIgnore.Split(';'))
                {
                    if (string.IsNullOrEmpty(fieldToIgnore))
                        continue;

                    columns = columns.Replace(fieldToIgnore, string.Empty).Trim();

                    columns = columns.Replace(", ,", ",").Trim();   // Vírgulas no meio.

                    if (columns.StartsWith(","))                    // Vírgulas no início.
                    {
                        columns = columns.Substring(1).Trim();
                    }

                    if (columns.EndsWith(","))                      // Vírgulas no fim.
                    {
                        columns = columns.Substring(0, columns.Length - 1).Trim();
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        protected void MapAttributeToField(Type entityType)
        {
            var map = new CustomPropertyTypeMap(
                entityType,
                (type, columnName) => type.GetProperties().FirstOrDefault(prop => this.GetDescriptionFromAttribute(prop) == columnName));

            SqlMapper.SetTypeMap(entityType, map);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private string GetDescriptionFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
            return descriptionAttribute?.Description;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    if (this._connection != null &&
                        this._connection.State == ConnectionState.Open)
                    {
                        this._connection.Dispose();
                        this._connection = null;
                    }

                    if (this._transaction != null)
                    {
                        this._transaction.Dispose();
                        this._transaction = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this._disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}