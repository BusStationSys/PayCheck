namespace PayCheck.Api
{
    public class AppSettings
    {
        public int ExpiresInMinutes { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string Password { get; set; }

        public string Secret { get; set; }

        public string Username { get; set; }
    }
}