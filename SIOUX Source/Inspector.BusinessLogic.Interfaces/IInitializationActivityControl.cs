/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.BusinessLogic.Interfaces
{
    /// <summary>
    /// Specific interface for a separate Initialization activity
    /// </summary>
    public interface IInitializationActivityControl : IActivityControl
    {
        /// <summary>
        /// Executes an initialization activity.
        /// </summary>
        void ExecuteInitialization();
    }
}
