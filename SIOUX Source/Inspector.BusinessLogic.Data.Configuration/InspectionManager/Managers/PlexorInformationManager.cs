/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Model.Plexor;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers
{
    /// <summary>
    /// PlexorInformationManager, manages the plexor's information
    /// </summary>
    public class PlexorInformationManager : IPlexorInformationManager
    {
        #region Members
        private List<PlexorInformation> m_PlexorsInformation;
        private ReadOnlyCollection<PlexorInformation> m_ReadOnlyPlexorsInformation;
        #endregion Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlexorInformationManager"/> class.
        /// </summary>
        public PlexorInformationManager()
        {
            m_PlexorsInformation = new List<PlexorInformation>();
            LoadPlexorsInformation();
        }
        #endregion Constructors

        #region IPlexorInformationManager Members
        /// <summary>
        /// Gets the plexors information.
        /// </summary>
        /// <value>The plexors information.</value>
        public ReadOnlyCollection<PlexorInformation> PlexorsInformation
        {
            get
            {
                if (m_ReadOnlyPlexorsInformation == null)
                {
                    m_ReadOnlyPlexorsInformation = new ReadOnlyCollection<PlexorInformation>(m_PlexorsInformation);
                }
                return m_ReadOnlyPlexorsInformation;
            }
        }

        /// <summary>
        /// Refresh the data by reloading the storage backend.
        /// </summary>
        public async Task Refresh()
        {
            m_PlexorsInformation.Clear(); // remove any previous results first
            await Task.Delay(1);
            LoadPlexorsInformation();
        }
        #endregion

        #region Private
        /// <summary>
        /// Loads the plexors information.
        /// </summary>
        private void LoadPlexorsInformation()
        {
            PlexorInformationLoader plexorInfoLoader = new PlexorInformationLoader();

            foreach (PlexorEntity plexorEntity in plexorInfoLoader.Plexors)
            {
                PlexorInformation plexorInfo = new PlexorInformation(
                    plexorEntity.Name,
                    plexorEntity.SerialNumber,
                    plexorEntity.BlueToothAddress,
                    plexorEntity.PN,
                    plexorEntity.CalibrationDate);
                m_PlexorsInformation.Add(plexorInfo);
            }

            plexorInfoLoader = null;
        }
        #endregion Private
    }
}
