using AutoMapper;
using Inspector.BusinessLogic.Data.Reporting.Measurements.Model;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;


namespace Inspector.BusinessLogic.Data.Reporting.Results.Automapper
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Necessary for mapping configuration")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary for mapping configuration")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505")]
    public class MappingProfile : Profile
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Necessary for mapping configuration")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary for mapping configuration")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505")]
        public MappingProfile()
        {
            // Map DateTimeStamp
            CreateMap<DateTimeStamp, POService.InspectionResults.Model.DateTimeStamp>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.TimeSettings, opt => opt.MapFrom(src => src.TimeSettings));

            CreateMap<TimeSetting, POService.InspectionResults.Model.TimeSetting>()
                .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
                .ForMember(dest => dest.DST, opt => opt.MapFrom(src => src.DST));

            // Mapping for Result class
            CreateMap<Result, POService.InspectionResults.Model.Result>()
                .ForMember(dest => dest.ObjectName, opt => opt.MapFrom(src => src.ObjectName))
                .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.ObjectID))
                .ForMember(dest => dest.MeasurePoint, opt => opt.MapFrom(src => src.MeasurePoint))
                .ForMember(dest => dest.MeasurePointID, opt => opt.MapFrom(src => src.MeasurePointID))
                .ForMember(dest => dest.FieldNo, opt => opt.MapFrom(src => src.FieldNo))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MeasureValue, opt => opt.MapFrom(src => src.MeasureValue))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.List, opt => opt.MapFrom(src => src.List))
                .ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(src => src.SequenceNumber))
                .ForMember(dest => dest.ScriptCommandId, opt => opt.MapFrom(src => src.ScriptCommandId))
                .ForMember(dest => dest.MaximumValue, opt => opt.MapFrom(src => src.MaximumValue))
                .ForMember(dest => dest.MinimumValue, opt => opt.MapFrom(src => src.MinimumValue))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.Uom))
                .ForMember(dest => dest.Offset, opt => opt.MapFrom(src => src.Offset))
                .ForMember(dest => dest.ObjectNameDescription, opt => opt.Ignore()) // Ignoring since it's marked as [XmlIgnore]
                .ForMember(dest => dest.MeasurePointDescription, opt => opt.Ignore()) // Ignoring since it's marked as [XmlIgnore]
                .ForCtorParam("sequenceNumber", opt => opt.MapFrom(src => src.SequenceNumber))
                .ForCtorParam("objectName", opt => opt.MapFrom(src => src.ObjectName))
                .ForCtorParam("objectId", opt => opt.MapFrom(src => src.ObjectID))
                .ForCtorParam("measurePoint", opt => opt.MapFrom(src => src.MeasurePoint))
                .ForCtorParam("measurePointId", opt => opt.MapFrom(src => src.MeasurePointID))
                .ForCtorParam("fieldNo", opt => opt.MapFrom(src => src.FieldNo))
                .ForCtorParam("time", opt => opt.MapFrom(src => TimeSpan.Parse(src.Time))) // Assuming Time is in a format that can be parsed to TimeSpan
                .ForCtorParam("measureValue", opt => opt.MapFrom(src => src.MeasureValue))
                .ForCtorParam("text", opt => opt.MapFrom(src => src.Text))
                .ForCtorParam("list", opt => opt.MapFrom(src => src.List))
                .ForCtorParam("objectNameDescription", opt => opt.MapFrom(src => src.ObjectNameDescription))
                .ForCtorParam("scId", opt => opt.MapFrom(src => src.ScriptCommandId))
                .ForCtorParam("maxValue", opt => opt.MapFrom(src => src.MaximumValue))
                .ForCtorParam("minValue", opt => opt.MapFrom(src => src.MinimumValue))
                .ForCtorParam("offset", opt => opt.MapFrom(src => src.Offset))
                .ForCtorParam("uom", opt => opt.MapFrom(src => src.Uom))
                .ForCtorParam("measurePointDescription", opt => opt.MapFrom(src => src.MeasurePointDescription));

            // Map MeasurementEquipment
            CreateMap<MeasurementEquipment, POService.InspectionResults.Model.MeasurementEquipment>()
                .ForMember(dest => dest.ID_DM1, opt => opt.MapFrom(src => src.ID_DM1))
                .ForMember(dest => dest.ID_DM2, opt => opt.MapFrom(src => src.ID_DM2))
                .ForMember(dest => dest.BT_Address, opt => opt.MapFrom(src => src.BT_Address));

            // Map InspectionProcedure
            CreateMap<InspectionProcedure, POService.InspectionResults.Model.InspectionProcedure>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version));

            // Map InspectionResult with explicit constructor parameters
            CreateMap<InspectionResult, POService.InspectionResults.Model.InspectionResult>()
                .ForCtorParam("status", opt => opt.MapFrom(src => src.Status))
                .ForCtorParam("prsIdentification", opt => opt.MapFrom(src => src.PRSIdentification))
                .ForCtorParam("prsName", opt => opt.MapFrom(src => src.PRSName))
                .ForCtorParam("prsCode", opt => opt.MapFrom(src => src.PRSCode))
                .ForCtorParam("gasControlLineName", opt => opt.MapFrom(src => src.GasControlLineName))
                .ForCtorParam("gclIdentification", opt => opt.MapFrom(src => src.GCLIdentification))
                .ForCtorParam("gclCode", opt => opt.MapFrom(src => src.GCLCode))
                .ForCtorParam("crc", opt => opt.MapFrom(src => src.CRC))
                .ForCtorParam("measurementEquipment", opt => opt.MapFrom(src => src.Measurement_Equipment))
                .ForCtorParam("inspectionProcedure", opt => opt.MapFrom(src => src.InspectionProcedure))
                .ForCtorParam("dateTimestamp", opt => opt.MapFrom(src => src.DateTimeStamp))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.InspectionProcedureId, opt => opt.MapFrom(src => src.InspectionProcedureId))
                .ForMember(dest => dest.StartPosition, opt => opt.MapFrom(src => src.StartPosition))
                .ForMember(dest => dest.StatusAsText, opt => opt.MapFrom(src => src.StatusAsText))
                .ForMember(dest => dest.PRSIdentification, opt => opt.MapFrom(src => src.PRSIdentification))
                .ForMember(dest => dest.PRSName, opt => opt.MapFrom(src => src.PRSName))
                .ForMember(dest => dest.PRSId, opt => opt.MapFrom(src => src.PRSId))
                .ForMember(dest => dest.PRSCode, opt => opt.MapFrom(src => src.PRSCode))
                .ForMember(dest => dest.GasControlLineName, opt => opt.MapFrom(src => src.GasControlLineName))
                .ForMember(dest => dest.GCLId, opt => opt.MapFrom(src => src.GCLId))
                .ForMember(dest => dest.GCLIdentification, opt => opt.MapFrom(src => src.GCLIdentification))
                .ForMember(dest => dest.GCLCode, opt => opt.MapFrom(src => src.GCLCode))
                .ForMember(dest => dest.CRC, opt => opt.MapFrom(src => src.CRC))
                .ForMember(dest => dest.Measurement_Equipment, opt => opt.MapFrom(src => src.Measurement_Equipment))
                .ForMember(dest => dest.InspectionProcedure, opt => opt.MapFrom(src => src.InspectionProcedure))
                .ForMember(dest => dest.DateTimeStamp, opt => opt.MapFrom(src => src.DateTimeStamp))
                .ForMember(dest => dest.Results, opt => opt.MapFrom(
                                        src => src.Results.Select( a =>
                                            new Inspector.POService.InspectionResults.Model.Result {
                                                ObjectName = a.ObjectName,
                                                ObjectID = a.ObjectID,
                                                MeasurePoint = a.MeasurePoint,
                                                MeasurePointID = a.MeasurePointID,
                                                Time = a.Time,
                                                SequenceNumber = a.SequenceNumber,
                                                MeasureValue = ConvertMeasureValue(a.MeasureValue),
                                                List = a.List,
                                                Text = a.Text,
                                                Uom = (Inspector.POService.InspectionResults.Model.UnitOfMeasurement)((int) a.Uom),
                                                MaximumValue = a.MaximumValue,
                                                MinimumValue = a.MinimumValue,
                                                Offset = a.Offset,
                                                ScriptCommandId = a.ScriptCommandId,
                                                LinkId = a.LinkId ?? Guid.Empty
                                            }
                                        )
                                      ));

            CreateMap<UnitOfMeasurement, Inspector.POService.InspectionResults.Model.UnitOfMeasurement>()
            .ConvertUsing(src => EnumConverter.Convert(src));

            CreateMap<MeasurementReportMeasuredEntity, Inspector.POService.InspectionResults.Model.Data>();

            CreateMap<MeasurementReport, POService.InspectionResults.Model.MeasurementReport>()
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom(src => src.Measurements.Select(a =>
                    new Inspector.POService.InspectionResults.Model.Measurement
                    {
                        Data = ConvertMeasurementData(a.Data),
                        LinkId = a.LinkId,
                        SampleRate = a.SampleRate,
                        Interval = a.Interval
                    }
                ).ToList()));
            
        }

        static Inspector.POService.InspectionResults.Model.MeasureValue ConvertMeasureValue(MeasureValue measureValue)
        {
            return new Inspector.POService.InspectionResults.Model.MeasureValue
            {
                Value = measureValue.Value,
                UOM = EnumConverter.Convert(measureValue.UOM)
            };
        }

        static Inspector.POService.InspectionResults.Model.Data ConvertMeasurementData (MeasurementReportMeasuredEntity data)
        {
            return new Inspector.POService.InspectionResults.Model.Data
            {
                MeasurementValues = ConvertMeasurementValues(data.MeasurementValues),
                ExtraMeasurementValues = ConvertMeasurementValues(data.ExtraMeasurementValues),
                Unit = data.Unit,
                ReportIoStatus = data.ReportIoStatus,
                StartTime = data.StartTime,
                MaxValueTimeStamp = data.MaxValueTimeStamp,
                EndTime = data.EndTime,
            };
        }

        static List<Inspector.POService.InspectionResults.Model.MeasurementValue> ConvertMeasurementValues (List<Inspector.Model.Measurement> measurement)
        {
            return (
                measurement
                    .Select(m =>
                    {
                        return new Inspector.POService.InspectionResults.Model.MeasurementValue
                        {
                            Value = m.Value,
                            IoStatus = m.IoStatus,
                            Time = m.Time
                        };
                    })
                    .ToList()
            );
        }
    }

    public static class EnumConverter
    {
        public static Inspector.POService.InspectionResults.Model.UnitOfMeasurement Convert(UnitOfMeasurement source)
        {
            return (Inspector.POService.InspectionResults.Model.UnitOfMeasurement)((int)source);
        }
    }
}
