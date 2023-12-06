namespace PayCheck.Web.Models
{
    public class LoginViewModel
    {
        public string CpfEmailUsername { get; set; }

        public string Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}