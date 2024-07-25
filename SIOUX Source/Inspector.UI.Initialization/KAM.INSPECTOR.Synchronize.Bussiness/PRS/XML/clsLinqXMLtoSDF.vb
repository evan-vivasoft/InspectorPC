'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'Validated 16-7-2015
'===============================================================================
'MOD 31

Imports System.Data
Imports System.Globalization
Imports DynamicCondition

Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.Entities.PrsConstants
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral


Namespace Model.Station
    ''' <summary>
    ''' Information of PRS and contained GasControlLines
    ''' </summary>
    ''' <remarks>Options:</remarks>
    ''' Do not delete the not inspected PRS or GCL on INSPECTOR
    Public Class clsLinqXMLToSDF
#Region "Constants"
#End Region

#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private sqlDbExport As PRSsql.DataAccess.StationInformation

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
        ''' <param name="sdfFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal sdfFile As String)
            CreateNewfile(sdfFile)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Load a sdf file for adding the XML record
        ''' </summary>
        ''' <param name="sqlDbINspectorFileName">SDF file from INSPECTOR  to read</param>
        ''' <remarks></remarks>
        Public Function LoadStationInformation(ByVal sqlDbINspectorFileName As String) As Boolean
            'Reset all counters
            ClearStationInformation()
            ' Reading the information from the sdf database
            sqlDbExport = New PRSsql.DataAccess.StationInformation(sqlDbINspectorFileName)
            Return True
        End Function

        ''' <summary>
        ''' Create a new sdf station information file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateNewfile(ByVal sqlDbImportFileName As String) As Boolean
            'Reset all counters
            ClearStationInformation()
            sqlDbExport = New PRSsql.DataAccess.StationInformation(sqlDbImportFileName)

            'Create a new database
            If sqlDbExport.DatabaseExists Then
                Try
                    sqlDbExport.DeleteDatabase()
                Catch ex As Exception
                    ' Stop, retourneer foutmelding
                    Debug.Print(ex.Message)
                    Return False
                End Try
            End If
            sqlDbExport.CreateDatabase()
            Return True
        End Function

        ''' <summary>
        ''' Clear the PRS data 
        ''' </summary>
        ''' <remarks></remarks>
        Public Function ClearStationInformation() As Boolean
            m_prs_id_New = 0
            m_prsObjects_id_New = 0
            m_gcl_id_New = 0
            m_gclObjects_id_New = 0
            Return True
        End Function

        ''' <summary>
        ''' Write the entity into a XML file
        ''' </summary>
        ''' <param name="selections">List (; separated) of the selections; Used for the PRS status</param>
        ''' <remarks></remarks>
        Public Function WriteStationInformation(ByVal sqlDbInspectorFileName As String, Optional selections As String = "") As Boolean
            'MOD 28
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, DbCreateStatus.StartedWrite, "")
            Dim prsSelectionConditionList

            Dim recordCounterTotal As Integer = 0

            If CheckFileExistsPC(sqlDbInspectorFileName) Then

                Dim sqlDbInspector As PRSsql.DataAccess.StationInformation
                sqlDbInspector = New PRSsql.DataAccess.StationInformation(sqlDbInspectorFileName)


                'Check if there is a selection criteria
                If selections <> "" Then
                    Dim selectionsSplit As String() = selections.Split(";") 'Split the string

                    Dim conditionArrayPrs() As Condition(Of PRSsql.DataAccess.PRS)
                    Dim conditionsArrayPrsGcl() As Condition(Of PRSsql.DataAccess.PRS)

                    Dim conditionFirstGcl As Condition(Of PRSsql.DataAccess.GasControlLine)
                    Dim conditionsArrayGcl() As Condition(Of PRSsql.DataAccess.GasControlLine)

                    ReDim Preserve conditionArrayPrs(-1)
                    ReDim Preserve conditionsArrayPrsGcl(-1)
                    ReDim Preserve conditionsArrayGcl(-1)

                    Dim i As Integer = -1
                    'split up the string and assign the criteria
                    For Each s As String In selectionsSplit
                        i += 1
                        Dim ints As Integer
                        ints = CastToIntOrNull(s)

                        'Condition for PRS only
                        ReDim Preserve conditionArrayPrs(conditionArrayPrs.Length)
                        conditionArrayPrs(conditionArrayPrs.Length - 1) = sqlDbExport.PRS.CreateCondition("StatusPRS", DynamicQuery.Condition.Compare.Equal, ints)

                        'Condition for PRS and GCL
                        ReDim Preserve conditionsArrayPrsGcl(conditionsArrayPrsGcl.Length)
                        conditionsArrayPrsGcl(conditionsArrayPrsGcl.Length - 1) = sqlDbExport.PRS.CreateCondition("StatusGCL", DynamicQuery.Condition.Compare.Equal, ints)

                        'Condition for GCL only
                        ReDim Preserve conditionsArrayGcl(conditionsArrayGcl.Length)
                        conditionsArrayGcl(conditionsArrayPrsGcl.Length - 1) = sqlDbExport.GasControlLines.CreateCondition("StatusGCL", DynamicQuery.Condition.Compare.Equal, ints)
                        'End If
                    Next
                    Dim conditionPrs As DynamicCondition.Condition(Of PRSsql.DataAccess.PRS)
                    Dim conditionGcl As DynamicCondition.Condition(Of PRSsql.DataAccess.GasControlLine)

                    conditionGcl = conditionFirstGcl


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
                    Next

                    prsSelectionConditionList = sqlDbInspector.PRS.Where(conditionPrs)
                    recordCounterTotal = sqlDbInspector.PRS.Where(conditionPrs).Count
                Else
                    'Select everything
                    prsSelectionConditionList = sqlDbInspector.PRS

                    recordCounterTotal = sqlDbInspector.PRS.Count
                End If


                'Export the data to a new database.
                Dim recordCounterProcessed As Integer = 0

                For Each prsSelectionCondition As PRSsql.DataAccess.PRS In prsSelectionConditionList
                    recordCounterProcessed += 1
                    RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, recordCounterTotal, recordCounterProcessed, DbCreateStatus.StartedCreate, prsSelectionCondition.PRSName.ToString)

                    'Check if the PRS already exists in the SDF database.
                    Dim prsNameCheck As String = prsSelectionCondition.PRSName.ToString
                    Dim result = sqlDbExport.PRS.Where(Function(c) c.PRSName = prsNameCheck)
                    If result.Count = 0 Then

                        'Inject PRS
                        Dim prsSqlRecord As New PRSsql.DataAccess.PRS With {
                            .PRS_Id = m_prs_id_New, _
                            .PRSCode = prsSelectionCondition.PRSCode, _
                            .PRSName = prsSelectionCondition.PRSName, _
                            .PRSIdentification = prsSelectionCondition.PRSIdentification, _
                            .Route = prsSelectionCondition.Route, _
                            .Information = prsSelectionCondition.Information, _
                            .InspectionProcedure = prsSelectionCondition.InspectionProcedure, _
                            .StatusPRS = prsSelectionCondition.StatusPRS, _
                            .StatusGCL = prsSelectionCondition.StatusGCL
                          }

                        'Inject PRS objects
                        For Each prsObjectsSelectionCondition As PRSsql.DataAccess.PRSObject In prsSelectionCondition.PRSObjects
                            Dim prsObjectSdfRecord As New PRSsql.DataAccess.PRSObject() With { _
                              .PRSObjects_Id = m_prsObjects_id_New, _
                              .PRSLinkID = m_prs_id_New, _
                              .ObjectName = prsObjectsSelectionCondition.ObjectName, _
                              .ObjectID = prsObjectsSelectionCondition.ObjectID, _
                              .MeasurePoint = prsObjectsSelectionCondition.MeasurePoint, _
                              .MeasurePointID = prsObjectsSelectionCondition.MeasurePointID, _
                              .FieldNo = prsObjectsSelectionCondition.FieldNo _
                            }
                            prsSqlRecord.PRSObjects.Add(prsObjectSdfRecord)
                            m_prsObjects_id_New += 1
                        Next

                        'Inject GCL
                        For Each gclSelectionCondition As PRSsql.DataAccess.GasControlLine In prsSelectionCondition.GasControlLines
                            Dim gclSqlRecord As New PRSsql.DataAccess.GasControlLine() With { _
                              .GasControlLine_Id = m_gcl_id_New, _
                              .PRSLinkID = m_prsObjects_id_New, _
                              .GasControlLineName = gclSelectionCondition.GasControlLineName, _
                              .PeMin = gclSelectionCondition.PeMin, _
                              .PeMax = gclSelectionCondition.PeMax, _
                              .VolumeVA = gclSelectionCondition.VolumeVA, _
                              .VolumeVAK = gclSelectionCondition.VolumeVAK, _
                              .PaRangeDM = gclSelectionCondition.PaRangeDM, _
                              .PeRangeDM = gclSelectionCondition.PeRangeDM, _
                              .GCLIdentification = gclSelectionCondition.GCLIdentification, _
                              .GCLCode = gclSelectionCondition.GCLCode, _
                              .InspectionProcedure = gclSelectionCondition.InspectionProcedure, _
                              .FSDStart = gclSelectionCondition.FSDStart, _
                              .StatusGCL = 1 _
                            }
                            'Inject GCL objects
                            For Each gclObjectSelectionCondition As PRSsql.DataAccess.GCLObject In gclSelectionCondition.GCLObjects
                                Dim gclObjectGclRecord As New PRSsql.DataAccess.GCLObject With { _
                                  .GCLObjects_Id = m_gclObjects_id_New, _
                                  .GCLLinkID = m_gcl_id_New, _
                                  .ObjectName = gclObjectSelectionCondition.ObjectName, _
                                  .ObjectID = gclObjectSelectionCondition.ObjectID, _
                                  .MeasurePoint = gclObjectSelectionCondition.MeasurePoint, _
                                  .MeasurePointID = gclObjectSelectionCondition.MeasurePointID, _
                                  .FieldNo = gclObjectSelectionCondition.FieldNo, _
                                  .ValueMax = gclObjectSelectionCondition.ValueMax, _
                                  .ValueMin = gclObjectSelectionCondition.ValueMin, _
                                  .UOV = gclObjectSelectionCondition.UOV
                                }

                                gclSqlRecord.GCLObjects.Add(gclObjectGclRecord)
                                m_gclObjects_id_New += 1
                            Next

                            prsSqlRecord.GasControlLines.Add(gclSqlRecord)
                            m_gcl_id_New += 1
                        Next
                        'Increase ID
                        m_prs_id_New += 1

                        sqlDbExport.PRS.InsertOnSubmit(prsSqlRecord)
                    End If
                Next
            End If

            'Save the database
            Try
                sqlDbExport.SubmitChanges()
                sqlDbExport.Connection.Close()
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, DbCreateStatus.SuccesWrite, "!Saving data completed")
            Catch ex As Exception
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, 0, 0, ex.Message, "")
                Return False
            End Try
            Return True

        End Function
