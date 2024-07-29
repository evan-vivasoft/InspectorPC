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
Imports JSONParser.LicenseHelper
Imports KAM.INSPECTOR.Main.SynchronisationStep
Imports System.ComponentModel
Imports JSONParser.SyncService

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


    ''Communicator database link
    'Public CommunicatorDb As Model.CommunicatorData.clsLinqToMDB
    ''Private xmlDatabase As New Model.Station.clsLinqXMLToXML

    'Private WithEvents IpPDASyncHandling As clsPDACommunicationHandling
    'Private WithEvents IpPCSyncHandling As clsPCCommunicationHandling
    ''MOD XXA
    'Private WithEvents IpAndroidSyncHandling As clsAndroidCommunicationHandling

    ''Events for file process handling
    'Public Event EvntdbFileProcessStatusEventHandler As EvntdbFileProcessStatus


    ''Events for copy file process handling
    'Public Event EvntCopyFileProgressStatus(totalFileSize As Integer, copiedFileSize As String, filename As String)

    ''Events with device/ inspector information
    'Public Event EvntDeviceSystemInfo(systemInfo As SYSTEM_INFO)
    'Public Event EvntDeviceMemoryStatus(memoryStatus As MEMORYSTATUS)
    'Public Event EvntDeviceStoreInformation(storeInformation As STORE_INFORMATION)
    'Public Event EvntDeviceSystemPowerStatus(powerStatus As SYSTEM_POWER_STATUS_EX)
    'Public Event EvntInspectionInformation(inspectorInformation As clsInspectorCommunication.StructInspectionInformation)

    ''Events for connection status
    'Public Event EvntConnectionStatus(status As enumConnectionStatus)
    'Private Const PathRapidll As String = "C:\Windows\System32\Rapi.dll"

    'Private ConnectionStatus As enumConnectionStatus

    ''Events for synchronisation status
    Public Event EvntSyncStarted()
    'Public Event EvntSyncFinished()
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

#Region "PDA connection/ disconnection handling"
    ''' <summary>
    ''' Handling of event PDA connection
    ''' This event will start the main thread of communication with device
    ''' An event EventConnectionStatus is triggered 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntConnection()
        'RaiseEvent EvntConnectionStatus(enumConnectionStatus.Connected)
        'ConnectionStatus = enumConnectionStatus.Connected
        Inspectortype = enumInspectorType.InspectorPda
        m_SyncPDAConnected = True
        'Starting the main thread
        'trdSyncTask = New Thread(AddressOf SyncThreadTask)
        'trdSyncTask.IsBackground = True
        'trdSyncTask.Start()
    End Sub

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
                        'Check in which directory INSPECTOR is installed
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        ConnectWithPlexorOnline()

                    Case SynchronisationStep.Check_license_information.ToString
                        stepExecute = False
                        'Check License Information
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        syncService.CheckLicenseInformation(AddressOf CheckLicenseInformation)

                    Case SynchronisationStep.Check_result.ToString
                        stepExecute = False
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        IsResultFilePresent = syncService.IsResultFilePresent
                        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")

                    Case SynchronisationStep.Transfer_result.ToString
                        stepExecute = False
                        'Update the software version in the database
                        RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        syncService.SendResultToPlexorOnline()

                        'Case SynchronisationStep.Remove_result.ToString
                        '    stepExecute = False
                        '    'Getting the communication paths of the selected device
                        '    RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        '    GetCommunicatorPaths("BCS-9C:B6:D0:9C:7F:C7")'deviceSyncHandling.InspectionInformation.SerialNumber)

                        'Case SynchronisationStep.Get_updated_data.ToString
                        '    stepExecute = False
                        '    'Create temp directories for synchronization 
                        '    modCommunicationPaths.InspectorSerialnumber = deviceSyncHandling.InspectionInformation.SerialNumber
                        '    modCommunicationPaths.CreateTempDirectory()
                        '    EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")

                        'Case SynchronisationStep.Get_last_result.ToString
                        '    stepExecute = False
                        '    'Copy the files from INSPECTOR to DPC
                        '    RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)
                        '    deviceSyncHandling.TransferDataIPToDpc()

                        'Case SynchronisationStep.Data_sync_complete.ToString
                        '    'Process the results from Inspector to PC
                        '    RaiseEventStatusStarted(synchronisationStepToExecute, syncStatusInfo)

                        '    'MOD 25
                        '    If m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
                        '        'XML to XML
                        '        'Check if a lock file is created in case of XML only
                        '        'If lock file wait for x seconds and check if lock if removed
                        '        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.lock")) Then
                        '            Thread.Sleep(500)
                        '            Exit Select
                        '        End If
                        '    End If
                        '    stepExecute = False
                        '    'No XML and lock file; The data can be processed
                        '    DbResultProcessFileFromInspector()

                        'Case SynchronisationStep.Set_last_upload.ToString
                        '    stepExecute = False
                        '    'Set the last upload/ update information
                        '    SetLastUploadInformation()
                        '    UpdateLastSyncDate(deviceSyncHandling.InspectionInformation, Now)

                        'Case SynchronisationStep.Transfer_error.ToString
                        '    stepExecute = False
                        '    'An error occured; Stop the transfer
                        '    Exit Do
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

        End Select
        stepExecute = True

    End Sub
