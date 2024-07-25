'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Xml.Serialization
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Result
    ''' <summary>
    ''' Information of PRS and contained GasControlLines
    ''' </summary>
    Public Class clsLinqXmlToXml
#Region "Constants"
        Private Const InspectionResult_TABLE_NAME As String = "InspectionResult"
        Private Const GCL_TABLE_NAME As String = "GasControlLine"
#End Region

#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError


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

        Public ReadOnly Property EntitiesResultImport As Model.Result.Entities.InspectionResultsDataEntity
            Get
                Return m_EntitiesResultImport
            End Get
        End Property
        Private m_EntitiesResultImport As New Model.Result.Entities.InspectionResultsDataEntity


#End Region

#Region "Constructors"
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="xmlFile"></param>
        ''' <param name="xsdFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal xmlFile As String, ByVal xsdFile As String)
            LoadResults(xmlFile, xsdFile)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub
#End Region

#Region "Public"
        Public Function LoadResults(ByVal xmlFile As String, ByVal xsdFile As String) As Boolean
            Dim xmlDataset As DataSet
            Dim ioFileWriteTime As DateTime
            ReadInspectionResults(xmlFile, xsdFile, xmlDataset, ioFileWriteTime)
            ExtractInspectionResults(xmlDataset, ioFileWriteTime)
            Return True
        End Function
        Public Function ClearResults() As Boolean
            m_EntitiesResultImport.InspectionResults.Clear()
            Return True
        End Function

        ''' <summary>
        ''' Write the entity into a XML file
        ''' </summary>
        ''' <param name="xmlFile">XML file to write</param>
        ''' <param name="xsdFile">XSD file for xml validation</param>
        ''' <remarks></remarks>
        Public Function WriteResults(ByVal xmlFile As String, xsdFile As String) As Boolean
            Dim objStreamWriter As New StreamWriter(xmlFile)
            Dim x As New XmlSerializer(m_EntitiesResultImport.GetType)
            x.Serialize(objStreamWriter, m_EntitiesResultImport)
            objStreamWriter.Close()

            ' Check if the create file is correct.
            Try
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
                Debug.Print("Write complete: " & Format(Now, "HH:mm:ss:fff"))
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, 0, 0, clsDbGeneral.DbCreateStatus.SuccesWrite, "!Saving data completed")
                Return True
            Catch ex As Exception
                'MOD 21
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Result, 0, 0, clsDbGeneral.DbCreateStatus.ErrorXsd, xmlFile)
                Return False
            End Try
        End Function
#End Region

