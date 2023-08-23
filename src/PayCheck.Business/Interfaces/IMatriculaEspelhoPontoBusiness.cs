namespace PayCheck.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IMatriculaEspelhoPontoBusiness
    {
        MatriculaEspelhoPontoResponse Get(Guid guid);

        IEnumerable<MatriculaEspelhoPontoResponse> Get(string competencia, string matricula);

        IEnumerable<MatriculaEspelhoPontoResponse> GetAll();

        IEnumerable<MatriculaEspelhoPontoResponse> GetByGuidColaborador(Guid guidColaborador);
    }
}