/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// ExecuteStepEventArgs
    /// </summary>
    public class ExecuteInspectionStepEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the script command.
        /// </summary>
        public ScriptCommandBase ScriptCommand { get; private set; }

        /// <summary>
        /// Gets the current inspection step.
        /// </summary>
        /// <value>The current inspection step.</value>
        public int CurrentInspectionStep { get; private set; }

        /// <summary>
        /// Gets the total inspection steps.
        /// </summary>
        /// <value>The total inspection steps.</value>
        public int TotalInspectionSteps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteInspectionStepEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExecuteInspectionStepEventArgs(ScriptCommandBase scriptCommand, int currentInspectionStep, int totalInspectionSteps)
        {
            this.ScriptCommand = scriptCommand;
            this.CurrentInspectionStep = currentInspectionStep;
            this.TotalInspectionSteps = totalInspectionSteps;
        }
    }
}
