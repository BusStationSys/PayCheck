namespace PayCheck.Infrastructure.Repository.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using ARVTech.Shared;
    using Dapper;
    using PayCheck.Application.Interfaces.Repository;

    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        private readonly string _columnsPessoas;
        private readonly string _columnsPessoasFisicas;
        private readonly string _columnsUsuarios;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="UsuarioRepository"/> class.
        ///// </summary>
        ///// <param name="connection"></param>
        //public UsuarioRepository(SqlConnection connection) :
        //    base(connection)
        //{
        //    this._connection = connection;

        //    this.MapAttributeToField(
        //        typeof(
        //            UsuarioEntity));

        //    this.MapAttributeToField(
        //        typeof(
        //            PessoaFisicaEntity));

        //    this._columnsUsuarios = base.GetAllColumnsFromTable(
        //        "USUARIOS",
        //        "U");

        //    this._columnsPessoas = base.GetAllColumnsFromTable(
        //        "PESSOAS",
        //        "P");

        //    this._columnsPessoasFisicas = base.GetAllColumnsFromTable(
        //        "PESSOAS_FISICAS",
        //        "PF");
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="UsuarioRepository"/> class.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public UsuarioRepository(SqlConnection connection, SqlTransaction? transaction) :
            base(connection, transaction)
        {
            this._connection = connection;
            this._transaction = transaction;

            this.MapAttributeToField(
                typeof(
                    UsuarioEntity));

            this.MapAttributeToField(
                typeof(
                    PessoaEntity));

            this.MapAttributeToField(
                typeof(
                    PessoaFisicaEntity));

            this._columnsUsuarios = base.GetAllColumnsFromTable(
                "USUARIOS",
                "U");

            this._columnsPessoas = base.GetAllColumnsFromTable(
                "PESSOAS",
                "P");

            this._columnsPessoasFisicas = base.GetAllColumnsFromTable(
                "PESSOAS_FISICAS",
                "PF");
        }

        /// <summary>
        /// Checks if the Username and Password match the registration in the "Usuários" table.
        /// </summary>
        /// <param name="cpfEmailUsername">CPF, Email or Username values.</param>
        /// <param name="password">Password value.</param>
        /// <returns>If success, the duly authenticated object. Otherwise, an exception is generated stating what happened.</returns>
        public UsuarioEntity Authenticate(string cpfEmailUsername, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(cpfEmailUsername))
                    throw new ArgumentNullException(
                        nameof(
                            cpfEmailUsername));
                else if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException(
                        nameof(
                            password));

                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = @"      SELECT TOP 1 {0},
                                                      {1},
                                                      {2}
                                           FROM [{3}].[dbo].[USUARIOS] AS U WITH(NOLOCK)
                                     INNER JOIN [{3}].[dbo].[PESSOAS_FISICAS] as PF WITH(NOLOCK)
                                             ON [U].[GUIDCOLABORADOR] = [PF].[GUID]
                                     INNER JOIN [{3}].[dbo].[PESSOAS] as P WITH(NOLOCK)
                                             ON [PF].[GUIDPESSOA] = [P].[GUID]
                                          WHERE ( LOWER(PF.CPF) = {4}Filtro
                                             OR LOWER(P.Email) = {4}Filtro
                                             OR LOWER(U.USERNAME) = {4}Filtro )  ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsUsuarios,
                    this._columnsPessoasFisicas,
                    this._columnsPessoas,
                    base._connection?.Database,
                    base.ParameterSymbol);

                var usuarioEntity = base._connection.Query<UsuarioEntity, PessoaFisicaEntity, PessoaEntity, UsuarioEntity>(
                    cmdText,
                    map: (mapUsuario, mapPessoaFisica, mapPessoa) =>
                    {
                        mapUsuario.Colaborador = mapPessoaFisica;
                        mapUsuario.Colaborador.Pessoa = mapPessoa;

                        return mapUsuario;
                    },
                    param: new
                    {
                        Filtro = cpfEmailUsername,
                    },
                    splitOn: "GUID,GUID,GUID",
                    transaction: this._transaction).FirstOrDefault();

                if (usuarioEntity != null)
                {
                    return this.CheckPasswordValid(
                        usuarioEntity.Guid,
                        password);
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UsuarioEntity Create(UsuarioEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the "Usuário" record.
        /// </summary>
        /// <param name="guid">Guid of "Usuário" record.</param>
        public void Delete(Guid guid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the "Usuário" record.
        /// </summary>
        /// <param name="guid">Guid of "Usuário" record.</param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public UsuarioEntity Get(Guid guid)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = @"      SELECT {0},
                                                {1},
                                                {2}
                                           FROM [{3}].[dbo].[USUARIOS] AS U WITH(NOLOCK)
                                     INNER JOIN [{3}].[dbo].[PESSOAS_FISICAS] as PF WITH(NOLOCK)
                                             ON [U].[GUIDCOLABORADOR] = [PF].[GUID]
                                     INNER JOIN [{3}].[dbo].[PESSOAS] as P WITH(NOLOCK)
                                             ON [PF].[GUIDPESSOA] = [P].[GUID]
                                          WHERE U.[GUID] = {4}Guid ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsUsuarios,
                    this._columnsPessoasFisicas,
                    this._columnsPessoas,
                    base._connection?.Database,
                    base.ParameterSymbol);

                var usuarioEntity = base._connection.Query<UsuarioEntity, PessoaFisicaEntity, PessoaEntity, UsuarioEntity>(
                    cmdText,
                    map: (mapUsuario, mapPessoaFisica, mapPessoa) =>
                    {
                        mapUsuario.Colaborador = mapPessoaFisica;
                        mapUsuario.Colaborador.Pessoa = mapPessoa;

                        return mapUsuario;
                    },
                    param: new
                    {
                        Guid = guid,
                    },
                    splitOn: "GUID,GUID,GUID",
                    transaction: this._transaction);

                return usuarioEntity.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Usuários" records.
        /// </summary>
        /// <returns>If success, the list with all "Usuários" records. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<UsuarioEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UsuarioEntity Update(Guid guid, UsuarioEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if the "Password" of "Usuário" record is valid.
        /// </summary>
        /// <param name="guid">Guid of "Usuário" record.</param>
        /// <param name="password">"Password" of "Usuário" record.</param>
        /// <returns>If success, the Entity with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        private UsuarioEntity CheckPasswordValid(Guid guid, string password)
        {
            try
            {
                string passwordQuery = PasswordCryptography.GetHashMD5(password);

                string cmdText = @" SELECT TOP 1 Guid
                                      FROM [{0}].[dbo].USUARIOS
                                     WHERE GUID = {1}Guid
                                       AND SENHA = {1}PasswordQuery COLLATE SQL_Latin1_General_CP1_CS_AS ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection?.Database,
                    base.ParameterSymbol);

                var usuarioEntity = base._connection.QueryFirstOrDefault(
                    cmdText,
                    param: new
                    {
                        Guid = guid,
                        PasswordQuery = passwordQuery,
                    });

                if (usuarioEntity != null)
                {
                    return this.Get(
                        ((UsuarioEntity)usuarioEntity).Guid);
                }
                else
                {
                    throw new Exception("CPF, Email, Username e Senha não conferem, verifique.");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}