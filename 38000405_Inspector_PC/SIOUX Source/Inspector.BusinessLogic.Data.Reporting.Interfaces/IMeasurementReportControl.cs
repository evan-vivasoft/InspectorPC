/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Model;
using Inspector.Model.MeasurementResult;

namespace Inspector.BusinessLogic.Data.Reporting.Interfaces
{
    /// <summary>
    /// Interface of MeasurementReportControl
    /// </summary>
    public interface IMeasurementReportControl : IDisposable
    {
        /// <summary>
        /// Starts the measurement report.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="startDateTime">The start date time.</param>
        void InitializeMeasurementReport(string prsName, string gclName, DateTime startDateTime);

        /// <summary>
        /// Sets up measurement file when required.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp")]
        void SetUpMeasurementFileWhenRequired();

        /// <summary>
        /// Adds the inspection procedure meta data.
        /// </summary>
        /// <param name="measurementReportMetadata">The measurement report meta data.</param>
        /// <exception cref="MeasurementReportControlException"></exception>
        void AddInspectionProcedureMetadata(InspectionProcedureMetadata measurementReportMetadata);

        /// <summary>
        /// Measurementses the received.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        void MeasurementsReceived(IList<Measurement> measurements);

        /// <summary>
        /// Starts the measurement.
        /// </summary>
        /// <param name="measurementUnit">The measurement unit.</param>
        /// <param name="measurementStartDate">The measurement start date.</param>
        /// <param name="storeIOStatus">Report the IO status.</param>
        /// <exception cref="MeasurementReportControlException"></exception>
        void StartMeasurement(string measurementUnit, DateTime measurementStartDate, bool storeIOStatus);

        /// <summary>
        /// Starts the extra data measurement.
        /// </summary>
        /// <exception cref="MeasurementReportControlException">Thrown when extra data could not be added to the mesurements.</exception>
        void StartExtraDataMeasurement();

        /// <summary>
        /// Thread function that handles all the processing of measurements
        /// </summary>
        void MeasurementDataWorkerThread();

        /// <summary>
        /// Adds the measurement metadata.
        /// </summary>
        /// <param name="measurementMetadata">The measurement metadata.</param>
        /// <exception cref="MeasurementReportControlException">Thrown when measurement meta data could not be added</exception>
        void AddMeasurementMetadata(MeasurementMetadata measurementMetadata);

        /// <summary>
        /// Finishes the measurement report.
        /// </summary>
        void FinishMeasurementReport();


        /// <summary>
        /// Registers the maximum value.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="dateTime">The date time.</param>
        void RegisterMaximumValue(double value, DateTime dateTime);
    }
}
