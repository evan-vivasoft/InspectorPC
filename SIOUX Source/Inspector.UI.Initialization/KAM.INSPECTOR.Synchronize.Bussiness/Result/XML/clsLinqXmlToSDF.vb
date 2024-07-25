'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'Gevalideerd 17-7-2015
'===============================================================================
'MOD 31

Imports System.Data
Imports System.Globalization
Imports System.IO
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Result
    ''' <summary>
    ''' Class handling the export from XML to MsAccess
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinqXMLToSDF
#Region "Constants"
        Private Const InspectionResult_TABLE_NAME As String = "InspectionResult"
        Private Const GCL_TABLE_NAME As String = "GasControlLine"
#End Region
#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_InspectionResult_id_New As Integer = 0
        Private m_Result_id_New As Integer = 0
        Private m_List_id_New As Integer = 0

        Private recordcounterProcessed As Integer = 0
        Private recordcounterTotal As Integer = 0

        Private m_sdfResultImport As Resultssql.DataAccess.Results
        Private m_EntitiesResultExport As New Model.Result.Entities.InspectionResultsDataEntity
#End Region

#Region "Constructors"
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="filenNameSqlDatabase"></param>
        ''' <remarks></remarks>
        Public Sub New(filenNameSqlDatabase As String)
            CreateNewfile(filenNameSqlDatabase)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Create a new sdf station information file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateNewfile(ByVal sqlDbImportFileName As String) As Boolean
            'Reset all counters
            ClearCounters()
            m_sdfResultImport = New Resultssql.DataAccess.Results(sqlDbImportFileName)

            'Create a new database
            If m_sdfResultImport.DatabaseExists Then
                Try
                    m_sdfResultImport.DeleteDatabase()
                Catch ex As Exception
                    ' Stop, retourneer foutmelding
                    Debug.Print(ex.Message)
                    Return False
                End Try
            End If
            m_sdfResultImport.CreateDatabase()
            Return True
        End Function

        ''' <summary>
        ''' Import the results into the database
        ''' </summary>
        ''' <param name="xmlFile"></param>
        ''' <param name="xsdFile"></param>
        ''' <remarks></remarks>
        Public Function LoadResults(ByVal xmlFile As String, ByVal xsdFile As String) As Boolean
            'The XML file handling
            If File.Exists(xmlFile) = False Then
                xmlFile = Nothing
            End If

            If xmlFile <> Nothing Then
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
            End If

            Dim xmlDataset As DataSet
            Dim ioFileWriteTime As DateTime
            ReadInspectionResults(xmlFile, xsdFile, xmlDataset, ioFileWriteTime)
            ExtractInspectionResults_FromXmlToSdf(xmlDataset)

            Return True

        End Function


        ''' <summary>
        ''' Save the results into the database
        ''' </summary>
        ''' <remarks></remarks>
        Public Function WriteResults() As Boolean
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedWrite, "!Saving database")

            'Create the SDF database
            Try
                m_sdfResultImport.SubmitChanges()
                m_sdfResultImport.Connection.Close()
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, 0, 0, DbCreateStatus.SuccesWrite, "!Saving data completed")
            Catch ex As Exception
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, 0, 0, DbCreateStatus.ErrorWrite, ex.Message)
                Return False
            End Try
            Return True

        End Function


#End Region


#Region "Helpers"
        ''' <summary>
        ''' Clear all counters
        ''' </summary>
        ''' <remarks></remarks>
        Private Function ClearCounters() As Boolean
            m_InspectionResult_id_New = 0
            m_Result_id_New = 0
            m_List_id_New = 0

            recordcounterProcessed = 0
            recordcounterTotal = 0
            Return True
        End Function
#End Region

#Region "Handling of Inspecton results results"
        ''' <summary>
        ''' Extracts the InspectionResults objects.
        ''' </summary>
        ''' <param name="xmlDataset"></param>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_FromXmlToSdf(ByVal xmlDataset As DataSet)
            Dim resultsTable As DataTable = xmlDataset.Tables(InspectionResult_TABLE_NAME)

            recordcounterProcessed = 0
            recordcounterTotal = resultsTable.Rows.Count
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, "")

            ' Doorloop nu alle records-rows in de tabel
            For Each ipXmlResultsRecord In resultsTable.Rows 'DataRow In prsTable.Rows
                recordcounterProcessed += 1
                Dim inspectonResultSqlRecord As New Resultssql.DataAccess.InspectionResult With { _
                    .InspectionResult_Id = m_InspectionResult_id_New, _
                    .Status = 3, _
                    .PRSIdentification = ipXmlResultsRecord("PRSIdentification").ToString(), _
                    .PRSName = ipXmlResultsRecord("PRSName").ToString(), _
                    .PRSCode = ipXmlResultsRecord("PRSCode").ToString() _
                }

                'Check if GasControlLineName contains any name.
                If ipXmlResultsRecord("GasControlLineName").ToString() <> "" Then
                    inspectonResultSqlRecord.GasControlLineName = ipXmlResultsRecord("GasControlLineName").ToString()
                    inspectonResultSqlRecord.GCLIdentification = ipXmlResultsRecord("GCLIdentification").ToString()
                    inspectonResultSqlRecord.GCLCode = ipXmlResultsRecord("GCLCode").ToString()
                End If

                'Getting the inspectionprocedure infomartion
                Dim foundRowsInspectionProcedure() As DataRow
                foundRowsInspectionProcedure = xmlDataset.Tables("InspectionProcedure").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsInspectionProcedure.Length > 0 Then
                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    inspectonResultSqlRecord.InspectionProcedureName = foundRowsInspectionProcedure(0).Item("Name")
                    inspectonResultSqlRecord.InspectionProcedureVersion = foundRowsInspectionProcedure(0).Item("Version")
                Else
                    inspectonResultSqlRecord.InspectionProcedureName = ""
                    inspectonResultSqlRecord.InspectionProcedureVersion = ""
                End If

                'Getting the measurement equipment
                Dim foundRowsMeasurementEquipment() As DataRow
                foundRowsMeasurementEquipment = xmlDataset.Tables("Measurement_Equipment").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                'Optional XML element
                If foundRowsMeasurementEquipment.Length > 0 Then
                    'Read the Measurement_Equipment for this InspectionResult; Always only one
                    inspectonResultSqlRecord.ID_DM1 = foundRowsMeasurementEquipment(0).Item("ID_DM1")
                    inspectonResultSqlRecord.ID_DM2 = foundRowsMeasurementEquipment(0).Item("ID_DM2")
                    inspectonResultSqlRecord.BT_Address = foundRowsMeasurementEquipment(0).Item("BT_Address")
                End If

                'Getting the date time stemp
                Dim foundRowsDateTimeStamp() As DataRow
                foundRowsDateTimeStamp = xmlDataset.Tables("DateTimeStamp").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsDateTimeStamp.Length > 0 Then
                    'Set the dateTimeStampl Only one
                    inspectonResultSqlRecord.StartTime = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate")) & " " & CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("StartTime"))
                    inspectonResultSqlRecord.EndTime = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate")) & " " & CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("EndTime"))

                    Dim foundRowsTimeSettings() As DataRow
                    foundRowsTimeSettings = xmlDataset.Tables("TimeSettings").Select("DateTimeStamp_ID=" & foundRowsDateTimeStamp(0).Item("DateTimeStamp_ID"))
                    inspectonResultSqlRecord.TimeZone = CastToTypeTimeZoneValueOrDefault(foundRowsTimeSettings(0).Item("TimeZone"))
                    inspectonResultSqlRecord.DST = CastToTypeDstValueOrDefault(foundRowsTimeSettings(0).Item("DST"))
                End If

                ' Zoek nu de resultaten van deze InspectionResult erbij en voeg deze toe aan de Results Entiteit van InspectionResult
                ExtractResults_FromXmlToSdf(xmlDataset, ipXmlResultsRecord("InspectionResult_ID").ToString())
                ' Wijs het resultaat nu toe aan prsEntityResults list

                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, inspectonResultSqlRecord.PRSName)
                m_sdfResultImport.InspectionResults.InsertOnSubmit(inspectonResultSqlRecord)

                ' Increase ID
                m_InspectionResult_id_New += 1
            Next
        End Sub

#End Region
#Region "Handling of results"
        ''' <summary>
        ''' Extracts the Results object.
        ''' </summary>
        ''' <param name="xmlDataset"></param>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_FromXmlToSdf(ByVal xmlDataset As DataSet, ByVal liID_InspectionResults_ID As Integer)

            'Create a recordset with all the results of InspectionResult
            Dim foundRowsResult() As DataRow
            foundRowsResult = xmlDataset.Tables("Result").Select("InspectionResult_ID=" & liID_InspectionResults_ID)

            'Is something is found; extract the data
            If foundRowsResult.Length > 0 Then

                For liLoop = 0 To foundRowsResult.Length - 1

                    Dim resultSqlRecord As New Resultssql.DataAccess.Result With { _
                        .Result_Id = m_Result_id_New, _
                        .InspectionResultLinkID = m_InspectionResult_id_New, _
                        .ObjectName = foundRowsResult(liLoop).Item("ObjectName").ToString(), _
                        .ObjectID = foundRowsResult(liLoop).Item("ObjectID").ToString(), _
                        .MeasurePoint = foundRowsResult(liLoop).Item("MeasurePoint").ToString(), _
                        .MeasurePointID = foundRowsResult(liLoop).Item("MeasurePointID").ToString(), _
                        .FieldNo = CastToStringOrEmpty(foundRowsResult(liLoop).Item("FieldNo")), _
                        .Time = CastToTypeTimeValueOrDefault(foundRowsResult(liLoop).Item("Time").ToString()) _
                    }

                    'Search in measurevalue.
                    Dim foundRowsMeasureValue() As DataRow
                    foundRowsMeasureValue = xmlDataset.Tables("MeasureValue").Select("Result_Id=" & foundRowsResult(liLoop).Item("Result_Id"))
                    If foundRowsMeasureValue.Length > 0 Then
                        resultSqlRecord.UOM = foundRowsMeasureValue(0).Item("UOM")
                        resultSqlRecord.Value = foundRowsMeasureValue(0).Item("Value")
                    End If

                    ' Maak een recordset met alleen de LIST resultaten voor deze Result
                    Dim foundRowsList() As DataRow
                    foundRowsList = xmlDataset.Tables("List").Select("Result_ID=" & foundRowsResult(liLoop).Item("Result_Id"))

                    ' indien er wat gevonden is verwerken
                    If foundRowsList.Length > 0 Then
                        For ljLoop As Integer = 0 To foundRowsList.Length - 1
                            Dim resultListSqlRecord As New Resultssql.DataAccess.List With { _
                                .List_Id = m_List_id_New, _
                                .ResultLinkID = m_Result_id_New, _
                                .List_Column = foundRowsMeasureValue(0).Item("List") _
                            }

                            m_sdfResultImport.Lists.InsertOnSubmit(resultListSqlRecord)
                            m_List_id_New += 1
                        Next
                    End If

                    If Not IsDBNull(foundRowsResult(liLoop).Item("Text")) Then resultSqlRecord.Text = foundRowsResult(liLoop).Item("Text").ToString()

                    m_sdfResultImport.Results1.InsertOnSubmit(resultSqlRecord)
                    m_Result_id_New += 1
                Next
            End If
        End Sub
#End Region
    End Class
End Namespace




