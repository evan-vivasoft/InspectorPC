/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;

namespace Inspector.Model.InspectionStepResult
{
    /// <summary>
    /// An inspection step result that has the sequence number and up to 5 selected option answers including an optional remark
    /// </summary>
    public class InspectionStepResultSelections : InspectionStepResultBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the remark.
        /// </summary>
        /// <value>The remark.</value>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the answer selection1.
        /// </summary>
        /// <value>The answer selection1.</value>
        public string AnswerSelection1 { get; set; }

        /// <summary>
        /// Gets or sets the answer selection2.
        /// </summary>
        /// <value>The answer selection2.</value>
        public string AnswerSelection2 { get; set; }

        /// <summary>
        /// Gets or sets the answer selection3.
        /// </summary>
        /// <value>The answer selection3.</value>
        public string AnswerSelection3 { get; set; }

        /// <summary>
        /// Gets or sets the answer selection4.
        /// </summary>
        /// <value>The answer selection4.</value>
        public string AnswerSelection4 { get; set; }

        /// <summary>
        /// Gets or sets the answer selection5.
        /// </summary>
        /// <value>The answer selection5.</value>
        public string AnswerSelection5 { get; set; }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultSelections"/> class.
        /// </summary>
        public InspectionStepResultSelections()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultSelections"/> class.
        /// </summary>
        /// <param name="remark">The remark.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="answerSelection1">The first answer selection.</param>
        /// <param name="answerSelection2">The second answer selection.</param>
        /// <param name="answerSelection3">The third answer selection.</param>
        /// <param name="answerSelection4">The fourth answer selection.</param>
        /// <param name="answerSelection5">The fifth answer selection.</param>
        public InspectionStepResultSelections(string remark, long sequenceNumber, string answerSelection1 = "", string answerSelection2 = "",
                                              string answerSelection3 = "", string answerSelection4 = "", string answerSelection5 = "")
        {
            this.SequenceNumber = sequenceNumber;
            this.Remark = remark;
            this.AnswerSelection1 = answerSelection1;
            this.AnswerSelection2 = answerSelection2;
            this.AnswerSelection3 = answerSelection3;
            this.AnswerSelection4 = answerSelection4;
            this.AnswerSelection5 = answerSelection5;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStepResultSelections"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="answerSelection1">The first answer selection.</param>
        /// <param name="answerSelection2">The second answer selection.</param>
        /// <param name="answerSelection3">The third answer selection.</param>
        /// <param name="answerSelection4">The fourth answer selection.</param>
        /// <param name="answerSelection5">The fifth answer selection.</param>
        public InspectionStepResultSelections(long sequenceNumber, string answerSelection1 = "", string answerSelection2 = "",
                                              string answerSelection3 = "", string answerSelection4 = "", string answerSelection5 = "") :
            this(String.Empty, sequenceNumber, answerSelection1, answerSelection2, answerSelection3, answerSelection4, answerSelection5)
        { }
        #endregion Constructors

        #region Public
        /// <summary>
        /// Gets the ordered list of filled-in selections as a single list from the properties AnswerSelection1 up to AnswerSelection5
        /// </summary>
        /// <value>The list selections.</value>
        public List<string> ListSelections
        {
            get
            {
                List<string> listSelection = new List<string>();

                if (!String.IsNullOrEmpty(this.AnswerSelection1))
                {
                    listSelection.Add(this.AnswerSelection1);
                }

                if (!String.IsNullOrEmpty(this.AnswerSelection2))
                {
                    listSelection.Add(this.AnswerSelection2);
                }

                if (!String.IsNullOrEmpty(this.AnswerSelection3))
                {
                    listSelection.Add(this.AnswerSelection3);
                }

                if (!String.IsNullOrEmpty(this.AnswerSelection4))
                {
                    listSelection.Add(this.AnswerSelection4);
                }

                if (!String.IsNullOrEmpty(this.AnswerSelection5))
                {
                    listSelection.Add(this.AnswerSelection5);
                }

                return listSelection;
            }
        }
        #endregion Public
    }
}
