Feature: Calculations
	In order to test the calculations
	As the end user
	I want to verify the calculations in the ContinuousMeasurementWorker

@VerifyCalculations
Scenario: Verify calculations
	Given The InspectorSettings contains the following values:
	| SequenceNumber | UnitLowPressure | UnitHighPressure | FactorLowHighPressure | UnitChangeRate | UnitQVSLeakage | FactorMbarMinToUnitChangeRate | FactorMeasuredChangeRateToMbarMin | FactorQVS | VolumeVa | VolumeVak |
	| 1              | mbar            | bar              | 0.001                 | mbar/min       | dm3/h          | 1                             | 1                                 | 1         | 50.5     | 75        |
	| 2              | mbar            | bar              | 0.001                 | mbar/min       | dm3/h          | 0.17                          | 2                                 | 3         | 50       | 60        |
	And And the following measurements are received
	| SequenceNumber | Values                                                                                                                                                                         | Statuses                                                               |
	| 1              | 30.5;30.7;30.9;31.1;31.3;31.5;31.7;31.9;32.1;32.3;32.5;32.7;32.9;33.1;33.3;33.5;33.7;33.9;34.1;33.7;33.3;32.9;32.5;32.1;31.7;31.3;30.9;30.5;30.1;29.7;29.3;28.9;28.5;28.1;27.7 | 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0; |
	| 2              | 30.5;30.7;30.9;31.1;31.3;31.5;31.7;31.9;32.1;32.3;32.5;32.7;32.9;33.1;33.3;33.5;33.7;33.9;34.1;33.7;33.3;32.9;32.5;32.1;31.7;31.3;30.9;30.5;30.1;29.7;29.3;28.9;28.5;28.1;27.7 | 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0; |
	Then The following measurementValues should be received
	| SequenceNumber | Measurement | Minimum | Maximum | Average | LeakageValue | LeakageV1 | LeakageV2 | LeakageMembrane | IoStatus |
	| 1              | 27.7        | 27.7    | 34.1    | 31.6    | -48          | -145.4    | -216      | -4.8            | 0        |
	| 2              | 27.7        | 27.7    | 34.1    | 31.6    | -16.3        | -864.0    | -1036.8   | -28.8           | 0        |
	When I start a ScriptCommand5x

	@VerifyRounding
Scenario: Verify rounding
	Given The InspectorSettings contains the following values:
	| SequenceNumber | UnitLowPressure | UnitHighPressure | FactorLowHighPressure | UnitChangeRate | UnitQVSLeakage | FactorMbarMinToUnitChangeRate | FactorMeasuredChangeRateToMbarMin | FactorQVS | VolumeVa | VolumeVak |
	| 1              | mbar            | bar              | 0.001                 | mbar/min       | dm3/h          | 1                             | 1                                 | 1         | 50.5     | 75        |
	| 2              | mbar            | bar              | 0.001                 | mbar/min       | dm3/h          | 0.17                          | 2                                 | 3         | 50       | 60        |
	And And the following measurements are received
	| SequenceNumber | Values                                                                                                                                                                                   | Statuses                                                               |
	| 1              | 30.5;30.7;30.9;31.1;31.3;31.5;31.7;31.9;32.1;32.3;32.5;32.7;32.9;33.1;33.3;33.5;33.7;33.9;34.100001;33.7;33.3;32.9;32.5;32.1;31.7;31.3;30.9;30.5;30.1;29.7;29.3;28.9;28.5;28.1;27.700001 | 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0; |
	| 2              | 30.5;30.7;30.9;31.1;31.3;31.5;31.7;31.9;32.1;32.3;32.5;32.7;32.9;33.1;33.3;33.5;33.7;33.9;34.100001;33.7;33.3;32.9;32.5;32.1;31.7;31.3;30.9;30.5;30.1;29.7;29.3;28.9;28.5;28.1;27.700001 | 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0; |
	Then The following measurementValues should be received
	| SequenceNumber | Measurement | Minimum   | Maximum   | Average | LeakageValue | LeakageV1 | LeakageV2 | LeakageMembrane | IoStatus |
	| 1              | 27.700001   | 27.700001 | 34.100001 | 31.6    | -48          | -145.4    | -216      | -4.8            | 0        |
	| 2              | 27.700001   | 27.700001 | 34.100001 | 31.6    | -16.3        | -864.0    | -1036.8   | -28.8           | 0        |
	When I start a ScriptCommand5x