#End Region

#Region "Dataset parse"
        ''' <summary>
        ''' Reads the station information into Model.Station.Entities
        ''' </summary>
        ''' <param name="xmlFilePrs">Xml file with PRS data to read</param>
        ''' <param name="xsdFilePrs">XSD file with PRS structure</param>
        ''' <remarks></remarks>
        Public Function ReadStationInformation(ByVal xmlFilePrs As String, ByVal xsdFilePrs As String) As Boolean
            Using dataSetPrs As New DataSet()
                'Set the XML prs File
                dataSetPrs.Locale = CultureInfo.InvariantCulture
                dataSetPrs.ReadXmlSchema(xsdFilePrs)
                dataSetPrs.ReadXml(xmlFilePrs, XmlReadMode.ReadSchema)
                InjectPrsToGasControlLineRelation(dataSetPrs)

                Dim prsTable As DataTable = dataSetPrs.Tables(PRS_TABLE_NAME)


                'Extracting the data
                ExtractPrs_FromXmlToSdf(prsTable)

                sqlDbExport.SubmitChanges()
                Return True
            End Using
        End Function

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
        Private Sub ExtractPrs_FromXmlToSdf(ByVal prsTable As DataTable)
            Dim recordCounterProcessed As Integer = 0
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, prsTable.Rows.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, "")
            For Each prsXmlRecord As DataRow In prsTable.Rows
                recordCounterProcessed += 1
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, prsTable.Rows.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, prsXmlRecord("PRSName"))

                'Check if the PRS already exists in the SDF database.
                Dim prsNameCheck As String = prsXmlRecord("PRSName").ToString
                Dim result = sqlDbExport.PRS.Where(Function(c) c.PRSName = prsNameCheck)
                If result.Count <> 0 Then Continue For


                Dim prsSqlRecord As New PRSsql.DataAccess.PRS With { _
                .PRS_Id = m_prs_id_New, _
                .PRSCode = CastToStringOrEmpty(prsXmlRecord("PRSCode")), _
                .PRSName = CastToStringOrEmpty(prsXmlRecord("PRSName")), _
                .PRSIdentification = CastToStringOrEmpty(prsXmlRecord("PRSIdentification")), _
                .Route = CastToStringOrNothing(prsXmlRecord("Route")), _
                .Information = CastToStringOrEmpty(prsXmlRecord("Information")), _
                .InspectionProcedure = CastToStringOrEmpty(prsXmlRecord("InspectionProcedure")), _
                .StatusPRS = 1, _
                .StatusGCL = 1
                }

                ExtractPrsObject_FromXmlToSdf(prsTable, prsXmlRecord, prsSqlRecord)
                ExtractGcl_FromXmltoSdf(prsTable, prsXmlRecord, prsSqlRecord)

                sqlDbExport.PRS.InsertOnSubmit(prsSqlRecord)

                'Increase ID
                m_prs_id_New += 1
            Next
        End Sub


        ''' <summary>
        ''' Extracts the PRS object.
        ''' </summary>
        ''' <param name="prsTable">The PRS table.</param>
        ''' <param name="prsRow">The PRS row.</param>
        ''' <param name="prs">the SQL PRS object.</param>
        Private Sub ExtractPrsObject_FromXmlToSdf(prsTable As DataTable, prsRow As DataRow, ByRef prs As PRSsql.DataAccess.PRS)
            If prsTable.ChildRelations.Contains(PRS_PRSOBJECTS_RELATION_NAME) Then
                Dim childRelation As DataRelation = prsTable.ChildRelations(PRS_PRSOBJECTS_RELATION_NAME)
                Dim prsObjectRows As DataRow() = prsRow.GetChildRows(childRelation)

                For Each prsObjectXmlRecord As DataRow In prsObjectRows
                    Dim prsObjectSdfRecord As New PRSsql.DataAccess.PRSObject() With { _
                      .PRSObjects_Id = m_prsObjects_id_New, _
                      .PRSLinkID = m_prs_id_New, _
                      .ObjectName = CastToStringOrEmpty(prsObjectXmlRecord("ObjectName")), _
                      .ObjectID = CastToStringOrEmpty(prsObjectXmlRecord("ObjectID")), _
                      .MeasurePoint = CastToStringOrEmpty(prsObjectXmlRecord("MeasurePoint")), _
                      .MeasurePointID = CastToStringOrEmpty(prsObjectXmlRecord("MeasurePointID")), _
                      .FieldNo = CastToStringOrEmpty(prsObjectXmlRecord("FieldNo")) _
                    }
                    prs.PRSObjects.Add(prsObjectSdfRecord)
                    m_prsObjects_id_New += 1
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the gas control line object.
        ''' </summary>
        ''' <param name="prsTable">The PRS table.</param>
        ''' <param name="prsRow">The PRS row.</param>
        Private Sub ExtractGcl_FromXmltoSdf(prsTable As DataTable, prsRow As DataRow, ByRef prs As PRSsql.DataAccess.PRS)
            If prsTable.ChildRelations.Contains(PRS_GCL_RELATION_NAME) Then
                Dim childRelation As DataRelation = prsTable.ChildRelations(PRS_GCL_RELATION_NAME)
                Dim prsGasControlLines As DataRow() = prsRow.GetChildRows(childRelation)

                For Each gclXmlRecord As DataRow In prsGasControlLines
                    Dim gclSqlRecord As New PRSsql.DataAccess.GasControlLine() With { _
                      .GasControlLine_Id = m_gcl_id_New, _
                      .PRSLinkID = m_prsObjects_id_New, _
                      .GasControlLineName = CastToStringOrEmpty(gclXmlRecord("GasControlLineName")), _
                      .PeMin = CastToStringOrEmpty(gclXmlRecord("PeMin")), _
                      .PeMax = CastToStringOrEmpty(gclXmlRecord("PeMax")), _
                      .VolumeVA = CastToStringOrEmpty(gclXmlRecord("VolumeVA")), _
                      .VolumeVAK = CastToStringOrEmpty(gclXmlRecord("VolumeVAK")), _
                      .PaRangeDM = gclXmlRecord("PaRangeDM").ToString(), _
                      .PeRangeDM = gclXmlRecord("PeRangeDM").ToString(), _
                      .GCLIdentification = CastToStringOrEmpty(gclXmlRecord("GCLIdentification")), _
                      .GCLCode = CastToStringOrEmpty(gclXmlRecord("GCLCode")), _
                      .InspectionProcedure = CastToStringOrEmpty(gclXmlRecord("InspectionProcedure")), _
                      .FSDStart = If(CastToIntOrNull(gclXmlRecord("FSDStart").ToString()), -1), _
                      .StatusGCL = 1 _
                    }

                    ExtractGclObjects_FromXmltoSdf(gclXmlRecord, gclSqlRecord)
                    prs.GasControlLines.Add(gclSqlRecord)
                    m_gcl_id_New += 1
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the gas control line GCL objects.
        ''' </summary>
        ''' <param name="prsGasControlLine">The PRS gas control line.</param>
        ''' <param name="gcl">The Sql GCL object</param>
        Private Sub ExtractGclObjects_FromXmltoSdf(prsGasControlLine As DataRow, ByRef gcl As PRSsql.DataAccess.GasControlLine)
            If prsGasControlLine.Table.ChildRelations.Contains(GCL_GCLOBJECTS_RELATION_NAME) Then
                Dim subRelation As DataRelation = prsGasControlLine.Table.ChildRelations(GCL_GCLOBJECTS_RELATION_NAME)
                Dim gclObjects As DataRow() = prsGasControlLine.GetChildRows(subRelation)

                For Each gclObjectXmlRecord As DataRow In gclObjects
                    Dim gclObjectGclRecord As New PRSsql.DataAccess.GCLObject With { _
                      .GCLObjects_Id = m_gclObjects_id_New, _
                      .GCLLinkID = m_gcl_id_New, _
                      .ObjectName = CastToStringOrEmpty(gclObjectXmlRecord("ObjectName")), _
                      .ObjectID = CastToStringOrEmpty(gclObjectXmlRecord("ObjectID")), _
                      .MeasurePoint = CastToStringOrEmpty(gclObjectXmlRecord("MeasurePoint")), _
                      .MeasurePointID = CastToStringOrEmpty(gclObjectXmlRecord("MeasurePointID")), _
                      .FieldNo = CastToStringOrEmpty(gclObjectXmlRecord("FieldNo")) _
                    }

                    If gclObjectXmlRecord.Table.ChildRelations.Contains(GCLOBJECTS_BOUNDARIES_RELATION_NAME) Then
                        Dim subSubRelation As DataRelation = gclObjectXmlRecord.Table.ChildRelations(GCLOBJECTS_BOUNDARIES_RELATION_NAME)
                        Dim gclObjectBoundaries As DataRow() = gclObjectXmlRecord.GetChildRows(subSubRelation)

                        If gclObjectBoundaries.Length > 0 Then
                            Dim gclObjectBoundaryXmlRecord As DataRow = gclObjectBoundaries(0)
                            gclObjectGclRecord.ValueMax = CastToDoubleOrNan(gclObjectBoundaryXmlRecord("ValueMax").ToString())
                            gclObjectGclRecord.ValueMin = CastToDoubleOrNan(gclObjectBoundaryXmlRecord("ValueMin").ToString())
                            gclObjectGclRecord.UOV = gclObjectBoundaryXmlRecord("UOV").ToString()
                        End If
                    End If

                    gcl.GCLObjects.Add(gclObjectGclRecord)
                    m_gclObjects_id_New += 1
                Next
            End If
        End Sub
#End Region

    End Class
End Namespace



