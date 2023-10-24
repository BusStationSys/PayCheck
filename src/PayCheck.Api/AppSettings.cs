namespace PayCheck.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        public DataAccess DataAccess { get; set; }
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Authentication Authentication { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataAccess
    {
        public SqlServer SqlServer { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SqlServer
    {
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogLevel
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Authentication
    {
        public string Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}