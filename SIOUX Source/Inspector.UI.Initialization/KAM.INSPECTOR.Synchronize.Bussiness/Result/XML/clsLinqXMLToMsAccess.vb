'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

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
    Public Class clsLinqXMLToMsAccess
#Region "Constants"
        Private Const InspectionResult_TABLE_NAME As String = "InspectionResult"
        Private Const GCL_TABLE_NAME As String = "GasControlLine"
#End Region

#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        ' Primary key counters
        Private m_tblResultGCL_id_New As Integer = 0
        Private m_tblResultPRS_id_New As Integer = 0


        ' Voor Access tabel om aan toe te voegen
        Private m_msAccessResultImport As ResultMdbEntities.DataAccess.AccessInspectionResults
        Private m_EntitiesResultExport As New Model.Result.Entities.InspectionResultsDataEntity
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
        ''' <param name="xmlFile"></param>
        ''' <param name="xsdFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal xmlFile As String, ByVal xsdFile As String, filenNameMsAccessDatabase As String)
            LoadWriteResults(xmlFile, xsdFile, filenNameMsAccessDatabase)
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
        ''' Import the results into the database
        ''' </summary>
        ''' <param name="xmlFile"></param>
        ''' <param name="xsdFile"></param>
        ''' <param name="filenNameMsAccessDatabase"></param>
        ''' <remarks></remarks>
        Public Sub LoadWriteResults(ByVal xmlFile As String, ByVal xsdFile As String, filenNameMsAccessDatabase As String)
            If m_EntitiesResultExport.InspectionResults IsNot Nothing Then m_EntitiesResultExport.InspectionResults.Clear()
            ReadInspectionResults(xmlFile, xsdFile, filenNameMsAccessDatabase)
        End Sub
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

        ''' <summary>
        ''' Extracting a results record from XML to MsAccess format.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <param name="resultId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ExtractResultRecord(ByVal Dataset As DataSet, resultId As Integer) As String
            Dim lsResult As String = ""
            'Search in measurevalue.
            Dim foundRowsMeasureValue() As DataRow
            foundRowsMeasureValue = Dataset.Tables("MeasureValue").Select("Result_Id=" & resultId)
            If foundRowsMeasureValue.Length > 0 Then
                'Decimal separator is handled because the value is set as double
                lsResult = foundRowsMeasureValue(0).Item("Value") & "" & foundRowsMeasureValue(0).Item("UOM")
            End If

            ' Maak een recordset met alleen de LIST resultaten voor deze Result
            Dim foundRowsList() As DataRow
            foundRowsList = Dataset.Tables("List").Select("Result_ID=" & resultId)

            ' indien er wat gevonden is verwerken
            If foundRowsList.Length > 0 Then
                For ljLoop As Integer = 0 To foundRowsList.Length - 1
                    If lsResult <> "" Then lsResult &= "|"
                    lsResult &= foundRowsList(ljLoop).Item("List_Column").ToString()
                Next
            End If

            Return lsResult
        End Function
#End Region

#Region "Dataset parsers"
        ''' <summary>
        ''' Reads the InspectionResult information into model.Results.Entities.InspectionResultEntities
        ''' </summary>
        ''' <param name="xmlFile">Xml file to read</param>
        ''' <param name="xsdFile">XSD file</param>
        ''' <remarks></remarks>
        Private Sub ReadInspectionResults(ByVal xmlFile As String, ByVal xsdFile As String, filenNameMsAccessDatabase As String)
            If File.Exists(xmlFile) = False Then
                xmlFile = Nothing
            End If

            If xmlFile <> Nothing Then
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
            End If

            Using dataSet As New DataSet()
                dataSet.Locale = CultureInfo.InvariantCulture
                dataSet.ReadXmlSchema(xsdFile)
                If xmlFile <> Nothing Then
                    dataSet.ReadXml(xmlFile, XmlReadMode.ReadSchema)
                    ' Reading the MsAccess results database
                    m_msAccessResultImport = New ResultMdbEntities.DataAccess.AccessInspectionResults(filenNameMsAccessDatabase)
                    If m_msAccessResultImport.DatabaseExists = False Then
                        'Create new database
                        Dim pathName As String
                        pathName = Path.GetDirectoryName(filenNameMsAccessDatabase)
                        If Not CheckFileExistsPC(pathName) Then Directory.CreateDirectory(pathName)
                        m_msAccessResultImport.CreateDatabase()
                    End If
                    ' Bepaal de hoogste ID uit de beide tabellen om hiermee door te gaan tellen
                    Call GetNewTableID()

                    ExtractInspectionResults_Prs_FromXmlToAccess(dataSet)
                    ExtractInspectionResults_Gcl_FromXmlToAccess(dataSet)

                    Console.WriteLine("Write GCL data: " & Format(Now, "HH:mm:ss:fff"))
                    m_msAccessResultImport.SubmitChanges()
                    Console.WriteLine("Write GCL data gereed: " & Format(Now, "HH:mm:ss:fff"))
                Else
                End If
            End Using


        End Sub
