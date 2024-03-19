using System;

namespace NeeRuler
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(string message, NotificationType messageType)
        {
            Message = message;
            MessageType = messageType;
        }

        public string Message { get; private set; }
        public NotificationType MessageType { get; private set; }
    }

}