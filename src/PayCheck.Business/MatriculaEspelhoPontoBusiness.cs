namespace PayCheck.Business
{
    using System;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using ARVTech.Transmission.Engine.UniPayCheck.Results;
    using AutoMapper;

    public class MatriculaEspelhoPontoBusiness : BaseBusiness
    {
        private readonly int _idNormal = 1;
        private readonly int _idExtra050 = 2;
        private readonly int _idExtra070 = 3;
        private readonly int _idExtra100 = 4;
        private readonly int _idAdicionalNoturno = 5;
        private readonly int _idAtestado = 6;
        private readonly int _idPaternidade = 7;
        private readonly int _idSeguro = 8;
        private readonly int _idFalta = 9;
        private readonly int _idFaltaJustificada = 10;
        private readonly int _idAtraso = 11;
        private readonly int _idCreditoBH = 12;
        private readonly int _idDebitoBH = 13;
        private readonly int _idSaldoBH = 14;
        private readonly int _idDispensaNaoRemunerada = 15;

        public MatriculaEspelhoPontoBusiness(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaEspelhoPontoDto, MatriculaEspelhoPontoEntity>().ReverseMap();
                cfg.CreateMap<MatriculaDto, MatriculaEntity>().ReverseMap();
                cfg.CreateMap<PessoaFisicaDto, PessoaFisicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaDto, PessoaJuridicaEntity>().ReverseMap();
                cfg.CreateMap<PessoaDto, PessoaEntity>().ReverseMap();
            });

            this._mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        public void Delete(Guid guid)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                connection.BeginTransaction();

                connection.Repositories.MatriculaEspelhoPontoRepository.Delete(
                    guid);

                connection.CommitTransaction();
            }
            catch
            {
                if (connection.Transaction != null)
                {
                    connection.Rollback();
                }

                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="competencia"></param>
        /// <param name="guidMatricula"></param>
        public void DeleteByCompetenciaAndGuidMatricula(string competencia, Guid guidMatricula)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                if (string.IsNullOrEmpty(competencia))
                    throw new ArgumentNullException(
                        nameof(competencia));
                else if (guidMatricula == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guidMatricula));

                connection.BeginTransaction();

                connection.Repositories.MatriculaEspelhoPontoRepository.DeleteCalculosByCompetenciaAndGuidMatricula(
                    competencia,
                    guidMatricula);

                connection.CommitTransaction();
            }
            catch
            {
                if (connection.Transaction != null)
                {
                    connection.Rollback();
                }

                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public MatriculaDemonstrativoPagamentoDto Get(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                    throw new ArgumentNullException(
                        nameof(guid));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaDemonstrativoPagamentoRepository.Get(
                        guid);

                    return this._mapper.Map<MatriculaDemonstrativoPagamentoDto>(
                        entity);
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
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns></returns>
        public MatriculaEspelhoPontoDto GetByCompetenciaAndMatricula(string competencia, string matricula)
        {
            try
            {
                if (string.IsNullOrEmpty(competencia))
                    throw new ArgumentNullException(
                        nameof(
                            competencia));
                else if (string.IsNullOrEmpty(matricula))
                    throw new ArgumentNullException(
                        nameof(
                            matricula));

                using (var connection = this._unitOfWork.Create())
                {
                    var entity = connection.Repositories.MatriculaEspelhoPontoRepository.GetByCompetenciaAndMatricula(
                        competencia,
                        matricula);

                    return this._mapper.Map<MatriculaEspelhoPontoDto>(entity);
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
        /// <param name="espelhoPontoResult"></param>
        /// <returns></returns>
        public MatriculaEspelhoPontoDto Import(EspelhoPontoResult espelhoPontoResult)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                connection.BeginTransaction();

                //  Verifica se existe o registro do Colaborador.
                var pessoaFisicaDto = default(PessoaFisicaDto);

                using (var pessoaFisicaBusiness = new PessoaFisicaBusiness(this._unitOfWork))
                {
                    pessoaFisicaDto = pessoaFisicaBusiness.GetByNome(
                        espelhoPontoResult.Nome);
                }

                //  Se não existir o registro do Colaborador, adiciona.
                if (pessoaFisicaDto is null)
                {
                    pessoaFisicaDto = new PessoaFisicaDto
                    {
                        Nome = espelhoPontoResult.Nome,
                        NumeroCtps = espelhoPontoResult.NumeroCtps,
                        SerieCtps = espelhoPontoResult.SerieCtps,
                        UfCtps = espelhoPontoResult.UfCtps,
                        Cpf = espelhoPontoResult.Cpf,
                        Pessoa = new PessoaDto()
                        {
                            Cidade = "ESTEIO",
                            Endereco = "ENDERECO",
                            Uf = "RS",
                        },
                    };

                    using (var pessoaFisicaBusiness = new PessoaFisicaBusiness(this._unitOfWork))
                    {
                        pessoaFisicaDto = pessoaFisicaBusiness.SaveData(
                            pessoaFisicaDto);
                    }
                }

                //  Verifica se existe o registro do Empregador.
                var pessoaJuridicaDto = default(PessoaJuridicaDto);

                using (var pessoaJuridicaBusiness = new PessoaJuridicaBusiness(this._unitOfWork))
                {
                    pessoaJuridicaDto = pessoaJuridicaBusiness.GetByRazaoSocial(
                        espelhoPontoResult.RazaoSocial);
                }

                //  Se não existir o registro do Empregador, adiciona.
                if (pessoaJuridicaDto is null)
                {
                    pessoaJuridicaDto = new PessoaJuridicaDto
                    {
                        Cnpj = espelhoPontoResult.Cnpj,
                        RazaoSocial = espelhoPontoResult.RazaoSocial,
                        Pessoa = new PessoaDto()
                        {
                            Cidade = "ESTEIO",
                            Endereco = "ENDERECO",
                            Uf = "RS",
                        },
                    };

                    using (var pessoaJuridicaBusiness = new PessoaJuridicaBusiness(this._unitOfWork))
                    {
                        pessoaJuridicaDto = pessoaJuridicaBusiness.SaveData(
                            pessoaJuridicaDto);
                    }
                }

                //  Verifica se existe o registro da Matrícula.
                var matriculaDto = default(MatriculaDto);

                using (var matriculaBusiness = new MatriculaBusiness(this._unitOfWork))
                {
                    matriculaDto = matriculaBusiness.GetByMatricula(
                        espelhoPontoResult.Matricula);
                }

                //  Se não existir o registro da Matrícula, adiciona.
                if (matriculaDto is null)
                {
                    matriculaDto = new MatriculaDto
                    {
                        GuidColaborador = pessoaFisicaDto.Guid,
                        GuidEmpregador = pessoaJuridicaDto.Guid,
                        DataAdmissao = Convert.ToDateTime(
                            espelhoPontoResult.DataAdmissao),
                        Matricula = espelhoPontoResult.Matricula,
                    };

                    using (var matriculaBusiness = new MatriculaBusiness(this._unitOfWork))
                    {
                        matriculaDto = matriculaBusiness.SaveData(
                            matriculaDto);
                    }
                }

                string competencia = string.Concat("01/", espelhoPontoResult.Competencia);
                competencia = Convert.ToDateTime(competencia).ToString("yyyyMMdd");

                //  Verifica se existe o registro do Espelho Ponto da Matrícula.
                var matriculaEspelhoPontoDto = default(MatriculaEspelhoPontoDto);

                using (var matriculaEspelhoPontoBusiness = new MatriculaEspelhoPontoBusiness(
                    this._unitOfWork))
                {
                    //  Independente se existir um ou mais registros de Espelho de Ponto para a Matrícula, deve forçar a limpeza dos Itens dos Espelhos de Ponto que possam estar vinculado à Matrícula dentro da Competência.
                    matriculaEspelhoPontoBusiness.DeleteByCompetenciaAndGuidMatricula(
                        competencia,
                        (Guid)matriculaDto.Guid);

                    matriculaEspelhoPontoDto = matriculaEspelhoPontoBusiness.GetByCompetenciaAndMatricula(
                        competencia,
                        espelhoPontoResult.Matricula);

                    //  Se não existir o registro do Espelho de Ponto da Matrícula, adiciona.
                    if (matriculaEspelhoPontoDto is null)
                    {
                        matriculaEspelhoPontoDto = new MatriculaEspelhoPontoDto
                        {
                            GuidMatricula = matriculaDto.Guid,
                            Competencia = competencia,
                        };

                        matriculaEspelhoPontoDto = matriculaEspelhoPontoBusiness.SaveData(
                            matriculaEspelhoPontoDto);
                    }

                    // Processa os Vínculos dos Totalizadores.
                    decimal baseFgts = decimal.Zero;
                    decimal valorFgts = decimal.Zero;
                    decimal totalVencimentos = decimal.Zero;
                    decimal totalDescontos = decimal.Zero;
                    decimal baseIrrf = decimal.Zero;
                    decimal baseInss = decimal.Zero;
                    decimal totalLiquido = decimal.Zero;

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.BaseFgts))
                    //{
                    //    baseFgts = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.BaseFgts);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.ValorFgts))
                    //{
                    //    valorFgts = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.ValorFgts);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.TotalVencimentos))
                    //{
                    //    totalVencimentos = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.TotalVencimentos);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.TotalDescontos))
                    //{
                    //    totalDescontos = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.TotalDescontos);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.BaseIrrf))
                    //{
                    //    baseIrrf = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.BaseIrrf);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.BaseInss))
                    //{
                    //    baseInss = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.BaseInss);
                    //}

                    //if (!string.IsNullOrEmpty(demonstrativoPagamentoResult.TotalLiquido))
                    //{
                    //    totalLiquido = Convert.ToDecimal(
                    //        demonstrativoPagamentoResult.TotalLiquido);
                    //}

                    ////  Processa a Base Fgts.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idBaseFgts,
                    //    baseFgts);

                    ////  Processa o Valor Fgts.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idValorFgts,
                    //    valorFgts);

                    ////  Processa o Total de Vencimentos.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idTotalVencimentos,
                    //    totalVencimentos);

                    ////  Processa o Total de Descontos.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idTotalDescontos,
                    //    totalDescontos);

                    ////  Processa a Base Irrf.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idBaseIrrf,
                    //    baseIrrf);

                    ////  Processa a Base Inss.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idBaseInss,
                    //    baseInss);

                    ////  Processa o Total Líquido.
                    //this.processRecordMDPTotalizador(
                    //    (Guid)matriculaDemonstrativoPagamentoDto.Guid,
                    //    this._idTotalLiquido,
                    //    totalLiquido);
                }

                connection.CommitTransaction();

                return matriculaEspelhoPontoDto;
            }
            catch
            {
                if (connection.Transaction != null)
                {
                    connection.Rollback();
                }

                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public MatriculaEspelhoPontoDto SaveData(MatriculaEspelhoPontoDto dto)
        {
            var connection = this._unitOfWork.Create();

            try
            {
                var entity = this._mapper.Map<MatriculaEspelhoPontoEntity>(dto);

                connection.BeginTransaction();

                if (dto.Guid != null && dto.Guid != Guid.Empty)
                {
                    entity = connection.Repositories.MatriculaEspelhoPontoRepository.Update(
                        entity);
                }
                else
                {
                    entity = connection.Repositories.MatriculaEspelhoPontoRepository.Create(
                        entity);
                }

                connection.CommitTransaction();

                return this._mapper.Map<MatriculaEspelhoPontoDto>(
                    entity);
            }
            catch
            {
                if (connection.Transaction != null)
                {
                    connection.Rollback();
                }

                throw;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guidMatriculaDemonstrativoPagamento"></param>
        /// <param name="idEvento"></param>
        /// <param name="referencia"></param>
        /// <param name="valor"></param>
        private void processRecordMDPEvento(Guid guidMatriculaDemonstrativoPagamento, int idEvento, decimal? referencia, decimal valor)
        {
            try
            {
                //  Verifica se existe o registro do vínculo do Demonstrativo de Pagamento x Evento.
                var matriculaDemonstrativoPagamentoEventoDto = default(
                    MatriculaDemonstrativoPagamentoEventoDto);

                using (var matriculaDemonstrativoPagamentoEventoBusiness = new MatriculaDemonstrativoPagamentoEventoBusiness(
                    this._unitOfWork))
                {
                    matriculaDemonstrativoPagamentoEventoDto = matriculaDemonstrativoPagamentoEventoBusiness.GetByGuidMatriculaDemonstrativoPagamentoAndIdEvento(
                        guidMatriculaDemonstrativoPagamento,
                        idEvento);

                    //  Se não existir o registro do do vínculo do Demonstrativo de Pagamento x Evento, adiciona.
                    if (matriculaDemonstrativoPagamentoEventoDto is null)
                    {
                        matriculaDemonstrativoPagamentoEventoDto = new MatriculaDemonstrativoPagamentoEventoDto
                        {
                            GuidMatriculaDemonstrativoPagamento = guidMatriculaDemonstrativoPagamento,
                            IdEvento = idEvento,
                        };
                    }

                    matriculaDemonstrativoPagamentoEventoDto.Referencia = referencia;
                    matriculaDemonstrativoPagamentoEventoDto.Valor = valor;

                    matriculaDemonstrativoPagamentoEventoBusiness.SaveData(
                        matriculaDemonstrativoPagamentoEventoDto);
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
        /// <param name="guidMatriculaDemonstrativoPagamento"></param>
        /// <param name="idTotalizador"></param>
        /// <param name="valor"></param>
        private void processRecordMDPTotalizador(Guid guidMatriculaDemonstrativoPagamento, int idTotalizador, decimal valor)
        {
            try
            {
                //  Verifica se existe o registro do vínculo do Demonstrativo de Pagamento x Totalizador.
                var matriculaDemonstrativoPagamentoTotalizadorDto = default(
                    MatriculaDemonstrativoPagamentoTotalizadorDto);

                using (var matriculaDemonstrativoPagamentoTotalizadorBusiness = new MatriculaDemonstrativoPagamentoTotalizadorBusiness(
                    this._unitOfWork))
                {
                    matriculaDemonstrativoPagamentoTotalizadorDto = matriculaDemonstrativoPagamentoTotalizadorBusiness.GetByGuidMatriculaDemonstrativoPagamentoAndIdTotalizador(
                        guidMatriculaDemonstrativoPagamento,
                        idTotalizador);

                    //  Se não existir o registro do do vínculo do Demonstrativo de Pagamento x Totalizador, adiciona.
                    if (matriculaDemonstrativoPagamentoTotalizadorDto is null)
                    {
                        matriculaDemonstrativoPagamentoTotalizadorDto = new MatriculaDemonstrativoPagamentoTotalizadorDto
                        {
                            GuidMatriculaDemonstrativoPagamento = guidMatriculaDemonstrativoPagamento,
                            IdTotalizador = idTotalizador,
                        };
                    }

                    matriculaDemonstrativoPagamentoTotalizadorDto.Valor = valor;

                    matriculaDemonstrativoPagamentoTotalizadorBusiness.SaveData(
                        matriculaDemonstrativoPagamentoTotalizadorDto);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}