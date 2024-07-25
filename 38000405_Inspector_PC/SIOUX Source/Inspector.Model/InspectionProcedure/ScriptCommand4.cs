/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Data Model for Script Command 4
    /// </summary>
    public class ScriptCommand4 : ScriptCommandWithDescriptions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets the type question.
        /// </summary>
        /// <value>The type question.</value>
        public TypeQuestion TypeQuestion { get; set; }

        /// <summary>
        /// Gets or sets the text options.
        /// </summary>
        /// <value>The text options.</value>
        public List<string> TextOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand4"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the station step object.
        /// </summary>
        /// <value>The station step object.</value>
        public StationStepObject StationStepObject { get; set; }
        #endregion Properties
    }
}
