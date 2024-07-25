'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'===============================================================================
'Imports System.Linq
Imports Inspector.Model
Imports System.ComponentModel

Namespace Model.Station
    Public Class clsLinqToSql

        Private sqlDbImport As PRSsql.DataAccess.StationInformation
        Private sqlDbExport As PRSsql.DataAccess.StationInformation


#Region "Properties"
        ''' <summary>
        ''' Gets or sets the PRS entities.
        ''' </summary>
        ''' <value>The PRS entities.</value>
        Public Property PRSEntities() As Model.Station.Entities.PRSDataEntity
            Get
                Return m_PRSEntities
            End Get
            Set(value As Model.Station.Entities.PRSDataEntity)
                m_PRSEntities = value
            End Set
        End Property
        Private m_PRSEntities As New Model.Station.Entities.PRSDataEntity

        Private m_fileDate As DateTime

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_prs_id_New As Integer = 0
        Private m_prsObjects_id_New As Integer = 0
        Private m_GasControlLine_id_New As Integer = 0
        Private m_GCLObjects_id_New As Integer = 0

#End Region


        '' ''Not used
        ' ''Public Sub GetInfo()

        ' ''    Dim query = From prs In sqlDb.PRS() _
        ' ''                Where prs.Route = "ZZO" _
        ' ''                Select prs

        ' ''    For Each prs In query
        ' ''        Console.WriteLine("ID = {0}, name = {1}", prs.PRS_Id, prs.PRSName)
        ' ''    Next

        ' ''End Sub


#Region "Private"


