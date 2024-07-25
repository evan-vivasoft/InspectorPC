'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsCastingHelpersXML
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral

Namespace Model.Station
    ''' <summary>
    ''' class for converting a MsAccess database into a SDF database.
    ''' </summary>
    ''' <remarks>Options:</remarks>
    ''' Clear all PRS data in MsAccess database
    Public Class clsLinqMsAccessToSdf
#Region "Members"
        Public Event EvntdbFileProcessStatus As EvntdbFileProcessStatus
        Public Event EventdbFileError As EventdbFileError

        Private sqlDbExport As PRSsql.DataAccess.StationInformation
        Private msAccessExport_Entity As PRSMDB.DataAccess.StationInformation

        'Primary key counters; Used to generate the SDF file from Entity
        Private m_prs_id_New As Integer = 0
        Private m_prsObjects_id_New As Integer = 0
        Private m_gcl_id_New As Integer = 0
        Private m_gclObjects_id_New As Integer = 0
#End Region

#Region "Public"
        ''' <summary>
        ''' Export the PRS data from the msAccess database to the SQL CE 3.5 database
        ''' </summary>
        ''' <param name="sqlDbImport_FileName">File name of the SQL database</param>
        ''' <param name="msAccessExport_FileName">File name of the msAccess database</param>
        ''' <param name="selectionString">Optional selection string; to select a set of PRS</param>
        ''' <remarks></remarks>
        Public Function WriteStationInformation(ByVal sqlDbImport_FileName As String, ByVal msAccessExport_FileName As String, Optional selectionString As String = "") As Boolean
            'MOD 06
            If Not CheckFileExistsPC(msAccessExport_FileName) Then
                RaiseEvent EvntdbFileProcessStatus(clsDbGeneral.DbCreateType.Prs, 0, 0, clsDbGeneral.DbCreateStatus.FileNotExists, msAccessExport_FileName)
                Return False
            End If

            msAccessExport_Entity = New PRSMDB.DataAccess.StationInformation(msAccessExport_FileName) ' With {.Log = Console.Out}

            'TO DO check database verion
            sqlDbExport = New PRSsql.DataAccess.StationInformation(sqlDbImport_FileName)
            If sqlDbExport.DatabaseExists Then
                sqlDbExport.DeleteDatabase()
            Else
            End If
            sqlDbExport.CreateDatabase()

            'Start filling the database
            ExtractPrs_FromMsAccessToSdf(selectionString)
            Return True
        End Function

        ''' <summary>
        ''' Clear all tables from the MsAccess database
        ''' Can be used to delete the data after synchronisation
        ''' </summary>
        ''' <param name="msAccessExport_FileName">File name of the msAccess database</param>
        ''' <remarks></remarks>
        Public Function ClearAllPrsinMsAccess(ByVal msAccessExport_FileName As String) As Boolean
            msAccessExport_Entity = New PRSMDB.DataAccess.StationInformation(msAccessExport_FileName)

            Dim deletetblPrsDatas = From details In msAccessExport_Entity.tblPRSdatas() Select details
            Dim deletetblPrsObjects = From details In msAccessExport_Entity.tblPRSObjects() Select details
            Dim deletetblGclDatas = From details In msAccessExport_Entity.tblGCLdatas() Select details
            Dim deletetblGclObjectss = From details In msAccessExport_Entity.tblGCLObjects() Select details
            msAccessExport_Entity.tblPRSdatas.DeleteAllOnSubmit(deletetblPrsDatas)
            msAccessExport_Entity.tblPRSObjects.DeleteAllOnSubmit(deletetblPrsObjects)
            msAccessExport_Entity.tblGCLdatas.DeleteAllOnSubmit(deletetblGclDatas)
            msAccessExport_Entity.tblGCLObjects.DeleteAllOnSubmit(deletetblGclObjectss)

            Try
                msAccessExport_Entity.SubmitChanges()
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
        ''' Extract the PRS data from the msAccess database into the SQL database
        ''' </summary>
        ''' <param name="selectionString">Optional selection string; to select a set of PRS</param>
        ''' <remarks></remarks>
        Private Sub ExtractPrs_FromMsAccessToSdf(Optional selectionString As String = "")
            'Apply the selection string. For example: prsRecords.Route = "ZZO"
            If selectionString <> "" Then
                Dim query = From prsMsaRecords In msAccessExport_Entity.tblPRSdatas() Where selectionString Order By prsMsaRecords.PRSName _
                Select prsMsaRecords
                msAccessExport_Entity = query
            End If

            Dim recordCounterProcessed As Integer = 0
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, msAccessExport_Entity.tblPRSdatas.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, "")
            For Each prsMsaRecord In msAccessExport_Entity.tblPRSdatas.OrderBy(Function(t) t.PRSName)
                recordCounterProcessed += 1
                RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, msAccessExport_Entity.tblPRSdatas.Count, recordCounterProcessed, DbCreateStatus.StartedCreate, prsMsaRecord.PRSName)

                'Adding the data
                Dim prsSqlRecord As New PRSsql.DataAccess.PRS With { _
               .PRS_Id = m_prs_id_New, _
               .PRSCode = prsMsaRecord.PRSCode, _
               .PRSName = prsMsaRecord.PRSName, _
               .PRSIdentification = prsMsaRecord.PRSIdentification, _
               .Route = prsMsaRecord.Route, _
               .Information = prsMsaRecord.Information, _
               .InspectionProcedure = prsMsaRecord.InspectionProcedure, _
               .StatusPRS = 1, _
               .StatusGCL = 1 _
                }
                Console.WriteLine(Now & " ID = {0}, name = {1}", prsMsaRecord.PrsId, prsMsaRecord.PRSName)

                ExtractGcl_FromMsAccessToSdf(prsMsaRecord.tblGCLdatas, prsSqlRecord)
                ExtractPrsObject_FromMsAccessToSdf(prsMsaRecord.tblPRSObjects, prsSqlRecord)
                'Adding the prs to the SDF database
                sqlDbExport.PRS.InsertOnSubmit(prsSqlRecord)

                'Increase ID
                m_prs_id_New += 1
            Next
            'Submit the changes to the database
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, msAccessExport_Entity.tblPRSdatas.Count, recordCounterProcessed, DbCreateStatus.StartedWrite, "!Saving database")
            sqlDbExport.SubmitChanges()
            'Close the database for data exchange
            sqlDbExport.Connection.Close()
            RaiseEvent EvntdbFileProcessStatus(DbCreateType.Prs, msAccessExport_Entity.tblPRSdatas.Count, recordCounterProcessed, DbCreateStatus.SuccesWrite, "!Saving Completed database")
        End Sub

        ''' <summary>
        ''' Extracts the PRS object from the MsAccess database into the SQL dataset
        ''' </summary>
        ''' <param name="prsObjects">the object MsAccess tblPrsObject</param>
        ''' <param name="prs">the Sql object</param>
        ''' <remarks></remarks>
        Private Sub ExtractPrsObject_FromMsAccessToSdf(ByVal prsObjects As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblPRSObject), ByRef prs As PRSsql.DataAccess.PRS)
            If prsObjects.HasLoadedOrAssignedValues Then
                For Each prsObjectMsaRecord In prsObjects
                    Dim prsObjectSdfRecord As New PRSsql.DataAccess.PRSObject With { _
                    .PRSObjects_Id = m_prsObjects_id_New, _
                    .PRSLinkID = m_prs_id_New, _
                    .ObjectName = prsObjectMsaRecord.ObjectName, _
                    .ObjectID = prsObjectMsaRecord.ObjectID, _
                    .MeasurePoint = prsObjectMsaRecord.MeasurePoint, _
                    .MeasurePointID = prsObjectMsaRecord.MeasurePointID, _
                    .FieldNo = prsObjectMsaRecord.FieldNo _
                    }
                    prs.PRSObjects.Add(prsObjectSdfRecord)
                    m_prsObjects_id_New += 1
                Next
            End If
        End Sub

        ''' <summary>
        ''' Extracts the GCL from the MsAccess database into the SQL database
        ''' </summary>
        ''' <param name="gclSet">the object MsAccess tblGCLdata</param>
        ''' <param name="prs">the Sql PRS object</param>
        ''' <remarks></remarks>
        Private Sub ExtractGcl_FromMsAccessToSdf(ByVal gclSet As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblGCLdata), ByRef prs As PRSsql.DataAccess.PRS)
            'For inspector only one unique GCL for each PRS may occur. Communicator only selects one unique GCL
            Dim queryResult = gclSet.GroupBy(Function(r) r.GCLName).[Select](Function(g) g.First())

            'For Each gclMsaRecord In gclSet.OrderBy(Function(t) t.GCLName)
            For Each gclMsaRecord In queryResult
                Dim gclSqlRecord As New PRSsql.DataAccess.GasControlLine() With { _
                  .GasControlLine_Id = m_gcl_id_New, _
                  .PRSLinkID = gclMsaRecord.PRSLinkID, _
                  .GasControlLineName = gclMsaRecord.GCLName, _
                  .PeMin = gclMsaRecord.PeMin, _
                  .PeMax = gclMsaRecord.PeMax, _
                  .VolumeVA = gclMsaRecord.VolumeVA, _
                  .VolumeVAK = gclMsaRecord.VolumeVAK, _
                  .PaRangeDM = gclMsaRecord.PdRangeDM, _
                  .PeRangeDM = gclMsaRecord.PuRangeDM, _
                  .GCLIdentification = gclMsaRecord.GCLIdentification, _
                  .InspectionProcedure = gclMsaRecord.InspectionProcedure, _
                  .FSDStart = If(CastToIntOrNull(gclMsaRecord.FSDStartPosition), -1), _
                  .GCLCode = "", _
                  .StatusGCL = 1
                }
                'MOD 33
                ExtractGclObject_FromMsAccessToSdf(gclMsaRecord.tblGCLObjects, gclSqlRecord)
                prs.GasControlLines.Add(gclSqlRecord)
                m_gcl_id_New += 1
            Next
        End Sub

        ''' <summary>
        ''' Extracts the GCL objects from the MsAccess database into the SQL database
        ''' </summary>
        ''' <param name="gclObjects">the object MsAccess tblGCLObject</param>
        ''' <param name="gcl">the Sql GCL object</param>
        ''' <remarks></remarks>
        Private Sub ExtractGclObject_FromMsAccessToSdf(ByVal gclObjects As ALinq.EntitySet(Of KAM.COMMUNICATOR.Synchronize.Bussiness.tblGCLObject), ByRef gcl As PRSsql.DataAccess.GasControlLine)
            For Each gclObjectMsaRecord In gclObjects
                Dim gclObjectSdfRecord As New PRSsql.DataAccess.GCLObject With { _
                .GCLObjects_Id = m_gclObjects_id_New, _
                .GCLLinkID = m_gcl_id_New, _
                .ObjectName = gclObjectMsaRecord.ObjectName, _
                .ObjectID = gclObjectMsaRecord.ObjectID, _
                .MeasurePoint = gclObjectMsaRecord.MeasurePoint, _
                .MeasurePointID = gclObjectMsaRecord.MeasurePointID, _
                .FieldNo = gclObjectMsaRecord.FieldNo, _
                .Value = gclObjectMsaRecord.Value, _
                .Percentage = gclObjectMsaRecord.Percentage, _
                .ValueMax = gclObjectMsaRecord.ValueMax, _
                .ValueMin = gclObjectMsaRecord.ValueMin, _
                .UOV = gclObjectMsaRecord.Uov _
                }
                gcl.GCLObjects.Add(gclObjectSdfRecord)
                m_gclObjects_id_New += 1
            Next
        End Sub
#End Region
    End Class
End Namespace