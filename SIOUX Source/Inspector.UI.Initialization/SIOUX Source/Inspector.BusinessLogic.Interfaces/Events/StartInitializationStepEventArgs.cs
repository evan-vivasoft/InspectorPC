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
    /// StartInitializationStepEventArgs
    /// </summary>
    public class StartInitializationStepEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the initializatio step id.
        /// </summary>
        public string StepId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartInitializationStepEventArgs"/> class.
        /// </summary>
        /// <param name="stepId">The step id.</param>
        public StartInitializationStepEventArgs(string stepId)
        {
            this.StepId = stepId;
        }
    }
}
