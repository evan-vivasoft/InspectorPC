'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================
Imports System.Globalization
Imports System.IO
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral


Namespace Model.Result
    ''' <summary>
    ''' Class Adding the SDF results to the Access database
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinqSdfToMsAccess
#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private m_msAccessResultImport As ResultMdbEntities.DataAccess.AccessInspectionResults
        Private m_sdfResultExport As Resultssql.DataAccess.Results

        'Primary key counters
        Private m_tblResultGCL_id_New As Integer = 0
        Private m_tblResultPRS_id_New As Integer = 0

        Dim recordcounterProcessed As Integer = 0
        Dim recordcounterTotal As Integer = 0

#End Region

#Region "Helpers"
        ''' <summary>
        ''' Create new table ID's; For table GCL and PRS.
        ''' The highist value is searched.
        ''' </summary>
        ''' <remarks></remarks>
        Sub GetNewTableID()
            ' For tblResultGCL
            m_tblResultGCL_id_New = 0
            Try
                Dim q = (From prs In m_msAccessResultImport.tblResultGCL() Select prs.Id).Max
                m_tblResultGCL_id_New = CType(q, Integer) + 1
            Catch ex As Exception
            End Try

            ' For tblResultPRS
            m_tblResultPRS_id_New = 0
            Try
                Dim q = (From prs In m_msAccessResultImport.tblResultPRS() Select prs.Id).Max
                m_tblResultPRS_id_New = CType(q, Integer) + 1
            Catch ex As Exception
            End Try
        End Sub
#End Region
#Region "Public"
        ''' <summary>
        ''' Append the SDF result to an msAccess database
        ''' </summary>
        ''' <param name="filenNameMsAccessDatabase">MsAccess file to append the records</param>
        ''' <param name="fileNameSdfDatabase">SDf file to append the records from</param>
        ''' <remarks></remarks>
        Public Sub WriteResults(ByVal filenNameMsAccessDatabase As String, ByVal fileNameSdfDatabase As String)
            ' Reading the results from the sdf database
            m_sdfResultExport = New Resultssql.DataAccess.Results(fileNameSdfDatabase) ' With {.Log = Console.Out}
            ' Reading the MsAccess results database
            m_msAccessResultImport = New ResultMdbEntities.DataAccess.AccessInspectionResults(filenNameMsAccessDatabase)

            If m_msAccessResultImport.DatabaseExists = False Then
                'Create new database
                Dim pathName As String
                pathName = Path.GetDirectoryName(filenNameMsAccessDatabase)
                If Not CheckFileExistsPC(pathName) Then Directory.CreateDirectory(pathName)
                m_msAccessResultImport.CreateDatabase()
            End If

            'Getting the maximum ID's from the database. 
            Call GetNewTableID()

            'Extract all data 
            ExtractInspectionResults_Gcl_FromSdfToAccess()
            ExtractInspectionResults_Prs_FromSdfToAccess()

        End Sub

        Sub Dispose()
            ' Ruim array op en zorg dat MDB beschikbaar komt
            m_msAccessResultImport.Connection.Close()
            m_msAccessResultImport.Dispose()
            m_msAccessResultImport = Nothing
        End Sub

#End Region
#Region "Database Helpers"
        ''' <summary>
        ''' Function for GCL as PRS. 
        ''' Process the result from the SQL Result or List table into 
        ''' </summary>
        ''' <param name="resultRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ExtractResultRecord(ByVal resultRecord As Resultssql.DataAccess.Result) As String
            'MOD 15
            Dim lsResult As String = ""
            Dim lsResultListValue As String = ""
            Dim lsResultText As String = ""

            If resultRecord.Text <> "" Then
                'The result is a text
                lsResultText = resultRecord.Text
            End If
            If resultRecord.Value <> "" Or resultRecord.UOM <> "" Then
                'The result is a Value
                'MOD 05
                Dim tmpValue As String
                tmpValue = resultRecord.Value
                Dim culture As New CultureInfo(CultureInfo.CurrentCulture.Name)
                If culture.NumberFormat.CurrencyDecimalSeparator = "." Then
                    tmpValue = tmpValue.Replace(",", culture.NumberFormat.CurrencyDecimalSeparator)
                Else
                    tmpValue = tmpValue.Replace(".", culture.NumberFormat.CurrencyDecimalSeparator)
                End If

                'MOD 04 & " " changed by ""
                lsResultListValue = tmpValue & "" & resultRecord.UOM
            Else
                'The result is a list
                Dim queryList = From prs In m_sdfResultExport.Lists() Where prs.ResultLinkID = resultRecord.Result_Id Order By prs.List_Id Ascending Select prs
                If queryList.Count > 0 Then
                    For Each listRecord In queryList
                        If listRecord.List_Column <> "" Then
                            If lsResultListValue <> "" Then lsResultListValue &= "|"
                            lsResultListValue &= listRecord.List_Column
                        End If
                    Next
                End If
            End If

            'MOD 15
            If lsResultListValue <> "" Then
                If lsResultText <> "" Then
                    lsResultListValue += " |\ " & lsResultText
                Else
                    lsResult = lsResultListValue
                End If
            Else
                lsResult = lsResultText
            End If

            Return lsResult
        End Function

