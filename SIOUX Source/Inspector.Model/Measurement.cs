using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.Model
{
    public class Measurement
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the io status.
        /// </summary>
        /// <value>
        /// The io status.
        /// </value>
        public int IoStatus { get; set; }

        public DateTime Time { get; set; }
        public Measurement(double value, int iostatus)
        {
            Value = value;
            IoStatus = iostatus;
            Time = DateTime.Now;
        }
    }
}
