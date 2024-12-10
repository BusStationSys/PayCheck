namespace PayCheck.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public DataAccess DataAccess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Logging Logging { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AllowedHosts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Authentication Authentication { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        public SqlServer SqlServer { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SqlServer
    {
        /// <summary>
        /// 
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServerName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// 
        /// </summary>
        public LogLevel LogLevel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogLevel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MicrosoftAspNetCore { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ExpiresInMinutes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}