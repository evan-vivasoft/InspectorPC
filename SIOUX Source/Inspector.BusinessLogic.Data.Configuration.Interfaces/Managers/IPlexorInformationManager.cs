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
    /// the PlexorInformationManager loads the 'plexor.xml' file. This interface provides a method to retrieve the information about the plexor(s) described in that file.
    /// </summary>
    public interface IPlexorInformationManager : IInformationManager
    {
        /// <summary>
        /// Gets the plexors information.
        /// </summary>
        /// <returns>
        /// A readonly collection of PlexorInformation objects
        /// </returns>
        ReadOnlyCollection<PlexorInformation> PlexorsInformation { get; }
    }
}
