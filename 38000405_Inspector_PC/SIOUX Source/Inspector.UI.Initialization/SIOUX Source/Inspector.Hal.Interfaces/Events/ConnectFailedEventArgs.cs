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
    public class ConnectFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFailedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code. -1 denotes unknown error code</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ConnectFailedEventArgs(string message, int errorCode = -1)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
        }
    }
}