#End Region

    '#Region "General functions"
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

    Public Sub CheckLicenseInformation(isLicenseValid As Boolean)
        If isLicenseValid Then
            EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Else
            EvntHandlingsetNextSyncStep(SyncStatus.SError, enumErrorMessages.LicenseInformationNotValid, "")
        End If
    End Sub
    ''' <summary>
    ''' Set the last upload date and time in the file LastUpload.txt
    ''' </summary>
    ''' <remarks></remarks>
    '    Public Sub SetLastUploadInformation()
    '        Dim fileName As String = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
    '        If CheckFileExistsPC(fileName) Then Kill(fileName)

    '        'Create file to set the last data exchange information
    '        Dim textFileStream As System.IO.TextWriter
    '        textFileStream = System.IO.File.CreateText(fileName)
    '        textFileStream.WriteLine(FormatDateTime(Now, vbShortTime))
    '        'MOD 01 mm to MM
    '        textFileStream.WriteLine(Format(Now, "dd/MM/yy"))
    '        textFileStream.Close()
    '    End Sub

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

    '#Region "PRS database process handling"
    '    Private Sub DbPrsProcessFileToInspector()
    '        Dim functionStatus As Boolean = False
    '        Dim errormessage As enumErrorMessages = enumErrorMessages.NoError
    '        Dim additionalErrorMessage As String = ""
    '        '
    '        If Not CheckFileExistsPC(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.XML")) Then
    '            errormessage = enumErrorMessages.NonExistingFile
    '            additionalErrorMessage = " : " & Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.XML")
    '            If functionStatus = False Then GoTo errorHandler1
    '        End If
    '        If Not CheckFileExistsPC(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.XML")) Then
    '            errormessage = enumErrorMessages.NonExistingFile
    '            'MOD 11
    '            'MOD 41
    '            additionalErrorMessage = " : " & Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.XML")
    '            If functionStatus = False Then GoTo errorHandler1
    '        End If


    '        If m_prsDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then
    '            'All data from Msaccess database are copied into the sdf file
    '            Console.WriteLine("Loading Access database: " & Format(Now, "HH:mm:ss:fff"))

    '            If m_prsInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                'msAccess to XML
    '                Console.WriteLine("WriteStationInformation xml: " & Format(Now, "HH:mm:ss:fff"))
    '                Dim linqMsAccessToXml As New Model.Station.ClsLinqMsAccessToXml

    '                AddHandler linqMsAccessToXml.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                'Use of filter option
    '                Dim prsFilterOption As String = ""
    '                If m_PrsFilterOption <> "" Then prsFilterOption = "PDASNR=" & modCommunicationPaths.InspectorSerialnumber 'MOD 40

    '                functionStatus = linqMsAccessToXml.LoadStationInformation(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "prs.mdb"), prsFilterOption)
    '                If functionStatus = False Then GoTo errorHandler1
    '                functionStatus = linqMsAccessToXml.WriteStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"))
    '                If functionStatus = False Then GoTo errorHandler1

    '                'MOD 37/ MOD 38
    '                Dim statusSelection As String
    '                statusSelection = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector)

    '                If statusSelection <> "-1" Then
    '                    Dim linqXmlToXml As New Model.Station.clsLinqXMLToXML
    '                    AddHandler linqXmlToXml.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                    functionStatus = linqXmlToXml.LoadStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"))
    '                    If functionStatus = False Then GoTo errorHandler1

    '                    'Check if the PRS (stationinformation) file from inspector is available
    '                    If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml")) Then
    '                        functionStatus = linqXmlToXml.LoadStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"), True, Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "InspectionStatus.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionStatus.xsd"))
    '                        If functionStatus = False Then GoTo errorHandler1

    '                        'Delete the previous Access to XML stationinformation file
    '                        DeleteFilePC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml"))
    '                        'Create new stationinformation xml file
    '                        functionStatus = linqXmlToXml.WriteStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"), statusSelection)
    '                        If functionStatus = False Then GoTo errorHandler1

    '                    End If

    '                End If


    '                'MOD 29
    '                'Delete the information form the Access database
    '                If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = True Then linqMsAccessToXml.ClearAllPrsinMsAccess(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "prs.mdb"))

    '            ElseIf m_prsInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
    '                'msAccess to SDF
    '                Console.WriteLine("WriteStationInformation SDF: " & Format(Now, "HH:mm:ss:fff"))
    '                Dim linqmsAccessToSql As New Model.Station.clsLinqMsAccessToSdf
    '                AddHandler linqmsAccessToSql.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess
    '                functionStatus = linqmsAccessToSql.WriteStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf"), Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "prs.mdb"))
    '                If functionStatus = False Then GoTo errorHandler1
    '                'MOD 29
    '                'Delete the information form the Access database
    '                If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = True Then linqmsAccessToSql.ClearAllPrsinMsAccess(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "prs.mdb"))
    '            End If

    '            Console.WriteLine("Complete: " & Format(Now, "HH:mm:ss:fff"))
    '        ElseIf m_prsDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '            If m_prsInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
    '                'XML to SDF

    '                'MOD 31
    '                File.Create(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "Stationinformation.lock")).Dispose()
    '                Dim linqXmltoSdf As New Model.Station.clsLinqXMLToSDF
    '                AddHandler linqXmltoSdf.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                'Create new database
    '                linqXmltoSdf.CreateNewfile(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf"))

    '                'Checking the different xml files on the desktop PC 'Sort them for old to new
    '                Dim dirinfo As DirectoryInfo
    '                Dim allPrsFiles() As FileInfo

    '                dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '                allPrsFiles = dirinfo.GetFiles("*Stationinformation*.xml")

    '                Array.Sort(allPrsFiles, New clsCompareFileInfo)
    '                Array.Reverse(allPrsFiles)

    '                For Each prsFile As FileInfo In allPrsFiles
    '                    functionStatus = linqXmltoSdf.ReadStationInformation(prsFile.FullName.ToString, Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"))
    '                    If functionStatus = False Then GoTo errorHandler1
    '                Next

    '                'Create the sdf file
    '                'Get the status for deleting the PRS on Inspector
    '                Dim statusSelection As String
    '                statusSelection = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector)
    '                functionStatus = linqXmltoSdf.WriteStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.sdf"), statusSelection)
    '                If functionStatus = False Then GoTo errorHandler1

    '                DeleteFilePC(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "Stationinformation.lock"))

    '                'MOD 30 errormessage = enumErrorMessages.NonSupportedFileFormat
    '                'MOD 30 If functionStatus = False Then GoTo errorHandler1
    '            ElseIf m_prsInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                'XML to XML
    '                'MOD 26; Create lock file
    '                File.Create(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "Stationinformation.lock")).Dispose()

    '                Dim linqXmlToXml As New Model.Station.clsLinqXMLToXML
    '                AddHandler linqXmlToXml.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                'Checking the different xml files on the desktop PC
    '                Dim dirinfo As DirectoryInfo
    '                Dim allPrsFiles() As FileInfo

    '                dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '                allPrsFiles = dirinfo.GetFiles("*Stationinformation*.xml")

    '                'Sort the files from new to old
    '                Array.Sort(allPrsFiles, New clsCompareFileInfo)
    '                Array.Reverse(allPrsFiles)

    '                For Each prsFile As FileInfo In allPrsFiles
    '                    functionStatus = linqXmlToXml.LoadStationInformation(prsFile.FullName.ToString, Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"))
    '                    If functionStatus = False Then GoTo errorHandler1
    '                Next

    '                'MOD 37
    '                Dim statusSelection As String
    '                statusSelection = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector)
    '                If statusSelection <> "-1" Then
    '                    'Check if the PRS (stationinformation) file from inspector is available
    '                    If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml")) Then
    '                        functionStatus = linqXmlToXml.LoadStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"), True, Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "InspectionStatus.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionStatus.xsd"))
    '                        If functionStatus = False Then GoTo errorHandler1
    '                    End If
    '                Else
    '                    'MOD 37 Set statusSelection to "0;1" to export all stations from XML file
    '                    statusSelection = "0;1"
    '                End If

    '                'Create the xml file
    '                'MOD 28'Get the status for deleting the PRS on Inspector
    '                functionStatus = linqXmlToXml.WriteStationInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "StationInformation.xsd"), statusSelection)
    '                If functionStatus = False Then GoTo errorHandler1
    '                'MOD 26
    '                DeleteFilePC(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "Stationinformation.lock"))
    '            End If

    '        End If

    '        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
    '        Exit Sub

    'errorHandler1:
    '        'MOD 26
    '        DeleteFilePC(Path.Combine(modCommunicationPaths.PrsLoadDataDirPC, "Stationinformation.lock"))

    '        EvntHandlingsetNextSyncStep(SyncStatus.SError, errormessage, additionalErrorMessage)
    '        Exit Sub
    '    End Sub
    '#End Region
    '#Region "Result database process handling"
    '    ''' <summary>
    '    ''' Create a results file from PC to Inspector
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DbResultProcessFileToInspector()
    '        Dim functionStatus As Boolean = False
    '        Dim errormessage As enumErrorMessages = enumErrorMessages.NoError

    '        'MOD 38 
    '        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionResult, GsSettingResultCreateFile) = True Then
    '            'Create a temp result file; This temp file is copied to the PDA

    '            If m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then
    '                If m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                    'msAccess to XML
    '                    Dim linqMsAccessToXml As New Model.Result.ClsLinqMsAccessToXml
    '                    AddHandler linqMsAccessToXml.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                    linqMsAccessToXml.LoadResultsInformation(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Result.mdb"))
    '                    linqMsAccessToXml.WriteResultsInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                ElseIf m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
    '                    'msAccess to SDF
    '                    Dim linqMsAccessToSdf As New Model.Result.clsLinqMsAccessToSdf
    '                    AddHandler linqMsAccessToSdf.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                    linqMsAccessToSdf.LoadWriteResults(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf"), Path.Combine(modCommunicationPaths.ResultDataDirPC, "Result.mdb"))
    '                End If
    '            ElseIf m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then

    '                If m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                    'XML to XML
    '                    Dim linqXmlToXml As New Model.Result.clsLinqXmlToXml
    '                    AddHandler linqXmlToXml.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                    'Checking the different xml files on the desktop PC
    '                    Dim dirinfo As DirectoryInfo
    '                    Dim allResultsLastFiles() As FileInfo

    '                    'Loading results last files
    '                    dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '                    allResultsLastFiles = dirinfo.GetFiles("*ResultsLast*.xml")

    '                    If allResultsLastFiles.Length > 0 Then
    '                        Array.Sort(allResultsLastFiles, New clsCompareFileInfo)
    '                        Array.Reverse(allResultsLastFiles)

    '                        For Each resultsLastFile As FileInfo In allResultsLastFiles
    '                            functionStatus = linqXmlToXml.LoadResults(resultsLastFile.FullName.ToString, Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                            If functionStatus = False Then GoTo errorHandler1
    '                        Next

    '                        'Create the xml file
    '                        functionStatus = linqXmlToXml.WriteResults(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                        If functionStatus = False Then GoTo errorHandler1
    '                    End If

    '                ElseIf m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
    '                    'XML to SDF
    '                    'MOD 31
    '                    Dim linqXmlToSdf As New Model.Result.clsLinqXMLToSDF
    '                    AddHandler linqXmlToSdf.EvntdbFileProcessStatus, AddressOf EvntHandlingFileProcess

    '                    'Checking the different xml files on the desktop PC
    '                    Dim dirinfo As DirectoryInfo
    '                    Dim allResultsLastFiles() As FileInfo

    '                    functionStatus = linqXmlToSdf.CreateNewfile(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf"))
    '                    If functionStatus = False Then GoTo errorHandler1

    '                    'Loading results last files
    '                    dirinfo = New DirectoryInfo(modCommunicationPaths.PrsLoadDataDirPC)
    '                    allResultsLastFiles = dirinfo.GetFiles("*ResultsLast*.xml")

    '                    If allResultsLastFiles.Length > 0 Then
    '                        Array.Sort(allResultsLastFiles, New clsCompareFileInfo)
    '                        Array.Reverse(allResultsLastFiles)

    '                        For Each resultsLastFile As FileInfo In allResultsLastFiles
    '                            functionStatus = linqXmlToSdf.LoadResults(resultsLastFile.FullName.ToString, Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                            If functionStatus = False Then GoTo errorHandler1
    '                        Next
    '                    End If

    '                    functionStatus = linqXmlToSdf.WriteResults
    '                    If functionStatus = False Then GoTo errorHandler1

    '                    'MOD 31 errormessage = enumErrorMessages.NonSupportedFileFormat
    '                    'MOD 31 If functionStatus = False Then GoTo errorHandler1
    '                End If
    '            End If
    '        End If
    '        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
    '        Exit Sub

    'errorHandler1:
    '        EvntHandlingsetNextSyncStep(SyncStatus.SError, errormessage, "")
    '        Exit Sub
    '    End Sub
    '    ''' <summary>
    '    ''' Adding the results from Inspector to the results file on the PC
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub DbResultProcessFileFromInspector()
    '        Dim functionStatus As Boolean = False
    '        Dim errormessage As enumErrorMessages = enumErrorMessages.NoError

    '        'Create a temp result file; This temp file is copied to 
    '        If m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
    '            'Check if a results file from INSPECTOR exists.
    '            If Not CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.sdf")) Then GoTo ExitHandler
    '            If m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then

    '                'SDF to msAccess
    '                Dim SdfToMdb As New Model.Result.clsLinqSdfToMsAccess
    '                SdfToMdb.WriteResults(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Result.mdb"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.sdf"))
    '            ElseIf m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                'SDF to XML
    '                File.Create(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.lock")).Dispose()

    '                Dim SdfToXML As New Model.Result.clsLinqSdfToXml
    '                SdfToXML.LoadResultsInformation(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.sdf"), Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                SdfToXML.WriteResults(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))

    '                DeleteFilePC(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.lock"))

    '                'MOD 31 errormessage = enumErrorMessages.NonSupportedFileFormat
    '                'MOD 31 If functionStatus = False Then GoTo errorHandler1
    '            End If

    '        ElseIf m_resultInspectorFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '            'Check if a results file from INSPECTOR exists.
    '            If Not CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml")) Then GoTo ExitHandler
    '            If m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then
    '                'XML to msAccess
    '                Dim XmlToMsAccess As New Model.Result.clsLinqXMLToMsAccess
    '                XmlToMsAccess.LoadWriteResults(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"), Path.Combine(modCommunicationPaths.ResultDataDirPC, "Result.mdb"))

    '            ElseIf m_resultDpcFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
    '                'XML to XML
    '                Dim XmlToxml As New Model.Result.clsLinqXmlToXml
    '                'MOD 25; Create lock file to prevent file is read by multiple sync proces
    '                File.Create(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.lock")).Dispose()

    '                'Check if a results.xml file already exists in the main directory. If so load this data to append other data
    '                If CheckFileExistsPC(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.xml")) Then
    '                    XmlToxml.LoadResults(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                End If
    '                XmlToxml.LoadResults(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                'Save the results file
    '                XmlToxml.WriteResults(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.xml"), Path.Combine(modCommunicationPaths.XsdDirPC, "XSD", "InspectionResultsData.xsd"))
    '                'MOD 25; delete the lock file
    '                DeleteFilePC(Path.Combine(modCommunicationPaths.ResultDataDirPC, "Results.lock"))
    '            End If
    '        End If
    '        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
    '        Exit Sub

    'ExitHandler:
    '        EvntHandlingsetNextSyncStep(SyncStatus.Succes, enumErrorMessages.NoError, "")
    '        Exit Sub


    'errorHandler1:
    '        EvntHandlingsetNextSyncStep(SyncStatus.SError, errormessage, "")
    '        Exit Sub
    '    End Sub
    '#End Region

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
    '#End Region
End Class
