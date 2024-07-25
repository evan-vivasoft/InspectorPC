/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station
{
    /// <summary>
    /// PRSEntitiy
    /// </summary>
    internal class PRSEntity
    {
        public string Route { get; set; }
        public string PRSCode { get; set; }
        public string PRSName { get; set; }
        public string PRSIdentification { get; set; }
        public string Information { get; set; }
        public string InspectionProcedure { get; set; }

        public List<PRSObject> PRSObjects { get; set; }
        public List<GasControlLineEntity> GasControlLines { get; set; }

        public PRSEntity()
        {
            this.PRSObjects = new List<PRSObject>();
            this.GasControlLines = new List<GasControlLineEntity>();
        }
    }

    /// <summary>
    /// PRSPRSObjects
    /// </summary>
    internal class PRSObject
    {
        public string ObjectName { get; set; }
        public string ObjectID { get; set; }
        public string MeasurePoint { get; set; }
        public string MeasurePointID { get; set; }
        public int? FieldNo { get; set; }
    }
}