'===============================================================================
'Copyright Wigersma 21
'All rights reserved.
'===============================================================================

Imports System.IO
Imports System.Xml
Imports KAM.COMMUNICATOR.Infra
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model.SynchronisationStep
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Imports MediaDevices


''' <summary>
''' Class of INSPECTOR PC file handling
''' </summary>
''' <remarks></remarks>
Public Class clsAndroidCommunicationHandling
    Public Event EvntCopyFileProgressStatus(totalFileSize As Integer, copiedFileSize As String, filename As String)
    Public Event EvntStatus(stepStatus As SyncStatus, errorcode As SyncErrorMessages.enumErrorMessages, additionalMessage As String)
#Region "Properties"
    Public InspectionInformation As clsInspectorCommunication.StructInspectionInformation
    Private lConnectedMediaDevice As MediaDevice
    Private folderAndroidTablet As String = "\Tablet\Android\"
    'Private folderAndroidCard As String = "\Tablet\Card\"
    Private folderAndroidCard As String = "Card\Android\"
    Private folderSubInspectorData As String = "data\ws.gas.plexor\files\CONNEXION V5.x\INSPECTOR\Data\"
    '\data\ws.gas.plexor\files\CONNEXION V5.x\INSPECTOR\Data
    'Computer\Reinier's Galaxy Tab Active Pro\Tablet\Androi

#End Region

#Region "Properties"
    Public Property ConnectedMediaDevice As MediaDevice
        Get
            Return lConnectedMediaDevice
        End Get
        Set(value As MediaDevice)
            lConnectedMediaDevice = value
        End Set
    End Property

#End Region

    '#Region "Android Device connection"
    '    Private Function ConnectToDevice() As Boolean
    '        Dim devices = MediaDevice.GetDevices
    '        ConnectedDevice = devices.First

    '        ConnectedDevice.Connect()
    '        If ConnectedDevice.IsConnected Then Return True Else Return False

    '    End Function
    '#End Region

