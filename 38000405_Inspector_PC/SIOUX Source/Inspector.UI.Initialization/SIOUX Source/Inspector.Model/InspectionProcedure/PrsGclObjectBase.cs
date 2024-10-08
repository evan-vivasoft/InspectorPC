﻿/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionProcedure
{

    /// <summary>
    /// The base class with shared properties between the PrsObject and GclObject class.
    /// </summary>
    public class PrsGclObjectBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        /// <value>
        /// The object id.
        /// </value>
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>
        /// The measure point.
        /// </value>
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the measure point id.
        /// </summary>
        /// <value>
        /// The measure point id.
        /// </value>
        public string MeasurePointId { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>
        /// The field no.
        /// </value>
        public int? FieldNo { get; set; }
        #endregion Properties
    }
}
