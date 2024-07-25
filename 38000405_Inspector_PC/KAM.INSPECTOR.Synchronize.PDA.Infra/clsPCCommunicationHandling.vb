'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.IO
Imports Microsoft.Win32
Imports KAM.COMMUNICATOR.Infra
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model.SynchronisationStep
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Imports KAM.INSPECTOR.Infra
Imports KAM.COMMUNICATOR.Synchronize.Bussiness

''' <summary>
''' Class of INSPECTOR PC file handling
''' </summary>
''' <remarks></remarks>
Public Class clsPCCommunicationHandling
    Public Event EvntCopyFileProgressStatus(totalFileSize As Integer, copiedFileSize As String, filename As String)
    Public Event EvntStatus(stepStatus As SyncStatus, errorcode As SyncErrorMessages.enumErrorMessages, additionalMessage As String)
#Region "Properties"
    Public InspectionInformation As clsInspectorCommunication.StructInspectionInformation
#End Region

#Region "INSPECTOR File copy handling"


#Region "Functions to copy/Delete files INSPECTOR to/ from CONNEXION"
    ''' <summary>
    ''' Copy the data from INSPECTOR to working dir
    ''' Move the files measurementdata files (*.fpr)
    ''' XML files (StationInformation, InspectionStatus, InspectionProcedure)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransferDataIpToDpc()

        'Check dir exits. if not create
        If Not modFileHandling.DirExistsPC(modCommunicationPaths.MeasurementDataDirPC) Then Directory.CreateDirectory(modCommunicationPaths.MeasurementDataDirPC)


        'Copy all measurementdata files to desktop PC; Delete all measurementdata files from INSPECTOR
        'MOD 19
        modFileHandling.MoveMultipleFilesPC(modCommunicationPaths.IPMeasurementDataDir, modCommunicationPaths.MeasurementDataDirPC, "*.fpr")

        Try
            modFileHandling.CopyFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml"), True)
            modFileHandling.CopyFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "InspectionStatus.xml"), True)
            modFileHandling.CopyFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml"), True)
        Catch ex As Exception
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoSpecificMessage, ex.Message)
        End Try

        'Create backup of transfered files
        If Not modFileHandling.DirExistsPC(modCommunicationPaths.ArchiveDataDirPC) Then Directory.CreateDirectory(modCommunicationPaths.ArchiveDataDirPC)
        Dim fileName As String = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml")
        modFileHandling.CopyFilePC(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "StationInformation_" & Trim(Format(Now, "yyyymmddhhmm")) & ".xml"), True)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml")
        modFileHandling.CopyFilePC(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "Results_" & Trim(Format(Now, "yyyymmddhhmm")) & ".xml"), True)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "InspectionStatus.xml")
        modFileHandling.CopyFilePC(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "InspectionStatus_" & Trim(Format(Now, "yyyymmddhhmm")) & ".xml"), True)
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Delete the results file at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteResultsIP()
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"))
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "ResultsLast.xml"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Delete the inspection procedure, stationinformation and inspectionstatus at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteFilesIP()
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionProcedure.xml"))
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"))
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"))
        modFileHandling.DeleteFilePC(Path.Combine(modCommunicationPaths.IPDataDir, "PLEXOR.xml"))

        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Copy the files from desktop PC to INSPECTOR
    ''' The files are copied from the temporary directory TempSubDataDirPC_DPC2IP
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransferDataDpcToIP()
        Dim fileNameIP As String = ""
        Dim fileNamePC As String = ""

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")) Then
            'MOD 18  
            'Get the alternative file name
            Dim fileNameIpPrsTemp As String = ""
            fileNameIpPrsTemp = ModuleCommunicatorSettings.SettingFile.GetSetting(KAM.COMMUNICATOR.Infra.GsSectionPrs, KAM.COMMUNICATOR.Infra.GsSettingPrsAlternativeFileName)
            If fileNameIpPrsTemp = "" Or fileNameIpPrsTemp = "<No value>" Then fileNameIpPrsTemp = "StationInformation.xml"

            fileNameIP = Path.Combine(IPDataDir, fileNameIpPrsTemp)
            modFileHandling.DeleteFilePC(fileNameIP)

            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")) Then
            fileNameIP = Path.Combine(IPDataDir, "StationInformation.sdf")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
            If modFileHandling.CheckFileExistsPC(fileNamePC) Then Kill(fileNamePC)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml")) Then
            fileNameIP = Path.Combine(IPDataDir, "InspectionProcedure.xml")
            FileCopy(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")) Then
            fileNameIP = Path.Combine(IPDataDir, "ResultsLast.xml")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")) Then
            fileNameIP = Path.Combine(IPDataDir, "Results.sdf")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If

        'Copy file to local work directory
        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml")) Then
            fileNameIP = Path.Combine(IPDataDir, "PLEXOR.xml")
            FileCopy(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")) Then
            fileNameIP = Path.Combine(IPDataDir, "LastUpload.txt")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
            modFileHandling.CopyFilePC(fileNamePC, fileNameIP, True)
        End If


        Dim fileName As String = ""
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionStatus.xml")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
        If CheckFileExistsPC(fileName) Then Kill(fileName)

        'MOD 34
        SaveSettingsToInspector()

        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
#End Region
#Region "Inspector Paths determine"
    ''' <summary>
    ''' Determine the INSPECTOR path 
    ''' If no path is found return False
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetInspectorPathOnIP()
        'MOD 14
        Dim dataDir As String = ModuleSettings.SettingFile.GetSetting("APPLICATION", "XmlFilesPath")
        Dim measurementsDir = ModuleSettings.SettingFile.GetSetting("APPLICATION", "MeasurementFilesPath")

        If dataDir = "<No value>" Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NonExistingFile, " : dataDir")
            Exit Sub
        End If
        If measurementsDir = "<No value>" Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NonExistingFile, " : measurementsDir")
            Exit Sub
        End If
        'MOD 14 Dim configDir = "Config"
        'MOD 14 Dim dataDir = "Data\XML"
        'MOD 14 Dim measurementsDir = "Data\Measurements"

        modCommunicationPaths.IPGeneralDataDir = InspectionInformation.InstallPath
        'MOD 14 modCommunicationPaths.IPConfigDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, configDir)
        modCommunicationPaths.IPProgramDir = modCommunicationPaths.IPGeneralDataDir

        modCommunicationPaths.IPDataDir = Path.GetFullPath(dataDir)
        modCommunicationPaths.IPMeasurementDataDir = Path.GetFullPath(measurementsDir)

        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
