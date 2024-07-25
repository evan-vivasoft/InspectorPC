/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// InspectionErrorEventArgs
    /// </summary>
    public class InspectionErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int ErrorCode { get; set; }
    }
}