#End Region

#Region "Dataset parsers"
        ''' <summary>
        ''' Extract the GCL results from the SDF database and add it to the msAccess database.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_Gcl_FromSdfToAccess()
            ' Filter the data if GasControl Line <> 0 and the lastResult = 0 (This means the result is added by INSPECTOR and not marked as a 
            ' result added during synchronisation)
            Dim query = From prs In m_sdfResultExport.InspectionResults() Where prs.GasControlLineName <> "" And prs.LastResult = 0 Select prs

            For Each prsSdfRecord In query

                ' Change the date for add to the msAccess database 
                Dim ldDateStart As Date = Now
                Dim ldTimeStart As Date = Now
                TransferSdfDateTime(prsSdfRecord.StartTime, ldDateStart, ldTimeStart)

                Dim ldDateEnd As Date = Now
                Dim ldTimeEnd As Date = Now
                TransferSdfDateTime(prsSdfRecord.EndTime, ldDateEnd, ldTimeEnd)

                'MOD 03 .Id added
                '            .Crc = prsSdfRecord.CRC, _
                Dim gclMsAccessRecord As New ResultsMDBEntities.DataAccess.tblResultGCL With { _
               .Id = m_tblResultGCL_id_New, _
               .PRSCode = prsSdfRecord.PRSCode, _
               .PRSName = prsSdfRecord.PRSName, _
               .GasControlLine = prsSdfRecord.GasControlLineName, _
               .PRSIdentification = prsSdfRecord.PRSIdentification, _
               .InspectionProcedure = prsSdfRecord.InspectionProcedureName, _
               .InspectionProcedureVersion = prsSdfRecord.InspectionProcedureVersion, _
               .Dst = prsSdfRecord.DST, _
               .TimeZone = prsSdfRecord.TimeZone, _
               .DateStart = ldDateStart, _
               .TimeStart = ldTimeStart, _
               .DateEnd = ldDateEnd, _
               .TimeEnd = ldTimeEnd, _
               .GCLIdentification = prsSdfRecord.GCLIdentification, _
               .IdDm1 = prsSdfRecord.ID_DM1, _
               .IdDm2 = prsSdfRecord.ID_DM2, _
               .BTAddress = prsSdfRecord.BT_Address _
               }

                ' Extract the results from Results and list table and add to the record.
                ExtractResults_Gcl_FromSdfToAccess(prsSdfRecord.InspectionResult_Id, gclMsAccessRecord)

                ' Adding the Record to the SDF database; Applied after submit changes
                m_msAccessResultImport.tblResultGCL.InsertOnSubmit(gclMsAccessRecord)

                ' Increase ID
                m_tblResultGCL_id_New += 1

            Next

            ' Submit the changes to the database. Done as last step for speed up saving
            Console.WriteLine("Write GCL data: " & Format(Now, "HH:mm:ss:fff"))
            m_msAccessResultImport.SubmitChanges()
            Console.WriteLine("Write GCL data gereed: " & Format(Now, "HH:mm:ss:fff"))

        End Sub


        ''' <summary>
        ''' For GCL Extract the data from the results table
        ''' </summary>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <param name="gclMsAccessRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_Gcl_FromSdfToAccess(liID_InspectionResults_ID As Integer, ByRef gclMsAccessRecord As ResultsMDBEntities.DataAccess.tblResultGCL)

            Dim query = From prs In m_sdfResultExport.Results1() Where prs.InspectionResultLinkID = liID_InspectionResults_ID Select prs
            If query.Count = 0 Then Exit Sub

            For Each resultRecord In query
                Dim lsResult As String = ExtractResultRecord(resultRecord)

                Select Case resultRecord.FieldNo
                    Case 1 : gclMsAccessRecord.Result1 = lsResult
                    Case 2 : gclMsAccessRecord.Result2 = lsResult
                    Case 3 : gclMsAccessRecord.Result3 = lsResult
                    Case 4 : gclMsAccessRecord.Result4 = lsResult
                    Case 5 : gclMsAccessRecord.Result5 = lsResult
                    Case 6 : gclMsAccessRecord.Result6 = lsResult
                    Case 7 : gclMsAccessRecord.Result7 = lsResult
                    Case 8 : gclMsAccessRecord.Result8 = lsResult
                    Case 9 : gclMsAccessRecord.Result9 = lsResult
                    Case 10 : gclMsAccessRecord.Result10 = lsResult
                    Case 11 : gclMsAccessRecord.Result11 = lsResult
                    Case 12 : gclMsAccessRecord.Result12 = lsResult
                    Case 13 : gclMsAccessRecord.Result13 = lsResult
                    Case 14 : gclMsAccessRecord.Result14 = lsResult
                    Case 15 : gclMsAccessRecord.Result15 = lsResult
                    Case 16 : gclMsAccessRecord.Result16 = lsResult
                    Case 17 : gclMsAccessRecord.Result17 = lsResult
                    Case 18 : gclMsAccessRecord.Result18 = lsResult
                    Case 19 : gclMsAccessRecord.Result19 = lsResult
                    Case 20 : gclMsAccessRecord.Result20 = lsResult
                    Case 21 : gclMsAccessRecord.Result21 = lsResult
                    Case 22 : gclMsAccessRecord.Result22 = lsResult
                    Case 23 : gclMsAccessRecord.Result23 = lsResult
                    Case 24 : gclMsAccessRecord.Result24 = lsResult
                    Case 25 : gclMsAccessRecord.Result25 = lsResult
                    Case 26 : gclMsAccessRecord.Result26 = lsResult
                    Case 27 : gclMsAccessRecord.Result27 = lsResult
                    Case 28 : gclMsAccessRecord.Result28 = lsResult
                    Case 29 : gclMsAccessRecord.Result29 = lsResult
                    Case 30 : gclMsAccessRecord.Result30 = lsResult
                    Case 31 : gclMsAccessRecord.Result31 = lsResult
                    Case 32 : gclMsAccessRecord.Result32 = lsResult
                    Case 33 : gclMsAccessRecord.Result33 = lsResult
                    Case 34 : gclMsAccessRecord.Result34 = lsResult
                    Case 35 : gclMsAccessRecord.Result35 = lsResult
                    Case 36 : gclMsAccessRecord.Result36 = lsResult
                    Case 37 : gclMsAccessRecord.Result37 = lsResult
                    Case 38 : gclMsAccessRecord.Result38 = lsResult
                    Case 39 : gclMsAccessRecord.Result39 = lsResult
                    Case 40 : gclMsAccessRecord.Result40 = lsResult
                    Case 41 : gclMsAccessRecord.Result41 = lsResult
                    Case 42 : gclMsAccessRecord.Result42 = lsResult
                    Case 43 : gclMsAccessRecord.Result43 = lsResult
                    Case 44 : gclMsAccessRecord.Result44 = lsResult
                    Case 45 : gclMsAccessRecord.Result45 = lsResult
                    Case 46 : gclMsAccessRecord.Result46 = lsResult
                    Case 47 : gclMsAccessRecord.Result47 = lsResult
                    Case 48 : gclMsAccessRecord.Result48 = lsResult
                    Case 49 : gclMsAccessRecord.Result49 = lsResult
                    Case 50 : gclMsAccessRecord.Result50 = lsResult
                    Case 51 : gclMsAccessRecord.Result51 = lsResult
                    Case 52 : gclMsAccessRecord.Result52 = lsResult
                    Case 53 : gclMsAccessRecord.Result53 = lsResult
                    Case 54 : gclMsAccessRecord.Result54 = lsResult
                    Case 55 : gclMsAccessRecord.Result55 = lsResult
                    Case 56 : gclMsAccessRecord.Result56 = lsResult
                    Case 57 : gclMsAccessRecord.Result57 = lsResult
                    Case 58 : gclMsAccessRecord.Result58 = lsResult
                    Case 59 : gclMsAccessRecord.Result59 = lsResult
                    Case 60 : gclMsAccessRecord.Reserved1 = lsResult
                    Case 61 : gclMsAccessRecord.Reserved2 = lsResult
                    Case 62 : gclMsAccessRecord.Reserved3 = lsResult
                    Case 63 : gclMsAccessRecord.Reserved4 = lsResult
                    Case 64 : gclMsAccessRecord.Reserved5 = lsResult
                    Case 65 : gclMsAccessRecord.Reserved6 = lsResult
                    Case 66 : gclMsAccessRecord.Reserved7 = lsResult
                    Case 67 : gclMsAccessRecord.Reserved8 = lsResult
                    Case 68 : gclMsAccessRecord.Reserved9 = lsResult
                    Case 69 : gclMsAccessRecord.Reserved10 = lsResult
                End Select
            Next

        End Sub

#End Region

#Region "Save SDF Results in tblResultPRS"
        ''' <summary>
        ''' Extract the PRS results from the SDF database and add it to the msAccess database.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_Prs_FromSdfToAccess()
            ' Filter the data if GasControl Line = 0 and the lastResult = 0 (This means the result is added by INSPECTOR and not marked as a 
            ' result added during synchronisation)
            Dim query = From prs In m_sdfResultExport.InspectionResults() Where prs.GasControlLineName = "" And prs.LastResult = 0 Select prs

            Dim recordcounterTotal = query.Count
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Result, recordcounterTotal, recordcounterProcessed, DbCreateStatus.StartedCreate, "")
            For Each prsSdfRecord In query

                ' Change the date for add to the msAccess database 
                Dim ldDateStart As Date = Now
                Dim ldTimeStart As Date = Now
                TransferSdfDateTime(prsSdfRecord.StartTime, ldDateStart, ldTimeStart)

                Dim ldDateEnd As Date = Now
                Dim ldTimeEnd As Date = Now
                TransferSdfDateTime(prsSdfRecord.EndTime, ldDateEnd, ldTimeEnd)

                '                              .Crc = prsSdfRecord.CRC, _
                Dim prsMsAccessRecord As New ResultsMDBEntities.DataAccess.tblResultPRS With { _
               .Id = m_tblResultPRS_id_New, _
               .PRSCode = prsSdfRecord.PRSCode, _
               .PRSName = prsSdfRecord.PRSName, _
               .InspectionProcedure = prsSdfRecord.InspectionProcedureName, _
               .InspectionProcedureVersion = prsSdfRecord.InspectionProcedureVersion, _
               .Dst = prsSdfRecord.DST, _
               .TimeZone = prsSdfRecord.TimeZone, _
               .DateStart = ldDateStart, _
               .TimeStart = ldTimeStart, _
               .DateEnd = ldDateEnd, _
               .TimeEnd = ldTimeEnd, _
               .PRSIdentification = prsSdfRecord.PRSIdentification _
               }

                ' Extract the results from Results and list table and add to the record.
                ExtractResults_Prs_FromSdfToAccess(prsSdfRecord.InspectionResult_Id, prsMsAccessRecord)

                ' Adding the Record to the SDF database
                m_msAccessResultImport.tblResultPRS.InsertOnSubmit(prsMsAccessRecord)

                ' Increase ID
                m_tblResultPRS_id_New += 1

            Next

            'Save all settings at once. This is done for performance.
            Debug.Print("Write results data: " & Format(Now, "HH:mm:ss:fff"))
            m_msAccessResultImport.SubmitChanges()
            Debug.Print("Write resultn PRS data Completed: " & Format(Now, "HH:mm:ss:fff"))

        End Sub
        ''' <summary>
        ''' For GCL Extract the data from the results table
        ''' </summary>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <param name="prsMsAccessRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_Prs_FromSdfToAccess(ByVal liID_InspectionResults_ID As Integer, ByVal prsMsAccessRecord As ResultsMDBEntities.DataAccess.tblResultPRS)

            Dim query = From prs In m_sdfResultExport.Results1() Where prs.InspectionResultLinkID = liID_InspectionResults_ID Select prs
            If query.Count = 0 Then Exit Sub

            For Each resultRecord In query
                Dim lsResult As String = ExtractResultRecord(resultRecord)
                Select Case resultRecord.FieldNo
                    Case 1 : prsMsAccessRecord.Result1 = lsResult
                    Case 2 : prsMsAccessRecord.Result2 = lsResult
                    Case 3 : prsMsAccessRecord.Result3 = lsResult
                    Case 4 : prsMsAccessRecord.Result4 = lsResult
                    Case 5 : prsMsAccessRecord.Result5 = lsResult
                    Case 6 : prsMsAccessRecord.Result6 = lsResult
                    Case 7 : prsMsAccessRecord.Result7 = lsResult
                    Case 8 : prsMsAccessRecord.Result8 = lsResult
                    Case 9 : prsMsAccessRecord.Result9 = lsResult
                    Case 10 : prsMsAccessRecord.Result10 = lsResult
                    Case 11 : prsMsAccessRecord.Result11 = lsResult
                    Case 12 : prsMsAccessRecord.Result12 = lsResult
                    Case 13 : prsMsAccessRecord.Result13 = lsResult
                    Case 14 : prsMsAccessRecord.Result14 = lsResult
                    Case 15 : prsMsAccessRecord.Result15 = lsResult
                    Case 16 : prsMsAccessRecord.Result16 = lsResult
                    Case 17 : prsMsAccessRecord.Result17 = lsResult
                    Case 18 : prsMsAccessRecord.Result18 = lsResult
                    Case 19 : prsMsAccessRecord.Result19 = lsResult
                    Case 20 : prsMsAccessRecord.Result20 = lsResult
                    Case 21 : prsMsAccessRecord.Result21 = lsResult
                    Case 22 : prsMsAccessRecord.Result22 = lsResult
                    Case 23 : prsMsAccessRecord.Result23 = lsResult
                    Case 24 : prsMsAccessRecord.Result24 = lsResult
                    Case 25 : prsMsAccessRecord.Result25 = lsResult
                    Case 26 : prsMsAccessRecord.Result26 = lsResult
                    Case 27 : prsMsAccessRecord.Result27 = lsResult
                    Case 28 : prsMsAccessRecord.Result28 = lsResult
                    Case 29 : prsMsAccessRecord.Result29 = lsResult
                    Case 30 : prsMsAccessRecord.Result30 = lsResult
                    Case 31 : prsMsAccessRecord.Result31 = lsResult
                    Case 32 : prsMsAccessRecord.Result32 = lsResult
                    Case 33 : prsMsAccessRecord.Result33 = lsResult
                    Case 34 : prsMsAccessRecord.Result34 = lsResult
                    Case 35 : prsMsAccessRecord.Result35 = lsResult
                    Case 36 : prsMsAccessRecord.Result36 = lsResult
                    Case 37 : prsMsAccessRecord.Result37 = lsResult
                    Case 38 : prsMsAccessRecord.Result38 = lsResult
                    Case 39 : prsMsAccessRecord.Result39 = lsResult
                    Case 40 : prsMsAccessRecord.Result40 = lsResult
                    Case 41 : prsMsAccessRecord.Result41 = lsResult
                    Case 42 : prsMsAccessRecord.Result42 = lsResult
                    Case 43 : prsMsAccessRecord.Result43 = lsResult
                    Case 44 : prsMsAccessRecord.Result44 = lsResult
                    Case 45 : prsMsAccessRecord.Result45 = lsResult
                    Case 46 : prsMsAccessRecord.Result46 = lsResult
                    Case 47 : prsMsAccessRecord.Result47 = lsResult
                    Case 48 : prsMsAccessRecord.Result48 = lsResult
                    Case 49 : prsMsAccessRecord.Result49 = lsResult
                    Case 50 : prsMsAccessRecord.Result50 = lsResult
                    Case 51 : prsMsAccessRecord.Result51 = lsResult
                    Case 52 : prsMsAccessRecord.Result52 = lsResult
                    Case 53 : prsMsAccessRecord.Result53 = lsResult
                    Case 54 : prsMsAccessRecord.Result54 = lsResult
                    Case 55 : prsMsAccessRecord.Result55 = lsResult
                    Case 56 : prsMsAccessRecord.Result56 = lsResult
                    Case 57 : prsMsAccessRecord.Result57 = lsResult
                    Case 58 : prsMsAccessRecord.Result58 = lsResult
                    Case 59 : prsMsAccessRecord.Result59 = lsResult
                    Case 60 : prsMsAccessRecord.Reserved1 = lsResult
                    Case 61 : prsMsAccessRecord.Reserved2 = lsResult
                    Case 62 : prsMsAccessRecord.Reserved3 = lsResult
                    Case 63 : prsMsAccessRecord.Reserved4 = lsResult
                    Case 64 : prsMsAccessRecord.Reserved5 = lsResult
                    Case 65 : prsMsAccessRecord.Reserved6 = lsResult
                    Case 66 : prsMsAccessRecord.Reserved7 = lsResult
                    Case 67 : prsMsAccessRecord.Reserved8 = lsResult
                    Case 68 : prsMsAccessRecord.Reserved9 = lsResult
                    Case 69 : prsMsAccessRecord.Reserved10 = lsResult
                End Select
            Next

        End Sub

#End Region
    End Class
End Namespace