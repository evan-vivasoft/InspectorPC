'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Result
    ''' <summary>
    ''' Class for handling the export from MsAccess to SDF
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinqMsAccessToSdf

#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private m_sdfResultImport As Resultssql.DataAccess.Results
        Private m_msAccessResultExport As ResultMdbEntities.DataAccess.AccessInspectionResults

        Private recordcounterProcessed As Integer = 0
        Private recordcounterTotal As Integer = 0

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_InspectionResult_id_New As Integer = 0
        Private m_Result_id_New As Integer = 0
        Private m_List_id_New As Integer = 0

        ' Default voor Status 
        Private lsStatusDefault As String = "3"

#End Region

        Public WriteOnly Property ExportAllRecords() As Boolean
            Set(value As Boolean)
                m_ExportAll = value
            End Set
        End Property
        Private m_ExportAll As Boolean = False



#Region "Properties"


#End Region

#Region "Public"
        ''' <summary>
        ''' Append msAccess results to the SDF database.
        ''' </summary>
        ''' <param name="sqlDbImport_FileName">SDF file to append the results</param>
        ''' <param name="msAccessExport_FileName">msAccess file to export from</param>
        ''' <remarks></remarks>
        Public Sub LoadWriteResults(ByVal sqlDbImport_FileName As String, ByVal msAccessExport_FileName As String)
            ' Laad lijst met Units die in de Result velden duiden op een Value ipv een Text
            ' LET OP! om het simpel te houden moeten de langste termen het eerst in de lijst staan! Dus 'mbar' moet VOOR 'bar'. Reden hiervoor is dat er gekeken
            ' wordt of het resultaat EINDIGD met een bepaalde tekst, bij mbar eindigd het ook op bar dus dat gaat dan fout!

            ' Error message variabele leegmaken

            ' Lees de data van de Access database in
            m_msAccessResultExport = New ResultMdbEntities.DataAccess.AccessInspectionResults(msAccessExport_FileName) ' With {.Log = Console.Out}

            ' Creeer een nieuwe SDF database (altijd nieuw)
            m_sdfResultImport = New Resultssql.DataAccess.Results(sqlDbImport_FileName)
            If m_sdfResultImport.DatabaseExists Then
                Try
                    m_sdfResultImport.DeleteDatabase()
                Catch ex As Exception
                    ' Stop, retourneer foutmelding
                    Debug.Print(ex.Message)
                    Exit Sub
                End Try
            End If
            m_sdfResultImport.CreateDatabase()

            ClearCounters()

            ' Extract alle velden uit de Access database naar de SDF structuur en voer ze door
            ExtractInspectionResults_Gcl_FromMsAccessToSdf()
            ExtractInspectionResults_Prs_FromMsAccessToSdf()
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedWrite, "!Saving database")
            m_sdfResultImport.SubmitChanges()
            Debug.Print("Write complete: " & Format(Now, "HH:mm:ss:fff"))
            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.SuccesWrite, "!Saving database")
            m_sdfResultImport.Connection.Close()
        End Sub

        Sub Dispose()
            ' Ruim array op en zorg dat SDF beschikbaar komt
            m_sdfResultImport.Dispose()
        End Sub

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

