/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.BusinessLogic.Interfaces.Events;

namespace Inspector.BusinessLogic.Interfaces
{
    /// <summary>
    /// This is the base interface that is extended by the IInspectionActivityControl and the IInitializationActivityControl.
    /// Since the device needs te be initialized both when performing an InitializationActivity and when performing an InspectionActivity, the events that result from initialization are defined here.
    /// </summary>
    public interface IActivityControl : IDisposable
    {
        /// <summary>
        /// Occurs when an initialization step is started.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.StartInitializationStepEventArgs
        /// </returns>
        event EventHandler InitializationStepStarted;

        /// <summary>
        /// Occurs when an initialization step is finished.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.FinishInitializationStepEventArgs
        /// </returns>
        event EventHandler InitializationStepFinished;

        /// <summary>
        /// Occurs when the entire initialization is finished.
        /// </summary>
        /// InitializationResult
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.FinishInitializationEventArgs
        /// </returns>
        event EventHandler InitializationFinished;

        /// <summary>
        /// Occurs when a device is unpaired.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.DeviceUnPairedEventArgs
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "UnPaired")]
        event EventHandler DeviceUnPaired;

        /// <summary>
        /// Occurs when the unpair action has finished
        /// </summary>
        event EventHandler DeviceUnPairFinished;

        /// <summary>
        /// Occurs when the Activity requires information from the user.
        /// </summary>
        event EventHandler<UiRequestEventArgs> UiRequest;

        /// <summary>
        /// Returns the response from the user to the ActivityControl 
        /// </summary>
        /// <param name="response"></param>
        void SetUiResponse(UiResponse response);

        /// <summary>
        /// Unpairs the device given by the address
        /// </summary>
        /// <param name="address"></param>
        void UnPairDevice(string address);

        /// <summary>
        /// Unpairs all bt devices that are paired, and have a device name of “PLEXOR BT IRDA” or “PLEXOR BT WIS”
        /// </summary>
        void UnPairAllDevices();

        /// <summary>
        /// aborts the initialization
        /// </summary>
        void Abort();
    }
}
