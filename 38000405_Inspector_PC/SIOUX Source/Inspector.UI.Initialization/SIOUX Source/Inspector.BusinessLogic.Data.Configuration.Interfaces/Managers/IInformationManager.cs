/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// InformationManager shared interface over managers
    /// </summary>
    public interface IInformationManager
    {
        /// <summary>
        /// Refresh the data by reloading the storage backend.
        /// </summary>
        void Refresh();
    }
}
