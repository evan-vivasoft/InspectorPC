/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Threading.Tasks;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// This is the base interface that is extended by the specific information manager.
    /// This contains all functions that are identical in all information managers.
    /// </summary>
    public interface IInformationManager
    {
        /// <summary>
        /// Refresh the data by reloading the storage backend.
        /// </summary>
        Task Refresh();
    }
}