#End Region

        '#Region "Save stationInformation"
        '        ''' <summary>
        '        ''' Write the data to a SDF file
        '        ''' </summary>
        '        ''' <param name="fileName"></param>
        '        ''' <remarks></remarks>
        '        Public Sub WriteStationInformation(ByVal fileName As String)

        '            sqlDbExport = New PRSsql.DataAccess.StationInformation(fileName)
        '            If sqlDbExport.DatabaseExists Then
        '                sqlDbExport.DeleteDatabase()
        '            End If
        '            sqlDbExport.CreateDatabase()

        '            FromEntityExtractPrs()
        '        End Sub

        '        ''' <summary>
        '        ''' Fill the SDF database with the information form the entitie (m_PRSEntities)
        '        ''' </summary>
        '        ''' <remarks></remarks>
        '        Private Sub FromEntityExtractPrs()
        '            'Set m_prs_id_New for record key

        '            m_prs_id_New = 0
        '            m_prsObjects_id_New = 0
        '            m_GasControlLine_id_New = 0
        '            m_GCLObjects_id_New = 0

        '            For Each prsEntityRecord In m_PRSEntities.PresureRegulatorStations
        '                Dim prsSqlRecord As New PRSsql.DataAccess.PRS With { _
        '                .PRS_Id = m_prs_id_New, _
        '                .PRSCode = prsEntityRecord.PRSCode, _
        '                .PRSName = prsEntityRecord.PRSName, _
        '                .PRSIdentification = prsEntityRecord.PRSIdentification, _
        '                .Route = prsEntityRecord.Route, _
        '                .Information = prsEntityRecord.Information, _
        '                .InspectionProcedure = prsEntityRecord.InspectionProcedure _
        '                }

        '                FromEntityExtractGCL(prsEntityRecord.GasControlLine, prsSqlRecord)
        '                FromEntityExtractPrsObject(prsEntityRecord.PRSObjects, prsSqlRecord)
        '                'Adding the prs to the SDF database
        '                sqlDbExport.PRS.InsertOnSubmit(prsSqlRecord)
        '                sqlDbExport.SubmitChanges()
        '                'Increase ID
        '                m_prs_id_New += 1
        '            Next

        '        End Sub

        '        Private Sub FromEntityExtractPrsObject(prsObjects As System.Collections.Generic.List(Of Entities.PRSSyncObject), prs As PRSsql.DataAccess.PRS)
        '            For Each prsObjectEntityRecord In prsObjects
        '                Dim prsObjectSqlRecord As New PRSsql.DataAccess.PRSObject With { _
        '                .PRSObjects_Id = m_prsObjects_id_New, _
        '                .PRSLinkID = m_prs_id_New, _
        '                .ObjectName = prsObjectEntityRecord.ObjectName, _
        '                .ObjectID = prsObjectEntityRecord.ObjectID, _
        '                .MeasurePoint = prsObjectEntityRecord.MeasurePoint, _
        '                .MeasurePointID = prsObjectEntityRecord.MeasurePointID, _
        '                .FieldNo = prsObjectEntityRecord.FieldNo _
        '                }
        '                'sqlDbExport.PRSObjects.InsertOnSubmit(prsNewObject)
        '                'sqlDbExport.SubmitChanges()
        '                prs.PRSObjects.Add(prsObjectSqlRecord)
        '                m_prsObjects_id_New += 1
        '            Next
        '        End Sub

        '        Private Sub FromEntityExtractGCL(gcl As System.Collections.Generic.List(Of Entities.GclSyncEntity), prs As PRSsql.DataAccess.PRS)
        '            For Each gclEntityRecord In gcl
        '                Dim gclSqlRecord As New PRSsql.DataAccess.GasControlLine With { _
        '                .GasControlLine_Id = m_GasControlLine_id_New, _
        '                .PRSLinkID = m_prs_id_New, _
        '                .GasControlLineName = gclEntityRecord.GasControlLineName, _
        '                .PeMin = gclEntityRecord.PeMin, _
        '                .PeMax = gclEntityRecord.PeMax, _
        '                .VolumeVA = gclEntityRecord.VolumeVA, _
        '                .VolumeVAK = gclEntityRecord.VolumeVAK, _
        '                .PaRangeDM = CastFromTypeRangeDMOrUnset(gclEntityRecord.PaRangeDM), _
        '                .PeRangeDM = CastFromTypeRangeDMOrUnset(gclEntityRecord.PeRangeDM), _
        '                .GCLIdentification = gclEntityRecord.GCLIdentification, _
        '                .InspectionProcedure = gclEntityRecord.InspectionProcedure, _
        '                .FSDStart = gclEntityRecord.FSDStart, _
        '                .GCLCode = gclEntityRecord.GCLCode _
        '                }
        '                'sqlDbExport.GasControlLines.InsertOnSubmit(gclNew)
        '                'sqlDbExport.SubmitChanges()
        '                FromEntityExtractGCLObject(gclEntityRecord.GCLObjects, gclSqlRecord)
        '                prs.GasControlLines.Add(gclSqlRecord)
        '                m_GasControlLine_id_New += 1
        '            Next
        '        End Sub

        '        Private Sub FromEntityExtractGCLObject(gclObjects As System.Collections.Generic.List(Of Entities.GCLObject), gcl As PRSsql.DataAccess.GasControlLine)

        '            For Each gclObjectRecord In gclObjects
        '                Dim gclNewObject As New PRSsql.DataAccess.GCLObject With { _
        '                .GCLObjects_Id = m_GCLObjects_id_New, _
        '                .GCLLinkID = m_GasControlLine_id_New, _
        '                .ObjectName = gclObjectRecord.ObjectName, _
        '                .ObjectID = gclObjectRecord.ObjectID, _
        '                .MeasurePoint = gclObjectRecord.MeasurePoint, _
        '                .MeasurePointID = gclObjectRecord.MeasurePointID, _
        '                .FieldNo = gclObjectRecord.FieldNo _
        '                }

        '                Dim gclObjectBoundariesRecord = gclObjectRecord.Boundaries
        '                gclNewObject.Value = ""
        '                gclNewObject.ValueMin = CastToDoubleOrNan(gclObjectBoundariesRecord.ValueMin)
        '                gclNewObject.ValueMax = CastToDoubleOrNan(gclObjectBoundariesRecord.ValueMax)
        '                gclNewObject.UOV = CastFromTypeUnitsValueOrUnset(gclObjectBoundariesRecord.UOV)
        '                gclNewObject.Percentage = ""
        '                'sqlDbExport.GCLObjects.InsertOnSubmit(gclNewObject)
        '                gcl.GCLObjects.Add(gclNewObject)
        '                m_GCLObjects_id_New += 1
        '            Next
        '            'Try
        '            '    sqlDbExport.SubmitChanges()

        '            'Catch ex As Exception
        '            '    MsgBox(ex.Message)
        '            'End Try
        '        End Sub

        '#End Region

        '#Region "Load stationinformation"
        '        ''' <summary>
        '        ''' Load the SQL file into PRSEntities() As Model.Station.PRSEntities 
        '        ''' </summary>
        '        ''' <param name="fileName"></param>
        '        ''' <remarks></remarks>
        '        Public Sub LoadSqlImportData(ByVal fileName As String)
        '            sqlDbImport = New PRSsql.DataAccess.StationInformation(GetConnectionString(fileName)) ' With {.Log = Console.Out}
        '            PRSEntities = ReadStationInformation()


        '        End Sub
        '        ''' <summary>
        '        ''' Create an connection string with the SQL ce dtabase; *.sdf
        '        ''' </summary>
        '        ''' <param name="fileName"></param> File name of the sql CE (*.sdf) database
        '        ''' <returns></returns>
        '        ''' <remarks></remarks>
        '        Private Function GetConnectionString(ByVal fileName As String) As String
        '            m_fileDate = IO.File.GetLastWriteTime(fileName).ToUniversalTime

        '            Dim conn = "Data Source={0}"
        '            conn = String.Format(conn, fileName)
        '            Return conn
        '        End Function

        '        Public Sub Reload()
        '            PRSEntities = ReadStationInformation()
        '        End Sub
        '        ''' <summary>
        '        ''' Reads the station information file
        '        ''' </summary>
        '        ''' <returns></returns>
        '        ''' <remarks></remarks>
        '        Private Function ReadStationInformation() As Model.Station.Entities.PRSDataEntity
        '            Dim prsResults As Model.Station.Entities.PRSDataEntity = Nothing
        '            prsResults = ToEntityExtractPrsObject()
        '            Return prsResults
        '        End Function

        '        Private Function ToEntityExtractPrsObject() As Model.Station.Entities.PRSDataEntity
        '            Dim prsEntitiesResults As New Model.Station.Entities.PRSDataEntity
        '            Dim prsEntityResults As New List(Of Model.Station.Entities.PRSEntity)()

        '            ' Dim query = From prsRecords In sqlDb.PRS() _
        '            'Where prsRecords.Route = "ZZO" _
        '            'Select prsRecords

        '            For Each prsSqlRecord In sqlDbImport.PRS()
        '                Dim prsEntityRecord As New Model.Station.Entities.PRSEntity() With { _
        '                .FileDate = m_fileDate.ToUniversalTime, _
        '               .PRSName = prsSqlRecord.PRSName.ToString(), _
        '               .PRSIdentification = prsSqlRecord.PRSIdentification.ToString(), _
        '               .PRSCode = prsSqlRecord.PRSCode.ToString(), _
        '               .Route = prsSqlRecord.Route, _
        '               .Information = prsSqlRecord.Information.ToString(), _
        '               .InspectionProcedure = prsSqlRecord.InspectionProcedure.ToString() _
        '                }

        '                Console.WriteLine(Now & " ID = {0}, name = {1}", prsEntityRecord.PRS_Id, prsEntityRecord.PRSName)

        '                ' ExtractPRSObject(prsRecord.PRS_Id)
        '                ToEntityExtractGasControlLineObject(prsSqlRecord.PRS_Id, prsEntityRecord)
        '                prsEntityResults.Add(prsEntityRecord)
        '            Next

        '            prsEntitiesResults.PresureRegulatorStations = prsEntityResults
        '            Return prsEntitiesResults
        '        End Function


        '        Private Sub ToEntityExtractGasControlLineObject(prsLinkID As Integer, prs As Model.Station.Entities.PRSEntity)

        '            'Search on PRSId in sql database
        '            Dim query = From gclSqlRecords In sqlDbImport.GasControlLines() _
        '                Where gclSqlRecords.PRSLinkID = prsLinkID _
        '                Select gclSqlRecords


        '            For Each gclSqlRecord In query
        '                Dim gclEntityRecord As New Model.Station.Entities.GclSyncEntity() With { _
        '                  .PRSName = "", _
        '                  .PRSIdentification = "", _
        '                  .GasControlLineName = gclSqlRecord.GasControlLineName.ToString(), _
        '                  .PeMin = gclSqlRecord.PeMin.ToString(), _
        '                  .PeMax = gclSqlRecord.PeMax.ToString(), _
        '                  .VolumeVA = gclSqlRecord.VolumeVA.ToString(), _
        '                  .VolumeVAK = gclSqlRecord.VolumeVAK.ToString(), _
        '                  .PaRangeDM = CastToTypeRangeDMOrUnset(gclSqlRecord.PaRangeDM.ToString()), _
        '                  .PeRangeDM = CastToTypeRangeDMOrUnset(gclSqlRecord.PeRangeDM.ToString()), _
        '                  .GCLIdentification = gclSqlRecord.GCLIdentification.ToString(), _
        '                  .GCLCode = gclSqlRecord.GCLCode, _
        '                  .InspectionProcedure = gclSqlRecord.InspectionProcedure.ToString(), _
        '                  .FSDStart = If(CastToIntOrNull(gclSqlRecord.FSDStart.ToString()), -1) _
        '                }

        '                ToEntityExtractGasControlLineGclObjects(gclSqlRecord.GasControlLine_Id, gclEntityRecord)
        '                prs.GasControlLine.Add(gclEntityRecord)


        '            Next

        '        End Sub


        '        ''' <summary>
        '        ''' Extracts the gas control line GCL objects.
        '        ''' </summary>
        '        ''' <param name="gclLinkID">The PRS gas control line.</param>
        '        ''' <param name="gcl">The GCL.</param>
        '        Private Sub ToEntityExtractGasControlLineGclObjects(gclLinkID As Integer, gcl As Model.Station.Entities.GclSyncEntity)

        '            'Search on PRSId in sql database
        '            Dim query = From gclObjectSqlRecords In sqlDbImport.GCLObjects() _
        '                Where gclObjectSqlRecords.GCLLinkID = gclLinkID _
        '                Select gclObjectSqlRecords

        '            gcl.GCLObjects = New List(Of Model.Station.Entities.GCLObject)

        '            For Each gclObjectSqlRecord In query

        '                Dim gclObjectEntityRecord As New Model.Station.Entities.GCLObject() With { _
        '                      .ObjectName = CastToTextOrNan(gclObjectSqlRecord.ObjectName), _
        '                      .ObjectID = CastToTextOrNan(gclObjectSqlRecord.ObjectID), _
        '                      .MeasurePoint = CastToTextOrNan(gclObjectSqlRecord.MeasurePoint), _
        '                      .MeasurePointID = CastToTextOrNan(gclObjectSqlRecord.MeasurePointID), _
        '                      .FieldNo = CastToIntOrNull(gclObjectSqlRecord.FieldNo) _
        '                    }

        '                If gclObjectSqlRecord.ValueMax.Length > 0 Or gclObjectSqlRecord.ValueMin.Length > 0 Then
        '                    gclObjectEntityRecord.Boundaries = New Model.Station.Entities.TypeSyncObjectIDBoundaries() With { _
        '                      .ValueMax = CastToDoubleOrNan(gclObjectSqlRecord.ValueMax), _
        '                      .ValueMin = CastToDoubleOrNan(gclObjectSqlRecord.ValueMin), _
        '                      .UOV = CastToTypeUnitsValueOrUnset(gclObjectSqlRecord.UOV) _
        '                    }
        '                End If

        '                gcl.GCLObjects.Add(gclObjectEntityRecord)
        '            Next

        '        End Sub
        '#End Region

#Region "Casting helpers"
    


        ''' <summary>
        ''' This procedure gets the "Description" attribute of an enum constant, if any.
        ''' Otherwise it gets the string name of the enum member.
        ''' </summary>
        ''' <param name="enumConstant"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EnumDescription(ByVal enumConstant As [Enum]) As String
            Dim fi As Reflection.FieldInfo = enumConstant.GetType().GetField(enumConstant.ToString())
            Dim aattr() As DescriptionAttribute = DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
            If aattr.Length > 0 Then
                Return aattr(0).Description
            Else
                Return (enumConstant.ToString())
            End If
        End Function

#End Region
    End Class

End Namespace