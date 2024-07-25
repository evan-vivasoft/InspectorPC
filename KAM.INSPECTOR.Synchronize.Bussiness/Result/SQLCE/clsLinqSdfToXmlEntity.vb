'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'Gevalideerd 17-7-2015
'===============================================================================
'MOD 31

Imports System.Linq
Imports System.IO
Imports System.Xml.Serialization
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Result
    'MOD 31
    Public Class clsLinqSdfToXml
#Region "Class members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private m_sdfResultExport As Resultssql.DataAccess.Results
        Private m_EntitiesResultImport As New Model.Result.Entities.InspectionResultsDataEntity

        Dim recordcounterProcessed As Integer = 0
        Dim recordcounterTotal As Integer = 0

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_InspectionResult_id_New As Integer = 0

#End Region


#Region "Properties"
        ''' <summary>
        ''' Append from INSPECTOR to export file
        ''' in case false. Only results with status 3, 4 or 5 are exported
        ''' </summary>
        ''' <value>set value</value>
        Public Property AppendAllResults() As Boolean
            Get
                Return m_AppendAllResults
            End Get
            Set(ByVal value As Boolean)
                m_AppendAllResults = value
            End Set
        End Property
        Private m_AppendAllResults As Boolean = True
#End Region


#Region "Constructors"
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Export the results from the SDF database to a xml entity. Use function WriteResultsInformation to create a xml file
        ''' </summary>
        ''' <param name="fileNameSdfDatabase">File name of the msAcces database</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal fileNameSdfDatabase As String, ByVal xmlFile As String, xsdFile As String)
            LoadResultsInformation(fileNameSdfDatabase, xmlFile, xsdFile)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Export the results from the SDF database to a xml entity. Use function WriteResultsInformation to create a xml file
        ''' </summary>
        ''' <param name="fileNameSdfDatabase">File name of the msAcces database</param>
        ''' <param name="xmlFile">XML output file</param>
        ''' <param name="xsdFile">XSD file</param>
        ''' <remarks></remarks>
        Public Sub LoadResultsInformation(ByVal fileNameSdfDatabase As String, ByVal xmlFile As String, xsdFile As String)
            'clear the data
            m_EntitiesResultImport.InspectionResults.Clear()
            ClearCounters()


            Dim xmlToxml As New Model.Result.clsLinqXmlToXml
            'Check if a results.xml file already exists in the main directory. If so load this data to append other data
            If CheckFileExistsPC(xmlFile) Then
                XmlToxml.LoadResults(xmlFile, xsdFile)
                m_EntitiesResultImport = XmlToxml.EntitiesResultImport
            End If

            ' Reading the results from the sdf database
            m_sdfResultExport = New Resultssql.DataAccess.Results(fileNameSdfDatabase) ' With {.Log = Console.Out}

            'Add SDF data to XML file
            ExtractInspectionResults_PrsGcl_FromSdfToEntity()
        End Sub

        ''' <summary>
        ''' Write the entity into a XML file
        ''' </summary>
        ''' <param name="xmlFile">XML file to write</param>
        ''' <param name="xsdFile">XSD file for xml validation</param>
        ''' <remarks></remarks>
        Public Sub WriteResults(ByVal xmlFile As String, xsdFile As String)
            If m_EntitiesResultImport.InspectionResults.Count > 0 Then
                Dim objStreamWriter As New StreamWriter(xmlFile)
                Dim x As New XmlSerializer(m_EntitiesResultImport.GetType)
                x.Serialize(objStreamWriter, m_EntitiesResultImport)
                objStreamWriter.Close()

                'Check if the create file is correct.
                Try
                    xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
                    Debug.Print("Write complete: " & Format(Now, "HH:mm:ss:fff"))
                    RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, 0, 0, DbCreateStatus.SuccesWrite, "!Saving data completed")
                Catch ex As Exception
                    'MOD 21
                    RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, 0, 0, DbCreateStatus.ErrorXsd, xmlFile)
                End Try
            Else
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, 0, 0, DbCreateStatus.SuccesWrite, "!No data")
            End If
        End Sub
