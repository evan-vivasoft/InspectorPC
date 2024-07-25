'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.IO
Imports System.Xml.Serialization
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Result
    ''' <summary>
    ''' Class for handling the export from MsAccess to XML
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ClsLinqMsAccessToXml
#Region "Class members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private m_msAccessResultExport As ResultMdbEntities.DataAccess.AccessInspectionResults
        Private m_EntitiesResultImport As New Model.Result.Entities.InspectionResultsDataEntity

        Dim recordcounterProcessed As Integer = 0
        Dim recordcounterTotal As Integer = 0


        'Primary key counters; Used to generate the SDF file from Entity
        Private m_prs_id_New As Integer = 0


        ' Default for Status 
        Private Const LsStatusDefault As String = "3"

        Private ipResultsList As New List(Of Model.Result.Entities.InspectionResultsEntity)
#End Region
#Region "Properties"

        ''' <summary>
        ''' Set to export all records
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property ExportAllRecords() As Boolean
            Get
                Return m_ExportAll
            End Get
            Set(value As Boolean)
                m_ExportAll = value
            End Set
        End Property
        Private m_ExportAll As Boolean = False
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
        ''' Export the results from the msAccess database to a xml entity. Use function WriteResultsInformation to create a xml file
        ''' </summary>
        ''' <param name="MsAccessExport_Filename">File name of the msAcces database</param>
        ''' <param name="selections">Optional selection string. No funtion yet</param>
        ''' <remarks></remarks>
        Public Sub LoadResultsInformation(ByVal msAccessExport_Filename As String, Optional selections As String = "")
            ipResultsList.Clear()
            m_msAccessResultExport = New ResultMdbEntities.DataAccess.AccessInspectionResults(msAccessExport_Filename)
            recordcounterProcessed = 0
            recordcounterTotal = 0
            m_prs_id_New = 0
            ExtractInspectionResults_Gcl_FromMsAccessToEntity()
            ExtractInspectionResults_Prs_FromMsAccessToXml()
            m_EntitiesResultImport.InspectionResults = ipResultsList
        End Sub

        ''' <summary>
        ''' Write the entity into a XML file
        ''' </summary>
        ''' <param name="xmlFile">XML file to write</param>
        ''' <param name="xsdFile">XSD file for xml validation</param>
        ''' <remarks></remarks>
        Public Sub WriteResultsInformation(ByVal xmlFile As String, xsdFile As String)
            'MOD 17
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

#Region "Handling of getting GCL results from database"
        ''' <summary>
        ''' Extract the results from the MsAccess database into the Entity
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_Gcl_FromMsAccessToEntity()
            Dim query
            query = From prs In m_msAccessResultExport.tblResultGCL() Order By prs.PRSName Ascending, prs.GasControlLine Ascending, prs.DateStart Descending, prs.TimeStart Descending Select prs

            'Variables to check if the last result is exported
            Dim lsLastPrsName As String = "START", lsLastGasControlLine As String = "START"

            recordcounterTotal = m_msAccessResultExport.tblResultGCL.Count + m_msAccessResultExport.tblResultPRS.Count
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, "")

            'Check all records
            For Each gclMsaRecord In query
                'MOD 51 recordcounterProcessed += 1
                Dim lbExport As Boolean = True 'Set true; Every record is export
                Dim lsTestPrsName As String = ""
                Dim lsTestGasControlLine As String = ""
                If m_ExportAll = False Then
                    'In case only the last date should be exported
                    Try
                        'Save the corrent names for check 
                        lsTestPrsName = gclMsaRecord.PRSName.ToString.ToLower
                        lsTestGasControlLine = gclMsaRecord.GasControlLine.ToString.ToLower
                    Catch ex As Exception
                    End Try
                    'If already is exported then do not export again.
                    If lsTestPrsName = lsLastPrsName.ToLower And lsTestGasControlLine = lsLastGasControlLine.ToLower Then
                        lbExport = False
                    End If
                End If
                'Export the result
                If lbExport = True Then

                    recordcounterProcessed += 1
                    RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, gclMsaRecord.PRSName)
                    'Assign the record
                    Dim gclXmlRecord As New Model.Result.Entities.InspectionResultsEntity() With { _
                   .InspectionResult_Id = m_prs_id_New, _
                   .Status = LsStatusDefault, _
                   .PRSIdentification = CastToStringOrEmpty(gclMsaRecord.PRSIdentification), _
                   .PRSName = CastToStringOrEmpty(gclMsaRecord.PRSName), _
                   .PRSCode = CastToStringOrEmpty(gclMsaRecord.PRSCode), _
                   .GasControlLineName = CastToStringOrEmpty(gclMsaRecord.GasControlLine), _
                   .GCLIdentification = CastToStringOrEmpty(gclMsaRecord.GCLIdentification), _
                   .GCLCode = "", _
                   .CRC = "", _
                   .LastResult = "1" _
                   }

                    'Read the Measurement_Equipment for this InspectionResult; Always only one
                    Dim measurementEquipmentEntityRecord As New Model.Result.Entities.Measurement_EquipmentEntity() With { _
                          .ID_DM1 = CastToStringOrEmpty(gclMsaRecord.IdDm1), _
                          .ID_DM2 = CastToStringOrEmpty(gclMsaRecord.IdDm2), _
                          .BT_Address = CastToStringOrEmpty(gclMsaRecord.BTAddress) _
                          }
                    'Assign object to active InspectionResultObject
                    gclXmlRecord.Measurement_Equipment.Add(measurementEquipmentEntityRecord)

                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    Dim inspectionProcedureRecord As New Model.Result.Entities.InspectionProcedureEntity() With { _
                          .InspectionProcedureName = CastToStringOrEmpty(gclMsaRecord.InspectionProcedure), _
                          .InspectionProcedureVersion = CastToStringOrEmpty(gclMsaRecord.InspectionProcedureVersion) _
                          }
                    'Assign object to active InspectionResultObject
                    gclXmlRecord.InspectionProcedure.Add(inspectionProcedureRecord)

                    'Set the dateTimeStampl Only one
                    Dim dateTimeStampRecord As New Model.Result.Entities.DateTimeStampEntity() With { _
                            .StartDate = CastToTypeDateValueOrDefault(gclMsaRecord.DateStart), _
                            .StartTime = CastToTypeTimeValueOrDefault(gclMsaRecord.TimeStart), _
                            .EndTime = CastToTypeTimeValueOrDefault(gclMsaRecord.TimeEnd) _
                            }
                    Dim dateTimeStampTimesettings As New Model.Result.Entities.TimeSettingsEntity() With { _
                            .TimeZone = CastToTypeTimeZoneValueOrDefault(gclMsaRecord.TimeZone), _
                            .DST = CastToTypeDstValueOrDefault(gclMsaRecord.DST) _
                        }
                    dateTimeStampRecord.TimeSettings.Add(dateTimeStampTimesettings)

                    'Assign object to active InspectionResultObject
                    gclXmlRecord.DateTimeStamp.Add(dateTimeStampRecord)
                    'Read the Results
                    ExtractResults_Gcl_FromMsAccessToEntity(gclMsaRecord, gclXmlRecord)
                    ipResultsList.Add(gclXmlRecord)
                    ' Increase ID
                    m_prs_id_New += 1

                    'Save the name for the next check
                    lsLastPrsName = lsTestPrsName
                    lsLastGasControlLine = lsTestGasControlLine
                End If
            Next

        End Sub

        ''' <summary>
        ''' Get and save all records from the ResultsGCL table
        ''' </summary>
        ''' <param name="gclMsaRecord"></param>
        ''' <param name="gclXMLRecords"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_Gcl_FromMsAccessToEntity(ByVal gclMsaRecord As ResultsMDBEntities.DataAccess.tblResultGCL, ByRef gclXMLRecords As Model.Result.Entities.InspectionResultsEntity)
            'Get all values and store them in a array
            Dim lsResultFields() As String
            Call SetRecordsValueResultGcl(gclMsaRecord, lsResultFields)
            Dim hitResult As Boolean = False
            For i As Integer = 1 To lsResultFields.Length - 1
                If lsResultFields(i) <> "" Then
                    Dim lsValue As structResult = GetRecordValueResult(lsResultFields(i))
                    If lsValue.Hit = True Then
                        hitResult = True
                        Dim gclXmlRecord As New Model.Result.Entities.ResultsEntity() With { _
                        .ObjectName = "", _
                        .ObjectID = "", _
                        .MeasurePoint = "", _
                        .MeasurePointID = "", _
                        .FieldNo = i, _
                        .Time = CastToTypeTimeValueOrDefault(gclMsaRecord.TimeStart) _
                        }
                        'depending on the kind of value, set the correct output
                        If lsValue.Text <> "" Then
                            gclXmlRecord.Text = lsValue.Text
                        Else
                            Dim measureValue As New Model.Result.Entities.MeasureValue() With { _
                                    .UOM = lsValue.UOM, _
                                    .Value = lsValue.Value _
                           }
                            'lsValue.Value
                            gclXmlRecord.MeasureValue.Add(measureValue)
                        End If
                        gclXMLRecords.Result.Add(gclXmlRecord)

                    End If
                End If
            Next
            'If no result is found. Add one result to file to ensure XSD validation
            If hitResult = False Then
                Dim gclXmlRecord As New Model.Result.Entities.ResultsEntity() With { _
                        .ObjectName = "", _
                        .ObjectID = "", _
                        .MeasurePoint = "", _
                        .MeasurePointID = "", _
                        .FieldNo = 1, _
                        .Time = CastToTypeTimeValueOrDefault(gclMsaRecord.TimeStart), _
                        .Text = "" _
                        }
                gclXMLRecords.Result.Add(gclXmlRecord)
            End If
        End Sub


