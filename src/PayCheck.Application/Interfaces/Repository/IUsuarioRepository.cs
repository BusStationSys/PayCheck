namespace PayCheck.Application.Interfaces.Repository
{
    using ARVTech.DataAccess.Core.Entities.UniPayCheck;
    using PayCheck.Application.Interfaces.Actions;

    /// <summary>
    /// 
    /// </summary>
    public interface IUsuarioRepository : ICreateRepository<UsuarioEntity, UsuarioEntity>, IReadRepository<UsuarioEntity, Guid>, IUpdateRepository<UsuarioEntity,Guid, UsuarioEntity>, IDeleteRepository<Guid>
    {
        UsuarioEntity Authenticate(string cpfEmailUsername, string password);
    }
}