/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Data Model for Script Command 5X
    /// </summary>
    public class ScriptCommand5X : ScriptCommandBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the script command5 X.
        /// </summary>
        /// <value>The script command5 X.</value>
        public ScriptCommand5XType ScriptCommand { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        public string Instruction { get; set; }

        /// <summary>
        /// Gets or sets the diagital manometer.
        /// </summary>
        /// <value>The diagital manometer.</value>
        public DigitalManometer DigitalManometer { get; set; }

        /// <summary>
        /// Gets or sets the measurement frequency.
        /// </summary>
        /// <value>The measurement frequency.</value>
        public TypeMeasurementFrequency MeasurementFrequency { get; set; }

        /// <summary>
        /// Gets or sets the measurement period.
        /// </summary>
        /// <value>The measurement period.</value>
        public int MeasurementPeriod { get; set; }

        /// <summary>
        /// Gets or sets the extra measurement period.
        /// </summary>
        /// <value>The extra measurement period.</value>
        public int ExtraMeasurementPeriod { get; set; }

        /// <summary>
        /// Gets or sets the leakage.
        /// </summary>
        /// <value>The leakage.</value>
        public Leakage Leakage { get; set; }

        /// <summary>
        /// Gets or sets the station step object.
        /// </summary>
        /// <value>The station step object.</value>
        public StationStepObject StationStepObject { get; set; }
        #endregion Properties
    }
}