#End Region

#Region "Handling of getting PRS results from database"

        Private Sub ExtractInspectionResults_Prs_FromMsAccessToXml()
            Dim query
            query = From prs In m_msAccessResultExport.tblResultPRS() Order By prs.PRSName Ascending, prs.DateStart Descending, prs.TimeStart Descending Select prs

            'Variables to check if the last result is exported
            Dim lsLastPrsName As String = "START", lsLastGasControlLine As String = "START"

            recordcounterTotal = m_msAccessResultExport.tblResultGCL.Count + m_msAccessResultExport.tblResultPRS.Count
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, "")

            'Check all records
            For Each prsMsaRecord In query
                'MOD 51 recordcounterProcessed += 1
                Dim lbExport As Boolean = True 'Set true; Every record is export
                Dim lsTestPrsName As String = ""

                If m_ExportAll = False Then
                    'In case only the last date should be exported
                    Try
                        'Save the corrent names for check 
                        lsTestPrsName = prsMsaRecord.PRSName.ToString.ToLower
                    Catch ex As Exception
                    End Try
                    'If already is exported then do not export again.
                    If lsTestPrsName = lsLastPrsName.ToLower Then
                        lbExport = False
                    End If
                End If
                'Export the result
                If lbExport = True Then
                    recordcounterProcessed += 1
                    RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, prsMsaRecord.PRSName)

                    'Assign the record
                    Dim prsXmlRecords As New Model.Result.Entities.InspectionResultsEntity() With { _
                   .InspectionResult_Id = m_prs_id_New, _
                   .Status = LsStatusDefault, _
                   .PRSIdentification = CastToStringOrEmpty(prsMsaRecord.PRSIdentification), _
                   .PRSName = CastToStringOrEmpty(prsMsaRecord.PRSName), _
                   .PRSCode = CastToStringOrEmpty(prsMsaRecord.PRSCode), _
                   .CRC = "", _
                   .LastResult = "1" _
                   }

                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    Dim inspectionProcedureRecord As New Model.Result.Entities.InspectionProcedureEntity() With { _
                          .InspectionProcedureName = CastToStringOrEmpty(prsMsaRecord.InspectionProcedure), _
                          .InspectionProcedureVersion = CastToStringOrEmpty(prsMsaRecord.InspectionProcedureVersion) _
                          }
                    'Assign object to active InspectionResultObject
                    prsXmlRecords.InspectionProcedure.Add(inspectionProcedureRecord)

                    'Set the dateTimeStampl Only one
                    Dim dateTimeStampRecord As New Model.Result.Entities.DateTimeStampEntity() With { _
                            .StartDate = CastToTypeDateValueOrDefault(prsMsaRecord.DateStart), _
                            .StartTime = CastToTypeTimeValueOrDefault(prsMsaRecord.TimeStart), _
                            .EndTime = CastToTypeTimeValueOrDefault(prsMsaRecord.TimeEnd) _
                            }
                    Dim dateTimeStampTimesettings As New Model.Result.Entities.TimeSettingsEntity() With { _
                            .TimeZone = CastToTypeTimeZoneValueOrDefault(prsMsaRecord.TimeZone), _
                            .DST = CastToTypeDstValueOrDefault(prsMsaRecord.DST) _
                        }
                    dateTimeStampRecord.TimeSettings.Add(dateTimeStampTimesettings)

                    'Assign object to active InspectionResultObject
                    prsXmlRecords.DateTimeStamp.Add(dateTimeStampRecord)
                    'Read the Results
                    ExtractResults_Prs_FromMsAccessToXml(prsMsaRecord, prsXmlRecords)
                    ipResultsList.Add(prsXmlRecords)
                    ' Increase ID
                    m_prs_id_New += 1

                    'Save the name for the next check
                    lsLastPrsName = lsTestPrsName
                End If
            Next
        End Sub

        ''' <summary>
        ''' Get and save all records from the ResultsPRS table
        ''' </summary>
        ''' <param name="prsMsaRecord"></param>
        ''' <param name="prsXMLRecords"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_Prs_FromMsAccessToXml(ByVal prsMsaRecord As ResultsMDBEntities.DataAccess.tblResultPRS, ByRef prsXMLRecords As Model.Result.Entities.InspectionResultsEntity)
            'Get all values and store them in a array
            Dim lsResultFields() As String
            Call SetRecordsValueResultPrs(prsMsaRecord, lsResultFields)

            Dim hitResult As Boolean = False

            For i As Integer = 1 To lsResultFields.Length - 1
                If lsResultFields(i) <> "" Then

                    Dim lsValue As structResult = GetRecordValueResult(lsResultFields(i))
                    If lsValue.Hit = True Then
                        hitResult = True
                        Dim resultEntityRecord As New Model.Result.Entities.ResultsEntity() With { _
                        .ObjectName = "", _
                        .ObjectID = "", _
                        .MeasurePoint = "", _
                        .MeasurePointID = "", _
                        .FieldNo = i, _
                        .Time = CastToTypeTimeValueOrDefault(prsMsaRecord.TimeStart) _
                        }
                        'depending on the kind of value, set the correct output
                        If lsValue.Text <> "" Then
                            'MOD 44
                            If InStr(lsValue.Text, "|") Then
                                Dim teststr As String
                                teststr = lsValue.Text
                                Dim listItemsFound As String() = teststr.Split(New Char() {"|"c})

                                For Each listItem As String In listItemsFound
                                    If Left(Trim(listItem.ToString), 1) <> "\" Then resultEntityRecord.List.Add(Trim(listItem.ToString()))
                                    If Left(Trim(listItem.ToString), 1) = "\" Then
                                        resultEntityRecord.Text = Trim(Right(listItem.ToString(), Len(listItem) - 1))
                                    End If

                                Next

                            Else
                                resultEntityRecord.Text = lsValue.Text
                            End If

                        Else
                            Dim measureValue As New Model.Result.Entities.MeasureValue() With { _
                                    .UOM = lsValue.UOM, _
                                    .Value = lsValue.Value _
                           }
                            resultEntityRecord.MeasureValue.Add(measureValue)
                        End If
                        prsXMLRecords.Result.Add(resultEntityRecord)

                    End If
                End If
            Next
            'If no result is found. Add one result to file to ensure XSD validation
            If hitResult = False Then
                Dim prsXmlRecord As New Model.Result.Entities.ResultsEntity() With { _
                        .ObjectName = "", _
                        .ObjectID = "", _
                        .MeasurePoint = "", _
                        .MeasurePointID = "", _
                        .FieldNo = 1, _
                        .Time = CastToTypeTimeValueOrDefault("00:00:00"), _
                        .Text = "" _
                        }
                prsXMLRecords.Result.Add(prsXmlRecord)
            End If
        End Sub

#End Region
    End Class

End Namespace