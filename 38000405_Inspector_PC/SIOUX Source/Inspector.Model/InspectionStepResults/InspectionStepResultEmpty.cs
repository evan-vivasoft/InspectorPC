/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionStepResult
{
    /// <summary>
    /// An inspection step result that has no data, only the sequence number
    /// </summary>
    public class InspectionStepResultEmpty : InspectionStepResultBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultEmpty"/> class.
        /// </summary>
        public InspectionStepResultEmpty()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultEmpty"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        public InspectionStepResultEmpty(long sequenceNumber)
        {
            this.SequenceNumber = sequenceNumber;
        }
        #endregion Constructors
    }
}
