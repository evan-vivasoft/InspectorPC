using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.Model.InspectionProcedure
{
    public class ScriptCommandWithDescriptions : ScriptCommandBase
    {
        /// <summary>
        /// Gets or sets the measure point description.
        /// </summary>
        /// <value>
        /// The measure point description.
        /// </value>
        public string MeasurePointDescription { get; set; }

        /// <summary>
        /// Gets or sets the object name description.
        /// </summary>
        /// <value>
        /// The object name description.
        /// </value>
        public string ObjectNameDescription { get; set; }


        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>
        /// The measure point.
        /// </value>
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>
        /// The field no.
        /// </value>
        public int? FieldNo { get; set; }
    }
}
