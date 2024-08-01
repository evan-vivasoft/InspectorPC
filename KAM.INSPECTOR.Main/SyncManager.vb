'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Threading
Imports OpenNETCF.Desktop.Communication
Imports System.IO
Imports System.Linq
Imports System.Globalization
Imports System.Windows.Forms
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Infra
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Imports KAM.COMMUNICATOR.Synchronize.Infra.My.Resources
Imports KAM.COMMUNICATOR.Synchronize.Bussiness
Imports KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model.SynchronisationStep
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.Main.SyncErrorMessages
Imports KAM.INSPECTOR.Main.SynchronisationStep
Imports System.ComponentModel
Imports System.Configuration
Imports System.Reflection
Imports Inspector.POService.SyncService

''' <summary>
''' Class for handling the data exchange between Inspector and desktop PC
''' </summary>
''' <remarks></remarks>
Public Class SyncManager

#Region "Class members"
    'Main thread task for handling communication with PDA
    Private trdSyncTask As Thread
    Private synchronisationStepToExecute As SynchronisationStep
    Private stepExecute As Boolean = True
    Private syncStatusInfo As SyncStatusStepModel
    Private syncService As SyncService
    Private IsResultFilePresent As Boolean

    Public Event EvntSyncStarted()
    Public Event EvntSyncFinished()
    Public Event EvntsyncStepStatus(status As SyncStatusStepModel)
    Public Event EvntSyncError()



    Public Inspectortype As enumInspectorType
    Private m_SyncPDAConnected As Boolean = False

#Region "Enum"
    Enum enumConnectionStatus
        Connected
        Disconnected
    End Enum


    Enum enumInspectorType
        InspectorPc
        InspectorPda
        InspectorAndroid 'MOD XXA
    End Enum
#End Region

#End Region
#Region "Properties"

    'MOD 38 ''' <summary>
    'MOD 38''' Create a result file to INSPECTOR PC
    'MOD 38 ''' </summary>
    'MOD 38''' <remarks></remarks>
    'MOD 38 Private m_ResultsCreateFile As Boolean

    '''' <summary>
    '''' Delete the prs data from the msAccess database after synchronisation
    '''' </summary>
    '''' <remarks></remarks>
    'MOD 29 Private m_PrsDeletePRSfromAccess As Boolean

    ''' <summary>
    ''' Use filter option on PRS data; msAccess database only
    ''' </summary>
    ''' <remarks></remarks>
    Private m_PrsFilterOption As String

    ''' <summary>
    ''' Return if inspector PC is installed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property InspectorPcInstalled As Boolean
        Get
            Return m_InspectorPcInstalled
        End Get
    End Property
    Private m_InspectorPcInstalled As Boolean = False


    ''' <summary>
    ''' Return if inspector PDA can be used. Microsoft device centrum of Microsoft Active sync should be installed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property InspectorPdaInstalled As Boolean
        Get
            Return m_InspectorPdaInstalled
        End Get
    End Property
    Private m_InspectorPdaInstalled As Boolean = True

#End Region

