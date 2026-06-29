namespace PayCheck.Web.Common
{
    using System.ComponentModel;

    public enum NotificationType
    {
        [Description("Sucesso")]
        Primary,    //  Azul

        [Description("Sucesso")]
        Secondary,  //  Cinza

        [Description("Sucesso")]
        Success,    //  Verde

        [Description("Erro")]
        Danger,     //  Vermelho

        [Description("Aviso")]
        Warning,    //  Amarelo

        [Description("Informação")]
        Info,       //  Azul-claro

        [Description("Informação")]
        Light,      //  Claro

        [Description("Informação")]
        Dark,       //  Escuro
    }
}