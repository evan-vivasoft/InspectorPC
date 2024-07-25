/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// GclObject
    /// </summary>
    public class GclObject : PrsGclObjectBase
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
        /// <param name="gclBoundary">The GCL boundary.</param>
        internal GclObject(string objectName, string objectId, string measurePoint, string measurePointId, int? fieldNo, GclBoundaries gclBoundary)
        {
            ObjectName = objectName;
            ObjectId = objectId;
            MeasurePoint = measurePoint;
            MeasurePointId = measurePointId;
            FieldNo = fieldNo;
            GclBoundary = gclBoundary;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the GCL boundary.
        /// </summary>
        /// <value>
        /// The GCL boundary.
        /// </value>
        public GclBoundaries GclBoundary { get; set; }
        #endregion Properties
    }
}