#Region "Dataset parse"
        ''' <summary>
        ''' Extracts the InspectionResults objects.
        ''' </summary>
        ''' <param name="Dataset"></param>
        ''' <param name="fileDate"></param>
        ''' <remarks></remarks>
        Private Sub ExtractInspectionResults(ByVal dataset As DataSet, ByVal fileDate As DateTime)
            ' Dim ipResultsList As New List(Of Model.Result.Entities.InspectionResultsEntity)

            ' Stel een datatable in voor de InspectionResults tabel
            Dim resultsTable As DataTable = dataset.Tables(InspectionResult_TABLE_NAME)

            ' Doorloop nu alle records-rows in de tabel
            For Each ipXmlResultsRecord As DataRow In resultsTable.Rows

                'Check if all results should be added or check the status of the result.
                If m_AppendAllResults = True Then
                ElseIf (ipXmlResultsRecord("Status") = 3 Or ipXmlResultsRecord("Status") = 4 Or ipXmlResultsRecord("Status") = 5) Then
                Else
                    Continue For
                End If

                ' Wijs nu alle data uit de tabel regel en de uit andere tabellen verzamelde informatie toe aan het object
                Dim ipResultEntityRecord As New Model.Result.Entities.InspectionResultsEntity() With { _
                    .FileDate = fileDate.ToUniversalTime, _
                    .InspectionResult_Id = ipXmlResultsRecord("InspectionResult_Id").ToString(), _
                    .Status = ipXmlResultsRecord("Status").ToString(), _
                    .PRSName = ipXmlResultsRecord("PRSName").ToString(), _
                    .PRSIdentification = ipXmlResultsRecord("PRSIdentification").ToString(), _
                    .PRSCode = ipXmlResultsRecord("PRSCode").ToString(), _
                    .CRC = ipXmlResultsRecord("CRC").ToString(), _
                    .LastResult = "0" _
                }

                'Check if GasControlLineName contains any name.
                If ipXmlResultsRecord("GasControlLineName").ToString() <> "" Then
                    ipResultEntityRecord.GasControlLineName = ipXmlResultsRecord("GasControlLineName").ToString()
                    ipResultEntityRecord.GCLIdentification = ipXmlResultsRecord("GCLIdentification").ToString()
                    ipResultEntityRecord.GCLCode = CastToStringOrEmpty(ipXmlResultsRecord("GCLCode"))
                End If

                'Getting the inspectionprocedure information
                Dim foundRowsInspectionProcedure() As DataRow
                foundRowsInspectionProcedure = dataset.Tables("InspectionProcedure").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsInspectionProcedure.Length > 0 Then
                    'Read the InspectionProcedure for this InspectionResult; Always only one)
                    Dim inspectionProcedureRecord As New Model.Result.Entities.InspectionProcedureEntity() With { _
                        .InspectionProcedureName = foundRowsInspectionProcedure(0).Item("Name"), _
                        .InspectionProcedureVersion = foundRowsInspectionProcedure(0).Item("Version") _
                    }
                    'Assign object to active InspectionResultObject
                    ipResultEntityRecord.InspectionProcedure.Add(inspectionProcedureRecord)
                Else
                    'It is no optional parameter
                    Dim inspectionProcedureRecord As New Model.Result.Entities.InspectionProcedureEntity() With { _
                        .InspectionProcedureName = "", _
                        .InspectionProcedureVersion = "" _
                        }
                    'Assign object to active InspectionResultObject
                    ipResultEntityRecord.InspectionProcedure.Add(inspectionProcedureRecord)
                End If

                'Getting the measurement equipment
                Dim foundRowsMeasurementEquipment() As DataRow
                foundRowsMeasurementEquipment = dataset.Tables("Measurement_Equipment").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                'Optional XML element
                If foundRowsMeasurementEquipment.Length > 0 Then
                    'Read the Measurement_Equipment for this InspectionResult; Always only one
                    Dim measurementEquipmentEntityRecord As New Model.Result.Entities.Measurement_EquipmentEntity() With { _
                        .ID_DM1 = foundRowsMeasurementEquipment(0).Item("ID_DM1"), _
                        .ID_DM2 = foundRowsMeasurementEquipment(0).Item("ID_DM2"), _
                        .BT_Address = foundRowsMeasurementEquipment(0).Item("BT_Address") _
                    }
                    'Assign object to active InspectionResultObject
                    ipResultEntityRecord.Measurement_Equipment.Add(measurementEquipmentEntityRecord)
                End If

                'Getting the date time stemp
                Dim foundRowsDateTimeStamp() As DataRow
                foundRowsDateTimeStamp = dataset.Tables("DateTimeStamp").Select("InspectionResult_ID=" & ipXmlResultsRecord("InspectionResult_ID"))
                If foundRowsDateTimeStamp.Length > 0 Then
                    'Set the dateTimeStampl Only one
                    Dim dateTimeStampRecord As New Model.Result.Entities.DateTimeStampEntity() With { _
                        .StartDate = CastToTypeDateValueOrDefault(foundRowsDateTimeStamp(0).Item("StartDate")), _
                        .StartTime = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("StartTime")), _
                        .EndTime = CastToTypeTimeValueOrDefault(foundRowsDateTimeStamp(0).Item("EndTime")) _
                    }
                    Dim foundRowsTimeSettings() As DataRow
                    foundRowsTimeSettings = dataset.Tables("TimeSettings").Select("DateTimeStamp_ID=" & foundRowsDateTimeStamp(0).Item("DateTimeStamp_ID"))
                    Dim dateTimeStampTimesettings As New Model.Result.Entities.TimeSettingsEntity() With { _
                        .TimeZone = CastToTypeTimeZoneValueOrDefault(foundRowsTimeSettings(0).Item("TimeZone")), _
                        .DST = CastToTypeDstValueOrDefault(foundRowsTimeSettings(0).Item("DST")) _
                    }
                    dateTimeStampRecord.TimeSettings.Add(dateTimeStampTimesettings)
                    'Assign object to active InspectionResultObject
                    ipResultEntityRecord.DateTimeStamp.Add(dateTimeStampRecord)
                End If

                ' Zoek nu de resultaten van deze InspectionResult erbij en voeg deze toe aan de Results Entiteit van InspectionResult
                ExtractResultsObject(dataset, ipXmlResultsRecord("InspectionResult_ID").ToString(), ipResultEntityRecord)
                ' Wijs het resultaat nu toe aan prsEntityResults list

                'ipResultsList.Add(ipResultEntityRecord)
                m_EntitiesResultImport.InspectionResults.Add(ipResultEntityRecord)
            Next
            'm_EntitiesResultImport.InspectionResults = ipResultsList
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
        ''' <param name="ipResultEntityRecord"></param>
        ''' <remarks></remarks>
        Private Sub ExtractResultsObject(ByVal Dataset As DataSet, ByVal liID_InspectionResults_ID As String, ByRef ipResultEntityRecord As Model.Result.Entities.InspectionResultsEntity)

            'Create a recordset with all the results of InspectionResult
            Dim foundRowsResult() As DataRow
            FoundRowsResult = Dataset.Tables("Result").Select("InspectionResult_ID=" & liID_InspectionResults_ID)

            'Is something is found; extract the data
            If FoundRowsResult.Length > 0 Then
                For liLoop = 0 To FoundRowsResult.Length - 1
                    ' Wijs nu alle data uit de tabel regel en de uit andere tabel verzamelde informatie toe aan het object
                    Dim resultEntityRecord As New Model.Result.Entities.ResultsEntity() With { _
                      .ObjectName = foundRowsResult(liLoop).Item("ObjectName").ToString(), _
                      .ObjectID = foundRowsResult(liLoop).Item("ObjectID").ToString(), _
                      .MeasurePoint = foundRowsResult(liLoop).Item("MeasurePoint").ToString(), _
                      .MeasurePointID = foundRowsResult(liLoop).Item("MeasurePointID").ToString(), _
                      .FieldNo = CastToStringOrEmpty(foundRowsResult(liLoop).Item("FieldNo")), _
                      .Time = CastToTypeTimeValueOrDefault(foundRowsResult(liLoop).Item("Time").ToString()) _
                      }
                    'If Not IsDBNull(foundRowsResult(liLoop).Item("FieldNo")) Then
                    '    resultEntityRecord.FieldNo = foundRowsResult(liLoop).Item("FieldNo")
                    'End If

                    'Search in measurevalue.
                    Dim foundRowsMeasureValue() As DataRow
                    foundRowsMeasureValue = Dataset.Tables("MeasureValue").Select("Result_Id=" & foundRowsResult(liLoop).Item("Result_Id"))
                    If foundRowsMeasureValue.Length > 0 Then
                        Dim measureValue As New Model.Result.Entities.MeasureValue() With { _
                                .UOM = foundRowsMeasureValue(0).Item("UOM"), _
                                .Value = foundRowsMeasureValue(0).Item("Value") _
                        }
                        resultEntityRecord.MeasureValue.Add(measureValue)
                    End If

                    ' Maak een recordset met alleen de LIST resultaten voor deze Result
                    Dim foundRowsList() As DataRow
                    foundRowsList = Dataset.Tables("List").Select("Result_ID=" & foundRowsResult(liLoop).Item("Result_Id"))

                    ' indien er wat gevonden is verwerken
                    If foundRowsList.Length > 0 Then
                        For ljLoop As Integer = 0 To foundRowsList.Length - 1
                            'Dim entityRecord As New Model.Result.Entities.ListEntity() With { _
                            '  .List_Column = foundRowsList(ljLoop).Item("List_Column").ToString() _
                            '  }

                            ' Wijs het resultaat nu toe aan prsEntityResults list
                            resultEntityRecord.List.Add(foundRowsList(ljLoop).Item("List_Column").ToString())
                        Next
                    End If

                    If Not IsDBNull(foundRowsResult(liLoop).Item("Text")) Then resultEntityRecord.Text = foundRowsResult(liLoop).Item("Text").ToString()

                    'Set the value to the record
                    ipResultEntityRecord.Result.Add(resultEntityRecord)
                Next
            End If
        End Sub
#End Region


    End Class
End Namespace



