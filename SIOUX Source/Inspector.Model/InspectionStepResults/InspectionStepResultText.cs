/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionStepResult
{
    /// <summary>
    /// An inspection step result that has the sequence number and text as data
    /// </summary>
    public class InspectionStepResultText : InspectionStepResultBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the text that contains the answer to the script command.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultText"/> class.
        /// </summary>
        public InspectionStepResultText()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultText"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="text">The text.</param>
        public InspectionStepResultText(long sequenceNumber, string text)
        {
            this.SequenceNumber = sequenceNumber;
            this.Text = text;
        }
        #endregion Constructors
    }
}
