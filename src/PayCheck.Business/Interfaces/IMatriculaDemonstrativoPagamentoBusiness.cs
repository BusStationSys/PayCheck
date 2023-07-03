namespace PayCheck.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IMatriculaDemonstrativoPagamentoBusiness
    {
        MatriculaDemonstrativoPagamentoDto Get(Guid guid);

        IEnumerable<MatriculaDemonstrativoPagamentoDto> Get(string competencia, string matricula);

        IEnumerable<MatriculaDemonstrativoPagamentoDto> GetAll();
    }
}