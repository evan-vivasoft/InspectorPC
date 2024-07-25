using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Inspector.BusinessLogic;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Stub;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.Model.InspectionMeasurement;

using NUnit.Framework;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Inspector.Specs
{
    [Binding]
    public class CalculationsSteps
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class Setting
        {
            public int SequenceNumber { get; set; }
            public string UnitLowPressure { get; set; }
            public string UnitHighPressure { get; set; }
            public string FactorLowHighPressure { get; set; }
            public string UnitChangeRate { get; set; }
            public string UnitQVSLeakage { get; set; }
            public string FactorMbarMinToUnitChangeRate { get; set; }
            public string FactorMeasuredChangeRateToMbarMin { get; set; }
            public string FactorQVS { get; set; }
            public string VolumeVa { get; set; }
            public string VolumeVak { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class MeasurementValues
        {
            public int SequenceNumber { get; set; }
            public string Values { get; set; }
            public string Statuses { get; set; }
        }

        public class ScriptCommandResult
        {
            public int SequenceNumber { get; set; }
            public double Measurement { get; set; }
            public double Minimum { get; set; }
            public double Maximum { get; set; }
            public double Average { get; set; }
            public double LeakageValue { get; set; }
            public double LeakageV1 { get; set; }
            public double LeakageV2 { get; set; }
            public double LeakageMembrane { get; set; }
            public int IoStatus { get; set; }
        }

        private static List<Setting> m_Setting;
        private static List<MeasurementValues> m_MeasurementValues;
        private static List<ScriptCommandResult> m_ScriptCommandResults;

        private static string m_OriginalSettingsFileText;
        private static string m_OriginalStationInformationFileText;

        [Given(@"The InspectorSettings contains the following values:")]
        public void GivenTheInspectorSettingsContainsTheFollowingValues(Table table)
        {
            m_Setting = table.CreateSet<Setting>().ToList();
            m_OriginalSettingsFileText = File.ReadAllText("INSPECTORSettings - Original.xml");
            m_OriginalStationInformationFileText = File.ReadAllText(@"XML\StationInformation - Original.xml");
        }

        private static void SetSettingsFiles(Setting setting)
        {
            var settingsFileText = m_OriginalSettingsFileText;
            settingsFileText = settingsFileText.Replace("FLHPsetting", setting.FactorLowHighPressure);
            settingsFileText = settingsFileText.Replace("ULPsetting", setting.UnitLowPressure);
            settingsFileText = settingsFileText.Replace("UHPsetting", setting.UnitHighPressure);
            settingsFileText = settingsFileText.Replace("FMMTUCRsetting", setting.FactorMbarMinToUnitChangeRate);
            settingsFileText = settingsFileText.Replace("FMCRTMMsetting", setting.FactorMeasuredChangeRateToMbarMin);
            settingsFileText = settingsFileText.Replace("UCRsetting", setting.UnitChangeRate);
            settingsFileText = settingsFileText.Replace("UQVSLsetting", setting.UnitQVSLeakage);
            settingsFileText = settingsFileText.Replace("FQVSsetting", setting.FactorQVS);
            File.WriteAllText("INSPECTORSettings.xml", settingsFileText);

            var stationInformationText = m_OriginalStationInformationFileText;
            stationInformationText = stationInformationText.Replace("VVASetting", setting.VolumeVa);
            stationInformationText = stationInformationText.Replace("VVAKSetting", setting.VolumeVak);
            File.WriteAllText(@"XML\StationInformation.xml", stationInformationText);
        }

        [Given(@"And the following measurements are received")]
        public void GivenAndTheFollowingMeasurementsAreReceived(Table table)
        {
            m_MeasurementValues = table.CreateSet<MeasurementValues>().ToList();
        }

        private static IHal SetMeasurements(MeasurementValues measurementValues)
        {
            var stub = ContextRegistry.Context.Resolve<IHal>() as BluetoothHalStub;

            var values = measurementValues.Values.Split(';');
            var statuses = measurementValues.Statuses.Split(';');

            var measurements = values.Select((t, i) => new Measurement(double.Parse(t), int.Parse(statuses[i]))).ToList();

            stub?.AddMeasurements(measurements);

            return stub;
        }

        [When(@"I start a ScriptCommand5x")]
        public void WhenIStartAScriptCommand()
        {
            const string prsName = "Aachener Str MD"; //"5282720/AS 'T ANKER";//
            const string gclName = "Arbeitsschiene Schiene 1"; //"1044435 1";//

            for (var i = 0; i < m_Setting.Count; i++)
            {
                var setting = m_Setting[i];
                var result = m_ScriptCommandResults[i];
                var values = m_MeasurementValues[i];

                SetSettingsFiles(setting);

                using (var inspectionControl = ContextRegistry.Context.Resolve<IInspectionActivityControl>())
                {
                    var measurementsReceivedEvent = new ManualResetEvent(false);
                    var inspectionFinishedEvent = new ManualResetEvent(false);

                    measurementsReceivedEvent.Reset();
                    inspectionFinishedEvent.Reset();

                    inspectionControl.InspectionFinished += (o, eventArgs) =>
                    {
                        inspectionFinishedEvent.Set();
                    };

                    var aborted = false;

                    inspectionControl.MeasurementsReceived += (sender, args) =>
                    {
                        var iac = sender as InspectionActivityControl;
                        var mea = args as MeasurementEventArgs;

                        if (iac == null || mea == null) return;

                        if (aborted) return;

                        Console.WriteLine(mea.Measurements.First().IoStatus);

                        aborted = true;

                        var measurement = mea.Measurements.Last();

                        Task.Factory.StartNew(() => iac.Abort());

                        Verify(result, measurement);

                        measurementsReceivedEvent.Set();
                    };

                    var stub = SetMeasurements(values);

                    inspectionControl.ExecuteInspection(prsName, gclName);

                    measurementsReceivedEvent.WaitOne();

                    inspectionFinishedEvent.WaitOne();

                    inspectionControl.Dispose();
                    stub?.Dispose();

                    ContextRegistry.Context.Release();
                }
            }

            //use a switch statement to initialize the correct measurement (will now always be the same one.
        }

        [Then(@"The following measurementValues should be received")]
        public void ThenTheFollowingMeasurementValuesShouldBeReceived(Table table)
        {
            m_ScriptCommandResults = table.CreateSet<ScriptCommandResult>().ToList();
        }

        private static void Verify(ScriptCommandResult result, ScriptCommand5XMeasurement measurement)
        {
            Assert.AreEqual(result.Average, measurement.Average);
            Assert.AreEqual(result.IoStatus, measurement.IoStatus);
            Assert.AreEqual(result.LeakageMembrane, measurement.LeakageMembrane);
            Assert.AreEqual(result.LeakageV1, measurement.LeakageV1);
            Assert.AreEqual(result.LeakageV2, measurement.LeakageV2);
            Assert.AreEqual(result.LeakageValue, measurement.LeakageValue);
            Assert.AreEqual(result.Maximum, measurement.Maximum);
            Assert.AreEqual(result.Measurement, measurement.Measurement);
            Assert.AreEqual(result.Minimum, measurement.Minimum);
        }
    }
}
