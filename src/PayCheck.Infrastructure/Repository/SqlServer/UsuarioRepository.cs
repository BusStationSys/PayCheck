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
        /// Creates the "Usuário" record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UsuarioEntity Create(UsuarioEntity entity)
        {
            try
            {
                string cmdText = @"     DECLARE @NewGuidUsuario UniqueIdentifier
                                            SET @NewGuidUsuario = NEWID()

                                    INSERT INTO [{0}].[dbo].[USUARIOS]
                                                ([GUID],
                                                 [GUIDCOLABORADOR],
                                                 [EMAIL],
                                                 [USERNAME],
                                                 [PASSWORD],
                                                 [IDASPNETUSER],
                                                 [DATA_PRIMEIRO_ACESSO],
                                                 [DATA_INCLUSAO])
                                         VALUES (@NewGuidUsuario,
                                                 {1}GuidColaborador,
                                                 {1}Email,
                                                 {1}Username,
                                                 {1}Password,
                                                 {1}IdAspNetUser,
                                                 {1}DataPrimeiroAcesso,
                                                 GETDATE())

                                          SELECT @NewGuidUsuario ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection.Database,
                    base.ParameterSymbol);

                var guid = base._connection.QuerySingle<Guid>(
                    sql: cmdText,
                    param: entity,
                    transaction: this._transaction);

                return this.Get(
                    guid);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Check if the "Password" of "Usuário" record is valid.
        /// </summary>
        /// <param name="guid">Guid of "Usuário" record.</param>
        /// <param name="password">"Password" of "Usuário" record.</param>
        /// <returns>If success, the Entity with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public UsuarioEntity CheckPasswordValid(Guid guid, string password)
        {
            try
            {
                string passwordQuery = PasswordCryptography.GetHashMD5(password);

                string cmdText = @" SELECT TOP 1 Guid
                                      FROM [{0}].[dbo].USUARIOS
                                     WHERE GUID = {1}Guid
                                       AND PASSWORD = {1}PasswordQuery COLLATE SQL_Latin1_General_CP1_CS_AS ";

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
                        usuarioEntity.Guid);
                }

                return null;
            }
            catch
            {
                throw;
            }
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
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(
                        nameof(
                            guid));
                }

                //  Maneira utilizada para trazer os relacionamentos 0:N.
                Dictionary<Guid, UsuarioEntity> usuariosResult = new();

                string cmdText = @"          SELECT {0},
                                                    {1},
                                                    {2}
                                               FROM [{3}].[dbo].[USUARIOS] AS U WITH(NOLOCK)
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS_FISICAS] as PF WITH(NOLOCK)
                                                 ON [U].[GUIDCOLABORADOR] = [PF].[GUID]
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS] as P WITH(NOLOCK)
                                                 ON [PF].[GUIDPESSOA] = [P].[GUID]
                                              WHERE [U].[GUID] = {4}Guid  ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsUsuarios,
                    this._columnsPessoasFisicas,
                    this._columnsPessoas,
                    base._connection?.Database,
                    base.ParameterSymbol);

                base._connection.Query<UsuarioEntity, PessoaFisicaEntity, PessoaEntity, UsuarioEntity>(
                    cmdText,
                    map: (mapUsuario, mapPessoaFisica, mapPessoa) =>
                    {
                        if (!usuariosResult.ContainsKey(mapUsuario.Guid))
                        {
                            //mapUsuario.Colaborador = mapPessoaFisica;
                            //mapUsuario.Colaborador.Pessoa = mapPessoa;

                            usuariosResult.Add(
                                mapUsuario.Guid,
                                mapUsuario);
                        }

                        UsuarioEntity current = usuariosResult[mapUsuario.Guid];

                        if (mapPessoaFisica != null && current.Colaborador != mapPessoaFisica)
                        {
                            current.Colaborador = mapPessoaFisica;

                            if (mapPessoa != null && current.Colaborador.Pessoa != mapPessoa)
                            {
                                current.Colaborador.Pessoa = mapPessoa;
                            }
                        }

                        //if (mapUsuarioCabanha != null && !current.UsuariosCabanhas.Contains(mapUsuarioCabanha))
                        //{
                        //    mapUsuarioCabanha.Usuario = current;
                        //    // mapCabanha.Conta = mapConta;
                        //    // mapCabanha.Associacao = mapAssociacao;

                        //    current.UsuariosCabanhas.Add(
                        //        mapUsuarioCabanha);
                        //}

                        return null;
                    },
                    param: new
                    {
                        Guid = guid,
                    },
                    splitOn: "GUID,GUID,GUID",
                    transaction: this._transaction);

                return usuariosResult.Values.FirstOrDefault();
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
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 0:N.
                Dictionary<Guid, UsuarioEntity> usuariosResult = new();

                string cmdText = @"          SELECT {0},
                                                    {1},
                                                    {2}
                                               FROM [{3}].[dbo].[USUARIOS] AS U WITH(NOLOCK)
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS_FISICAS] as PF WITH(NOLOCK)
                                                 ON [U].[GUIDCOLABORADOR] = [PF].[GUID]
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS] as P WITH(NOLOCK)
                                                 ON [PF].[GUIDPESSOA] = [P].[GUID] ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsUsuarios,
                    this._columnsPessoasFisicas,
                    this._columnsPessoas,
                    base._connection?.Database,
                    base.ParameterSymbol);

                base._connection.Query<UsuarioEntity, PessoaFisicaEntity, PessoaEntity, UsuarioEntity>(
                    cmdText,
                    map: (mapUsuario, mapPessoaFisica, mapPessoa) =>
                    {
                        if (!usuariosResult.ContainsKey(mapUsuario.Guid))
                        {
                            //mapUsuario.Colaborador = mapPessoaFisica;
                            //mapUsuario.Colaborador.Pessoa = mapPessoa;

                            usuariosResult.Add(
                                mapUsuario.Guid,
                                mapUsuario);
                        }

                        UsuarioEntity current = usuariosResult[mapUsuario.Guid];

                        if (mapPessoaFisica != null && current.Colaborador != mapPessoaFisica)
                        {
                            current.Colaborador = mapPessoaFisica;

                            if (mapPessoa != null && current.Colaborador.Pessoa != mapPessoa)
                            {
                                current.Colaborador.Pessoa = mapPessoa;
                            }
                        }

                        //if (mapUsuarioCabanha != null && !current.UsuariosCabanhas.Contains(mapUsuarioCabanha))
                        //{
                        //    mapUsuarioCabanha.Usuario = current;
                        //    // mapCabanha.Conta = mapConta;
                        //    // mapCabanha.Associacao = mapAssociacao;

                        //    current.UsuariosCabanhas.Add(
                        //        mapUsuarioCabanha);
                        //}

                        return null;
                    },
                    splitOn: "GUID,GUID,GUID",
                    transaction: this._transaction);

                return usuariosResult.Values;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Checks if the Username and Password match the registration in the "Usuários" table.
        /// </summary>
        /// <param name="cpfEmailUsername">CPF, Email or Username values.</param>
        /// <returns>If success, the duly authenticated object. Otherwise, an exception is generated stating what happened.</returns>
        public UsuarioEntity GetByUsername(string cpfEmailUsername)
        {
            try
            {
                if (string.IsNullOrEmpty(cpfEmailUsername))
                    throw new ArgumentNullException(
                        nameof(
                            cpfEmailUsername));

                //  Maneira utilizada para trazer os relacionamentos 0:N.
                Dictionary<Guid, UsuarioEntity> usuariosResult = new();

                string cmdText = @"          SELECT {0},
                                                    {1},
                                                    {2}
                                               FROM [{3}].[dbo].[USUARIOS] AS U WITH(NOLOCK)
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS_FISICAS] as PF WITH(NOLOCK)
                                                 ON [U].[GUIDCOLABORADOR] = [PF].[GUID]
                                    LEFT OUTER JOIN [{3}].[dbo].[PESSOAS] as P WITH(NOLOCK)
                                                 ON [PF].[GUIDPESSOA] = [P].[GUID]
                                              WHERE ( LOWER(PF.CPF) = {4}Filtro
                                                       OR LOWER(P.Email) = {4}Filtro
                                                       OR LOWER(U.USERNAME) = {4}Filtro ) ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsUsuarios,
                    this._columnsPessoasFisicas,
                    this._columnsPessoas,
                    base._connection?.Database,
                    base.ParameterSymbol);

                base._connection.Query<UsuarioEntity, PessoaFisicaEntity, PessoaEntity, UsuarioEntity>(
                    cmdText,
                    map: (mapUsuario, mapPessoaFisica, mapPessoa) =>
                    {
                        if (!usuariosResult.ContainsKey(mapUsuario.Guid))
                        {
                            //mapUsuario.Colaborador = mapPessoaFisica;
                            //mapUsuario.Colaborador.Pessoa = mapPessoa;

                            usuariosResult.Add(
                                mapUsuario.Guid,
                                mapUsuario);
                        }

                        UsuarioEntity current = usuariosResult[mapUsuario.Guid];

                        if (mapPessoaFisica != null && current.Colaborador != mapPessoaFisica)
                        {
                            current.Colaborador = mapPessoaFisica;

                            if (mapPessoa != null && current.Colaborador.Pessoa != mapPessoa)
                            {
                                current.Colaborador.Pessoa = mapPessoa;
                            }
                        }

                        return null;
                    },
                    param: new
                    {
                        Filtro = cpfEmailUsername,
                    },
                    splitOn: "GUID,GUID,GUID",
                    transaction: this._transaction);

                return usuariosResult.Values.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UsuarioEntity Update(Guid guid, UsuarioEntity entity)
        {
            try
            {
                string cmdText = @" UPDATE [{0}].[dbo].[USUARIOS]
                                       SET [GUIDCOLABORADOR] = {1}GuidColaborador,
                                           [EMAIL] = {1}Email,
                                           [USERNAME] = {1}Username,
                                           [DATA_PRIMEIRO_ACESSO] = {1}DataPrimeiroAcesso,
                                           [DATA_ULTIMA_ALTERACAO] = GETUTCDATE(),
                                           [PASSWORD] = {1}Password
                                     WHERE [GUID] = {1}Guid ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection.Database,
                    this.ParameterSymbol);

                base._connection.Execute(
                    cmdText,
                    param: entity,
                    transaction: this._transaction);

                return this.Get(
                    entity.Guid);
            }
            catch
            {
                throw;
            }
        }
    }
}