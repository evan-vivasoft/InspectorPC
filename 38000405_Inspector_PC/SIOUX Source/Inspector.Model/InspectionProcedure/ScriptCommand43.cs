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
    /// Data Model for Script Command 43
    /// </summary>
    public class ScriptCommand43 : ScriptCommandWithDescriptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        public string Instruction { get; set; }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        /// <value>The list items.</value>
        public List<string> ListItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand43"/> is required.
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
