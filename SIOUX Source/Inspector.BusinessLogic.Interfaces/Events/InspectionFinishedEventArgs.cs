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
    /// InspectionFinishedEventArgs
    /// </summary>
    public class InspectionFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        public InspectionStatus Result { get; private set; }

        /// <summary>
        /// Gets the message id.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets or sets the partial inspection.
        /// </summary>
        /// <value>
        /// The partial inspection. Null if an inspection without a measurement is executed.
        /// </value>
        public SectionSelection PartialInspection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinishInitializationEventArgs"/> class.
        /// </summary>
        /// <param name="result">The inspection result</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="partialInspection">The resulting partial inspection.</param>
        public InspectionFinishedEventArgs(InspectionStatus result, int errorCode, SectionSelection partialInspection)
        {
            this.Result = result;
            this.ErrorCode = errorCode;
            this.PartialInspection = partialInspection;
        }
    }
}
