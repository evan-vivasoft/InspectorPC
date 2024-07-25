using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.Report.Model
{
    public class Boundary
    {
        /// <summary>
        /// Gets or sets the value max.
        /// </summary>
        /// <value>
        /// The value max.
        /// </value>
        public double ValueMax { get; set; }

        /// <summary>
        /// Gets or sets the value min.
        /// </summary>
        /// <value>
        /// The value min.
        /// </value>
        public double ValueMin { get; set; }

        /// <summary>
        /// Gets or sets the UOV.
        /// </summary>
        /// <value>
        /// The UOV.
        /// </value>
        public string UOV { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Boundary"/> class.
        /// </summary>
        public Boundary()
        {
            UOV = "-";
            ValueMax = 0.0;
            ValueMin = 0.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Boundary"/> class.
        /// </summary>
        /// <param name="max">The max.</param>
        /// <param name="min">The min.</param>
        /// <param name="uov">The uov.</param>
        public Boundary(double max, double min, string uov)
        {
            UOV = uov;
            ValueMax = max;
            ValueMin = min;
        }
    }
}
