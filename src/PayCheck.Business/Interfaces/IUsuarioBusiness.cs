namespace PayCheck.Business.Interfaces
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IUsuarioBusiness
    {
        UsuarioResponse Authenticate(string cpfEmailUsername, string password);
    }
}