#Region "Extract results from tblResultGCL to SDF"

        Private Sub ExtractInspectionResults_Gcl_FromMsAccessToSdf()

            ' Dataset wordt op basis van de propertie ExportAllRecords beperkt tot alleen de laatste resultaten of alles wordt geexporteerd
            Dim query

            If m_ExportAll = False Then
                ' Er is voor gekozen om de data te sorteren op basis van start datum en tijd
                query = From prs In m_msAccessResultExport.tblResultGCL() Order By prs.PRSName Ascending, prs.GasControlLine Ascending, prs.DateStart Descending, prs.TimeStart Descending Select prs
            Else
                ' Alles moet geexporteerd worden
                query = From prs In m_msAccessResultExport.tblResultGCL() Select prs
            End If


            ' Deze variabelen onthouden altijd de laatst geexporteerde record. Hieraan kan dan de record die moet worden verwerkt gecontroleerd worden
            ' is het een andere regelstraat of gasstation dan moet de eerste weer geexporteerd worden omdat dat de laatste datum is
            Dim lsLastPrsName As String = "START", lsLastGasControlLine As String = "START"
            recordcounterTotal = m_msAccessResultExport.tblResultGCL.Count + m_msAccessResultExport.tblResultPRS.Count

            RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, "")
            ' Doorloop nu alle records in de Query

            For Each prsMsaRecord In query
                recordcounterProcessed += 1
                Dim lbOK As Boolean = True
                Dim lsTestPrsName As String = ""
                Dim lsTestGasControlLine As String = ""
                If m_ExportAll = False Then
                    ' Alleen laatste datum exporteren!
                    ' Hieraan kan dan de record die moet worden verwerkt gecontroleerd worden
                    ' is het een andere regelstraat of gasstation dan moet de eerste weer geexporteerd worden omdat dat de laatste datum is
                    Try
                        lsTestPrsName = prsMsaRecord.PRSName.ToString.ToLower
                        lsTestGasControlLine = prsMsaRecord.GasControlLine.ToString.ToLower
                    Catch ex As Exception
                    End Try
                    ' Indien al geweest niet nogmaals doen!
                    If lsTestPrsName = lsLastPrsName.ToLower And lsTestGasControlLine = lsLastGasControlLine.ToLower Then
                        lbOK = False
                    End If
                End If
                ' Exporteren indien gewenst
                If lbOK = True Then
                    RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, prsMsaRecord.PRSName)

                    ' Maak StartTime en EndTime op (datum moet er namelijk bij in!)
                    Dim ldStart As Date = Now
                    Try
                        ldStart = prsMsaRecord.DateStart
                        ldStart = prsMsaRecord.DateStart & " " & prsMsaRecord.TimeStart
                    Catch ex As Exception
                    End Try
                    Dim ldEnd As Date = Now
                    Try
                        If prsMsaRecord.DateEnd > "30-01-1900" Then
                            ldEnd = prsMsaRecord.DateEnd
                            ldEnd = prsMsaRecord.DateEnd & " " & prsMsaRecord.TimeEnd
                        Else
                            ldEnd = prsMsaRecord.DateStart
                            ldEnd = prsMsaRecord.DateStart & " " & prsMsaRecord.TimeEnd
                        End If
                    Catch ex As Exception
                    End Try

                    ' En toewijzen aan recordset
                    Dim gclSqlRecord As New Resultssql.DataAccess.InspectionResult With { _
                        .InspectionResult_Id = m_InspectionResult_id_New, _
                        .Status = lsStatusDefault, _
                        .PRSIdentification = prsMsaRecord.PRSIdentification, _
                        .PRSName = prsMsaRecord.PRSName, _
                        .PRSCode = prsMsaRecord.PRSCode, _
                        .GasControlLineName = prsMsaRecord.GasControlLine, _
                        .GCLIdentification = prsMsaRecord.GCLIdentification, _
                        .GCLCode = "", _
                        .CRC = "", _
                        .LastResult = "1", _
                        .ID_DM1 = prsMsaRecord.IdDm1, _
                        .ID_DM2 = prsMsaRecord.IdDm2, _
                        .BT_Address = prsMsaRecord.BTAddress, _
                        .InspectionProcedureName = prsMsaRecord.InspectionProcedure, _
                        .InspectionProcedureVersion = prsMsaRecord.InspectionProcedureVersion, _
                        .StartTime = ldStart, _
                        .EndTime = ldEnd, _
                        .TimeZone = prsMsaRecord.TimeZone, _
                        .DST = prsMsaRecord.Dst _
                    }

                    ' Sla nu de resultaten van deze record op in de RESULT tabel van de SDF database
                    ExtractResults_Gcl_FromMsAccessToSqlCe(prsMsaRecord)

                    ' Adding the Record to the SDF database
                    m_sdfResultImport.InspectionResults.InsertOnSubmit(gclSqlRecord)

                    ' Increase ID
                    m_InspectionResult_id_New += 1

                    ' Onthoud voor de volgende controle
                    lsLastPrsName = lsTestPrsName
                    lsLastGasControlLine = lsTestGasControlLine

                End If
            Next

            ' Nu pas doorvoeren in de database omdat dat de snelheid ten goed komt
            Debug.Print("Write GCL data: " & Format(Now, "HH:mm:ss:fff"))
        End Sub


        Private Sub ExtractResults_Gcl_FromMsAccessToSqlCe(ByVal prsMsaRecord As ResultsMDBEntities.DataAccess.tblResultGCL)
            ' Sla nu de resultaten van de InspectionResult record op in de RESULT tabel van de SDF database
            ' Doorloop alle Result velden van de tabel

            ' Zet alle resultaten in een array
            Dim lsResultFields() As String
            Call SetRecordsValueResultGcl(prsMsaRecord, lsResultFields)

            ' Doorloop nu al deze datavelden en schrijf ze zonodig weg naar de database
            For i As Integer = 1 To lsResultFields.Length - 1
                If lsResultFields(i) <> "" Then
                    Dim lsValue As structResult = GetRecordValueResult(lsResultFields(i))
                    If lsValue.Hit = True Then
                        Dim resultSqlRecord As New Resultssql.DataAccess.Result With { _
                            .Result_Id = m_Result_id_New, _
                            .InspectionResultLinkID = m_InspectionResult_id_New, _
                            .ObjectName = "", _
                            .ObjectID = "", _
                            .MeasurePoint = "", _
                            .MeasurePointID = "", _
                            .FieldNo = i, _
                            .Time = prsMsaRecord.TimeStart, _
                            .Text = lsValue.Text, _
                            .Value = lsValue.Value, _
                            .UOM = lsValue.UOM _
                        }

                        'Adding the prs to the SDF database
                        m_sdfResultImport.Results1.InsertOnSubmit(resultSqlRecord)

                        'Increase ID
                        m_Result_id_New += 1
                    End If
                End If
            Next

        End Sub


