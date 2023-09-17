namespace PayCheck.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IMatriculaDemonstrativoPagamentoBusiness
    {
        MatriculaDemonstrativoPagamentoResponseDto Get(Guid guid);

        IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> Get(string competencia, string matricula);

        IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> GetAll();

        IEnumerable<MatriculaDemonstrativoPagamentoResponseDto> GetByGuidColaborador(Guid guidColaborador);

        MatriculaDemonstrativoPagamentoResponseDto SaveData(MatriculaDemonstrativoPagamentoRequestCreateDto? createDto = null, MatriculaDemonstrativoPagamentoRequestUpdateDto? updateDto = null);
    }
}