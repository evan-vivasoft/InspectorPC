'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports DynamicCondition
Imports System.IO
Imports System.Xml.Serialization
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Station
    ''' <summary>
    ''' Class for converting an MsAccess database into a XML file
    ''' </summary>
    ''' <remarks>Options:</remarks>
    ''' Clear all PRS data in MsAccess database
    ''' Export filtering on PRS code; Can be used for Inspector selection
    Public Class ClsLinqMsAccessToXml
#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private m_msAccessExport As PRSMDB.DataAccess.StationInformation
        Private m_EntitiesPrsImport As New Model.Station.Entities.PRSDataEntity

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_prs_id_New As Integer = 0
        Private m_prsObjects_id_New As Integer = 0
        Private m_gcl_id_New As Integer = 0
        Private m_gclObjects_id_New As Integer = 0
#End Region
#Region "Properties"


#End Region
#Region "Constructors"
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

#End Region

#Region "Public"
        ''' <summary>
        ''' Export the PRS data from the msAccess database to a xml entity. Use function WriteStationInformation to create a xml file.
        ''' </summary>
        ''' <param name="msAccessExport_FileName">File name of the msAccess database</param>
        ''' <param name="selections">Optional selection string; to select a set of PRS</param>
        ''' <remarks></remarks>
        Public Function LoadStationInformation(ByVal msAccessExport_FileName As String, Optional selections As String = "") As Boolean
            'MOD 06
            If Not CheckFileExistsPC(msAccessExport_FileName) Then
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, 0, 0, clsDbGeneral.DbCreateStatus.FileNotExists, msAccessExport_FileName)
                Return False
            End If

            m_msAccessExport = New PRSMDB.DataAccess.StationInformation(msAccessExport_FileName) ' With {.Log = Console.Out}

            'Clearing the data
            ClearStationInformation()

            'Start extracting the data from  the database
            ExtractPrs_FromMsAccessToEntity(selections)
            Return True
        End Function

        ''' <summary>
        ''' Clear the PRS data 
        ''' </summary>
        ''' <remarks></remarks>
        Public Function ClearStationInformation() As Boolean
            m_EntitiesPrsImport.GasControlLine.Clear()
            m_EntitiesPrsImport.PresureRegulatorStations.Clear()
            m_prs_id_New = 0
            m_prsObjects_id_New = 0
            m_gcl_id_New = 0
            m_gclObjects_id_New = 0
            Return True
        End Function

        ''' <summary>
        ''' Write the entity into a XML file
        ''' </summary>
        ''' <param name="xmlFile">XML file to write</param>
        ''' <param name="xsdFile">XSD file for xml validation</param>
        ''' <remarks></remarks>
        Public Function WriteStationInformation(ByVal xmlFile As String, xsdFile As String) As Boolean
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, 0, 0, clsDbGeneral.DbCreateStatus.StartedWrite, "!Saving data started")

            Dim objStreamWriter As New StreamWriter(xmlFile)
            Dim x As New XmlSerializer(m_EntitiesPrsImport.GetType)
            x.Serialize(objStreamWriter, m_EntitiesPrsImport)
            objStreamWriter.Close()
            'Check if the create file is correct.
            Try
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, 0, 0, clsDbGeneral.DbCreateStatus.SuccesWrite, "!Saving data completed")
                Return True
            Catch ex As Exception
                'MOD 21
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, 0, 0, clsDbGeneral.DbCreateStatus.ErrorXsd, xmlFile)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Clear all tables from the MsAccess database
        ''' Can be used to delete the data after synchronisation
        ''' </summary>
        ''' <param name="msAccessExport_FileName">File name of the msAccess database</param>
        ''' <remarks></remarks>
        Public Function ClearAllPrsinMsAccess(ByVal msAccessExport_FileName As String) As Boolean
            m_msAccessExport = New PRSMDB.DataAccess.StationInformation(msAccessExport_FileName)

            Dim deletetblPrsDatas = From details In m_msAccessExport.tblPRSdatas() Select details
            Dim deletetblPrsObjects = From details In m_msAccessExport.tblPRSObjects() Select details
            Dim deletetblGclDatas = From details In m_msAccessExport.tblGCLdatas() Select details
            Dim deletetblGclObjectss = From details In m_msAccessExport.tblGCLObjects() Select details
            m_msAccessExport.tblPRSdatas.DeleteAllOnSubmit(deletetblPrsDatas)
            m_msAccessExport.tblPRSObjects.DeleteAllOnSubmit(deletetblPrsObjects)
            m_msAccessExport.tblGCLdatas.DeleteAllOnSubmit(deletetblGclDatas)
            m_msAccessExport.tblGCLObjects.DeleteAllOnSubmit(deletetblGclObjectss)

            Try
                m_msAccessExport.SubmitChanges()
                Return True
            Catch ex As Exception
                Console.WriteLine(ex)
                Return False
                ' Provide for exceptions 
            End Try

        End Function

