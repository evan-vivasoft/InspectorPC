'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Xml.Serialization
Imports DynamicCondition

Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.Entities.PrsConstants
Imports Inspector.Model.InspectionProcedure
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

' ''THANKS for the help, in the end I JUST was able to get a dataset.vb file generated using the xsd.exe tool. It works for now, however, I still think something isn't set right in Visual Studio 2008 or at least the "Generate Dataset" menu option from the context menu on an XSD file is gone.

' ''I'll just need to remember that if I modify the XSD file from here on out that Visual Studio isn't updating the .vb file automatically, I'll probably get stuck with reusing the xsd.exe program.

' ''For others the command is (using Visual Studio 2008 Command Prompt Window Tool, in Admin mode if using Windows Vista).
' ''xsd.exe /d /l:VB "XSD FILE LOCATION PATH"
' ''/d means create a dataset code. /l is the language.

' ''The .vb file will be created in C:\Windows\System32.


Namespace Model.Station
    ''' <summary>
    ''' Information of PRS and contained GasControlLines
    ''' </summary>
    ''' <remarks>Options:</remarks>
    ''' Do not delete the not inspected PRS or GCL on INSPECTOR
    Public Class clsLinqXMLToXML
#Region "Constants"


#End Region

#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

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

        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="xmlFile"></param>
        ''' <param name="xsdFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal xmlFile As String, xsdFile As String)
            LoadStationInformation(xmlFile, xsdFile)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Load a xml file into public propertry PRSEntities
        ''' This is done to read more XML files into a single entity
        ''' With write Entity is written into the XML file
        ''' </summary>
        ''' <param name="xmlFile">XML file to read</param>
        ''' <param name="xsdFile">XSD file for xml validation </param>
        ''' <remarks></remarks>
        Public Function LoadStationInformation(ByVal xmlFile As String, ByVal xsdFile As String, Optional usePrsSatus As Boolean = False, Optional xmlFilePrsStatus As String = "", Optional xsdFilePrsStatus As String = "") As Boolean
            ReadStationInformation(xmlFile, xsdFile, usePrsSatus, xmlFilePrsStatus, xsdFilePrsStatus)
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
        ''' <param name="selections">List (; separated) of the selections; Used for the PRS status</param>
        ''' <remarks></remarks>
        Public Function WriteStationInformation(ByVal xmlFile As String, xsdFile As String, Optional selections As String = "") As Boolean
            'MOD 28
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, DbCreateStatus.StartedWrite, "")
            'Check if there is a selection criteria
            If selections <> "" Then
                Dim selectionsSplit As String() = selections.Split(";") 'Split the string
                Dim conditionFirstPrs As Condition(Of Model.Station.Entities.PRSSyncEntity)
                Dim conditionFirstPrsGcl As Condition(Of Model.Station.Entities.PRSSyncEntity)
                Dim conditionArrayPrs() As Condition(Of Model.Station.Entities.PRSSyncEntity)
                Dim conditionsArrayPrsGcl() As Condition(Of Model.Station.Entities.PRSSyncEntity)

                Dim conditionFirstGcl As Condition(Of Model.Station.Entities.GclSyncEntity)
                Dim conditionsArrayGcl() As Condition(Of Model.Station.Entities.GclSyncEntity)

                ReDim Preserve conditionArrayPrs(-1)
                ReDim Preserve conditionsArrayPrsGcl(-1)
                ReDim Preserve conditionsArrayGcl(-1)

                Dim i As Integer = -1
                'split up the string and assign the criteria
                For Each s As String In selectionsSplit
                    i += 1
                    Dim ints As Integer
                    ints = CastToIntOrNull(s)
                    'If i = 0 Then
                    'Condition for PRS only
                    'conditionFirstPrs = m_EntitiesPrsImport.PresureRegulatorStations.CreateCondition("PRS_Status", DynamicQuery.Condition.Compare.Equal, ints)
                    'Condition for PRS and GCL
                    'conditionFirstPrsGcl = Condition.Combine(conditionFirstPrs, DynamicQuery.Condition.Compare.And, m_EntitiesPrsImport.PresureRegulatorStations.CreateCondition("GCL_OverallStatus", DynamicQuery.Condition.Compare.Equal, ints))
                    'condition for GCL only
                    'conditionFirstGcl = m_EntitiesPrsImport.GasControlLine.CreateCondition("GCL_Status", DynamicQuery.Condition.Compare.Equal, ints)

                    'ReDim Preserve conditionArrayPrs(-1)
                    'ReDim Preserve conditionsArrayPrsGcl(-1)
                    'Else
                    'Condition for PRS only
                    ReDim Preserve conditionArrayPrs(conditionArrayPrs.Length)
                    conditionArrayPrs(conditionArrayPrs.Length - 1) = m_EntitiesPrsImport.PresureRegulatorStations.CreateCondition("PRS_Status", DynamicQuery.Condition.Compare.Equal, ints)

                    'Condition for PRS and GCL
                    ReDim Preserve conditionsArrayPrsGcl(conditionsArrayPrsGcl.Length)
                    conditionsArrayPrsGcl(conditionsArrayPrsGcl.Length - 1) = m_EntitiesPrsImport.PresureRegulatorStations.CreateCondition("GCL_OverallStatus", DynamicQuery.Condition.Compare.Equal, ints)


                    'Condition for GCL only
                    ReDim Preserve conditionsArrayGcl(conditionsArrayGcl.Length)
                    conditionsArrayGcl(conditionsArrayPrsGcl.Length - 1) = m_EntitiesPrsImport.GasControlLine.CreateCondition("GCL_Status", DynamicQuery.Condition.Compare.Equal, ints)
                    'End If
                Next
                Dim conditionPrs As DynamicCondition.Condition(Of Model.Station.Entities.PRSSyncEntity)
                Dim conditionGcl As DynamicCondition.Condition(Of Model.Station.Entities.GclSyncEntity)

                'MOD 28 For j = 0 To conditionArrayPrs.Count - 1
                '    For i = 0 To conditionsArrayPrsGcl.Count - 1
                '        conditionsArrayPrsGcl(conditionsArrayPrsGcl.Count - 1) = Condition.Combine(conditionArrayPrs(j), DynamicQuery.Condition.Compare.And, conditionsArrayPrsGcl(i))
                '    Next i
                'Next j

                'Check the amount of criteria
                '                If i = 0 Then
                'Only one 
                'MOD 28 conditionPrs = conditionFirstPrsGcl
                conditionGcl = conditionFirstGcl

                'MOD 28 Else
                'conditionPrs = Condition.Combine(conditionFirstPrsGcl, DynamicQuery.Condition.Compare.Or, conditionsArrayPrsGcl(i - 1))
                'conditionGcl = Condition.Combine(conditionFirstGcl, DynamicQuery.Condition.Compare.Or, conditionsArrayGcl(i - 1))

                For i = 0 To conditionArrayPrs.Count - 1
                    For j = 0 To conditionsArrayPrsGcl.Count - 1
                        Dim conditionTemp
                        conditionTemp = Condition.Combine(conditionArrayPrs(i), DynamicQuery.Condition.Compare.And, conditionsArrayPrsGcl(j))
                        If j = 0 And i = 0 Then
                            conditionPrs = conditionTemp
                        Else
                            conditionPrs = Condition.Combine(conditionPrs, DynamicQuery.Condition.Compare.Or, conditionTemp)
                        End If

                    Next j
                    'MOD 28 conditionPrs = Condition.Combine(conditionPrs, DynamicQuery.Condition.Compare.Or, conditionsArrayPrsGcl(i))
                Next

                For i = 0 To conditionsArrayGcl.Count - 1
                    If i = 0 Then
                        conditionGcl = conditionsArrayGcl(i)
                    Else
                        conditionGcl = Condition.Combine(conditionGcl, DynamicQuery.Condition.Compare.Or, conditionsArrayGcl(i))
                    End If
                Next
                'End If
                Dim filteredSyncEntities As New Model.Station.Entities.PRSDataEntity

                Dim filterListPrs As IEnumerable(Of Model.Station.Entities.PRSSyncEntity) = m_EntitiesPrsImport.PresureRegulatorStations.Where(conditionPrs)
                filteredSyncEntities.PresureRegulatorStations = New List(Of Model.Station.Entities.PRSSyncEntity)(filterListPrs)

                Dim filterListGcl As IEnumerable(Of Model.Station.Entities.GclSyncEntity) = m_EntitiesPrsImport.GasControlLine.Where(conditionGcl)
                filteredSyncEntities.GasControlLine = New List(Of Model.Station.Entities.GclSyncEntity)(filterListGcl)

                Dim objStreamWriter As New StreamWriter(xmlFile)
                Dim x As New XmlSerializer(filteredSyncEntities.GetType)
                x.Serialize(objStreamWriter, filteredSyncEntities)
                objStreamWriter.Close()
            Else
                Dim objStreamWriter As New StreamWriter(xmlFile)
                Dim x As New XmlSerializer(m_EntitiesPrsImport.GetType)
                x.Serialize(objStreamWriter, m_EntitiesPrsImport)
                objStreamWriter.Close()
            End If

            'Check if the create file is correct.
            Try
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, DbCreateStatus.SuccesWrite, "!Saving data completed")
                Return True
            Catch ex As Exception
                'MOD 21
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, DbCreateStatus.ErrorXsd, xmlFile)
                Return False
            End Try

        End Function
