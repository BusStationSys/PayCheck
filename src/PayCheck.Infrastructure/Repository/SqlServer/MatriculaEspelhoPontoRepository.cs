﻿namespace PayCheck.Infrastructure.Repository.SqlServer
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
                Dictionary<Guid, MatriculaEspelhoPontoEntity> matriculasEspelhosPontoResult = new Dictionary<Guid, MatriculaEspelhoPontoEntity>();

                string cmdText = $@"      SELECT {this._columnsMatriculasEspelhosPonto},
                                                 {this._columnsMatriculas},
                                                 {this._columnsPessoasFisicas},
                                                 {this._columnsPessoasJuridicas},
                                                 {this._columnsMatriculasEspelhosPontoCalculos},
                                                 {this._columnsMatriculasEspelhosPontoMarcacoes},
                                                 {this._columnsCalculos}
                                            FROM [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamento}] as {base.TableAliasMatriculasDemonstrativosPagamento} WITH(NOLOCK)

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamentoEventos}] as {base.TableAliasMatriculasDemonstrativosPagamentoEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUID] = {base.TableAliasMatriculasDemonstrativosPagamentoEventos}.[GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameEventos}] as {base.TableAliasEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamentoEventos}].[IDEVENTO] = [{base.TableAliasEventos}].[ID]

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamentoTotalizadores}] as {base.TableAliasMatriculasDemonstrativosPagamentoTotalizadores} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUID] = {base.TableAliasMatriculasDemonstrativosPagamentoTotalizadores}.[GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameTotalizadores}] as {base.TableAliasTotalizadores} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamentoTotalizadores}].[IDTOTALIZADOR] = [{base.TableAliasTotalizadores}].[ID]

                                           WHERE UPPER([{base.TableAliasMatriculasDemonstrativosPagamento}].[GUID]) = {base.ParameterSymbol}Guid ";

                base._connection.Query<MatriculaEspelhoPontoEntity>(
                    cmdText,
                    new[]
                    {
                        typeof(MatriculaEspelhoPontoEntity),
                        typeof(MatriculaEntity),
                        typeof(PessoaFisicaEntity),
                        typeof(PessoaJuridicaEntity),
                        typeof(MatriculaDemonstrativoPagamentoEventoEntity),
                        typeof(EventoEntity),
                        typeof(MatriculaDemonstrativoPagamentoTotalizadorEntity),
                        typeof(TotalizadorEntity),
                    },
                    obj =>
                    {
                        var matriculaEspelhoPontoEntity = (MatriculaEspelhoPontoEntity)obj[0];
                        var matriculaEntity = (MatriculaEntity)obj[1];
                        var pessoaFisicaEntity = (PessoaFisicaEntity)obj[2];
                        var pessoaJuridicaEntity = (PessoaJuridicaEntity)obj[3];
                        var matriculaDemonstrativoPagamentoEventoEntity = (MatriculaDemonstrativoPagamentoEventoEntity)obj[4];
                        var eventoEntity = (EventoEntity)obj[5];
                        var matriculaDemonstrativoPagamentoTotalizadorEntity = (MatriculaDemonstrativoPagamentoTotalizadorEntity)obj[6];
                        var totalizadorEntity = (TotalizadorEntity)obj[7];

                        if (!matriculasEspelhosPontoResult.ContainsKey(matriculaEspelhoPontoEntity.Guid))
                        {
                            // matriculaEspelhoPontoEntity.MatriculaDemonstrativoPagamentoEventos = new List<MatriculaDemonstrativoPagamentoEventoEntity>();
                            // matriculaEspelhoPontoEntity.MatriculaDemonstrativoPagamentoTotalizadores = new List<MatriculaDemonstrativoPagamentoTotalizadorEntity>();

                            matriculaEntity.Colaborador = pessoaFisicaEntity;
                            matriculaEntity.Empregador = pessoaJuridicaEntity;

                            matriculaEspelhoPontoEntity.Matricula = matriculaEntity;

                            matriculasEspelhosPontoResult.Add(
                                matriculaEspelhoPontoEntity.Guid,
                                matriculaEspelhoPontoEntity);
                        }

                        MatriculaEspelhoPontoEntity current = matriculasEspelhosPontoResult[matriculaEspelhoPontoEntity.Guid];

                        //if (matriculaDemonstrativoPagamentoEventoEntity != null &&
                        //    !current.MatriculaDemonstrativoPagamentoEventos.Any(
                        //        mdpe => mdpe.IdEvento == matriculaDemonstrativoPagamentoEventoEntity.IdEvento))
                        //{
                        //    matriculaDemonstrativoPagamentoEventoEntity.Evento = eventoEntity;

                        //    current.MatriculaDemonstrativoPagamentoEventos.Add(
                        //        matriculaDemonstrativoPagamentoEventoEntity);
                        //}

                        //if (matriculaDemonstrativoPagamentoTotalizadorEntity != null &&
                        //    !current.MatriculaDemonstrativoPagamentoTotalizadores.Any(
                        //        mdpt => mdpt.IdTotalizador == matriculaDemonstrativoPagamentoTotalizadorEntity.IdTotalizador))
                        //{
                        //    matriculaDemonstrativoPagamentoTotalizadorEntity.Totalizador = totalizadorEntity;

                        //    current.MatriculaDemonstrativoPagamentoTotalizadores.Add(
                        //        matriculaDemonstrativoPagamentoTotalizadorEntity);
                        //}

                        return null;
                    },
                    param: new
                    {
                        Guid = guid,
                    },
                    splitOn: "GUID,GUID,GUID,GUID,GUID,ID,GUID,ID",
                    transaction: this._transaction);

                return matriculasEspelhosPontoResult.Values.FirstOrDefault();
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
                    base.TableNameMatriculasDemonstrativosPagamento,
                    base.TableAliasMatriculasDemonstrativosPagamento,
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
                                            FROM [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamento}] as {base.TableAliasMatriculasDemonstrativosPagamento} WITH(NOLOCK)

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculas}] as {base.TableAliasMatriculas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUIDMATRICULA] = [{base.TableAliasMatriculas}].[GUID] 

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasFisicas}] as {base.TableAliasPessoasFisicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = [{base.TableAliasPessoasFisicas}].[GUID]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNamePessoasJuridicas}] as {base.TableAliasPessoasJuridicas} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculas}].[GUIDEMPREGADOR] = [{base.TableAliasPessoasJuridicas}].[GUID] 

                                 LEFT OUTER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameMatriculasDemonstrativosPagamentoEventos}] as {base.TableAliasMatriculasDemonstrativosPagamentoEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamento}].[GUID] = {base.TableAliasMatriculasDemonstrativosPagamentoEventos}.[GUIDMATRICULA_DEMONSTRATIVO_PAGAMENTO]

                                      INNER JOIN [{this._connection?.Database}].[dbo].[{base.TableNameEventos}] as {base.TableAliasEventos} WITH(NOLOCK)
                                              ON [{base.TableAliasMatriculasDemonstrativosPagamentoEventos}].[IDEVENTO] = [{base.TableAliasEventos}].[ID]

                                           WHERE [{base.TableAliasMatriculas}].[GUIDCOLABORADOR] = {base.ParameterSymbol}GuidColaborador

                                        ORDER BY [{base.TableAliasMatriculasDemonstrativosPagamento}].[COMPETENCIA] Desc,
                                                 [{base.TableAliasMatriculas}].[MATRICULA],
                                                 [{base.TableAliasPessoasFisicas}].[NOME],
                                                 [{base.TableAliasEventos}].[ID] ";

                var matriculasDemonstrativosPagamentoEntity = base._connection.Query<MatriculaEspelhoPontoEntity, MatriculaEntity, PessoaFisicaEntity, PessoaJuridicaEntity, MatriculaDemonstrativoPagamentoEventoEntity, EventoEntity, MatriculaEspelhoPontoEntity>(
                    cmdText,
                    map: (mapMatriculaEspelhoPonto, mapMatricula, mapPessoaFisica, mapPessoaJuridica, mapMatriculaDemonstrativoPagamentoEventos, mapEvento) =>
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

                string cmdText = @" UPDATE [{0}].[dbo].[MATRICULAS_DEMONSTRATIVOS_PAGAMENTO]
                                       SET [GUIDMATRICULA] = {1}GuidMatricula,
                                           [COMPETENCIA] = {1}Competencia
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