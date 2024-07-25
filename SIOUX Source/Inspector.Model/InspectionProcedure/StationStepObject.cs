/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


using System;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// Contains the additional information for a step that used in the step and written to the result files
    /// (The XML file denotes these properties in PRSObject and GCLObject)
    /// </summary>
    public class StationStepObject
    {
        public int? FieldNo;
        public Guid InspectionPointId;
        public string ObjectName;
        public string ObjectID;
        public string MeasurePoint;
        public string MeasurePointID;
        public StationStepObjectBoundaries Boundaries;
    }

    /// <summary>
    /// StationStepObject Boundaries
    /// </summary>
    public class StationStepObjectBoundaries
    {
        public double ValueMax;
        public double ValueMin;
        public UnitOfMeasurement UOV;
        public double? Offset;
        public Guid? ScriptCommandId;
    }
}
