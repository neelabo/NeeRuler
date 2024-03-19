using NeeRuler.Properties;
using System.IO;
using System.Windows;

namespace NeeRuler
{
    public static class NotificationBox
    {
        public static MessageBoxResult Show(string text, NotificationType type)
        {
            var caption = AppContext.ProductName + " " + type.ToString();
            return Show(text, caption, type);
        }

        public static MessageBoxResult Show(string text, string caption, NotificationType type)
        {
            var image = type switch
            {
                NotificationType.Error => MessageBoxImage.Error,
                NotificationType.Warning => MessageBoxImage.Warning,
                _ => MessageBoxImage.None,
            };

            return MessageBox.Show(text, caption, MessageBoxButton.OK, image);
        }

        public static MessageBoxResult ShowFailedToLoadFile(string fileName, string message, NotificationType type)
        {
            return Show(string.Format(TextResources.GetString("Notify.FailedToLoadFile"), Path.GetFileName(fileName)) + "\n\n" + message, type);
        }

    }
}