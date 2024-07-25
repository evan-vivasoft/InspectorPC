/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Model;
using Inspector.UI.Initialization.ViewModels;

namespace Inspector.UI.Initialization.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class InitializationStep : ViewModelBase
    {
        private string m_StepId;
        private string m_StepName;
        private InitializationStepResult m_StepResult;
        private string m_StepMessage;
        private int m_StepErrorCode;

        /// <summary>
        /// Gets or sets the step id.
        /// </summary>
        /// <value>
        /// The step id.
        /// </value>
        public string StepId
        {
            get { return m_StepId; }
            set
            {
                if (m_StepId != value)
                {
                    m_StepId = value;
                    OnPropertyChanged("StepId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the step.
        /// </summary>
        /// <value>
        /// The name of the step.
        /// </value>
        public string StepName
        {
            get { return m_StepName; }
            set
            {
                if (m_StepName != value)
                {
                    m_StepName = value;
                    OnPropertyChanged("StepName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the step result.
        /// </summary>
        /// <value>
        /// The step result.
        /// </value>
        public InitializationStepResult StepResult
        {
            get { return m_StepResult; }
            set
            {
                if (m_StepResult != value)
                {
                    m_StepResult = value;
                    OnPropertyChanged("StepResult");
                }
            }
        }

        /// <summary>
        /// Gets or sets the step message.
        /// </summary>
        /// <value>
        /// The step message.
        /// </value>
        public string StepMessage
        {
            get { return m_StepMessage; }
            set
            {
                if (m_StepMessage != value)
                {
                    m_StepMessage = value;
                    OnPropertyChanged("StepMessage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the step error code.
        /// </summary>
        /// <value>
        /// The step error code.
        /// </value>
        public int StepErrorCode
        {
            get { return m_StepErrorCode; }
            set
            {
                if (m_StepErrorCode != value)
                {
                    m_StepErrorCode = value;
                    OnPropertyChanged("StepErrorCode");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationStep"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public InitializationStep(string id, string name, InitializationStepResult result, string message, int errorCode)
        {
            this.StepId = id;
            this.StepName = name;
            this.StepResult = result;
            this.StepMessage = message;
            this.StepErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationStep"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public InitializationStep(string id, string name)
        {
            this.StepId = id;
            this.StepName = name;
            this.StepResult = InitializationStepResult.UNSET;
            this.StepMessage = String.Empty;
            this.StepErrorCode = -1;
        }
    }
}
