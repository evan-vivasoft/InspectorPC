using AutoMapper;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station;
using Inspector.POService;
using Inspector.POService.InspectionProcedure;
using Inspector.POService.StationInformation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexorEntity = Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor.PlexorEntity;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.AutMapper
{
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed: Complex mappings are necessary for business logic.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505", Justification = "Reviewed: Complex mappings are necessary for business logic.")]
    public class MappingProfile : Profile
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Necessary for mapping configuration")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary for mapping configuration")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505", Justification = "Reviewed: Complex mappings are necessary for business logic.")]
        public MappingProfile()
        {
            CreateMap<InspectionProcedureEntityJsonParserProject, Model.InspectionProcedure.InspectionProcedureEntity>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
               .ForMember(dest => dest.InspectionSequence, opt => opt.MapFrom(src => src.InspectionSequence));

            CreateMap<ScriptCommandEntityBase, Model.InspectionProcedure.ScriptCommandEntityBase>()
                .Include<ScriptCommand1Entity, Model.InspectionProcedure.ScriptCommand1Entity>()
                .Include<ScriptCommand2Entity, Model.InspectionProcedure.ScriptCommand2Entity>()
                .Include<ScriptCommand3Entity, Model.InspectionProcedure.ScriptCommand3Entity>()
                .Include<ScriptCommand4Entity, Model.InspectionProcedure.ScriptCommand4Entity>()
                .Include<ScriptCommand41Entity, Model.InspectionProcedure.ScriptCommand41Entity>()
                .Include<ScriptCommand42Entity, Model.InspectionProcedure.ScriptCommand42Entity>()
                .Include<ScriptCommand43Entity, Model.InspectionProcedure.ScriptCommand43Entity>()
                .Include<ScriptCommand5XEntity, Model.InspectionProcedure.ScriptCommand5XEntity>()
                .Include<ScriptCommand70Entity, Model.InspectionProcedure.ScriptCommand70Entity>();

            CreateMap<ScriptCommand2Entity, Model.InspectionProcedure.ScriptCommand2Entity>();
            CreateMap<ScriptCommand3Entity, Model.InspectionProcedure.ScriptCommand3Entity>();
            CreateMap<ScriptCommand4Entity, Model.InspectionProcedure.ScriptCommand4Entity>();
            CreateMap<ScriptCommand1Entity, Model.InspectionProcedure.ScriptCommand1Entity>();
            CreateMap<ScriptCommand41Entity, Model.InspectionProcedure.ScriptCommand41Entity>();
            CreateMap<ScriptCommand42Entity, Model.InspectionProcedure.ScriptCommand42Entity>();
            CreateMap<ScriptCommand43Entity, Model.InspectionProcedure.ScriptCommand43Entity>();
            CreateMap<ScriptCommand5XEntity, Model.InspectionProcedure.ScriptCommand5XEntity>();
            CreateMap<ScriptCommand70Entity, Model.InspectionProcedure.ScriptCommand70Entity>();

            CreateMap<PRSEntityJson, PRSEntity>()
               .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.PRSCode, opt => opt.MapFrom(src => src.PRSCode))
               .ForMember(dest => dest.PRSName, opt => opt.MapFrom(src => src.PRSName))
               .ForMember(dest => dest.PRSIdentification, opt => opt.MapFrom(src => src.PRSIdentification))
               .ForMember(dest => dest.Information, opt => opt.MapFrom(src => src.Information))
               .ForMember(dest => dest.InspectionProcedure, opt => opt.MapFrom(src => src.InspectionProcedure))
               .ForMember(dest => dest.InspectionProcedureId, opt => opt.MapFrom(src => src.InspectionProcedureId))
               .ForMember(dest => dest.PRSObjects, opt => opt.MapFrom(src => src.PRSObjects))
               .ForMember(dest => dest.GasControlLines, opt => opt.MapFrom(src => src.GasControlLines));

            CreateMap<PRSObjectJson, PRSObject>()
               .ForMember(dest => dest.ObjectName, opt => opt.MapFrom(src => src.ObjectName))
               .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.ObjectID))
               .ForMember(dest => dest.MeasurePoint, opt => opt.MapFrom(src => src.MeasurePoint))
               .ForMember(dest => dest.MeasurePointID, opt => opt.MapFrom(src => src.MeasurePointID))
               .ForMember(dest => dest.FieldNo, opt => opt.MapFrom(src => src.FieldNo))
               .ForMember(dest => dest.InspectionPointId, opt => opt.MapFrom(src => src.InspectionPointId));

            CreateMap<GasControlLineEntityJson, GasControlLineEntity>()
               .ForMember(dest => dest.PRSName, opt => opt.MapFrom(src => src.PRSName))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.PRSIdentification, opt => opt.MapFrom(src => src.PRSIdentification))
               .ForMember(dest => dest.GasControlLineName, opt => opt.MapFrom(src => src.GasControlLineName))
               .ForMember(dest => dest.PeMin, opt => opt.MapFrom(src => src.PeMin))
               .ForMember(dest => dest.PeMax, opt => opt.MapFrom(src => src.PeMax))
               .ForMember(dest => dest.VolumeVA, opt => opt.MapFrom(src => src.VolumeVA))
               .ForMember(dest => dest.VolumeVAK, opt => opt.MapFrom(src => src.VolumeVAK))
               .ForMember(dest => dest.PaRangeDM, opt => opt.MapFrom(src => src.PaRangeDM))
               .ForMember(dest => dest.PeRangeDM, opt => opt.MapFrom(src => src.PeRangeDM))
               .ForMember(dest => dest.GCLIdentification, opt => opt.MapFrom(src => src.GCLIdentification))
               .ForMember(dest => dest.GCLCode, opt => opt.MapFrom(src => src.GCLCode))
               .ForMember(dest => dest.InspectionProcedure, opt => opt.MapFrom(src => src.InspectionProcedure))
               .ForMember(dest => dest.StartPosition, opt => opt.MapFrom(src => src.StartPosition))
               .ForMember(dest => dest.FSDStart, opt => opt.MapFrom(src => src.FSDStart))
               .ForMember(dest => dest.GCLObjects, opt => opt.MapFrom(src => src.GCLObjects));

            CreateMap<GCLObjectJson, GCLObject>()
               .ForMember(dest => dest.ObjectName, opt => opt.MapFrom(src => src.ObjectName))
               .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.ObjectID))
               .ForMember(dest => dest.MeasurePoint, opt => opt.MapFrom(src => src.MeasurePoint))
               .ForMember(dest => dest.MeasurePointID, opt => opt.MapFrom(src => src.MeasurePointID))
               .ForMember(dest => dest.FieldNo, opt => opt.MapFrom(src => src.FieldNo))
               .ForMember(dest => dest.Boundaries, opt => opt.MapFrom(src => src.Boundaries))
               .ForMember(dest => dest.InspectionPointId, opt => opt.MapFrom(src => src.InspectionPointId));

            CreateMap<TypeObjectIDBoundariesJson, TypeObjectIDBoundaries>()
               .ForMember(dest => dest.ValueMax, opt => opt.MapFrom(src => src.ValueMax))
               .ForMember(dest => dest.ValueMin, opt => opt.MapFrom(src => src.ValueMin))
               .ForMember(dest => dest.ScriptCommandId, opt => opt.MapFrom(src => src.ScriptCommandId))
               .ForMember(dest => dest.Offset, opt => opt.MapFrom(src => src.Offset))
               .ForMember(dest => dest.UOV, opt => opt.MapFrom(src => src.UOV));

            CreateMap<Inspector.POService.PlexorInformation.PlexorEntity, PlexorEntity>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BlueToothAddress, opt => opt.MapFrom(src => src.BlueToothAddress))
                .ForMember(dest => dest.PN, opt => opt.MapFrom(src => src.PN))
                .ForMember(dest => dest.CalibrationDate, opt => opt.MapFrom(src => src.CalibrationDate))
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber));
        }
    }
}
