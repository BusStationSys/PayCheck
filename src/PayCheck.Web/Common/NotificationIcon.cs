namespace PayCheck.Web.Common
{
    using System.ComponentModel;

    public enum NotificationIcon
    {
        [Description("fa-check")]
        FaCheck,

        [Description("fa-ban")]
        FaBan,

        [Description("fa-exclamation")]
        FaExclamation,

        [Description("fa-info")]
        FaInfo,

        [Description("fa-circle")]
        FaCircle,

        [Description("fa-moon")]
        FaMoon,
    }
}