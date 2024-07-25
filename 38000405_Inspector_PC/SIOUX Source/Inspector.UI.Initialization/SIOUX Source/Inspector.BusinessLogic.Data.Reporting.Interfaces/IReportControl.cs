/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Reporting.Interfaces
{
    /// <summary>
    /// Interface of report control
    /// </summary>
    public interface IReportControl
    {
        /// <summary>
        /// Adds the temporary file to result.
        /// </summary>
        /// <exception cref="InspectorReportControlException">Thrown when the temporary file could not be added to the result.</exception>
        void AddTemporaryFileToResult();

        /// <summary>
        /// Starts the inspection report.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="startTime">The start time.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspector report control failed to start.</exception>
        void StartInspectionReport(InspectionStatus status, DateTime startTime);

        /// <summary>
        /// Adds the inspection procedure.
        /// </summary>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspection procedure could not be added.</exception>
        void AddInspectionProcedure(InspectionProcedureGenericInformation inspectionProcedure);

        /// <summary>
        /// Adds the manometer id1.
        /// </summary>
        /// <param name="meterNumber">The meter number.</param>
        /// <param name="manometerIdentification">The manometer identification.</param>     
        /// <exception cref="InspectorReportControlException">Thrown when manometer identification could not be added to the report file.</exception>
        void AddManometerIdentification(MeterNumber meterNumber, string manometerIdentification);

        /// <summary>
        /// Adds the bluetooth address.
        /// </summary>
        /// <param name="bluetoothAddress">The bluetooth address.</param>
        /// <exception cref="InspectorReportControlException">Thrown when bluetooth address could not be added to the report file.</exception>
        void AddBluetoothAddress(string bluetoothAddress);

        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the result could not be added to the result file.</exception>
        void AddResult(InspectionProcedureStepResult result);

        /// <summary>
        /// Updates the remark.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="remark">The remark.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the remark could not be updated.</exception>
        void UpdateRemark(long sequenceNumber, string remark);

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspection status could not be updated.</exception>
        void UpdateStatus(InspectionStatus status);

        /// <summary>
        /// Finishes the inspection report.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="endTime">The end time.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inpsection report could not be finished.</exception>
        void FinishInspectionReport(InspectionStatus status, DateTime endTime);
    }
}
