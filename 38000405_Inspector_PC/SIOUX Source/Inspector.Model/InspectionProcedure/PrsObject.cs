﻿/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// PRSObject
    /// </summary>
    public class PrsObject : PrsGclObjectBase
    {
        #region Class Members
        #endregion Class Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrsObject"/> class.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="measurePointId">The measure point id.</param>
        /// <param name="fieldNo">The field no.</param>
        internal PrsObject(string objectName, string objectNameDescription, string objectId, string measurePoint, string measurePointDescription, string measurePointId, int? fieldNo)
        {
            ObjectName = objectName;
            ObjectNameDescription = objectNameDescription;
            ObjectId = objectId;
            MeasurePoint = measurePoint;
            MeasurePointDescription = measurePointDescription;
            MeasurePointId = measurePointId;
            FieldNo = fieldNo;
        }
        #endregion Constructors
    }
}
