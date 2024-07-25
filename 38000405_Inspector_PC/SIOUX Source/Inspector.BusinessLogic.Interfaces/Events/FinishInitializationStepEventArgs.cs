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
    /// FinishInitializationStepEventArgs
    /// </summary>
    public class FinishInitializationStepEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the initialization step id.
        /// </summary>
        public string StepId { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public InitializationStepResult Result { get; private set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets or sets the manometer.
        /// </summary>
        /// <value>
        /// The manometer.
        /// </value>
        public InitializationManometer Manometer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinishInitializationStepEventArgs"/> class.
        /// </summary>
        /// <param name="stepId">The step id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message from the device.</param>
        public FinishInitializationStepEventArgs(string stepId, InitializationStepResult result, string message, int errorCode, InitializationManometer manometer)
        {
            this.StepId = stepId;
            this.Result = result;
            this.Message = message;
            this.ErrorCode = errorCode;
            this.Manometer = manometer;
        }
    }
}