#Region "INSPECTOR File copy handling"
    ''' <summary>
    ''' Copy a file from the android device to the local PC
    ''' </summary>
    ''' <param name="fileNameLocal"></param>
    ''' <param name="fileNameAndroid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' MOD 47 done
    Private Function copyFileFromAndroidDevice(fileNameLocal As String, fileNameAndroid As String) As Boolean
        Try
            Using stream As FileStream = File.OpenWrite(fileNameLocal)
                lConnectedMediaDevice.DownloadFile(fileNameAndroid, stream)
                Return True
            End Using
        Catch ex As Exception
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoSpecificMessage, ex.Message)
            Return False
        End Try
    End Function


#Region "PDA File copy handling"

    Private Sub CopyFileToDevice(localFileName As String, remoteFileName As String)
        If lConnectedMediaDevice.FileExists(remoteFileName) Then DeleFileFromDevice(remoteFileName)
        lConnectedMediaDevice.UploadFile(localFileName, remoteFileName)
    End Sub

    ''' <summary>
    ''' Copy mulitple files with extension from INSPECTOR to desktop PC
    ''' </summary>
    ''' <param name="localFileDir">Directory on the desktop PC</param>
    ''' <param name="remoteFileDir">Directory on the PDA</param>
    ''' <param name="fileExtenstion">File extension to copy (*.* = copy all files)</param>
    ''' <remarks></remarks>
    ''' MOD 47 done
    Private Sub CopyMultipleFilesFromDevice(localFileDir As String, remoteFileDir As String, fileExtenstion As String)

        Dim filedir = lConnectedMediaDevice.GetDirectoryInfo(remoteFileDir)
        Dim filesSearch = filedir.EnumerateFiles("*." & fileExtenstion, SearchOption.TopDirectoryOnly)

        For Each File In filesSearch
            Dim memoryStream As MemoryStream = New System.IO.MemoryStream
            lConnectedMediaDevice.DownloadFile(File.FullName, memoryStream)
            memoryStream.Position = 0
            Me.writeStreamToDisk(localFileDir & "\" + File.Name, memoryStream)
        Next

    End Sub
    ''' <summary>
    ''' Copy a file from from INSPECTOR to the desktop PC
    ''' </summary>
    ''' <param name="localFileName">Name of destination file on desktop PC</param>
    ''' <param name="remoteFileName">Name of source file on the PDA</param>
    ''' <remarks></remarks>
    ''' MOD 47
    Private Sub CopyFileFromDevice(localFileName As String, remoteFileName As String)
        If lConnectedMediaDevice.FileExists(remoteFileName) Then lConnectedMediaDevice.DownloadFile(remoteFileName, localFileName)
    End Sub
    ''' <summary>
    ''' Delete a file at INSPECTOR
    ''' </summary>
    ''' <param name="remoteFileName">Name of source file on the PDA</param>
    ''' <remarks></remarks>
    ''' MOD 47 done
    Private Sub DeleFileFromDevice(remoteFileName As String)
        Try
            lConnectedMediaDevice.DeleteFile(remoteFileName)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Move mulitple files with extension from PDA device to PC
    ''' </summary>
    ''' <param name="localFileDir">Directory on the PC</param>
    ''' <param name="remoteFileDir">Directory on the PDA</param>
    ''' <param name="fileExtenstion">File extension to move (*.* = move all files)</param>
    ''' <remarks></remarks>
    ''' MOD 47 
    Private Sub MoveMultipleFilesFromDevice(localFileDir As String, remoteFileDir As String, fileExtenstion As String)
        Dim filedir = lConnectedMediaDevice.GetDirectoryInfo(remoteFileDir)
        Dim filesSearch = filedir.EnumerateFiles(fileExtenstion, SearchOption.TopDirectoryOnly)

        For Each File In filesSearch
            Dim memoryStream As MemoryStream = New System.IO.MemoryStream
            lConnectedMediaDevice.DownloadFile(File.FullName, memoryStream)
            memoryStream.Position = 0
            Me.writeStreamToDisk(localFileDir & "\" + File.Name, memoryStream)
            lConnectedMediaDevice.DeleteFile(File.FullName)
        Next

    End Sub

    ''' MOD XXa done
    Private Sub writeStreamToDisk(ByVal filePath As String, ByVal memoryStream As MemoryStream)
        Using File As FileStream = New FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write)
            Dim bytes As Byte() = New Byte(memoryStream.Length - 1) {}
            memoryStream.Read(bytes, 0, CInt(memoryStream.Length))
            File.Write(bytes, 0, bytes.Length)
            memoryStream.Close()
        End Using
    End Sub

#End Region

#End Region


#Region "Functions to copy/Delete files INSPECTOR to/ from CONNEXION"


    'MOD 47 Done
    ''' <summary>
    ''' Copy the data from INSPECTOR to working dir
    ''' Move the files measurementdata files (*.fpr)
    ''' XML files (StationInformation, InspectionStatus, InspectionProcedure)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransferDataIPToDpc()
        If Not lConnectedMediaDevice.IsConnected Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")
            Exit Sub
        End If

        'Check dir exits. if not create
        If Not modFileHandling.DirExistsPC(modCommunicationPaths.MeasurementDataDirPC) Then Directory.CreateDirectory(modCommunicationPaths.MeasurementDataDirPC)

        'Copy all measurementdata files to desktop PC; Delete all measurementdata files from PDA
        MoveMultipleFilesFromDevice(modCommunicationPaths.MeasurementDataDirPC, modCommunicationPaths.IPMeasurementDataDir, "*.fpr")

        Try
            CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"))
            CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Inspectionstatus.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"))
            CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"))
        Catch ex As Exception
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoSpecificMessage, ex.Message)
        End Try

        'Create backup of transfered files

        'Check dir exits. if not create
        If Not modFileHandling.DirExistsPC(modCommunicationPaths.ArchiveDataDirPC) Then Directory.CreateDirectory(modCommunicationPaths.ArchiveDataDirPC)

        Dim fileName As String = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml")
        If CheckFileExistsPC(fileName) Then FileCopy(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "StationInformation_" & Trim(Format(Now, "yyyymmddHHmm")) & ".xml"))
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml")
        If CheckFileExistsPC(fileName) Then FileCopy(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "Results_" & Trim(Format(Now, "yyyymmddHHmm")) & ".xml"))
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "InspectionStatus.xml")
        If CheckFileExistsPC(fileName) Then FileCopy(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "InspectionStatus_" & Trim(Format(Now, "yyyymmddHHmm")) & ".xml"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub

    'MOD 47 Done
    ''' <summary>
    ''' Delete the results file at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteResultsIP()
        'Always delete xml and SDF files
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "ResultsLast.xml"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub

    'MOD 47 Done
    ''' <summary>
    ''' Delete the inspection procedure, stationinformation and inspectionstatus at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteFilesIP()
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionProcedure.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "PLEXOR.xml"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub

    ''' <summary>
    ''' Copy the files from desktop PC to INSPECTOR
    ''' The files are copied from the temporary directory TempSubDataDirPC_DPC2IP
    ''' </summary>
    ''' <remarks></remarks>
    'MOD 47 Done
    Public Sub TransferDataDpcToIP()
        Dim fileNamePda As String = ""
        Dim fileNamePC As String

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "StationInformation.xml")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")
            CopyFileToDevice(fileNamePC, fileNamePda)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "InspectionProcedure.xml")
            FileCopy(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml")
            CopyFileToDevice(fileNamePC, fileNamePda)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "ResultsLast.xml")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")
            CopyFileToDevice(fileNamePC, fileNamePda)
        End If

        'Copy file to local work directory
        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "PLEXOR.xml")
            FileCopy(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml")
            CopyFileToDevice(fileNamePC, fileNamePda)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")) Then
            fileNamePda = Path.Combine(IPDataDir, "LastUpload.txt")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
            CopyFileToDevice(fileNamePC, fileNamePda)
        End If


        'Update PLEXOR device list

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
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
        If CheckFileExistsPC(fileName) Then Kill(fileName)

        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")

    End Sub
#End Region



#Region "Inspector Paths determine"
    ''' <summary>
    ''' Determine the INSPECTOR android path 
    ''' If no path is found return False
    ''' </summary>
    ''' <remarks></remarks>
    'MOD 47 Done
    Public Sub GetInspectorPathOnIP()

        If Not lConnectedMediaDevice.IsConnected Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")
            Exit Sub
        End If

        Try
            If lConnectedMediaDevice.DirectoryExists(folderAndroidCard + folderSubInspectorData) Then
                'External flash Card
                modCommunicationPaths.IPGeneralDataDir = folderAndroidCard & folderSubInspectorData
            ElseIf lConnectedMediaDevice.DirectoryExists(folderAndroidTablet & folderSubInspectorData) Then
                'Internal flash
                modCommunicationPaths.IPGeneralDataDir = folderAndroidTablet & folderSubInspectorData
            Else
                RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoInspectorInstalled, "")
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox("Exception Android", ex.Message)
        End Try


        modCommunicationPaths.IPConfigDir = modCommunicationPaths.IPGeneralDataDir
        modCommunicationPaths.IPProgramDir = modCommunicationPaths.IPGeneralDataDir


        modCommunicationPaths.IPDataDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, "XML")
        modCommunicationPaths.IPMeasurementDataDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, "Measurements")
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")

    End Sub
#End Region

#Region "Get INSPECTOR Information"
    'MOD 47 Done
    Public Sub GetInspectorInformation()
        'If the PDA is not connected to the desktop PC
        If Not lConnectedMediaDevice.IsConnected Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")
            Exit Sub
        End If

        Dim pcfileNameIPSnr As String = Path.Combine(modCommunicationPaths.TempMainDataDirPC, "~$InspectorInfo" & Format(Now, "yyyymmddHHmmss") & ".XML")
        Dim androidfileNameIPSnr As String = ""
        If lConnectedMediaDevice.FileExists(folderAndroidCard & folderSubInspectorData & "XML\InspectorInfo.XML") Then
            'External flash Card
            androidfileNameIPSnr = folderAndroidCard & folderSubInspectorData & "XML\InspectorInfo.XML"
        ElseIf ConnectedDevice.FileExists(folderAndroidTablet & folderSubInspectorData & "XML\InspectorInfo.XML") Then
            'Internal flash
            androidfileNameIPSnr = folderAndroidTablet & folderSubInspectorData & "XML\InspectorInfo.XML"
        Else
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoFileInspectorInfo, "") '.CommunicatorPDAresx.str_InspectorInfoFileNotFound)
            Exit Sub
        End If

        'Copy the INSPECTOR information file to desktop
        If copyFileFromAndroidDevice(pcfileNameIPSnr, androidfileNameIPSnr) = False Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoSpecificMessage, "")
            Exit Sub
        End If

        'Reading the xml file into the object
        Dim xmlFile As XmlReader
        xmlFile = XmlReader.Create(pcfileNameIPSnr, New XmlReaderSettings())
        Dim ds As New DataSet
        ds.ReadXml(xmlFile)

        InspectionInformation.SerialNumber = ds.Tables(0).Rows(0).Item("PDASerialNumber").ToString
        InspectionInformation.Version = ds.Tables(0).Rows(0).Item("Version").ToString
        InspectionInformation.SubVersion = ds.Tables(0).Rows(0).Item("SubVersion").ToString
        InspectionInformation.LicenceNumber = ds.Tables(0).Rows(0).Item("RegistrationCode").ToString

        xmlFile.Close()

        'Delete temp file
        If CheckFileExistsPC(pcfileNameIPSnr) Then Kill(pcfileNameIPSnr)
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Exit Sub
    End Sub


#End Region


End Class
