namespace PayCheck.Web.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IAuthService
    {
        Task<string> GetTokenAsync();
    }
}