#End Region
#Region "Handling of Prs results"
        ''' <summary>
        ''' Extracts the InspectionResults objects.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_Prs_FromXmlToAccess(ByVal dataset As DataSet)
            Dim prsTable As DataTable = dataset.Tables(InspectionResult_TABLE_NAME)

            Dim query = From prs In prsTable.Rows Where prs("GasControlLineName").ToString = "" Select prs

            ' Doorloop nu alle records-rows in de tabel
            For Each ipXmlResultsRecord In query 'DataRow In prsTable.Rows

                'Check if all results should be added or check the status of the result.
                If m_AppendAllResults = True Then
                ElseIf (ipXmlResultsRecord("Status") = 3 Or ipXmlResultsRecord("Status") = 4 Or ipXmlResultsRecord("Status") = 5) Then
                Else
                    Continue For
                End If

                Dim prsMsAccessRecord As New ResultsMDBEntities.DataAccess.tblResultPRS With { _
                .Id = m_tblResultPRS_id_New, _
                .PRSName = ipXmlResultsRecord("PRSName").ToString(), _
                .PRSIdentification = ipXmlResultsRecord("PRSIdentification").ToString(), _
                .PRSCode = ipXmlResultsRecord("PRSCode").ToString() _
                }


                'Getting the inspectionprocedure infomartion
                Dim foundRowsInspectionProcedure() As DataRow
                foundRowsInspectionProcedure = dataset.Tables("InspectionProcedure").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsInspectionProcedure.Length > 0 Then
                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    prsMsAccessRecord.InspectionProcedure = foundRowsInspectionProcedure(0).Item("Name")
                    prsMsAccessRecord.InspectionProcedureVersion = foundRowsInspectionProcedure(0).Item("Version")
                Else
                    prsMsAccessRecord.InspectionProcedure = ""
                    prsMsAccessRecord.InspectionProcedureVersion = ""
                End If

                'Getting the date time stemp
                Dim foundRowsDateTimeStamp() As DataRow
                foundRowsDateTimeStamp = dataset.Tables("DateTimeStamp").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsDateTimeStamp.Length > 0 Then
                    'Set the dateTimeStampl Only one
                    prsMsAccessRecord.DateStart = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate"))
                    prsMsAccessRecord.TimeStart = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("StartTime"))
                    prsMsAccessRecord.DateEnd = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate"))
                    prsMsAccessRecord.TimeEnd = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("EndTime"))

                    Dim foundRowsTimeSettings() As DataRow
                    foundRowsTimeSettings = dataset.Tables("TimeSettings").Select("DateTimeStamp_ID=" & foundRowsDateTimeStamp(0).Item("DateTimeStamp_ID"))
                    prsMsAccessRecord.TimeZone = CastToTypeTimeZoneValueOrDefault(foundRowsTimeSettings(0).Item("TimeZone"))
                    prsMsAccessRecord.Dst = CastToTypeDstValueOrDefault(foundRowsTimeSettings(0).Item("DST"))
                End If


                ' Zoek nu de resultaten van deze InspectionResult erbij en voeg deze toe aan de Results Entiteit van InspectionResult
                ExtractResults_prs_FromXmlToAccess(dataset, ipXmlResultsRecord("InspectionResult_ID").ToString(), prsMsAccessRecord)
                ' Wijs het resultaat nu toe aan prsEntityResults list

                m_msAccessResultImport.tblResultPRS.InsertOnSubmit(prsMsAccessRecord)

                ' Increase ID
                m_tblResultPRS_id_New += 1
            Next

        End Sub

        ' ''' <summary>
        ' ''' Injects the PRS-to-gascontrolline relation.
        ' ''' </summary>
        ' ''' <param name="dataSet">The data set.</param>
        'Private Shared Sub InjectPrsToGasControlLineRelation(ByVal dataSet As DataSet)
        '    Dim parentKeyColumns As DataColumn() = New DataColumn() {dataSet.Tables(InspectionResult_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSet.Tables(InspectionResult_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
        '    Dim childKeyColumns As DataColumn() = New DataColumn() {dataSet.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSet.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
        '    Dim relation As New DataRelation(InspectionResult_Result_RELATION_NAME, parentKeyColumns, childKeyColumns)
        '    dataSet.Relations.Add(relation)
        'End Sub

        ''' <summary>
        ''' Extracts the Results object.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <param name="gclMsAccessRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_prs_FromXmlToAccess(ByVal dataset As DataSet, ByVal liID_InspectionResults_ID As Integer, ByRef gclMsAccessRecord As ResultsMDBEntities.DataAccess.tblResultPRS)

            'Create a recordset with all the results of InspectionResult
            Dim foundRowsResult() As DataRow
            foundRowsResult = dataset.Tables("Result").Select("InspectionResult_ID=" & liID_InspectionResults_ID)

            'Is something is found; extract the data
            If foundRowsResult.Length > 0 Then

                For liLoop = 0 To foundRowsResult.Length - 1

                    'MOD 35
                    Dim lsResult As String = ""
                    Dim lsResultListValue As String = ""
                    lsResultListValue = ExtractResultRecord(dataset, foundRowsResult(liLoop).Item("Result_Id"))

                    Dim lsResultText As String = ""
                    If Not IsDBNull(foundRowsResult(liLoop).Item("Text")) Then
                        lsResultText = foundRowsResult(liLoop).Item("Text").ToString()
                    End If

                    If lsResultListValue <> "" Then
                        If lsResultText <> "" Then
                            lsResult = lsResultListValue & " |\ " & lsResultText
                        Else
                            lsResult = lsResultListValue
                        End If
                    Else
                        lsResult = lsResultText
                    End If

                    'MOD 35 Dim lsResult As String = ""
                    'MOD 35 If Not IsDBNull(foundRowsResult(liLoop).Item("Text")) Then
                    'MOD 35 lsResult = foundRowsResult(liLoop).Item("Text").ToString()
                    'MOD 35 Else
                    'MOD 35     lsResult = ExtractResultRecord(dataset, foundRowsResult(liLoop).Item("Result_Id"))
                    'MOD 35 End If

                    Select Case foundRowsResult(liLoop).Item("FieldNo").ToString()
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
            End If
        End Sub
