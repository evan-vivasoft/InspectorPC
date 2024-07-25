/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionStepResult;

namespace Inspector.BusinessLogic.Interfaces
{
    /// <summary>
    /// Specific interface for an inspection activity
    /// </summary>
    public interface IInspectionActivityControl : IActivityControl
    {
        /// <summary>
        /// Execute an inspection based on the prsName and the GclName.
        /// Inspections are search in the InspectionProcedure xml file.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        bool ExecuteInspection(string prsName, string gclName);

        /// <summary>
        /// Execute an inspection based on the inspectionProcedureName, prsName and GclName.
        /// Inspections are search in the InspectionProcedure xml file.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure to execute.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>
        /// True if the inspection is succesfully started, false if another inspection is already running which is not yet completed
        /// </returns>
        bool ExecuteInspection(string inspectionProcedureName, string prsName, string gclName);

        /// <summary>
        /// Executes a partial inspection, based on the selected sections to be executed, the prsName and gclName 
        /// Inspections are search in the InspectionProcedure xml file.
        /// </summary>
        /// <param name="sectionSelection">The section selection.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        bool ExecutePartialInspection(SectionSelection sectionSelection, string prsName, string gclName);

        /// <summary>
        /// Stores a remark in the InspectionReport.
        /// </summary>
        /// <param name="remarkStepResult">The remark step result.</param>
        void StoreRemark(InspectionStepResultText remarkStepResult);

        /// <summary>
        /// Called by the UI when an inspection steo is complete.
        /// This is only used for manual inspection steps
        /// </summary>
        /// <param name="inspectionStepResult">The inspection step result.</param>
        void InspectionStepComplete(InspectionStepResultBase inspectionStepResult);

        /// <summary>
        /// Retry the last failed action as notified by the InspectionError event.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when a retry is attempted when the inspection is not in an error situation.</exception>
        void Retry();

        /// <summary>
        /// Starts the continuous measurements of the current inspection procedure step.
        /// </summary>
        void StartContinuousMeasurement();

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        void StopContinuousMeasurement();

        /// <summary>
        /// Occurs when the inspection is finished.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.InspectionFinishedEventArgs
        /// </returns>
        event EventHandler InspectionFinished;

        /// <summary>
        /// Occurs when [execute step].
        /// </summary>
        event EventHandler ExecuteInspectionStep;

        /// <summary>
        /// Occurs when there was an error while executing the inspection step.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.InspectionErrorEventArgs
        /// </returns>
        event EventHandler InspectionError;

        /// <summary>
        /// Occurs when there are measurements received.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.MeasurementEventArgs
        /// </returns>
        event EventHandler MeasurementsReceived;

        /// <summary>
        /// Occurs when the continuous measurement is completed.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.ExecuteInspectionStepEventArgs
        /// </returns>
        event EventHandler MeasurementsCompleted;

        /// <summary>
        /// Occurs when an extra measurement period is started.
        /// </summary>
        /// <returns>
        /// Empty EventArgs
        /// </returns>
        event EventHandler ExtraMeasurementStarted;

        /// <summary>
        /// Occurs when a measurement result is available.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.MeasurementResultEventArgs
        /// </returns>
        event EventHandler MeasurementResult;

        /// <summary>
        /// Occurs when the continuous measurment is started.
        /// </summary>
        /// <returns>
        /// Empty EventArgs
        /// </returns>
        event EventHandler ContinuousMeasurementStarted;

        /// <summary>
        /// Occurs when it is detected that the safety has been triggered.
        /// </summary>
        /// <returns>
        /// Inspector.BusinessLogic.Interfaces.Events.SafetyValueTriggeredEventArgs
        /// </returns>
        event EventHandler SafetyValueTriggered;
    }
}