#End Region

#Region "Dataset parse"
        ''' <summary>
        ''' Reads the station information into Model.Station.Entities
        ''' </summary>
        ''' <param name="xmlFilePrs">Xml file with PRS data to read</param>
        ''' <param name="xsdFilePrs">XSD file with PRS structure</param>
        ''' <param name="usePrsSatus">Use the inspection status file</param>
        ''' <param name="xmlFilePrsStatus">Xml file with inspection status data to read</param>
        ''' <param name="xsdFilePrsStatus">XSD file with inspection status structure</param>
        ''' <remarks></remarks>
        Private Sub ReadStationInformation(ByVal xmlFilePrs As String, ByVal xsdFilePrs As String, Optional usePrsSatus As Boolean = False, Optional xmlFilePrsStatus As String = "", Optional xsdFilePrsStatus As String = "")
            Using dataSetPrs As New DataSet()
                dataSetPrs.Locale = CultureInfo.InvariantCulture
                dataSetPrs.ReadXmlSchema(xsdFilePrs)
                dataSetPrs.ReadXml(xmlFilePrs, XmlReadMode.ReadSchema)
                InjectPrsToGasControlLineRelation(dataSetPrs)

                Dim prsTable As DataTable = dataSetPrs.Tables(PRS_TABLE_NAME)

                If usePrsSatus = True Then
                    'Check if the status file is present. If not then the status is set to 1
                    If CheckFileExistsPC(xmlFilePrsStatus) Then
                        Dim dataSetPrsStatus As New DataSet()
                        dataSetPrsStatus.Locale = CultureInfo.InvariantCulture
                        dataSetPrsStatus.ReadXmlSchema(xsdFilePrsStatus)
                        dataSetPrsStatus.ReadXml(xmlFilePrsStatus, XmlReadMode.ReadSchema)

                        Dim prsStatusTable As DataTable = dataSetPrsStatus.Tables(PRSSTATUS_TABLE_NAME)
                        ExtractPrs_FromXmlToEntity(prsTable, IO.File.GetLastWriteTime(xmlFilePrs), True, prsStatusTable)
                    Else
                        ExtractPrs_FromXmlToEntity(prsTable, IO.File.GetLastWriteTime(xmlFilePrs), True, Nothing)
                    End If
                Else
                    'Extracting the data
                    ExtractPrs_FromXmlToEntity(prsTable, IO.File.GetLastWriteTime(xmlFilePrs))
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Injects the PRS-to-gascontrolline relation.
        ''' </summary>
        ''' <param name="dataSetPrs">The data set.</param>
        Private Shared Sub InjectPrsToGasControlLineRelation(dataSetPrs As DataSet)
            Dim parentKeyColumns As DataColumn() = New DataColumn() {dataSetPrs.Tables(PRS_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSetPrs.Tables(PRS_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
            Dim childKeyColumns As DataColumn() = New DataColumn() {dataSetPrs.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSetPrs.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
            Dim relation As New DataRelation(PRS_GCL_RELATION_NAME, parentKeyColumns, childKeyColumns, False)
            dataSetPrs.Relations.Add(relation)
        End Sub

        ''' <summary>
        ''' Extracts the PRS objects.
        ''' </summary>
        ''' <param name="prsTable">The PRS table.</param>
        Private Sub ExtractPrs_FromXmlToEntity(ByVal prsTable As DataTable, ByVal fileDate As DateTime, Optional usePrsStatus As Boolean = False, Optional ByVal prsStatusTable As DataTable = Nothing)
            Dim recordCounterProcessed As Integer = 0
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, prsTable.Rows.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, "")
            For Each prsXmlRecord As DataRow In prsTable.Rows
                recordCounterProcessed += 1
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, prsTable.Rows.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, prsXmlRecord("PRSName"))
                'Check if the PRSName already exists.
                Dim gclOverallStatus As InspectionStatus = 1
                Dim prsStatus As InspectionStatus = 1
                Dim prsNameCheck As String = prsXmlRecord("PRSName").ToString



                Dim resultPrsSearch = m_EntitiesPrsImport.PresureRegulatorStations.Find(Function(c) c.PRSName = prsNameCheck)
                'MOD 42
                If usePrsStatus = True Then
                    If prsStatusTable Is Nothing Then
                        gclOverallStatus = InspectionStatus.NoInspection
                        prsStatus = InspectionStatus.NoInspection
                    Else
                        gclOverallStatus = OverallGasControlLineStatus(prsTable, prsXmlRecord, prsStatusTable, prsNameCheck)
                        Select Case gclOverallStatus
                            Case InspectionStatus.NoInspection '1
                                prsStatus = InspectionStatus.NoInspection
                            Case InspectionStatus.StartNotCompleted '2
                                prsStatus = InspectionStatus.StartNotCompleted
                            Case Else
                                'MOD 43 'Do not transfer back to inspector if no Inspection procedure is set to PRS
                                If prsXmlRecord("InspectionProcedure") = "" Then
                                    prsStatus = InspectionStatus.Completed
                                Else
                                    prsStatus = GettingStatus(prsStatusTable, prsNameCheck)

                                End If
                        End Select

                    End If
                End If

                If resultPrsSearch Is Nothing Then

                    Dim prsEntityRecord As New Model.Station.Entities.PRSSyncEntity() With { _
                    .FileDate = fileDate.ToUniversalTime, _
                    .PRS_Id = m_prs_id_New, _
                    .PRS_Status = prsStatus, _
                    .GCL_OverallStatus = gclOverallStatus, _
                    .Route = CastToStringOrNothing(prsXmlRecord("Route")), _
                    .PRSCode = CastToStringOrEmpty(prsXmlRecord("PRSCode")), _
                    .PRSName = CastToStringOrEmpty(prsXmlRecord("PRSName")), _
                    .PRSIdentification = CastToStringOrEmpty(prsXmlRecord("PRSIdentification")), _
                    .Information = CastToStringOrEmpty(prsXmlRecord("Information")), _
                    .InspectionProcedure = CastToStringOrEmpty(prsXmlRecord("InspectionProcedure")) _
                    }
                    ExtractPrsObject_FromXmlToEntity(prsTable, prsXmlRecord, prsEntityRecord)
                    m_EntitiesPrsImport.PresureRegulatorStations.Add(prsEntityRecord)
                    'Increase ID
                    m_prs_id_New += 1
                End If

                ExtractGcl_FromXmltoEntity(prsTable, prsXmlRecord, usePrsStatus, prsStatus, prsStatusTable)



                ''Original
                'If result IsNot Nothing Then
                'Else
                '    If usePrsStatus = True Then
                '        If prsStatusTable Is Nothing Then
                '            gclOverallStatus = InspectionStatus.NoInspection
                '            prsStatus = InspectionStatus.NoInspection
                '        Else
                '            gclOverallStatus = OverallGasControlLineStatus(prsTable, prsXmlRecord, prsStatusTable, prsNameCheck)
                '            Select Case gclOverallStatus
                '                Case InspectionStatus.NoInspection '1
                '                    prsStatus = InspectionStatus.NoInspection
                '                Case InspectionStatus.StartNotCompleted '2
                '                    prsStatus = InspectionStatus.StartNotCompleted
                '                Case Else
                '                    prsStatus = GettingStatus(prsStatusTable, prsNameCheck)
                '            End Select

                '        End If
                '    End If

                '    Dim prsEntityRecord As New Model.Station.Entities.PRSSyncEntity() With { _
                '    .FileDate = fileDate.ToUniversalTime, _
                '    .PRS_Id = m_prs_id_New, _
                '    .PRS_Status = prsStatus, _
                '    .GCL_OverallStatus = gclOverallStatus, _
                '    .Route = CastToStringOrNothing(prsXmlRecord("Route")), _
                '    .PRSCode = CastToStringOrEmpty(prsXmlRecord("PRSCode")), _
                '    .PRSName = CastToStringOrEmpty(prsXmlRecord("PRSName")), _
                '    .PRSIdentification = CastToStringOrEmpty(prsXmlRecord("PRSIdentification")), _
                '    .Information = CastToStringOrEmpty(prsXmlRecord("Information")), _
                '    .InspectionProcedure = CastToStringOrEmpty(prsXmlRecord("InspectionProcedure")) _
                '    }
                '    ExtractPrsObject_FromXmlToEntity(prsTable, prsXmlRecord, prsEntityRecord)

                '    ExtractGcl_FromXmltoEntity(prsTable, prsXmlRecord, usePrsStatus, prsStatus, prsStatusTable)

                '    m_EntitiesPrsImport.PresureRegulatorStations.Add(prsEntityRecord)
                '    'Increase ID


            Next
        End Sub


        ''' <summary>
        ''' Extracts the PRS object.
        ''' </summary>
        ''' <param name="prsTable">The PRS table.</param>
        ''' <param name="prsRow">The PRS row.</param>
        Private Sub ExtractPrsObject_FromXmlToEntity(prsTable As DataTable, prsRow As DataRow, prs As Model.Station.Entities.PRSSyncEntity)
            If prsTable.ChildRelations.Contains(PRS_PRSOBJECTS_RELATION_NAME) Then
                Dim childRelation As DataRelation = prsTable.ChildRelations(PRS_PRSOBJECTS_RELATION_NAME)
                Dim prsObjectRows As DataRow() = prsRow.GetChildRows(childRelation)

                For Each prsObjectXmlRecord As DataRow In prsObjectRows
                    Dim prsObjectsEntityRecord As New Model.Station.Entities.PRSSyncObject() With { _
                      .ObjectName = CastToStringOrEmpty(prsObjectXmlRecord("ObjectName")), _
                      .ObjectID = CastToStringOrEmpty(prsObjectXmlRecord("ObjectID")), _
                      .MeasurePoint = CastToStringOrEmpty(prsObjectXmlRecord("MeasurePoint")), _
                      .MeasurePointID = CastToStringOrEmpty(prsObjectXmlRecord("MeasurePointID")), _
                      .FieldNo = CastToStringOrEmpty(prsObjectXmlRecord("FieldNo")) _
                    }
                    prs.PRSObjects.Add(prsObjectsEntityRecord)
                    m_prsObjects_id_New += 1
                Next
            End If
        End Sub


        ''' <summary>
        ''' Extracts the gas control line object.
        ''' </summary>
        ''' <param name="prsTable">The PRS table.</param>
        ''' <param name="prsRow">The PRS row.</param>
        Private Sub ExtractGcl_FromXmltoEntity(prsTable As DataTable, prsRow As DataRow, useGclStatus As Boolean, prsStatus As Integer, Optional ByVal prsStatusTable As DataTable = Nothing) ', ByRef gcl As List(Of Model.Station.Entities.GclSyncEntity))
            If prsTable.ChildRelations.Contains(PRS_GCL_RELATION_NAME) Then
                Dim childRelation As DataRelation = prsTable.ChildRelations(PRS_GCL_RELATION_NAME)
                Dim prsGasControlLines As DataRow() = prsRow.GetChildRows(childRelation)

                For Each gclXmlRecord As DataRow In prsGasControlLines
                    'MOD 42
                    Dim resultGclSearch = m_EntitiesPrsImport.GasControlLine.Find(Function(c) c.PRSName = gclXmlRecord("PRSName") And c.GasControlLineName = gclXmlRecord("GasControlLineName"))
                    If resultGclSearch Is Nothing Then

                        Dim gclStatus As InspectionStatus = 1
                        If useGclStatus = True Then
                            'Check if the inspectionstatus is set
                            If prsStatusTable Is Nothing Then
                                gclStatus = InspectionStatus.NoInspection
                            Else
                                gclStatus = GettingStatus(prsStatusTable, gclXmlRecord("PRSName"), gclXmlRecord("GasControlLineName"))
                            End If
                        End If

                        Dim gclEntityRecord As New Model.Station.Entities.GclSyncEntity() With { _
                           .PRS_Status = prsStatus, _
                           .GCL_Status = gclStatus, _
                          .PRSName = CastToStringOrEmpty(gclXmlRecord("PRSName")), _
                          .PRSIdentification = CastToStringOrEmpty(gclXmlRecord("PRSIdentification")), _
                          .GasControlLineName = CastToStringOrEmpty(gclXmlRecord("GasControlLineName")), _
                          .PeMin = CastToStringOrEmpty(gclXmlRecord("PeMin")), _
                          .PeMax = CastToStringOrEmpty(gclXmlRecord("PeMax")), _
                          .VolumeVA = CastToStringOrEmpty(gclXmlRecord("VolumeVA")), _
                          .VolumeVAK = CastToStringOrEmpty(gclXmlRecord("VolumeVAK")), _
                          .PaRangeDM = CastToTypeRangeDMOrUnset(gclXmlRecord("PaRangeDM").ToString()), _
                          .PeRangeDM = CastToTypeRangeDMOrUnset(gclXmlRecord("PeRangeDM").ToString()), _
                          .GCLIdentification = CastToStringOrEmpty(gclXmlRecord("GCLIdentification")), _
                          .GCLCode = CastToStringOrEmpty(gclXmlRecord("GCLCode")), _
                          .InspectionProcedure = CastToStringOrEmpty(gclXmlRecord("InspectionProcedure")), _
                          .FSDStart = If(CastToIntOrNull(gclXmlRecord("FSDStart").ToString()), -1) _
                        }

                        ExtractGclObjects_FromXmltoEntity(gclXmlRecord, gclEntityRecord)
                        m_EntitiesPrsImport.GasControlLine.Add(gclEntityRecord)
                        m_gcl_id_New += 1
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the gas control line GCL objects.
        ''' </summary>
        ''' <param name="prsGasControlLine">The PRS gas control line.</param>
        ''' <param name="gclObject">The GCL object</param>
        Private Sub ExtractGclObjects_FromXmltoEntity(prsGasControlLine As DataRow, gclObject As Model.Station.Entities.GclSyncEntity)
            If prsGasControlLine.Table.ChildRelations.Contains(GCL_GCLOBJECTS_RELATION_NAME) Then
                Dim subRelation As DataRelation = prsGasControlLine.Table.ChildRelations(GCL_GCLOBJECTS_RELATION_NAME)
                Dim gclObjects As DataRow() = prsGasControlLine.GetChildRows(subRelation)

                gclObject.GCLObjects = New List(Of Model.Station.Entities.GCLObject)

                For Each gclObjectXmlRecord As DataRow In gclObjects
                    Dim gclObjectEntityRecord As New Model.Station.Entities.GCLObject() With { _
                      .ObjectName = CastToStringOrEmpty(gclObjectXmlRecord("ObjectName")), _
                      .ObjectID = CastToStringOrEmpty(gclObjectXmlRecord("ObjectID")), _
                      .MeasurePoint = CastToStringOrEmpty(gclObjectXmlRecord("MeasurePoint")), _
                      .MeasurePointID = CastToStringOrEmpty(gclObjectXmlRecord("MeasurePointID")), _
                      .FieldNo = CastToStringOrEmpty(gclObjectXmlRecord("FieldNo")) _
                    }

                    ExtractGclObjectsBoundaries_FromXmltoEntity(gclObjectXmlRecord, gclObjectEntityRecord)
                    gclObject.GCLObjects.Add(gclObjectEntityRecord)
                    m_gclObjects_id_New += 1
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the GCL object boundaries object.
        ''' </summary>
        ''' <param name="gclObject">The GCL object.</param>
        ''' <param name="gclObjectEntityRecord">The type object id.</param>
        Private Sub ExtractGclObjectsBoundaries_FromXmltoEntity(gclObject As DataRow, gclObjectEntityRecord As Model.Station.Entities.GCLObject)
            If gclObject.Table.ChildRelations.Contains(GCLOBJECTS_BOUNDARIES_RELATION_NAME) Then
                Dim subSubRelation As DataRelation = gclObject.Table.ChildRelations(GCLOBJECTS_BOUNDARIES_RELATION_NAME)
                Dim gclObjectBoundaries As DataRow() = gclObject.GetChildRows(subSubRelation)

                If gclObjectBoundaries.Length > 0 Then
                    Dim gclObjectBoundaryXmlRecord As DataRow = gclObjectBoundaries(0)
                    gclObjectEntityRecord.Boundaries = New Model.Station.Entities.TypeSyncObjectIDBoundaries() With { _
                      .ValueMax = CastToDoubleOrNan(gclObjectBoundaryXmlRecord("ValueMax").ToString()), _
                      .ValueMin = CastToDoubleOrNan(gclObjectBoundaryXmlRecord("ValueMin").ToString()), _
                      .UOV = CastToTypeUnitsValueOrUnset(gclObjectBoundaryXmlRecord("UOV").ToString()) _
                    }
                End If
            End If
        End Sub
#End Region

        Private Function GettingStatus(prsStatusTable As DataTable, ByVal prsName As String, Optional gclName As String = "") As InspectionStatus
            Dim status As InspectionStatus
            Dim foundRowsPrsStatus() As DataRow
            If gclName = "" Then
                foundRowsPrsStatus = prsStatusTable.Select("PRSName = '" & prsName & "' and GasControlLineName is NULL")
            Else
                foundRowsPrsStatus = prsStatusTable.Select("PRSName = '" & prsName & "' and GasControlLineName = '" & gclName & "'")
            End If
            If foundRowsPrsStatus.Count > 0 Then
                status = foundRowsPrsStatus.First.Item("StatusID")
            Else
                'MOD 28
                status = InspectionStatus.NoInspection
            End If
            Return status
        End Function

        ''' <summary>
        ''' Determine the overall status of the GCL
        ''' Set to default to "Unset"
        ''' </summary>
        ''' <param name="prsStatusTable"></param>
        ''' <param name="prsName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function OverallGasControlLineStatus(prsTable As DataTable, prsRow As DataRow, prsStatusTable As DataTable, ByVal prsName As String) As InspectionStatus
            Dim overallGasControlStatus As InspectionStatus

            'Set the state default to "Unset" 
            overallGasControlStatus = InspectionStatus.Unset

            'MOD 28

            Dim amountGclRecords As Integer = 0
            Dim amountGclStatusRecords As Integer = 0

            'Get the amount of GCL records in the PRS station file
            If prsTable.ChildRelations.Contains(PRS_GCL_RELATION_NAME) Then
                Dim childRelation As DataRelation = prsTable.ChildRelations(PRS_GCL_RELATION_NAME)
                Dim prsGasControlLines As DataRow() = prsRow.GetChildRows(childRelation)
                amountGclRecords = prsGasControlLines.Count
            End If

            'Execute only if the PRS file contains GCL of the selected PRS
            If amountGclRecords > 0 Then
                'Check the status of all GCL for the selected PRS 
                Dim foundRowsGCLStatus() As DataRow
                foundRowsGCLStatus = prsStatusTable.Select("PRSName = '" & prsName & "' and GasControlLineName is NOT NULL") '"' and GasControlLineName is NULL")

                If foundRowsGCLStatus.Count > 0 Then
                    'Set the status of the first record
                    'MOD 28
                    For Each foundRecord In foundRowsGCLStatus
                        Dim foundStatus As InspectionStatus = foundRecord.Item("StatusID")
                        overallGasControlStatus = ApplyStatus(foundStatus, overallGasControlStatus)
                        amountGclStatusRecords += 1
                    Next
                End If
            End If

            'Mod 28 if the amount of GCL records is not the same as the amount of GCL status records set the GCL overall status to NoInspection
            If amountGclRecords <> amountGclStatusRecords Then overallGasControlStatus = InspectionStatus.NoInspection
            Return overallGasControlStatus

        End Function
        ''' <summary>
        ''' Determine which state should be applied. 
        ''' This state is used to leave the data on INSPECTOR
        ''' </summary>
        ''' <param name="getStatus"></param>
        ''' <param name="currentOverallStatus"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ApplyStatus(getStatus As InspectionStatus, currentOverallStatus As InspectionStatus) As InspectionStatus
            Select Case getStatus
                Case InspectionStatus.Unset '0
                Case InspectionStatus.NoInspection '1
                    Return InspectionStatus.NoInspection
                Case InspectionStatus.StartNotCompleted '2
                    Return InspectionStatus.StartNotCompleted
                Case InspectionStatus.Completed '3
                Case InspectionStatus.GclOrPrsDeletedByUser '4
                Case InspectionStatus.CompletedValueOutOfLimits '5
                Case InspectionStatus.NoInspectionFound '6
                Case InspectionStatus.Warning '7
            End Select
            Return currentOverallStatus
        End Function

    End Class
End Namespace



