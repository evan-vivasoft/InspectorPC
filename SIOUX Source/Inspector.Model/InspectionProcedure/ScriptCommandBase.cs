/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Base class for Data model entities for Script Commands
    /// </summary>
    public abstract class ScriptCommandBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public long SequenceNumber { get; set; }

        public Guid LinkId { get; set; }
        #endregion Properties
    }
}