#End Region


#Region "Handling of GCL results"
        ''' <summary>
        ''' Extracts the InspectionResults objects.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults_Gcl_FromXmlToAccess(ByVal dataset As DataSet)
            Dim prsTable As DataTable = dataset.Tables(InspectionResult_TABLE_NAME)

            Dim query = From prs In prsTable.Rows Where prs("GasControlLineName").ToString <> "" Select prs

            ' Doorloop nu alle records-rows in de tabel
            For Each ipXmlResultsRecord In query 'DataRow In prsTable.Rows

                'Check if all results should be added or check the status of the result.
                If m_AppendAllResults = True Then
                ElseIf (ipXmlResultsRecord("Status") = 3 Or ipXmlResultsRecord("Status") = 4 Or ipXmlResultsRecord("Status") = 5) Then
                Else
                    Continue For
                End If

                Dim gclMsAccessRecord As New ResultsMDBEntities.DataAccess.tblResultGCL With { _
                .Id = m_tblResultGCL_id_New, _
                .PRSName = ipXmlResultsRecord("PRSName").ToString(), _
                .PRSIdentification = ipXmlResultsRecord("PRSIdentification").ToString(), _
                .PRSCode = ipXmlResultsRecord("PRSCode").ToString() _
                }

                'Check if GasControlLineName contains any name.
                If ipXmlResultsRecord("GasControlLineName").ToString() <> "" Then
                    gclMsAccessRecord.GasControlLine = ipXmlResultsRecord("GasControlLineName").ToString()
                    gclMsAccessRecord.GCLIdentification = ipXmlResultsRecord("GCLIdentification").ToString()
                    'prsSqlRecord.GCLCode = ipXmlResultsRecord("GCLCode").ToString()
                End If

                'Getting the inspectionprocedure infomartion
                Dim foundRowsInspectionProcedure() As DataRow
                foundRowsInspectionProcedure = dataset.Tables("InspectionProcedure").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsInspectionProcedure.Length > 0 Then
                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    gclMsAccessRecord.InspectionProcedure = foundRowsInspectionProcedure(0).Item("Name")
                    gclMsAccessRecord.InspectionProcedureVersion = foundRowsInspectionProcedure(0).Item("Version")
                Else
                    gclMsAccessRecord.InspectionProcedure = ""
                    gclMsAccessRecord.InspectionProcedureVersion = ""
                End If

                'Getting the measurement equipment
                Dim foundRowsMeasurementEquipment() As DataRow
                foundRowsMeasurementEquipment = dataset.Tables("Measurement_Equipment").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                'Optional XML element
                If foundRowsMeasurementEquipment.Length > 0 Then
                    'Read the Measurement_Equipment for this InspectionResult; Always only one
                    gclMsAccessRecord.IdDm1 = foundRowsMeasurementEquipment(0).Item("ID_DM1")
                    gclMsAccessRecord.IdDm2 = foundRowsMeasurementEquipment(0).Item("ID_DM2")
                    gclMsAccessRecord.BTAddress = foundRowsMeasurementEquipment(0).Item("BT_Address")
                End If

                'Getting the date time stemp
                Dim foundRowsDateTimeStamp() As DataRow
                foundRowsDateTimeStamp = dataset.Tables("DateTimeStamp").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsDateTimeStamp.Length > 0 Then
                    'Set the dateTimeStampl Only one
                    gclMsAccessRecord.DateStart = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate"))
                    gclMsAccessRecord.TimeStart = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("StartTime"))
                    gclMsAccessRecord.DateEnd = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate"))
                    gclMsAccessRecord.TimeEnd = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("EndTime"))

                    Dim foundRowsTimeSettings() As DataRow
                    foundRowsTimeSettings = dataset.Tables("TimeSettings").Select("DateTimeStamp_ID=" & foundRowsDateTimeStamp(0).Item("DateTimeStamp_ID"))
                    gclMsAccessRecord.TimeZone = CastToTypeTimeZoneValueOrDefault(foundRowsTimeSettings(0).Item("TimeZone"))
                    gclMsAccessRecord.Dst = CastToTypeDstValueOrDefault(foundRowsTimeSettings(0).Item("DST"))
                End If


                ' Zoek nu de resultaten van deze InspectionResult erbij en voeg deze toe aan de Results Entiteit van InspectionResult
                ExtractResults_Gcl_FromXmlToAccess(dataset, ipXmlResultsRecord("InspectionResult_ID").ToString(), gclMsAccessRecord)
                ' Wijs het resultaat nu toe aan prsEntityResults list

                m_msAccessResultImport.tblResultGCL.InsertOnSubmit(gclMsAccessRecord)

                ' Increase ID
                m_tblResultGCL_id_New += 1
            Next
        End Sub

        ' ''' <summary>
        ' ''' Injects the PRS-to-gascontrolline relation.
        ' ''' </summary>
        ' ''' <param name="dataSet">The data set.</param>
        'Private Shared Sub InjectPrsToGasControlLineRelation(ByVal dataSet As DataSet)
        '    Dim parentKeyColumns As DataColumn() = New DataColumn() {dataSet.Tables(InspectionResult_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSet.Tables(InspectionResult_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
        '    Dim childKeyColumns As DataColumn() = New DataColumn() {dataSet.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART1_OF_2), dataSet.Tables(GCL_TABLE_NAME).Columns(PRSGCL_PRIMARY_KEY_PART2_OF_2)}
        '    Dim relation As New DataRelation(InspectionResult_Result_RELATION_NAME, parentKeyColumns, childKeyColumns)
        '    dataSet.Relations.Add(relation)
        'End Sub

        ''' <summary>
        ''' Extracts the Results object.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <param name="liID_InspectionResults_ID"></param>
        ''' <param name="gclMsAccessRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResults_Gcl_FromXmlToAccess(ByVal dataset As DataSet, ByVal liID_InspectionResults_ID As Integer, ByRef gclMsAccessRecord As ResultsMDBEntities.DataAccess.tblResultGCL)

            'Create a recordset with all the results of InspectionResult
            Dim foundRowsResult() As DataRow
            foundRowsResult = dataset.Tables("Result").Select("InspectionResult_ID=" & liID_InspectionResults_ID)

            'Is something is found; extract the data
            If foundRowsResult.Length > 0 Then

                For liLoop = 0 To foundRowsResult.Length - 1
                    Dim lsResult As String = ""
                    Dim lsResultListValue As String = ""
                    lsResultListValue = ExtractResultRecord(dataset, foundRowsResult(liLoop).Item("Result_Id"))

                    'MOD 13
                    Dim lsResultText As String = ""
                    If Not IsDBNull(foundRowsResult(liLoop).Item("Text")) Then
                        lsResultText = foundRowsResult(liLoop).Item("Text").ToString()
                    End If

                    If lsResultListValue <> "" Then
                        If lsResultText <> "" Then
                            'MOD 35
                            lsResult = lsResultListValue & " |\ " & lsResultText
                        Else
                            lsResult = lsResultListValue
                        End If
                    Else
                        lsResult = lsResultText
                    End If

                    Select Case foundRowsResult(liLoop).Item("FieldNo").ToString()
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
            End If
        End Sub
#End Region


    End Class
End Namespace




