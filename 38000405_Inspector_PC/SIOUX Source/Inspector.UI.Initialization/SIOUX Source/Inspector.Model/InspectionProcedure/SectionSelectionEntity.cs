/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// SectionSelectionEntity
    /// </summary>
    public class SectionSelectionEntity
    {
        #region Properties
        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public long SequenceNumber { get; private set; }

        /// <summary>
        /// Gets the section.
        /// </summary>
        public string Section { get; private set; }

        /// <summary>
        /// Gets the subsection.
        /// </summary>
        public string Subsection { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionSelectionEntity"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="section">The section.</param>
        /// <param name="subsection">The subsection.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        internal SectionSelectionEntity(long sequenceNumber, string section, string subsection, bool isSelected = false)
        {
            this.SequenceNumber = sequenceNumber;
            this.Section = section;
            this.Subsection = subsection;
            this.IsSelected = isSelected;
        }
        #endregion Constructors
    }
}
