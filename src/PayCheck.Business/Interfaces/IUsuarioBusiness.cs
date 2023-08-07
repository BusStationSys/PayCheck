﻿namespace PayCheck.Business.Interfaces
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IUsuarioBusiness
    {
        UsuarioResponse GetByUsername(string cpfEmailUsername);

        UsuarioResponse CheckPasswordValid(Guid guid, string password);

        UsuarioResponse SaveData(UsuarioRequestCreateDto? createDto = null, UsuarioRequestUpdateDto? updateDto = null);
    }
}