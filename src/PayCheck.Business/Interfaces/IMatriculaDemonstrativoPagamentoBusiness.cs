namespace PayCheck.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IMatriculaDemonstrativoPagamentoBusiness
    {
        MatriculaDemonstrativoPagamentoResponse Get(Guid guid);

        IEnumerable<MatriculaDemonstrativoPagamentoResponse> Get(string competencia, string matricula);

        IEnumerable<MatriculaDemonstrativoPagamentoResponse> GetAll();

        IEnumerable<MatriculaDemonstrativoPagamentoResponse> GetByGuidColaborador(Guid guidColaborador);

        MatriculaDemonstrativoPagamentoResponse SaveData(MatriculaDemonstrativoPagamentoRequestCreateDto? createDto = null, MatriculaDemonstrativoPagamentoRequestUpdateDto? updateDto = null);
    }
}