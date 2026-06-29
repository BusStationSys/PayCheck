namespace PayCheck.Web.Common
{
    public class Notification
    {
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public string? Link { get; set; }

        public string? LinkMessage { get; set; }

        public string? LinkText { get; set; }

        public NotificationIcon? Icon { get; set; }

        public Notification()
        { }

        public Notification(string message)
        {
            this.Message = message;
        }
    }
}