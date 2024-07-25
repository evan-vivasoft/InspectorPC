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
    /// SectionSelection
    /// </summary>
    public class SectionSelection
    {
        #region Class Members
        private List<SectionSelectionEntity> m_SectionSelectionEntities;
        #endregion Class Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionSelection"/> class.
        /// </summary>
        public SectionSelection()
        {
            m_SectionSelectionEntities = new List<SectionSelectionEntity>();
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the name of the inspection procedure.
        /// </summary>
        /// <value>
        /// The name of the inspection procedure.
        /// </value>
        public string InspectionProcedureName { get; set; }

        /// <summary>
        /// Gets or sets the section seletion entities.
        /// </summary>
        public List<SectionSelectionEntity> SectionSelectionEntities
        {
            get
            {
                if (m_SectionSelectionEntities == null)
                {
                    m_SectionSelectionEntities = new List<SectionSelectionEntity>();
                }
                return m_SectionSelectionEntities;
            }
            set
            {
                m_SectionSelectionEntities = value;
            }
        }
        #endregion Properties

        #region Internal functions
        /// <summary>
        /// Adds the section selection entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        internal void AddSectionSelectionEntity(SectionSelectionEntity entity)
        {
            m_SectionSelectionEntities.Add(entity);
        }
        #endregion Internal functions
    }
}
