/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Model;
using Inspector.Model.InspectionMeasurement;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Test
{
    [TestFixture]
    public class ContinuousMeasurementWorkerTest
    {
        private List<ScriptCommand5XMeasurement> m_CalculatedMeasurements;

        [Test]
        public void MeasurementCalculationsTest()
        {
            m_CalculatedMeasurements = new List<ScriptCommand5XMeasurement>();

            using (var worker = new ContinuousMeasurementWorker())
            {
                worker.MeasurementValuesReceived += new EventHandler<MeasurementValuesEventArgs>(worker_MeasurementValuesReceived);
                worker.MeasurementFrequency = 10;
                worker.MeasurementUnit = "bar";
                worker.LowHighPressureFactor = 0.001;
                worker.QVSFactor = 1;
                worker.MbarMinToLeakageFactor = 1;
                worker.VolumeVa = 150;
                worker.VolumeVak = 150;
                worker.Resolution = 4;
                worker.MeasuredLeakageToMbarMinFactor = 1;

                var workerThread = new Thread(() => worker.WorkerThread())
                {
                    Name = "ContinuousMeasurementWorkerTestThread"
                };

                workerThread.Start();

                // send 10 measurements in two sections
                worker.Enqueue = new List<Measurement> { new Measurement(-1.0,0), new Measurement(-0.0012,0), new Measurement(0.25,0) };
                worker.Enqueue = new List<Measurement> { new Measurement(1.0,0), new Measurement(1.0,0), new Measurement(1.0,0), new Measurement(1.0,0), new Measurement(1.0,0), new Measurement(1.0,0), new Measurement(1.0,0) };

                worker.FinishMeasurements(); // should block until everything is finished

                ValidateCalculatedMeasurements();
            }
        }

        [Test]
        public void MeasurementCalculationDifferentFactor()
        {
            //TODO: Add unit tests for calculations
            m_CalculatedMeasurements = new List<ScriptCommand5XMeasurement>();

            using (var worker = new ContinuousMeasurementWorker())
            {
                worker.MeasurementValuesReceived += worker_MeasurementValuesReceived;
                worker.MeasurementFrequency = 10;
                worker.MeasurementUnit = "bar";
                worker.LowHighPressureFactor = 1;
                worker.QVSFactor = 2;
                worker.MeasuredLeakageToMbarMinFactor = 1;
                worker.MbarMinToLeakageFactor = 3;
                worker.VolumeVa = 150;
                worker.VolumeVak = 150;
                worker.Resolution = 4;

                var workerThread = new Thread(() => worker.WorkerThread())
                {
                    Name = "ContinuousMeasurementWorkerTestThread"
                };

                workerThread.Start();

                // send 10 measurements in two sections
                worker.Enqueue = new List<Measurement> { new Measurement(-1.0, 0), new Measurement(-0.0012, 0), new Measurement(0.25, 0) };
                worker.Enqueue = new List<Measurement> { new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0) };

                worker.FinishMeasurements(); // should block until everything is finished

                ValidateCalculatedMeasurementsDifferentFactors();
            }
        }

        [Test]
        public void MeasurementCalculationsNoVolumeVaAndVolumeVak()
        {
            m_CalculatedMeasurements = new List<ScriptCommand5XMeasurement>();

            using (ContinuousMeasurementWorker worker = new ContinuousMeasurementWorker())
            {
                worker.MeasurementValuesReceived += new EventHandler<MeasurementValuesEventArgs>(worker_MeasurementValuesReceived);
                worker.MeasurementFrequency = 10;
                worker.MeasurementUnit = "bar";
                worker.LowHighPressureFactor = 0.001;
                worker.QVSFactor = 1;
                worker.MbarMinToLeakageFactor = 1;
                worker.VolumeVa = double.NaN;
                worker.VolumeVak = double.NaN;
                worker.Resolution = 4;
                worker.MeasuredLeakageToMbarMinFactor = 1;

                var workerThread = new Thread(() => worker.WorkerThread())
                {
                    Name = "ContinuousMeasurementWorkerTestThread"
                };

                workerThread.Start();

                // send 10 measurements in two sections
                worker.Enqueue = new List<Measurement> { new Measurement(-1.0, 0), new Measurement(-0.0012, 0), new Measurement(0.25, 0) };
                worker.Enqueue = new List<Measurement> { new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0), new Measurement(1.0, 0) };

                worker.FinishMeasurements(); // should block until everything is finished

                ValidateCalculatedMeasurementsWithoutVolumeVaAndVolumeVak();
            }
        }

        private void ValidateCalculatedMeasurementsWithoutVolumeVaAndVolumeVak()
        {
            // A total of 10 measurements should be available
            Assert.AreEqual(10, m_CalculatedMeasurements.Count);


            for (var i = 0; i < m_CalculatedMeasurements.Count; i++)
            {
                Assert.AreEqual(i, m_CalculatedMeasurements[i].SequenceNumber, "Calculated measurements not correctly ordered.");
            }

            // Check first three results on actual values
            var measurement = m_CalculatedMeasurements[0];
            Assert.AreEqual(-1, measurement.Average);
            Assert.AreEqual(0, measurement.LeakageValue);
            Assert.AreEqual(0, measurement.LeakageMembrane);
            Assert.AreEqual(double.NaN, measurement.LeakageV1);
            Assert.AreEqual(double.NaN, measurement.LeakageV2);
            Assert.AreEqual(-1.0, measurement.Maximum);
            Assert.AreEqual(-1.0, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);

            measurement = m_CalculatedMeasurements[1];
            Assert.AreEqual(-0.5006, measurement.Average);
            Assert.AreEqual(299640.0, measurement.LeakageValue);
            Assert.AreEqual(29964.0, measurement.LeakageMembrane);
            Assert.AreEqual(double.NaN, measurement.LeakageV1);
            Assert.AreEqual(double.NaN, measurement.LeakageV2);
            Assert.AreEqual(-0.0012, measurement.Maximum);
            Assert.AreEqual(-0.0012, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);

            measurement = m_CalculatedMeasurements[2];
            Assert.AreEqual(-0.2504, measurement.Average);
            Assert.AreEqual(250000.0, measurement.LeakageValue);
            Assert.AreEqual(25000.0, measurement.LeakageMembrane);
            Assert.AreEqual(double.NaN, measurement.LeakageV1);
            Assert.AreEqual(double.NaN, measurement.LeakageV2);
            Assert.AreEqual(0.25, measurement.Maximum);
            Assert.AreEqual(0.25, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);
        }

        /// <summary>
        /// Validates the calculated measurements.
        /// </summary>
        private void ValidateCalculatedMeasurements()
        {
            // A total of 10 measurements should be available
            Assert.AreEqual(10, m_CalculatedMeasurements.Count);


            for (int i = 0; i < m_CalculatedMeasurements.Count; i++)
            {
                Assert.AreEqual(i, m_CalculatedMeasurements[i].SequenceNumber, "Calculated measurements not correctly ordered.");
            }

            // Check first three results on actual values
            var measurement = m_CalculatedMeasurements[0];
            Assert.AreEqual(-1, measurement.Average);
            Assert.AreEqual(0, measurement.LeakageValue);
            Assert.AreEqual(0, measurement.LeakageMembrane);
            Assert.AreEqual(0, measurement.LeakageV1);
            Assert.AreEqual(0, measurement.LeakageV2);
            Assert.AreEqual(-1.0, measurement.Maximum);
            Assert.AreEqual(-1.0, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);


            measurement = m_CalculatedMeasurements[1];
            Assert.AreEqual(-0.5006, measurement.Average);
            Assert.AreEqual(299640.0, measurement.LeakageValue);
            Assert.AreEqual(29964.0, measurement.LeakageMembrane);
            Assert.AreEqual(2696760.0, measurement.LeakageV1);
            Assert.AreEqual(2696760.0, measurement.LeakageV2);
            Assert.AreEqual(-0.0012, measurement.Maximum);
            Assert.AreEqual(-0.0012, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);

            measurement = m_CalculatedMeasurements[2];
            Assert.AreEqual(-0.2504, measurement.Average);
            Assert.AreEqual(250000.0, measurement.LeakageValue);
            Assert.AreEqual(25000.0, measurement.LeakageMembrane);
            Assert.AreEqual(2250000.0, measurement.LeakageV1);
            Assert.AreEqual(2250000.0, measurement.LeakageV2);
            Assert.AreEqual(0.25, measurement.Maximum);
            Assert.AreEqual(0.25, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);
        }

        /// <summary>
        /// Validates the calculated measurements different factors.
        /// </summary>
        private void ValidateCalculatedMeasurementsDifferentFactors()
        {
            // A total of 10 measurements should be available
            Assert.AreEqual(10, m_CalculatedMeasurements.Count);


            for (int i = 0; i < m_CalculatedMeasurements.Count; i++)
            {
                Assert.AreEqual(i, m_CalculatedMeasurements[i].SequenceNumber, "Calculated measurements not correctly ordered.");
            }

            // Check first three results on actual values
            var measurement = m_CalculatedMeasurements[0];
            Assert.AreEqual(-1, measurement.Average);
            Assert.AreEqual(0, measurement.LeakageValue);
            Assert.AreEqual(0, measurement.LeakageMembrane);
            Assert.AreEqual(0, measurement.LeakageV1);
            Assert.AreEqual(0, measurement.LeakageV2);
            Assert.AreEqual(-1.0, measurement.Maximum);
            Assert.AreEqual(-1.0, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);

            measurement = m_CalculatedMeasurements[1];
            Assert.AreEqual(-0.5006, measurement.Average);
            Assert.AreEqual(898.92, measurement.LeakageValue);
            Assert.AreEqual(59.928, measurement.LeakageMembrane);
            Assert.AreEqual(5393.52, measurement.LeakageV1);
            Assert.AreEqual(5393.52, measurement.LeakageV2);
            Assert.AreEqual(-0.0012, measurement.Maximum);
            Assert.AreEqual(-0.0012, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);

            measurement = m_CalculatedMeasurements[2];
            Assert.AreEqual(-0.2504, measurement.Average);
            Assert.AreEqual(750.0, measurement.LeakageValue);
            Assert.AreEqual(50.0, measurement.LeakageMembrane);
            Assert.AreEqual(4500.0, measurement.LeakageV1);
            Assert.AreEqual(4500.0, measurement.LeakageV2);
            Assert.AreEqual(0.25, measurement.Maximum);
            Assert.AreEqual(0.25, measurement.Measurement);
            Assert.AreEqual(-1.0, measurement.Minimum);
        }

        /// <summary>
        /// Handles the MeasurementValuesReceived event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.BusinessLogic.Interfaces.Events.MeasurementValuesEventArgs"/> instance containing the event data.</param>
        private void worker_MeasurementValuesReceived(object sender, MeasurementValuesEventArgs e)
        {
            Assert.IsNotNull(e);
            Assert.IsNotNull(e.MeasurementValues);
            Assert.Greater(e.MeasurementValues.Count, 0); // do not expect empty results
            m_CalculatedMeasurements.AddRange(e.MeasurementValues);
        }
    }
}