#Region "Constructor"
    ''' <summary>
    ''' New; load 
    ''' Load the Communicator database for setting the paths of the inspector device
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'IpPCSyncHandling = New clsPCCommunicationHandling

    End Sub

    Public Sub InitializeConnection()
        'IpPCSyncHandling.GetInspectorInformation()
    End Sub
#End Region

#Region "Start Syncrhonization"
    ''' <summary>
    ''' Starting the synchronisation manual
    ''' This is used for the PC version of INSPECTOR PC
    ''' In case of PDA sync is active the sub is not executed
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSynchronisation()
        trdSyncTask = New Thread(AddressOf SyncThreadTask)
        trdSyncTask.IsBackground = True
        trdSyncTask.Start()
    End Sub
#End Region


#Region "Main Thread"

    ''' <summary>
    ''' Main thread loop handling the synchronization of the device
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SyncThreadTask()
        'MOD 07 Apply the selected culture also in the thread
        Dim cultureName As String = ModuleSettings.SettingFile.GetSetting(usctrl_TranslationSelection.GsSectionCulture, usctrl_TranslationSelection.GsSettingCultureName)
        If cultureName = SettingFile.NoValue Then
            cultureName = "en-GB"
            ModuleSettings.SettingFile.SaveSetting(usctrl_TranslationSelection.GsSectionCulture, usctrl_TranslationSelection.GsSettingCultureName) = cultureName
        End If
        Dim cultureInfo As CultureInfo = New CultureInfo(cultureName)
        'MOD 24 Thread.CurrentThread.CurrentCulture = cultureInfo
        Thread.CurrentThread.CurrentUICulture = cultureInfo

        RaiseEvent EvntSyncStarted()

        ' ''Start the synchronisation
        synchronisationStepToExecute = SynchronisationStep.Connect_with_plexoronline

        syncStatusInfo = New SyncStatusStepModel(SynchronisationStep.Connect_with_plexoronline, SynchronisationStep.SyncStatus.Started, enumErrorMessages.NoError, "")
        stepExecute = True

        'Main loop
        Do
            Thread.Sleep(250)
            If stepExecute = True Then
                Select Case synchronisationStepToExecute.ToString
                    Case SynchronisationStep.Connect_with_plexoronline.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        ConnectWithPlexorOnline()

                    Case SynchronisationStep.Check_license_information.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        syncService.CheckLicenseInformation(AddressOf CheckLicenseInformation)

                    Case SynchronisationStep.Check_result.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        IsResultFilePresent = False 'syncService.IsResultFilePresent
                        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")

                    Case SynchronisationStep.Transfer_result.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        syncService.SendResultToPlexorOnline()

                    Case SynchronisationStep.Remove_result.ToString
                        stepExecute = False
                        'Getting the communication paths of the selected device
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)

                    Case SynchronisationStep.Get_updated_data.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        GetUpdatedData()
                    Case SynchronisationStep.Get_last_result.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "A pseudo step.")

                    Case SynchronisationStep.Data_sync_complete.ToString
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "A pseudo step.")

                    Case SynchronisationStep.Set_last_upload.ToString
                        stepExecute = False
                        'Set the last upload/ update information
                        SetLastUploadInformation()
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "A pseudo step.")
                        'Transfer completed
                        syncStatusInfo.StepDateTime = Now
                        syncStatusInfo.StepId = SynchronisationStep.Set_last_upload
                        syncStatusInfo.StepResult = SynchronisationStep.SyncStatus.Succes
                        RaiseEvent EvntsyncStepStatus(syncStatusInfo)
                        RaiseEvent EvntSyncFinished()
                        Exit Do
                    Case SynchronisationStep.Transfer_error.ToString
                        stepExecute = False
                        Exit Do
                End Select
            End If
        Loop
        Exit Sub
    End Sub

    ''' <summary>
    ''' Main event handling of determine which step is performed and should be performed
    ''' </summary>
    ''' <param name="stepStatus"></param>
    ''' <param name="additionalmessage"></param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingsetNextSyncStep(stepStatus As SyncStatus, errorcode As SyncErrorMessages.enumErrorMessages, additionalmessage As String)

        If stepStatus <> SyncStatus.Succes Then
            synchronisationStepToExecute = SynchronisationStep.Transfer_error
            RaiseEventStatusEnd(stepStatus, syncStatusInfo, errorcode, additionalmessage)
            stepExecute = True
            Exit Sub
        Else
            RaiseEventStatusEnd(stepStatus, syncStatusInfo, enumErrorMessages.NoError, "")
        End If


        Select Case synchronisationStepToExecute.ToString
            Case SynchronisationStep.Connect_with_plexoronline.ToString
                synchronisationStepToExecute = SynchronisationStep.Check_license_information

            Case SynchronisationStep.Check_license_information.ToString
                synchronisationStepToExecute = SynchronisationStep.Check_result

            Case SynchronisationStep.Check_result.ToString
                synchronisationStepToExecute = If(IsResultFilePresent, SynchronisationStep.Transfer_result, SynchronisationStep.Get_updated_data)

            Case SynchronisationStep.Transfer_result.ToString
                synchronisationStepToExecute = SynchronisationStep.Remove_result

            Case SynchronisationStep.Remove_result.ToString
                synchronisationStepToExecute = SynchronisationStep.Get_updated_data

            Case SynchronisationStep.Get_updated_data.ToString
                synchronisationStepToExecute = SynchronisationStep.Get_last_result

            Case SynchronisationStep.Get_last_result.ToString
                synchronisationStepToExecute = SynchronisationStep.Data_sync_complete

            Case SynchronisationStep.Data_sync_complete.ToString
                synchronisationStepToExecute = SynchronisationStep.Set_last_upload

            Case SynchronisationStep.Transfer_error.ToString

            Case SynchronisationStep.Set_last_upload.ToString

        End Select
        stepExecute = True

    End Sub
