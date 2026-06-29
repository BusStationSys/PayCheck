namespace PayCheck.Web.Extensions
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;
    using PayCheck.Web.Common;

    /// <summary>
    /// Extension methods for the TempData dictionary in ASP.NET Core MVC. This class provides methods to add and retrieve notifications from the TempData dictionary, allowing for easy management of user messages across requests. The notifications are stored as JSON strings and can be deserialized into a list of Notification objects when retrieved.
    /// </summary>
    public static class TempDataExtension
    {
        private const string KEY = "Notifications";

        /// <summary>
        /// Adds a notification to the TempData dictionary. The notification is stored as a JSON string and can be retrieved later to display messages to the user.
        /// </summary>
        /// <param name="tempData">The TempData dictionary to add the notification to.</param>
        /// <param name="type">The type of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        public static void AddNotification(this ITempDataDictionary tempData, NotificationType type, string message)
        {
            var notifications = tempData.TryGetValue(KEY, out object? value) ?
                JsonConvert.DeserializeObject<List<Notification>>(value.ToString()) :
                new List<Notification>();

            notifications.Add(
                new Notification
                {
                    Message = message,
                    Type = type,
                });

            tempData[KEY] = JsonConvert.SerializeObject(
                notifications);
        }

        /// <summary>
        /// Retrieves the list of notifications from the TempData dictionary. If there are no notifications, it returns an empty list. The notifications are stored as a JSON string and are deserialized into a list of Notification objects.
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns>A list of notifications retrieved from the TempData dictionary.</returns>
        public static List<Notification> GetNotifications(this ITempDataDictionary tempData)
        {
            if (!tempData.TryGetValue(KEY, out object? value))
                return new List<Notification>();

            return JsonConvert.DeserializeObject<List<Notification>>(
                value.ToString());
        }
    }
}