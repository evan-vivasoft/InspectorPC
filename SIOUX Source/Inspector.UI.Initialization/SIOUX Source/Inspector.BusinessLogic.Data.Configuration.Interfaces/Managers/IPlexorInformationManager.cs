/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.ObjectModel;
using Inspector.Model.Plexor;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// PlexorInformationManager interface
    /// </summary>
    public interface IPlexorInformationManager : IInformationManager
    {
        /// <summary>
        /// Gets the plexors information.
        /// </summary>
        ReadOnlyCollection<PlexorInformation> PlexorsInformation { get; }
    }
}
