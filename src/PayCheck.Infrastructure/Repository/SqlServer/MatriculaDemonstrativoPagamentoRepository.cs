namespace PayCheck.Infrastructure.Repository.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using Dapper;
    using PayCheck.Application.Interfaces.Repository;

    public class MatriculaDemonstrativoPagamentoRepository : BaseRepository, IMatriculaDemonstrativoPagamentoRepository
    {
        private readonly string _columnsMatriculas;
        private readonly string _columnsMatriculasDemonstrativosPagamento;
        private readonly string _columnsPessoasFisicas;
        private readonly string _columnsPessoasJuridicas;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatriculaRepository"/> class.
        /// </summary>
        /// <param name="connection"></param>
        public MatriculaDemonstrativoPagamentoRepository(SqlConnection connection) :
            base(connection)
        {
            base._connection = connection;

            this.MapAttributeToField(
                typeof(
                    MatriculaEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaDemonstrativoPagamentoEntity));

            this._columnsMatriculas = base.GetAllColumnsFromTable(
                base.TableNameMatriculas,
                base.TableAliasMatriculas);

            this._columnsMatriculasDemonstrativosPagamento = base.GetAllColumnsFromTable(
                base.TableNameMatriculasDemonstrativosPagamento,
                base.TableAliasMatriculasDemonstrativosPagamento);

            this._columnsPessoasFisicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasFisicas,
                base.TableAliasPessoasFisicas);

            this._columnsPessoasJuridicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasJuridicas,
                base.TableAliasPessoasJuridicas);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatriculaRepository"/> class.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public MatriculaDemonstrativoPagamentoRepository(SqlConnection connection, SqlTransaction transaction) :
            base(connection, transaction)
        {
            base._connection = connection;
            this._transaction = transaction;

            this.MapAttributeToField(
                typeof(
                    MatriculaEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaDemonstrativoPagamentoEntity));

            this._columnsMatriculas = base.GetAllColumnsFromTable(
                base.TableNameMatriculas,
                base.TableAliasMatriculas);

            this._columnsMatriculasDemonstrativosPagamento = base.GetAllColumnsFromTable(
                base.TableNameMatriculasDemonstrativosPagamento,
                base.TableAliasMatriculasDemonstrativosPagamento);

            this._columnsPessoasFisicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasFisicas,
                base.TableAliasPessoasFisicas);

            this._columnsPessoasJuridicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasJuridicas,
                base.TableAliasPessoasJuridicas);
        }

        /// <summary>
        /// Creates the "Matrícula Demonstrativo Pagamento" record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public MatriculaDemonstrativoPagamentoEntity Create(MatriculaDemonstrativoPagamentoEntity entity)
        {
            try
            {
                string cmdText = @"     DECLARE @NewGuidMatriculaDemonstrativoPagamento UniqueIdentifier
                                            SET @NewGuidMatriculaDemonstrativoPagamento = NEWID()

                                    INSERT INTO [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                                ([GUID],
                                                 [GUIDMATRICULA],
                                                 [COMPETENCIA])
                                         VALUES (@NewGuidMatriculaDemonstrativoPagamento,
                                                 {1}GuidMatricula,
                                                 {1}Competencia)

                                          SELECT @NewGuidMatriculaDemonstrativoPagamento ";

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
        /// Deletes the "Matrícula Demonstrativo Pagamento" record.
        /// </summary>
        /// <param name="guid">Guid of "Matrícula Demonstrativo Pagamento" record.</param>
        public void Delete(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                string cmdText = @" DELETE
                                      FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                     WHERE [GUID] = {1}Guid ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection.Database,
                    this.ParameterSymbol);

                base._connection.Execute(
                    cmdText,
                    new
                    {
                        Guid = guid,
                    },
                    transaction: this._transaction);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes the "Eventos" And "Totalizadores" of "Matrícula Demonstrativo Pagamento" records by "Competência" And "Guid of Matrícula".
        /// </summary>
        /// <param name="competencia">"Competência" of "Matrícula" record.</param>
        /// <param name="guidMatricula">Guid of "Matrícula" record.</param>
        public void DeleteEventosAndTotalizadoresByCompetenciaAndGuidMatricula(string competencia, Guid guidMatricula)
        {
            try
            {
                if (string.IsNullOrEmpty(competencia))
                    throw new ArgumentNullException(
                        nameof(competencia));
                else if (guidMatricula == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guidMatricula));

                string cmdText = @" DELETE
                                      FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO_EVENTOS]
                                     WHERE [GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO] IN ( SELECT [GUID]
                                                                                          FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                                                                         WHERE [COMPETENCIA] = {1}Competencia
                                                                                           AND [GUIDMATRICULA] = {1}GuidMatricula )

                                    DELETE
                                      FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO_TOTALIZADORES]
                                     WHERE [GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO] IN ( SELECT [GUID]
                                                                                          FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                                                                         WHERE [COMPETENCIA] = {1}Competencia
                                                                                           AND [GUIDMATRICULA] = {1}GuidMatricula ) ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection.Database,
                    this.ParameterSymbol);

                base._connection.Execute(
                    cmdText,
                    new
                    {
                        Competencia = competencia,
                        GuidMatricula = guidMatricula,
                    },
                    transaction: this._transaction);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the "Matrícula Demonstrativo Pagamento" record.
        /// </summary>
        /// <param name="guid">Guid of "Matrícula Demonstrativo Pagamento" record.</param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public MatriculaDemonstrativoPagamentoEntity Get(Guid guid)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = $@"      SELECT {this._columnsMatriculasDemonstrativosPagamento},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas}
                                            FROM [{this._connection.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamento}] as {base.TableAliasMatriculasDemonstrativosPagamento} WITH(NOLOCK)
                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 
                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]
                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 
                                           WHERE UPPER([{base.TableAliasMatriculasDemonstrativosPagamento}].[GUID]) = {base.ParameterSymbol}Guid ";

                var matriculaDemonstrativoPagamentoEntity = base._connection.Query<MatriculaDemonstrativoPagamentoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaDemonstrativoPagamentoEntity>(
                    cmdText,
                    map: (mapMatriculaDemonstrativoPagamento, mapMatricula, mapPessoaFisica, mapPessoaJuridica) =>
                    {
                        mapMatricula.Colaborador = mapPessoaFisica;
                        mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaDemonstrativoPagamento.Matricula = mapMatricula;

                        return mapMatriculaDemonstrativoPagamento;
                    },
                    param: new
                    {
                        Guid = guid,
                    },
                    splitOn: "GUID,GUID,GUID,GUID",
                    transaction: this._transaction);

                return matriculaDemonstrativoPagamentoEntity.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Demonstrativos Pagamento" records by "Competência" And "Matrícula".
        /// </summary>
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaDemonstrativoPagamentoEntity> Get(string competencia, string matricula)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = @"      SELECT {0},
                                                {1}
                                           FROM [{2}].[dbo].[{3}] as {4} WITH(NOLOCK)
                                     INNER JOIN [{2}].[dbo].[{5}] as {6} WITH(NOLOCK)
                                             ON {6}.[GUID] = {4}.[GUIDMATRICULA]
                                          WHERE {4}.[COMPETENCIA] = {7}Competencia 
                                            AND {6}.[MATRICULA] = {7}Matricula ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsMatriculasDemonstrativosPagamento,
                    this._columnsMatriculas,
                    base._connection.Database,
                    base.TableNameMatriculasDemonstrativosPagamento,
                    base.TableAliasMatriculasDemonstrativosPagamento,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculaDemonstrativosPagamentoEntity = base._connection.Query<MatriculaDemonstrativoPagamentoEntity, MatriculaEntity, MatriculaDemonstrativoPagamentoEntity>(
                    cmdText,
                    map: (mapMatriculaDemonstrativoPagamento, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaDemonstrativoPagamento.Matricula = mapMatricula;

                        return mapMatriculaDemonstrativoPagamento;
                    },
                    param: new
                    {
                        Competencia = competencia,
                        Matricula = matricula,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculaDemonstrativosPagamentoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Demonstrativos Pagamento" records.
        /// </summary>
        /// <returns>If success, the list with all "Matrículas Demonstrativos Pagamento" records. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetAll()
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = $@"      SELECT {this._columnsMatriculasDemonstrativosPagamento},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas}
                                            FROM [{this._connection.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamento}] as {base.TableAliasMatriculasDemonstrativosPagamento} WITH(NOLOCK)

                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                        ORDER BY [{base.TableAliasMatriculasDemonstrativosPagamento}].[COMPETENCIA] Desc,
                                                 [{base.TableAliasMatriculas}].[MATRICULA],
                                                 [{base.TableAliasPessoasFisicas}].[NOME] ";

                var matriculasDemonstrativosPagamentoEntity = base._connection.Query<MatriculaDemonstrativoPagamentoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaDemonstrativoPagamentoEntity>(
                    cmdText,
                    map: (mapMatriculaDemonstrativoPagamento, mapMatricula, mapPessoaFisica, mapPessoaJuridica) =>
                    {
                        mapMatricula.Colaborador = mapPessoaFisica;
                        mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaDemonstrativoPagamento.Matricula = mapMatricula;

                        return mapMatriculaDemonstrativoPagamento;
                    },
                    splitOn: "GUID,GUID,GUID,GUID",
                    transaction: this._transaction);

                return matriculasDemonstrativosPagamentoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Demonstrativos Pagamento" records by "Competência".
        /// </summary>
        /// <param name="competencia"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetByCompetencia(string competencia)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = @"      SELECT {0},
                                                {1}
                                           FROM [{2}].[dbo].[{3}] as {4} WITH(NOLOCK)
                                     INNER JOIN [{2}].[dbo].[{5}] as {6} WITH(NOLOCK)
                                             ON {6}.[GUID] = {4}.[GUIDMATRICULA]
                                          WHERE {4}.[COMPETENCIA] = {7}Competencia ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsMatriculasDemonstrativosPagamento,
                    this._columnsMatriculas,
                    base._connection.Database,
                    base.TableNameMatriculasDemonstrativosPagamento,
                    base.TableAliasMatriculasDemonstrativosPagamento,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculasDemonstrativosPagamentoEntity = base._connection.Query<MatriculaDemonstrativoPagamentoEntity, MatriculaEntity, MatriculaDemonstrativoPagamentoEntity>(
                    cmdText,
                    map: (mapMatriculasDemonstrativoPagamento, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculasDemonstrativoPagamento.Matricula = mapMatricula;

                        return mapMatriculasDemonstrativoPagamento;
                    },
                    param: new
                    {
                        Competencia = competencia,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculasDemonstrativosPagamentoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Demonstrativos Pagamento" records by "Matrícula".
        /// </summary>
        /// <param name="matricula"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaDemonstrativoPagamentoEntity> GetByMatricula(string matricula)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 1:N.
                string cmdText = @"      SELECT {0},
                                                {1}
                                           FROM [{2}].[dbo].[{3}] as {4} WITH(NOLOCK)
                                     INNER JOIN [{2}].[dbo].[{5}] as {6} WITH(NOLOCK)
                                             ON {6}.[GUID] = {4}.[GUIDMATRICULA]
                                          WHERE {6}.[MATRICULA] = {7}Matricula ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    this._columnsMatriculasDemonstrativosPagamento,
                    this._columnsMatriculas,
                    base._connection.Database,
                    base.TableNameMatriculasDemonstrativosPagamento,
                    base.TableAliasMatriculasDemonstrativosPagamento,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculasDemonstrativosPagamentoEntity = base._connection.Query<MatriculaDemonstrativoPagamentoEntity, MatriculaEntity, MatriculaDemonstrativoPagamentoEntity>(
                    cmdText,
                    map: (mapMatriculaDemonstrativoPagamento, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaDemonstrativoPagamento.Matricula = mapMatricula;

                        return mapMatriculaDemonstrativoPagamento;
                    },
                    param: new
                    {
                        Matricula = matricula,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculasDemonstrativosPagamentoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the "Matrícula Demonstrativo Pagamento" record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public MatriculaDemonstrativoPagamentoEntity Update(Guid pk, MatriculaDemonstrativoPagamentoEntity entity)
        {
            try
            {
                entity.Guid = pk;

                string cmdText = @" UPDATE [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                       SET [GUIDMATRICULA] = {1}GuidMatricula,
                                           [COMPETENCIA] = {1}Competencia
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