#End Region

#Region "General functions"
    ''' <summary>
    ''' Try logging in into plexor online
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ConnectWithPlexorOnline()
        syncService = New SyncService
        syncService.ConnectWithPlexorOnline(AddressOf PlexorConnectionCallback)
    End Sub

    Public Sub PlexorConnectionCallback(maybeError As String)
        If maybeError IsNot Nothing Then
            EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Else
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.PlexorLoginError, maybeError)
        End If
    End Sub

    Public Sub CheckLicenseInformation(maybeError As String)
        If String.IsNullOrEmpty(maybeError) Then
            EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Else
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.LicenseInformationNotValid, maybeError)
        End If
    End Sub

    Private Sub GetUpdatedData()
        syncService.FetchUpdatedData(ModuleSettings.SettingFilename, AddressOf GetUpdatedDataCallBack)
    End Sub

    Private Sub GetUpdatedDataCallBack(maybeError As String)
        If String.IsNullOrEmpty(maybeError) Then
            EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Else
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.UpdatingDataError, maybeError)
        End If
    End Sub
    ''' <summary>
    ''' Set the last upload date and time in the file LastUpload.txt
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetLastUploadInformation()
        Dim directory As String = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location) + ConfigurationManager.AppSettings.Get("DataDir"))
        Dim fileName As String = Path.Combine(directory, "LastUpload.txt")
        Try
            ' Check if the file exists and delete it if necessary
            If File.Exists(fileName) Then
                File.Delete(fileName)
            End If

            ' Create the file and write data
            Using textFileStream As System.IO.TextWriter = System.IO.File.CreateText(fileName)
                textFileStream.WriteLine(FormatDateTime(Now, vbShortTime))
                textFileStream.WriteLine(Format(Now, "dd/MM/yy"))
            End Using
            EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Catch ex As DirectoryNotFoundException
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.LastUploadDateError, "Expected directory not found")
            SetLastUploadInformation()
        Catch ex As FileNotFoundException
            File.Create(fileName)
            SetLastUploadInformation()
        Catch ex As Exception
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.LastUploadDateError, ex.Message)
        End Try
    End Sub

    '    ''' <summary>
    '    ''' Delete the files from the temporary directory.
    '    ''' And delete the XML files from the directory with main data (System integration
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DeleteXmlMainFilesOnPc()
    '        Dim dirinfo As DirectoryInfo
    '        Dim allPrsFiles() As FileInfo

    '        'Delete all StationInformation files on the main PC  
    '        dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '        allPrsFiles = dirinfo.GetFiles("*Stationinformation*.xml")
    '        For Each prsFile As FileInfo In allPrsFiles
    '            DeleteFilePC(prsFile.FullName.ToString)
    '        Next

    '        'Delete all ResultsLast files on the main PC
    '        dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '        allPrsFiles = dirinfo.GetFiles("*ResultsLast*.xml")
    '        For Each prsFile As FileInfo In allPrsFiles
    '            DeleteFilePC(prsFile.FullName.ToString)
    '        Next

    '        'Delete all files in the temp dir on de local PC
    '        dirinfo = New DirectoryInfo(modCommunicationPaths.TempSubDataDirPC_IP2DPC)
    '        allPrsFiles = dirinfo.GetFiles("*.*")
    '        For Each prsFile As FileInfo In allPrsFiles
    '            DeleteFilePC(prsFile.FullName.ToString)
    '        Next

    '        'Delete all files in the temp dir on de local PC
    '        dirinfo = New DirectoryInfo(modCommunicationPaths.TempSubDataDirPC_DPC2IP)
    '        allPrsFiles = dirinfo.GetFiles("*.*")
    '        For Each prsFile As FileInfo In allPrsFiles
    '            DeleteFilePC(prsFile.FullName.ToString)
    '        Next

    '        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
    '    End Sub

    '#End Region


    'errorHandler1:
    '        EvntHandlingsetNextSyncStep(SyncStatus.SError, errormessage, "")
    '        Exit Sub
    '    End Sub
#End Region

    '#Region "Event handling of file process"
    '    ''' <summary>
    '    ''' handling of event of copy a file process
    '    ''' </summary>
    '    ''' <param name="totalFileSize">Total amount file size</param>
    '    ''' <param name="copiedFileSize">Amount copied</param>
    '    ''' <param name="fileName">The copied file name</param>
    '    ''' <remarks></remarks>
    '    Public Sub EvntHandlingCopyFileProgress(totalFileSize As Integer, copiedFileSize As Integer, fileName As String) Handles IpPDASyncHandling.EvntCopyFileProgressStatus
    '        RaiseEvent EvntCopyFileProgressStatus(totalFileSize, copiedFileSize, fileName)
    '    End Sub

    '    ''' <summary>
    '    ''' Handling the event of file process (convert from format to another format)
    '    ''' </summary>
    '    ''' <param name="fileType">Result or PRS</param>
    '    ''' <param name="total">Total amount of records to be processed</param>
    '    ''' <param name="processed">Current amount of records processed</param>
    '    ''' <param name="status">Status</param>
    '    ''' <param name="recordInfo">additional information</param>
    '    ''' <remarks></remarks>
    '    Private Sub EvntHandlingFileProcess(fileType As String, total As Integer, processed As Integer, status As DbCreateStatus, recordInfo As String)
    '        Select Case status
    '            Case clsDbGeneral.DbCreateStatus.SError
    '            Case clsDbGeneral.DbCreateStatus.ErrorWrite
    '        End Select
    '        RaiseEvent EvntdbFileProcessStatusEventHandler(fileType, total, processed, status, recordInfo)
    '    End Sub
    '#End Region

    '#Region "Events triggering of synchronization step status"
    '    ''' <summary>
    '    ''' Raising event sync process status. Current step is started
    '    ''' </summary>
    '    ''' <param name="stepID"></param>
    '    ''' <param name="syncStatusInfo"></param>
    '    ''' <remarks></remarks>
    Private Sub RaiseEventStatusStarted(stepID As SynchronisationStep, syncStatusInfo As SyncStatusStepModel)
        syncStatusInfo.StepDateTime = Now
        syncStatusInfo.StepId = stepID
        syncStatusInfo.StepResult = SynchronisationStep.SyncStatus.Started
        syncStatusInfo.StepErrorCode = enumErrorMessages.NoError
        Thread.Sleep(250)
    End Sub
    '    ''' <summary>
    '    ''' Raising the event with the sync status of current step
    '    ''' </summary>
    '    ''' <param name="status"></param>
    '    ''' <param name="errorCode"></param>
    '    ''' <param name="syncStatusInfo"></param>
    '    ''' <remarks></remarks>
    Private Sub RaiseEventStatusEnd(status As SyncStatus, syncStatusInfo As SyncStatusStepModel, errorCode As SyncErrorMessages.enumErrorMessages, additionalMessage As String)
        If status <> SyncStatus.Succes Then
            syncStatusInfo.StepDateTime = Now
            syncStatusInfo.StepErrorCode = errorCode
            syncStatusInfo.StepResult = status
            syncStatusInfo.SetpAdditionalMessage = additionalMessage
            RaiseEvent EvntSyncStepStatus(syncStatusInfo)
            RaiseEvent EvntSyncError()
            Exit Sub
        Else
            syncStatusInfo.StepDateTime = Now
            syncStatusInfo.StepResult = SynchronisationStep.SyncStatus.Succes
            RaiseEvent EvntSyncStepStatus(syncStatusInfo)
        End If
        Thread.Sleep(250)
    End Sub
End Class
