/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// GclBoundaries
    /// </summary>
    public class GclBoundaries
    {
        #region Class Members
        #endregion Class Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GclObject"/> class.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="measurePointId">The measure point id.</param>
        /// <param name="fieldNo">The field no.</param>
        internal GclBoundaries(double valueMax, double valueMin, string uov)
        {
            ValueMax = valueMax;
            ValueMin = valueMin;
            UOV = uov;
        }
        #endregion Constructors

        #region Properties
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
        #endregion Properties
    }
}
