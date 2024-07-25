/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station
{
    /// <summary>
    /// GasControlLineEntity
    /// </summary>
    internal class GasControlLineEntity
    {
        public Guid Id { get; set; }
        public string PRSName { get; set; }
        public string PRSIdentification { get; set; }
        public string GasControlLineName { get; set; }
        public string PeMin { get; set; }
        public string PeMax { get; set; }
        public string VolumeVA { get; set; }
        public string VolumeVAK { get; set; }
        public TypeRangeDM PaRangeDM { get; set; }
        public TypeRangeDM PeRangeDM { get; set; }
        public string GCLIdentification { get; set; }
        public string GCLCode { get; set; }
        public string InspectionProcedure { get; set; }

        public Guid? InspectionProcedureId { get; set; }
        public int? StartPosition { get; set; }
        public int FSDStart { get; set; }
        public List<GCLObject> GCLObjects;

        public GasControlLineEntity()
        {
            this.GCLObjects = new List<GCLObject>();
        }
    }

    /// <summary>
    /// TypeObjectID
    /// </summary>
    internal class GCLObject
    {
        public string ObjectName { get; set; }
        public string ObjectID { get; set; }
        public string MeasurePoint { get; set; }
        public string MeasurePointID { get; set; }
        public int? FieldNo { get; set; }

        public Guid InspectionPointId { get; set; }
        public TypeObjectIDBoundaries Boundaries { get; set; }

        public GCLObject()
        {
        }
    }

    /// <summary>
    /// TypeObjectIDBoundaries
    /// </summary>
    internal class TypeObjectIDBoundaries
    {
        public double ValueMax { get; set; }
        public double ValueMin { get; set; }
        public UnitOfMeasurement UOV { get; set; }
        public Guid? ScriptCommandId { get; set; }
        public double? Offset { get; set; }
    }
}
