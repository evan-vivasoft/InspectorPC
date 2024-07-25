using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    public class SafetyValueTriggeredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the measurement value.
        /// </summary>
        /// <value>
        /// The measurement value.
        /// </value>
        public double MeasurementValue { get; set; }

        /// <summary>
        /// Gets or sets the io status.
        /// </summary>
        /// <value>
        /// The io status.
        /// </value>
        public int IoStatus { get; set; }

        public SafetyValueTriggeredEventArgs(double measurementValue, int ioStatus)
        {
            MeasurementValue = measurementValue;
            IoStatus = ioStatus;
        }
    }
}