#End Region

#Region "Save Results in SDF tblResultPRS"

        Private Sub ExtractInspectionResults_Prs_FromMsAccessToSdf()

            ' Dataset wordt op basis van de propertie ExportAllRecords beperkt tot alleen de laatste resultaten of alles wordt geexporteerd
            Dim query
            If m_ExportAll = False Then
                ' Er is voor gekozen om de data te sorteren op basis van start datum en tijd
                query = From prs In m_msAccessResultExport.tblResultPRS() Order By prs.PRSName Ascending, prs.DateStart Descending, prs.TimeStart Descending Select prs
            Else
                ' Alles moet geexporteerd worden
                query = From prs In m_msAccessResultExport.tblResultPRS() Select prs
            End If

            ' Deze variabelen onthouden altijd de laatst geexporteerde record. Hieraan kan dan de record die moet worden verwerkt gecontroleerd worden
            ' is het een andere regelstraat of gasstation dan moet de eerste weer geexporteerd worden omdat dat de laatste datum is
            Dim lsLastPRSName As String = "START"
            ' Doorloop nu alle records in de Query
            For Each prsMsaRecord In query
                recordcounterProcessed += 1
                Dim lbOK As Boolean = True
                Dim lsTestPRSName As String = ""
                If m_ExportAll = False Then
                    ' Alleen laatste datum exporteren!
                    ' Hieraan kan dan de record die moet worden verwerkt gecontroleerd worden
                    ' is het een andere regelstraat of gasstation dan moet de eerste weer geexporteerd worden omdat dat de laatste datum is
                    Try
                        lsTestPRSName = prsMsaRecord.PRSName.ToString.ToLower
                    Catch ex As Exception
                    End Try
                    ' Indien al geweest niet nogmaals doen!
                    If lsTestPRSName = lsLastPRSName.ToLower Then
                        lbOK = False
                    End If
                End If
                ' Exporteren indien gewenst
                If lbOK = True Then
                    RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, recordcounterTotal, recordcounterProcessed, clsDbGeneral.DbCreateStatus.StartedCreate, prsMsaRecord.PRSName)

                    ' Maak StartTime en EndTime op (datum moet er namelijk bij in!)
                    Dim ldStart As Date = Now
                    Try
                        If prsMsaRecord.DateStart > "30-01-1900" Then
                            ldStart = prsMsaRecord.dateStart
                            ldStart = prsMsaRecord.dateStart & " " & prsMsaRecord.TimeStart
                        Else
                            ldStart = prsMsaRecord.DateEnd
                            ldStart = prsMsaRecord.DateEnd & " " & prsMsaRecord.TimeEnd
                        End If
                    Catch ex As Exception
                    End Try
                    Dim ldEnd As Date = Now
                    Try
                        If prsMsaRecord.DateEnd > "30-01-1900" Then
                            ldEnd = prsMsaRecord.DateEnd
                            ldEnd = prsMsaRecord.DateEnd & " " & prsMsaRecord.TimeEnd
                        Else
                            ldEnd = prsMsaRecord.DateStart
                            ldEnd = prsMsaRecord.DateStart & " " & prsMsaRecord.TimeEnd
                        End If
                    Catch ex As Exception
                    End Try

                    ' En toewijzen aan recordset
                    Dim prsSqlRecord As New Resultssql.DataAccess.InspectionResult With { _
                       .InspectionResult_Id = m_InspectionResult_id_New, _
                       .Status = lsStatusDefault, _
                       .PRSIdentification = prsMsaRecord.PRSIdentification, _
                       .PRSName = prsMsaRecord.PRSName, _
                       .PRSCode = prsMsaRecord.PRSCode, _
                       .GasControlLineName = "", _
                       .GCLIdentification = "", _
                       .GCLCode = "", _
                       .CRC = "", _
                       .LastResult = "1", _
                       .ID_DM1 = "", _
                       .ID_DM2 = "", _
                       .BT_Address = "", _
                       .InspectionProcedureName = prsMsaRecord.InspectionProcedure, _
                       .InspectionProcedureVersion = prsMsaRecord.InspectionProcedureVersion, _
                       .StartTime = ldStart, _
                       .EndTime = ldEnd, _
                       .TimeZone = prsMsaRecord.TimeZone, _
                       .DST = prsMsaRecord.Dst _
                    }

                    ' Sla nu de resultaten van deze record op in de RESULT tabel van de SDF database
                    ExtractResults_Prs_FromMsAccessToSdf(prsMsaRecord)

                    ' Adding the Record to the SDF database
                    m_sdfResultImport.InspectionResults.InsertOnSubmit(prsSqlRecord)

                    ' Increase ID
                    m_InspectionResult_id_New += 1

                    ' Onthoud voor de volgende controle
                    lsLastPRSName = lsTestPRSName

                End If
            Next

            ' Nu pas doorvoeren in de database omdat dat de snelheid ten goed komt
            Debug.Print("Wegschrijven PRS data: " & Format(Now, "HH:mm:ss:fff"))

        End Sub


        Private Sub ExtractResults_Prs_FromMsAccessToSdf(ByVal prsMsaRecord As ResultsMDBEntities.DataAccess.tblResultPRS)
            ' Sla nu de resultaten van de InspectionResult record op in de RESULT tabel van de SDF database
            ' Doorloop alle Result velden van de tabel

            ' Zet alle resultaten in een array
            Dim lsResultFields() As String
            Call SetRecordsValueResultPrs(prsMsaRecord, lsResultFields)

            ' Doorloop nu al deze datavelden en schrijf ze zonodig weg naar de database
            For liX As Integer = 1 To lsResultFields.Length - 1
                If lsResultFields(liX) <> "" Then
                    Dim lsValue As structResult = GetRecordValueResult(lsResultFields(liX))
                    If lsValue.Hit = True Then
                        Dim resultSqlRecord As New Resultssql.DataAccess.Result With { _
                            .Result_Id = m_Result_id_New, _
                            .InspectionResultLinkID = m_InspectionResult_id_New, _
                            .ObjectName = "", _
                            .ObjectID = "", _
                            .MeasurePoint = "", _
                            .MeasurePointID = "", _
                            .FieldNo = liX, _
                            .Time = "", _
                            .Text = lsValue.Text, _
                            .Value = lsValue.Value, _
                            .UOM = lsValue.UOM _
                          }

                        'Adding the prs to the SDF database
                        m_sdfResultImport.Results1.InsertOnSubmit(resultSqlRecord)

                        'Increase ID
                        m_Result_id_New += 1

                    End If
                End If
            Next

        End Sub
#End Region

    End Class

End Namespace