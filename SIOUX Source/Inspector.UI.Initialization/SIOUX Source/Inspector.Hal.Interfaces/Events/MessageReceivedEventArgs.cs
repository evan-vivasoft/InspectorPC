/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.Hal.Interfaces.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageReceivedEventArgs(string data)
        {
            this.Data = data;
        }
    }
}
