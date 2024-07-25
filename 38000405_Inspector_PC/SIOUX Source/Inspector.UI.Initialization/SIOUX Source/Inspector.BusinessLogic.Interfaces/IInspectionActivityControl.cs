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
    /// Interface for an Inspection
    /// </summary>
    public interface IInspectionActivityControl : IActivityControl
    {
        /// <summary>
        /// Executes the inspection.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        bool ExecuteInspection(string prsName, string gclName);

        /// <summary>
        /// Executes the inspection.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>
        /// True if the inspection is succesfully started, false if another inspection is already running which is not yet completed
        /// </returns>
        bool ExecuteInspection(string inspectionProcedureName, string prsName, string gclName);

        /// <summary>
        /// Executes the partial inspection.
        /// </summary>
        /// <param name="sectionSelection">The section selection.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        bool ExecutePartialInspection(SectionSelection sectionSelection, string prsName, string gclName);

        /// <summary>
        /// Stores the remark.
        /// </summary>
        /// <param name="remarkStepResult">The remark step result.</param>
        void StoreRemark(InspectionStepResultText remarkStepResult);

        /// <summary>
        /// Called when an inspection step is complete.
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
        /// Occurs when [inspection finished].
        /// </summary>
        event EventHandler InspectionFinished;

        /// <summary>
        /// Occurs when [execute step].
        /// </summary>
        event EventHandler ExecuteInspectionStep;

        /// <summary>
        /// Occurs when [inspection error].
        /// </summary>
        event EventHandler InspectionError;

        /// <summary>
        /// Occurs when [measurements received].
        /// </summary>
        event EventHandler MeasurementsReceived;

        /// <summary>
        /// Occurs when [measurements completed].
        /// </summary>
        event EventHandler MeasurementsCompleted;

        /// <summary>
        /// Occurs when [extra measurement started].
        /// </summary>
        event EventHandler ExtraMeasurementStarted;

        /// <summary>
        /// Occurs when [measurement result].
        /// </summary>
        event EventHandler MeasurementResult;

        /// <summary>
        /// Occurs when [continuous measurement started].
        /// </summary>
        event EventHandler ContinuousMeasurementStarted;
    }
}
