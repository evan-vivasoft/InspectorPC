/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface for a separate Initialization
    /// </summary>
    public interface IInitializationActivityControl : IActivityControl
    {
        /// <summary>
        /// Executes the activity.
        /// </summary>
        void ExecuteInitialization();
    }
}