#End Region

#Region "Helpers"
        ''' <summary>
        ''' Clear all counters
        ''' </summary>
        ''' <remarks></remarks>
        Private Function ClearCounters() As Boolean
            m_InspectionResult_id_New = 0

            recordcounterProcessed = 0
            recordcounterTotal = 0
            Return True
        End Function
#End Region


#Region "Handling of getting PRS and GCL results from sdf database"
        ''' <summary>
        ''' Extract the PRS and GCL results from the SDF database and add it to the XML file.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_PrsGcl_FromSdfToEntity()
            ' Filter the data if the lastResult = 0 (This means the result is added by INSPECTOR and not marked as a 
            ' result added during synchronisation)
            Dim query = From result In m_sdfResultExport.InspectionResults() Where result.LastResult = 0 Select result

            recordcounterTotal = query.Count
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, "")

            'Check all records
            For Each prsSdfRecord In query
                recordcounterProcessed += 1

                'Check if all results should be added or check the status of the result.
                If m_AppendAllResults = True Then
                ElseIf (prsSdfRecord.Status = 3 Or prsSdfRecord.Status = 4 Or prsSdfRecord.Status = 5) Then
                Else
                    Continue For
                End If

                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, prsSdfRecord.PRSName)

                ' Change the date for add to the msAccess database 
                Dim ldDateStart As Date = Now
                Dim ldTimeStart As Date = Now
                TransferSdfDateTime(prsSdfRecord.StartTime, ldDateStart, ldTimeStart)

                Dim ldDateEnd As Date = Now
                Dim ldTimeEnd As Date = Now
                TransferSdfDateTime(prsSdfRecord.EndTime, ldDateEnd, ldTimeEnd)

                'Assign the record
                Dim prsgclXmlRecords As New Model.Result.Entities.InspectionResultsEntity() With { _
                    .InspectionResult_Id = m_InspectionResult_id_New, _
                    .Status = prsSdfRecord.Status, _
                    .PRSIdentification = prsSdfRecord.PRSIdentification, _
                    .PRSName = prsSdfRecord.PRSName, _
                    .PRSCode = prsSdfRecord.PRSCode, _
                    .CRC = "", _
                    .LastResult = "1" _
                }

                If prsSdfRecord.GasControlLineName <> "" Then
                    prsgclXmlRecords.GasControlLineName = prsSdfRecord.GasControlLineName
                    prsgclXmlRecords.GCLIdentification = prsSdfRecord.GCLIdentification
                    prsgclXmlRecords.GCLCode = prsSdfRecord.GCLCode
                    'Read the Measurement_Equipment for this InspectionResult; Always only one
                    Dim measurementEquipmentEntityRecord As New Model.Result.Entities.Measurement_EquipmentEntity() With { _
                        .ID_DM1 = prsSdfRecord.ID_DM1, _
                        .ID_DM2 = prsSdfRecord.ID_DM2, _
                        .BT_Address = prsSdfRecord.BT_Address _
                    }
                    prsgclXmlRecords.Measurement_Equipment.Add(measurementEquipmentEntityRecord)
                End If

                'Read the InspectionProcedure for this InspectionResult; Always only one)
                Dim inspectionProcedureRecord As New Model.Result.Entities.InspectionProcedureEntity() With { _
                      .InspectionProcedureName = prsSdfRecord.InspectionProcedureName, _
                      .InspectionProcedureVersion = prsSdfRecord.InspectionProcedureVersion _
                      }
                'Assign object to active InspectionResultObject
                prsgclXmlRecords.InspectionProcedure.Add(inspectionProcedureRecord)

                'Set the dateTimeStampl Only one
                Dim dateTimeStampRecord As New Model.Result.Entities.DateTimeStampEntity() With { _
                    .StartDate = CastToTypeDateValueOrDefault(ldDateStart), _
                    .StartTime = CastToTypeTimeValueOrDefault(ldTimeStart), _
                    .EndTime = CastToTypeTimeValueOrDefault(ldTimeEnd) _
                }

                Dim dateTimeStampTimesettings As New Model.Result.Entities.TimeSettingsEntity() With { _
                    .TimeZone = CastToTypeTimeZoneValueOrDefault(prsSdfRecord.TimeZone), _
                    .DST = CastToTypeDstValueOrDefault(prsSdfRecord.DST) _
                }
                dateTimeStampRecord.TimeSettings.Add(dateTimeStampTimesettings)

                'Assign object to active InspectionResultObject
                prsgclXmlRecords.DateTimeStamp.Add(dateTimeStampRecord)
                'Read the Results
                ExtractInspectionResult_FromSdfToEntity(prsSdfRecord.InspectionResult_Id, prsgclXmlRecords)
                m_EntitiesResultImport.InspectionResults.Add(prsgclXmlRecords)

                ' Increase ID
                m_InspectionResult_id_New += 1
            Next
        End Sub


        ''' <summary>
        ''' Get and save all records from the sdf results
        ''' </summary>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <param name="xmlInspectionResultsRecords"></param>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResult_FromSdfToEntity(liID_InspectionResults_ID As Integer, ByRef xmlInspectionResultsRecords As Model.Result.Entities.InspectionResultsEntity)

            Dim query = From prs In m_sdfResultExport.Results1() Where prs.InspectionResultLinkID = liID_InspectionResults_ID Select prs
            If query.Count = 0 Then Exit Sub
            Dim hitResult As Boolean = False
            For Each resultRecord In query
                hitResult = True
                Dim gclXmlResultRecord As New Model.Result.Entities.ResultsEntity() With { _
                    .ObjectName = resultRecord.ObjectName, _
                    .ObjectID = resultRecord.ObjectID, _
                    .MeasurePoint = resultRecord.MeasurePoint, _
                    .MeasurePointID = resultRecord.MeasurePointID, _
                    .FieldNo = resultRecord.FieldNo, _
                    .Time = resultRecord.Time _
                }
                ExtractResults_FromSdfToXML(resultRecord, gclXmlResultRecord)
                xmlInspectionResultsRecords.Result.Add(gclXmlResultRecord)
            Next

            'Check if a result is present. Else create a empty result to ensure XML file is valid
            If hitResult = False Then
                'If no result is found. Add one result to file to ensure XSD validation
                If hitResult = False Then
                    Dim gclXmlResultRecord As New Model.Result.Entities.ResultsEntity() With { _
                        .ObjectName = "", _
                        .ObjectID = "", _
                        .MeasurePoint = "", _
                        .MeasurePointID = "", _
                        .FieldNo = 1, _
                        .Time = "00:00:00", _
                        .Text = "" _
                    }
                    xmlInspectionResultsRecords.Result.Add(gclXmlResultRecord)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Extract the Result data
        ''' </summary>
        ''' <param name="resultRecord"></param>
        ''' <param name="XmlResultRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_FromSdfToXML(ByVal resultRecord As KAM.COMMUNICATOR.Synchronize.Bussiness.Resultssql.DataAccess.Result, ByRef XmlResultRecord As Model.Result.Entities.ResultsEntity)
            'depending on the kind of value, set the correct output
            If resultRecord.Text <> "" Then
                XmlResultRecord.Text = resultRecord.Text
            End If
            If resultRecord.Value <> "" Or resultRecord.UOM <> "" Then
                Dim measureValue As New Model.Result.Entities.MeasureValue() With { _
                    .UOM = resultRecord.UOM, _
                    .Value = resultRecord.Value
                }
                XmlResultRecord.MeasureValue.Add(measureValue)
            Else
                'The result is a list
                Dim queryList = From prs In m_sdfResultExport.Lists() Where prs.ResultLinkID = resultRecord.Result_Id Order By prs.List_Id Ascending Select prs
                If queryList.Count > 0 Then
                    For Each listRecord In queryList
                        If listRecord.List_Column <> "" Then
                            XmlResultRecord.List.Add(listRecord.List_Column)
                        End If
                    Next
                End If
            End If

        End Sub
#End Region

    End Class
End Namespace
