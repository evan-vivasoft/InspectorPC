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
    /// Data Model for Script Command 41
    /// </summary>
    public class ScriptCommand41 : ScriptCommandWithDescriptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [show next list immediately].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show next list immediately]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNextListImmediately { get; set; }

        /// <summary>
        /// Gets or sets the script command list.
        /// </summary>
        /// <value>The script command list.</value>
        public List<ScriptCommand41List> ScriptCommandList { get; set; }

        /// <summary>
        /// Gets or sets the station step object.
        /// </summary>
        /// <value>The station step object.</value>
        public StationStepObject StationStepObject { get; set; }
        #endregion Properties
    }
}