#End Region

#Region "Get Inspector Information"
    ''' <summary>
    ''' Getting the Inspector information from the registry
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetInspectorInformation()
        Dim registerInformation As String = ""

        Dim windowsRegisterKey As String = ""


        '{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0} is the GUID of the installation of INSPECTOR PC
        If Environment.Is64BitOperatingSystem Then
            windowsRegisterKey = "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\UnInstall\{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}"
        Else
            windowsRegisterKey = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\UnInstall\{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}"
        End If

        registerInformation = Registry.GetValue(windowsRegisterKey, "InstallLocation", "")
        InspectionInformation.InstallPath = "C:\Users\User\Desktop\Wigersma_Sikkema\Inspector_PC\38000405_Inspector_PC\38000405_Inspector_PC\KAM.INSPECTOR.Main\bin" 'registerInformation
        registerInformation = Registry.GetValue(windowsRegisterKey, "DisplayVersion", "Key does not exist")
        InspectionInformation.Version = registerInformation
        InspectionInformation.SubVersion = ""
        registerInformation = Registry.GetValue(windowsRegisterKey, "License computerID", "Key does not exist")
        InspectionInformation.SerialNumber = registerInformation
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")

    End Sub
#End Region

    Private Sub eventHandlingFileCopyProgress(totalbytes As Integer, bytesLeft As Integer, fileName As String) 'Handles FileCopyProcessStatus
        RaiseEvent EvntCopyFileProgressStatus(totalbytes, bytesLeft, fileName)
    End Sub

#End Region
End Class