#End Region
#Region "Dataset parse"
        ''' <summary>
        ''' Extract the PRS data from the msAccess database into the Entity
        ''' </summary>
        ''' <param name="selections">List (; separated) of the selections</param>
        ''' <remarks></remarks>
        Private Sub ExtractPrs_FromMsAccessToEntity(Optional selections As String = "")
            Dim queryResult

            'Check if there is a selection criteria
            If selections <> "" Then
                Dim queryResultFilter = From prsMsaRecords In m_msAccessExport.tblPRSdatas
                Dim selectionsSplit As String() = selections.Split(";") 'Split the string
                Dim conditionFirst As Condition(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblPRSdata)
                Dim conditionsArray() As Condition(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblPRSdata)

                Dim i As Integer = -1
                'split up the string and assign the criteria
                For Each s As String In selectionsSplit
                    i += 1
                    If i = 0 Then
                        conditionFirst = queryResultFilter.CreateCondition("PRSCode", DynamicQuery.Condition.Compare.Equal, s)
                    Else
                        ReDim Preserve conditionsArray(i - 1)
                        conditionsArray(i - 1) = queryResultFilter.CreateCondition("PRSCode", DynamicQuery.Condition.Compare.Equal, s)
                    End If
                Next
                Dim c As DynamicCondition.Condition(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblPRSdata)
                'Check the amount of criteria
                If i = 0 Then
                    'Only one 
                    c = conditionFirst
                Else
                    c = Condition.Combine(conditionFirst, DynamicQuery.Condition.Compare.Or, conditionsArray(i - 1))
                End If

                Dim filterdata = m_msAccessExport.tblPRSdatas.Where(c)
                queryResult = From prsMsaRecords In filterdata Order By prsMsaRecords.PRSName Select prsMsaRecords
            Else
                queryResult = m_msAccessExport.tblPRSdatas.OrderBy(Function(t) t.PRSName)
            End If

            Dim prsEntityList As New List(Of Model.Station.Entities.PRSSyncEntity)()
            Dim gclEntityList As New List(Of Model.Station.Entities.GclSyncEntity)()

            Dim recordCounterProcessed As Integer = 0
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, m_msAccessExport.tblPRSdatas.Count, recordCounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, "")
            For Each prsMsaRecord In queryResult
                recordCounterProcessed += 1
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, m_msAccessExport.tblPRSdatas.Count, recordCounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, prsMsaRecord.PRSName)

                'Adding the data
                Dim prsEntityRecord As New Model.Station.Entities.PRSSyncEntity() With { _
               .FileDate = Now.ToUniversalTime, _
               .PRS_Id = m_prs_id_New, _
               .Route = CastToStringOrNothing(prsMsaRecord.Route), _
               .PRSCode = CastToStringOrEmpty(prsMsaRecord.PRSCode), _
               .PRSName = CastToStringOrEmpty(prsMsaRecord.PRSName), _
               .PRSIdentification = CastToStringOrEmpty(prsMsaRecord.PRSIdentification), _
               .Information = CastToStringOrEmpty(prsMsaRecord.Information), _
               .InspectionProcedure = CastToStringOrEmpty(prsMsaRecord.InspectionProcedure) _
               }

                Console.WriteLine(Now & " ID = {0}, name = {1}", prsMsaRecord.PrsId, prsMsaRecord.PRSName)

                ExtractPrsObject_FromMsAccessToEntity(prsMsaRecord.tblPRSObjects, prsEntityRecord)
                ExtractGcl_FromMsAccessToEntity(prsMsaRecord.tblGCLdatas, prsEntityRecord, gclEntityList)
                prsEntityList.Add(prsEntityRecord)
                'Increase ID
                m_prs_id_New += 1
            Next

            m_EntitiesPrsImport.PresureRegulatorStations = prsEntityList
            m_EntitiesPrsImport.GasControlLine = gclEntityList

        End Sub

        ''' <summary>
        ''' Extracts the PRS object from the MsAccess database into the SQL dataset
        ''' </summary>
        ''' <param name="prsObjects">the object MsAccess tblPrsObject</param>
        ''' <param name="prs">Model.Station.Entities.PRSEntit</param>
        ''' <remarks></remarks>
        Private Sub ExtractPrsObject_FromMsAccessToEntity(ByVal prsObjects As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblPRSObject), ByRef prs As Model.Station.Entities.PRSSyncEntity)
            If prsObjects.HasLoadedOrAssignedValues Then
                For Each prsObjectMsaRecord In prsObjects
                    Dim prsObjectsEntityRecord As New Model.Station.Entities.PRSSyncObject() With { _
                    .ObjectName = CastToStringOrEmpty(prsObjectMsaRecord.ObjectName), _
                    .ObjectID = CastToStringOrEmpty(prsObjectMsaRecord.ObjectID), _
                    .MeasurePoint = CastToStringOrEmpty(prsObjectMsaRecord.MeasurePoint), _
                    .MeasurePointID = CastToStringOrEmpty(prsObjectMsaRecord.MeasurePointID), _
                    .FieldNo = Convert.ToInt16(prsObjectMsaRecord.FieldNo) _
                    }
                    prs.PRSObjects.Add(prsObjectsEntityRecord)
                    m_prsObjects_id_New += 1
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the GCL from the MsAccess database into the SQL database
        ''' </summary>
        ''' <param name="gclSet">the object MsAccess tblGCLdata</param>
        ''' <param name="gcl">List(Of Model.Station.Entities.GclSyncEntity)</param>
        ''' <remarks></remarks>
        Private Sub ExtractGcl_FromMsAccessToEntity(ByVal gclSet As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblGCLdata), ByVal prs As Model.Station.Entities.PRSSyncEntity, ByRef gcl As List(Of Model.Station.Entities.GclSyncEntity))
            'For inspector only one unique GCL for each PRS may occur. Communicator only selects one unique GCL
            Dim queryResult = gclSet.GroupBy(Function(r) r.GCLName).[Select](Function(g) g.First())

            'For Each gclMsaRecord In gclSet.OrderBy(Function(t) t.GCLName)
            For Each gclMsaRecord In queryResult
                Dim gclEntityRecord As New Model.Station.Entities.GclSyncEntity() With { _
                  .PRSName = CastToStringOrEmpty(prs.PRSName), _
                  .PRSIdentification = CastToStringOrEmpty(prs.PRSIdentification), _
                  .GasControlLineName = CastToStringOrEmpty(gclMsaRecord.GCLName), _
                  .PeMin = CastToStringOrEmpty(gclMsaRecord.PeMin), _
                  .PeMax = CastToStringOrEmpty(gclMsaRecord.PeMax), _
                  .VolumeVA = CastToStringOrEmpty(gclMsaRecord.VolumeVA), _
                  .VolumeVAK = CastToStringOrEmpty(gclMsaRecord.VolumeVAK), _
                  .PaRangeDM = CastToTypeRangeDMOrUnset(gclMsaRecord.PdRangeDM), _
                  .PeRangeDM = CastToTypeRangeDMOrUnset(gclMsaRecord.PuRangeDM), _
                  .GCLIdentification = CastToStringOrEmpty(gclMsaRecord.GCLIdentification), _
                  .InspectionProcedure = CastToStringOrEmpty(gclMsaRecord.InspectionProcedure), _
                  .FSDStart = If(CastToIntOrNull(gclMsaRecord.FSDStartPosition), -1), _
                  .GCLCode = "" _
                }
                'MOD 33
                ExtractGclObject_FromMsAccessToEntity(gclMsaRecord.tblGCLObjects, gclEntityRecord)
                gcl.Add(gclEntityRecord)
                m_gcl_id_New += 1

            Next
        End Sub
        ''' <summary>
        ''' Extracts the GCL objects from the MsAccess database into the SQL database
        ''' </summary>
        ''' <param name="gclObjects">the object MsAccess tblGCLObject</param>
        ''' <param name="gclObject">gclObject As Model.Station.Entities.GclSyncEntity</param>
        ''' <remarks></remarks>
        Private Sub ExtractGclObject_FromMsAccessToEntity(ByVal gclObjects As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblGCLObject), gclObject As Model.Station.Entities.GclSyncEntity)
            gclObject.GCLObjects = New List(Of Model.Station.Entities.GCLObject)

            'MOD 22
            If gclObjects.Count = 0 Then
                Dim gclObjectEntityRecord As New Model.Station.Entities.GCLObject() With { _
                .ObjectName = "", _
                .ObjectID = "", _
                .MeasurePoint = "", _
                .MeasurePointID = "", _
                .FieldNo = "70" _
                }

                'MOD 34; "mbar" replace by "-"
                gclObjectEntityRecord.Boundaries = New Model.Station.Entities.TypeSyncObjectIDBoundaries() With { _
                     .ValueMax = 10, _
                     .ValueMin = 0, _
                     .UOV = CastToTypeUnitsValueOrUnset("-") _
                   }
                gclObject.GCLObjects.Add(gclObjectEntityRecord)
                Exit Sub
            End If

            For Each gclObjectMsaRecord In gclObjects
                Dim gclObjectEntityRecord As New Model.Station.Entities.GCLObject() With { _
                .ObjectName = CastToStringOrEmpty(gclObjectMsaRecord.ObjectName), _
                .ObjectID = CastToStringOrEmpty(gclObjectMsaRecord.ObjectID), _
                .MeasurePoint = CastToStringOrEmpty(gclObjectMsaRecord.MeasurePoint), _
                .MeasurePointID = CastToStringOrEmpty(gclObjectMsaRecord.MeasurePointID), _
                .FieldNo = Convert.ToInt16(gclObjectMsaRecord.FieldNo) _
                }

                gclObjectEntityRecord.Boundaries = New Model.Station.Entities.TypeSyncObjectIDBoundaries() With { _
                     .ValueMax = CastToDoubleOrNan(gclObjectMsaRecord.ValueMax), _
                     .ValueMin = CastToDoubleOrNan(gclObjectMsaRecord.ValueMin), _
                     .UOV = CastToTypeUnitsValueOrUnset(gclObjectMsaRecord.Uov) _
                   }

                gclObject.GCLObjects.Add(gclObjectEntityRecord)
                m_gclObjects_id_New += 1
            Next
        End Sub

#End Region


    End Class
End Namespace