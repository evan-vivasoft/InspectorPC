/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Data Model for Script Command 2
    /// </summary>
    public class ScriptCommand2 : ScriptCommandBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        /// <value>The section.</value>
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the sub section.
        /// </summary>
        /// <value>The sub section.</value>
        public string SubSection { get; set; }
        #endregion Properties
    }
}
