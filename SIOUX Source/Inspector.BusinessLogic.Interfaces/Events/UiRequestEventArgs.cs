using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    public class UiRequestEventArgs : EventArgs
    {
        public string RequestMessage { get; set; }

        public UIRequestResponseType ResponseType { get; set; }

        public UiRequestEventArgs(string requestMessage, UIRequestResponseType responseType)
        {
            RequestMessage = requestMessage;
            ResponseType = responseType;
        }
    }
}
