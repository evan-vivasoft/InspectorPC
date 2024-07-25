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
    /// InspectionProcedureInformation
    /// </summary>
    public class InspectionProcedureInformation
    {
        private Stack<ScriptCommandBase> m_ScriptCommandSequence;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the script command sequence.
        /// </summary>
        /// <value>The script command sequence.</value>
        public Stack<ScriptCommandBase> ScriptCommandSequence
        {
            get
            {
                if (m_ScriptCommandSequence == null)
                {
                    m_ScriptCommandSequence = new Stack<ScriptCommandBase>();
                }
                return m_ScriptCommandSequence;
            }
            set
            {
                m_ScriptCommandSequence = value;
            }
        }
    }
}
