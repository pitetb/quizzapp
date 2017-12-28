using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Controllers
{   
    /// <summary>
    /// Notification with or without a string message
    /// </summary>
    public class ControllerNotificationEventArgs : EventArgs
    {
        public ControllerNotificationEventArgs() { }
        public ControllerNotificationEventArgs(string message)
        {
            Message = message;
        }

        // Message
        public string Message { get; protected set; }
    }

    /// <summary>
    /// Notification with outgoing data
    /// </summary>
    /// <typeparam name="TOutgoing">Outgoing data type</typeparam>
    public class ControllerNotificationEventArgs<TOutgoing> : ControllerNotificationEventArgs
    {
        public ControllerNotificationEventArgs() { }

        public ControllerNotificationEventArgs(string message) : base(message) { }

        public ControllerNotificationEventArgs(string message, TOutgoing data)
            : this(message)
        {
            Data = data;
        }

        // Data
        public TOutgoing Data { get; protected set; }
    }

}
