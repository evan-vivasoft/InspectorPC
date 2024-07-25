/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Model;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// FinishInitializationEventArgs
    /// </summary>
    public class FinishInitializationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        public InitializationResult Result { get; private set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY: No errors encountered
        /// ErrorCodes.INITIALIZATION_FINISHED_ERROR: The initialization had an error in one of the steps
        /// ErrorCodes.INITIALIZATION_FINISHED_WARNING: The initialization had a warning in one or more of the steps
        /// ErrorCodes.INITIALIZATION_FINISHED_TIMEOUT: The initialization encountered a timeout on one or more of the manometers
        /// Other error code: The error code that occurred (e.g.: ErrorCodes.COMMUNICATIONCONTROL_ALREADY_CLAIMED)
        /// </value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinishInitializationEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorCode">The error code.</param>
        public FinishInitializationEventArgs(InitializationResult result, int errorCode)
        {
            this.Result = result;
            this.ErrorCode = errorCode;
        }
    }
}
