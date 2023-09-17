namespace PayCheck.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IMatriculaEspelhoPontoBusiness
    {
        MatriculaEspelhoPontoResponseDto Get(Guid guid);

        IEnumerable<MatriculaEspelhoPontoResponseDto> Get(string competencia, string matricula);

        IEnumerable<MatriculaEspelhoPontoResponseDto> GetAll();

        IEnumerable<MatriculaEspelhoPontoResponseDto> GetByGuidColaborador(Guid guidColaborador);

        MatriculaEspelhoPontoResponseDto SaveData(MatriculaEspelhoPontoRequestCreateDto? createDto = null, MatriculaEspelhoPontoRequestUpdateDto? updateDto = null);
    }
}