/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Data Model for Script Command 3
    /// </summary>
    public class ScriptCommand3 : ScriptCommandBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }
        #endregion Properties
    }
}
