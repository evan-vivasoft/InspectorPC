/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.BusinessLogic.Interfaces
{
    /// <summary>
    /// IActivityControl
    /// </summary>
    public interface IActivityControl : IDisposable
    {
        /// <summary>
        /// Occurs when [initialization step started].
        /// </summary>
        event EventHandler InitializationStepStarted;

        /// <summary>
        /// Occurs when [initialization step finished].
        /// </summary>
        event EventHandler InitializationStepFinished;

        /// <summary>
        /// Occurs when [initialization finished].
        /// </summary>
        event EventHandler InitializationFinished;

        /// <summary>
        /// aborts the initialization
        /// </summary>
        void Abort();
    }
}
