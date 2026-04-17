namespace PayCheck.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GetTokenAsync();
    }
}