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

    public class MatriculaEspelhoPontoRepository : BaseRepository, IMatriculaEspelhoPontoRepository
    {
        private readonly string _columnsCalculos;
        private readonly string _columnsMatriculas;
        private readonly string _columnsMatriculasEspelhosPonto;
        private readonly string _columnsMatriculasEspelhosPontoCalculos;
        private readonly string _columnsMatriculasEspelhosPontoMarcacoes;
        private readonly string _columnsPessoasFisicas;
        private readonly string _columnsPessoasJuridicas;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatriculaEspelhoPontoRepository"/> class.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public MatriculaEspelhoPontoRepository(SqlConnection connection, SqlTransaction? transaction) :
            base(connection, transaction)
        {
            base._connection = connection;
            this._transaction = transaction;

            this.MapAttributeToField(
                typeof(
                    CalculoEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaEspelhoPontoEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaEspelhoPontoCalculoEntity));

            this.MapAttributeToField(
                typeof(
                    MatriculaEspelhoPontoMarcacaoEntity));

            this.MapAttributeToField(
                typeof(
                    PessoaFisicaEntity));

            this.MapAttributeToField(
                typeof(
                    PessoaJuridicaEntity));

            this._columnsCalculos = base.GetAllColumnsFromTable(
                base.TableNameCalculos,
                base.TableAliasCalculos);

            this._columnsMatriculas = base.GetAllColumnsFromTable(
                base.TableNameMatriculas,
                base.TableAliasMatriculas);

            this._columnsMatriculasEspelhosPonto = base.GetAllColumnsFromTable(
                base.TableNameMatriculasEspelhosPonto,
                base.TableAliasMatriculasEspelhosPonto);

            this._columnsMatriculasEspelhosPontoCalculos = base.GetAllColumnsFromTable(
                base.TableNameMatriculasEspelhosPontoCalculos,
                base.TableAliasMatriculasEspelhosPontoCalculos);

            this._columnsMatriculasEspelhosPontoMarcacoes = base.GetAllColumnsFromTable(
                base.TableNameMatriculasEspelhosPontoMarcacoes,
                base.TableAliasMatriculasEspelhosPontoMarcacoes);

            this._columnsPessoasFisicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasFisicas,
                base.TableAliasPessoasFisicas,
                "PF.FOTO");

            this._columnsPessoasJuridicas = base.GetAllColumnsFromTable(
                base.TableNamePessoasJuridicas,
                base.TableAliasPessoasJuridicas,
                "PJ.LOGOTIPO");
        }

        /// <summary>
        /// Creates the "Matrícula Espelho Ponto" record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public MatriculaEspelhoPontoEntity Create(MatriculaEspelhoPontoEntity entity)
        {
            try
            {
                string cmdText = @"     DECLARE @NewGuidMatriculaEspelhoPonto UniqueIdentifier
                                            SET @NewGuidMatriculaEspelhoPonto = NEWID()

                                    INSERT INTO [{0}].[dbo].[MATRICULAS_ESPELHOS_PONTO]
                                                ([GUID],
                                                 [GUIDMATRICULA],
                                                 [COMPETENCIA])
                                         VALUES (@NewGuidMatriculaEspelhoPonto,
                                                 {1}GuidMatricula,
                                                 {1}Competencia)

                                          SELECT @NewGuidMatriculaEspelhoPonto ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection?.Database,
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
        /// Deletes the "Matrícula Espelho Ponto" record.
        /// </summary>
        /// <param name="guid">Guid of "Matrícula Espelho Ponto" record.</param>
        public void Delete(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                string cmdText = @" DELETE
                                      FROM [{0}].[dbo].[MATRICULAS_ESPELHOS_PONTO]
                                     WHERE [GUID] = {1}Guid ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection?.Database,
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
        /// Deletes the "Cálculos" And "Marcações" of "Matrícula Espelho Ponto" records by "Competência" And "Guid of Matrícula".
        /// </summary>
        /// <param name="competencia">"Competência" of "Matrícula" record.</param>
        /// <param name="guidMatricula">Guid of "Matrícula" record.</param>
        public void DeleteCalculosAndMarcacoesByCompetenciaAndGuidMatricula(string competencia, Guid guidMatricula)
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
                                      FROM [{0}].[dbo].[MATRICULAS_ESPELHOS_PONTO_CALCULOS]
                                     WHERE [GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO] IN ( SELECT [GUID]
                                                                                          FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                                                                         WHERE [COMPETENCIA] = {1}Competencia
                                                                                           AND [GUIDMATRICULA] = {1}GuidMatricula )

                                    DELETE
                                      FROM [{0}].[dbo].[MATRICULAS_ESPELHOS_PONTO_MARCACOES]
                                     WHERE [GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO] IN ( SELECT [GUID]
                                                                                          FROM [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                                                                         WHERE [COMPETENCIA] = {1}Competencia
                                                                                           AND [GUIDMATRICULA] = {1}GuidMatricula ) ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection?.Database,
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
        /// Gets the "Matrícula Espelho Ponto" record.
        /// </summary>
        /// <param name="guid">Guid of "Matrícula Espelho Ponto" record.</param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public MatriculaEspelhoPontoEntity Get(Guid guid)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 0:N.
                var matriculasEspelhoPontoResult = new Dictionary<Guid, MatriculaEspelhoPontoEntity>();

                string cmdText = $@"      SELECT {this._columnsMatriculasEspelhosPonto},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas},
                                                 {this._columnsMatriculasEspelhosPontoCalculos},
                                                 {this._columnsMatriculasEspelhosPontoMarcacoes},
                                                 {this._columnsCalculos}
                                            FROM [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPonto}] as {base.TableAliasMatriculasEspelhosPonto} WITH(NOLOCK)

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPontoMarcacoes}] as {base.TableAliasMatriculasEspelhosPontoMarcacoes} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUID] = {base.TableAliasMatriculasEspelhosPontoMarcacoes}.[GUIDMATRICULA_ESPELHO_PONTO]

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPontoCalculos}] as {base.TableAliasMatriculasEspelhosPontoCalculos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUID] = {base.TableAliasMatriculasEspelhosPontoCalculos}.[GUIDMATRICULA_ESPELHO_PONTO]

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameCalculos}] as {base.TableAliasCalculos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPontoCalculos}].[IDCALCULO] = [{base.TableAliasCalculos}].[ID]

                                           WHERE UPPER([{base.TableAliasMatriculasEspelhosPonto}].[GUID]) = {base.ParameterSymbol}Guid

                                        ORDER BY [{base.TableAliasMatriculasEspelhosPonto}].[COMPETENCIA] Desc,
                                                 [{base.TableAliasMatriculas}].[MATRICULA],
                                                 [{base.TableAliasPessoasFisicas}].[NOME] ";

                //[{base.TableAliasCalculos}].[ID],
                //[{base.TableAliasMatriculasEspelhosPontoMarcacoes}].[DATA] ";

                var matriculasEspelhosPontoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaEspelhoPontoCalculoEntity, MatriculaEspelhoPontoMarcacaoEntity, CalculoEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula, mapPessoaFisica, mapPessoaJuridica, mapMatriculaEspelhoPontoCalculos, mapMatriculaEspelhoPontoMarcacoes, mapCalculos) =>
                    {
                        if (!matriculasEspelhoPontoResult.ContainsKey(mapMatriculaEspelhoPonto.Guid))
                        {
                            mapMatricula.Colaborador = mapPessoaFisica;
                            mapMatricula.Empregador = mapPessoaJuridica;

                            mapMatriculaEspelhoPonto.Matricula = mapMatricula;

                            mapMatriculaEspelhoPonto.MatriculaEspelhoPontoMarcacoes = new List<MatriculaEspelhoPontoMarcacaoEntity>();

                            matriculasEspelhoPontoResult.Add(
                                mapMatriculaEspelhoPonto.Guid,
                                mapMatriculaEspelhoPonto);
                        }

                        MatriculaEspelhoPontoEntity current = matriculasEspelhoPontoResult[mapMatriculaEspelhoPonto.Guid];

                        if (mapMatriculaEspelhoPontoMarcacoes != null && !current.MatriculaEspelhoPontoMarcacoes.Contains(mapMatriculaEspelhoPontoMarcacoes))
                        {
                            current.MatriculaEspelhoPontoMarcacoes.Add(
                                mapMatriculaEspelhoPontoMarcacoes);
                        }

                        //if (mapMatriculaDemonstrativoPagamentoEventos != null && !current.MatriculaDemonstrativoPagamentoEventos.Contains(mapMatriculaDemonstrativoPagamentoEventos))
                        //{
                        //    mapMatriculaDemonstrativoPagamentoEventos.Evento = mapEvento;

                        //    current.MatriculaDemonstrativoPagamentoEventos.Add(
                        //        mapMatriculaDemonstrativoPagamentoEventos);
                        //}

                        return null;
                    },
                    param: new
                    {
                        Guid = guid,
                    },
                    splitOn: "GUID,GUID,GUID,GUID,GUID,GUID,ID",
                    transaction: this._transaction);

                return matriculasEspelhoPontoResult.Values.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Espelhos Ponto" records by "Competência" And "Matrícula".
        /// </summary>
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaEspelhoPontoEntity> Get(string competencia, string matricula)
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
                    this._columnsMatriculasEspelhosPonto,
                    this._columnsMatriculas,
                    base._connection?.Database,
                    base.TableNameMatriculasEspelhosPonto,
                    base.TableAliasMatriculasEspelhosPonto,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculaEspelhosPontoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaEspelhoPonto.Matricula = mapMatricula;

                        return mapMatriculaEspelhoPonto;
                    },
                    param: new
                    {
                        Competencia = competencia,
                        Matricula = matricula,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculaEspelhosPontoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Espelhos Ponto" records.
        /// </summary>
        /// <returns>If success, the list with all "Matrículas Espelhos Ponto" records. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaEspelhoPontoEntity> GetAll()
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 0:N.
                Dictionary<Guid, MatriculaEspelhoPontoEntity> matriculasEspelhosPontoResult = new Dictionary<Guid, MatriculaEspelhoPontoEntity>();

                string cmdText = $@"      SELECT {this._columnsMatriculasEspelhosPonto},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas},
                                                 {this._columnsMatriculasEspelhosPontoCalculos},
                                                 {this._columnsMatriculasEspelhosPontoMarcacoes},
                                                 {this._columnsCalculos}
                                            FROM [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPonto}] as {base.TableAliasMatriculasEspelhosPonto} WITH(NOLOCK)

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPontoCalculos}] as {base.TableAliasMatriculasEspelhosPontoCalculos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUID] = {base.TableAliasMatriculasEspelhosPontoCalculos}.[GUIDMATRICULA_ESPELHO_PONTO]

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPontoMarcacoes}] as {base.TableAliasMatriculasEspelhosPontoMarcacoes} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUID] = {base.TableAliasMatriculasEspelhosPontoMarcacoes}.[GUIDMATRICULA_ESPELHO_PONTO]

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameCalculos}] as {base.TableAliasCalculos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPontoCalculos}].[IDCALCULO] = [{base.TableAliasCalculos}].[ID]

                                        ORDER BY [{base.TableAliasMatriculasEspelhosPonto}].[COMPETENCIA] Desc,
                                                 [{base.TableAliasMatriculas}].[MATRICULA],
                                                 [{base.TableAliasPessoasFisicas}].[NOME],
                                                 [{base.TableAliasCalculos}].[ID] ";

                var matriculasEspelhosPontoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaEspelhoPontoCalculoEntity, MatriculaEspelhoPontoMarcacaoEntity, CalculoEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula, mapPessoaFisica, mapPessoaJuridica, mapMatriculaEspelhoPontoCalculos, mapMatriculaEspelhoPontoMarcacoes, mapCalculos) =>
                    {
                        if (!matriculasEspelhosPontoResult.ContainsKey(mapMatriculaEspelhoPonto.Guid))
                        {
                            mapMatricula.Colaborador = mapPessoaFisica;
                            mapMatricula.Empregador = mapPessoaJuridica;

                            mapMatriculaEspelhoPonto.Matricula = mapMatricula;

                            //mapMatriculaEspelhoPonto.MatriculaDemonstrativoPagamentoEventos = new List<MatriculaDemonstrativoPagamentoEventoEntity>();

                            matriculasEspelhosPontoResult.Add(
                                mapMatriculaEspelhoPonto.Guid,
                                mapMatriculaEspelhoPonto);
                        }

                        MatriculaEspelhoPontoEntity current = matriculasEspelhosPontoResult[mapMatriculaEspelhoPonto.Guid];

                        //if (mapMatriculaDemonstrativoPagamentoEventos != null && !current.MatriculaDemonstrativoPagamentoEventos.Contains(mapMatriculaDemonstrativoPagamentoEventos))
                        //{
                        //    mapMatriculaDemonstrativoPagamentoEventos.Evento = mapEvento;

                        //    current.MatriculaDemonstrativoPagamentoEventos.Add(
                        //        mapMatriculaDemonstrativoPagamentoEventos);
                        //}

                        return null;
                    },
                    splitOn: "GUID,GUID,GUID,GUID,GUID,GUID,ID",
                    transaction: this._transaction);

                return matriculasEspelhosPontoResult.Values;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Espelhos Ponto" records by "Competência".
        /// </summary>
        /// <param name="competencia"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaEspelhoPontoEntity> GetByCompetencia(string competencia)
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
                    this._columnsMatriculasEspelhosPonto,
                    this._columnsMatriculas,
                    base._connection?.Database,
                    base.TableNameMatriculasEspelhosPonto,
                    base.TableAliasMatriculasEspelhosPonto,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculasEspelhosPontoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculasEspelhosPonto, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculasEspelhosPonto.Matricula = mapMatricula;

                        return mapMatriculasEspelhosPonto;
                    },
                    param: new
                    {
                        Competencia = competencia,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculasEspelhosPontoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Espelhos Ponto" records by "GuidColaborador".
        /// </summary>
        /// <param name="guidColaborador"></param>
        /// <returns></returns>
        public IEnumerable<MatriculaEspelhoPontoEntity> GetByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                //  Maneira utilizada para trazer os relacionamentos 0:N.
                var matriculasEspelhosPontoResult = new Dictionary<Guid, MatriculaEspelhoPontoEntity>();

                string cmdText = $@"      SELECT {this._columnsMatriculasEspelhosPonto},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas},
                                                 {this._columnsMatriculasEspelhosPontoCalculos},
                                                 {this._columnsCalculos}
                                            FROM [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasEspelhosPonto}] as {base.TableAliasMatriculasEspelhosPonto} WITH(NOLOCK)

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamentoEventos}] as {base.TableAliasMatriculasDemonstrativosPagamentoEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasEspelhosPonto}].[GUID] = {base.TableAliasMatriculasDemonstrativosPagamentoEventos}.[GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameEventos}] as {base.TableAliasEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamentoEventos}].[IDEVENTO] = [{base.TableAliasEventos}].[ID]

                                           WHERE [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = {base.ParameterSymbol}GuidColaborador

                                        ORDER BY [{base.TableAliasMatriculasEspelhosPonto}].[COMPETENCIA] Desc,
                                                 [{base.TableAliasMatriculas}].[MATRICULA],
                                                 [{base.TableAliasPessoasFisicas}].[NOME],
                                                 [{base.TableAliasEventos}].[ID] ";

                var matriculasDemonstrativosPagamentoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaEspelhoPontoMarcacaoEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula, mapPessoaFisica, mapPessoaJuridica, mapMatriculaEspelhoPontoMarcacao) =>
                    {
                        if (!matriculasEspelhosPontoResult.ContainsKey(mapMatriculaEspelhoPonto.Guid))
                        {
                            mapMatricula.Colaborador = mapPessoaFisica;
                            mapMatricula.Empregador = mapPessoaJuridica;

                            mapMatriculaEspelhoPonto.Matricula = mapMatricula;

                            // mapMatriculaEspelhoPonto.MatriculaDemonstrativoPagamentoEventos = new List<MatriculaDemonstrativoPagamentoEventoEntity>();

                            matriculasEspelhosPontoResult.Add(
                                mapMatriculaEspelhoPonto.Guid,
                                mapMatriculaEspelhoPonto);
                        }

                        MatriculaEspelhoPontoEntity current = matriculasEspelhosPontoResult[mapMatriculaEspelhoPonto.Guid];

                        //if (mapMatriculaDemonstrativoPagamentoEventos != null && !current.MatriculaDemonstrativoPagamentoEventos.Contains(mapMatriculaDemonstrativoPagamentoEventos))
                        //{
                        //    mapMatriculaDemonstrativoPagamentoEventos.Evento = mapEvento;

                        //    current.MatriculaDemonstrativoPagamentoEventos.Add(
                        //        mapMatriculaDemonstrativoPagamentoEventos);
                        //}

                        return null;
                    },
                    param: new
                    {
                        GuidColaborador = guidColaborador,
                    },
                    splitOn: "GUID,GUID,GUID,GUID,GUID,ID",
                    transaction: this._transaction);

                return matriculasEspelhosPontoResult.Values;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all "Matrículas Espelhos Ponto" records by "Matrícula".
        /// </summary>
        /// <param name="matricula"></param>
        /// <returns>If success, the object with the persistent database record. Otherwise, an exception detailing the problem.</returns>
        public IEnumerable<MatriculaEspelhoPontoEntity> GetByMatricula(string matricula)
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
                    this._columnsMatriculasEspelhosPonto,
                    this._columnsMatriculas,
                    base._connection?.Database,
                    base.TableNameMatriculasEspelhosPonto,
                    base.TableAliasMatriculasEspelhosPonto,
                    base.TableNameMatriculas,
                    base.TableAliasMatriculas,
                    base.ParameterSymbol);

                var matriculasEspelhosPontoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula) =>
                    {
                        //mapMatricula.Colaborador = mapPessoaFisica;
                        //mapMatricula.Empregador = mapPessoaJuridica;

                        mapMatriculaEspelhoPonto.Matricula = mapMatricula;

                        return mapMatriculaEspelhoPonto;
                    },
                    param: new
                    {
                        Matricula = matricula,
                    },
                    splitOn: "GUID,GUID",
                    transaction: this._transaction);

                return matriculasEspelhosPontoEntity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the "Matrícula Espelho Ponto" record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public MatriculaEspelhoPontoEntity Update(Guid pk, MatriculaEspelhoPontoEntity entity)
        {
            try
            {
                entity.Guid = pk;

                string cmdText = @" UPDATE [{0}].[dbo].[MATRICULAS_ESPELHOS_PONTO]
                                       SET [GUIDMATRICULA] = {1}GuidMatricula,
                                           [COMPETENCIA] = {1}Competencia,
                                           [DATA_ULTIMA_ALTERACAO] = GETUTCDATE(),
                                           [DATA_CONFIRMACAO] = {1}DataConfirmacao,
                                           [IP_CONFIRMACAO] = {1}IpConfirmacao
                                     WHERE [GUID] = {1}Guid ";

                cmdText = string.Format(
                    CultureInfo.InvariantCulture,
                    cmdText,
                    base._connection?.Database,
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