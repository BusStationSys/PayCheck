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
        public int ExpiresInMinutes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
    }
}