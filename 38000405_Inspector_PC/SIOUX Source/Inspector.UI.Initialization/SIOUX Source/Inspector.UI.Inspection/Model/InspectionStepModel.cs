/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.UI.Inspection.ViewModels;
using Inspector.Model;

namespace Inspector.UI.Inspection.Model
{
    /// <summary>
    /// InspectionStepModel
    /// </summary>
    public class InspectionStepModel : ViewModelBase
    {
        private string m_ScriptCommand;
        private string m_SequenceNumber;
        private InitializationStepResult m_Result;
        /// <summary>
        /// Gets or sets the script command.
        /// </summary>
        /// <value>
        /// The script command.
        /// </value>
        public string ScriptCommand
        {
            get { return m_ScriptCommand; }
            set
            {
                if (m_ScriptCommand != value)
                {
                    m_ScriptCommand = value;
                    OnPropertyChanged("ScriptCommand");
                }
            }
        }

        /// <summary>
        /// Gets or sets the result
        /// </summary>
        /// <value>
        /// The result
        /// </value>
        public InitializationStepResult Result
        {
            get { return m_Result; }
            set
            {
                if (m_Result != value)
                {
                    m_Result= value;
                    OnPropertyChanged("Result");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public string SequenceNumber
        {
            get { return m_SequenceNumber; }
            set
            {
                if (m_SequenceNumber != value)
                {
                    m_SequenceNumber = value;
                    OnPropertyChanged("SequenceNumber");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepModel"/> class.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        public InspectionStepModel(string scriptCommand, string sequenceNumber, InitializationStepResult result = InitializationStepResult.UNSET)
        {
            this.ScriptCommand = scriptCommand;
            this.Result = result;
            this.SequenceNumber = sequenceNumber;
        }
    }
}
