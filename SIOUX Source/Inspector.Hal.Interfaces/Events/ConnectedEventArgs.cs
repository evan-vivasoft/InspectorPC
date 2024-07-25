/*
//===============================================================================
// Copyright Wigersma & Sikkema
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Infra;

namespace Inspector.Hal.Interfaces.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public DeviceType DeviceType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedEventArgs"/> class.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ConnectedEventArgs(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }
    }
}
