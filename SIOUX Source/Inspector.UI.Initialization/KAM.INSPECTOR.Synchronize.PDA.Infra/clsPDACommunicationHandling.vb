'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports OpenNETCF.Desktop.Communication
Imports System.IO
Imports System.Xml
Imports System.Data
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Infra
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model.SynchronisationStep
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Imports KAM.COMMUNICATOR.Synchronize.Bussiness

''' <summary>
''' Class of INSPECTOR PDA file handling
''' </summary>
''' <remarks></remarks>
Public Class clsPDACommunicationHandling
#Region "Class members"
    'PDA rapi handling
    Private WithEvents myPdaRapi As New RAPI
    Event EvntRapiConnected()
    Event EvntRapiDisconnected()

    Event EvntCopyFileProgressStatus(totalFileSize As Integer, copiedFileSize As String, filename As String)
    Public Event EvntStatus(stepStatus As SyncStatus, errorcode As SyncErrorMessages.enumErrorMessages, additionalMessage As String)

#End Region
#Region "Properties"
    Public InspectionInformation As clsInspectorCommunication.StructInspectionInformation

    'Set the file format of the data files
    Public Property DataFileFormat As clsDbGeneral.enumFileFormat
        Get
            Return m_DataFileFormat
        End Get
        Set(value As clsDbGeneral.enumFileFormat)
            m_DataFileFormat = value
        End Set
    End Property
    Private m_DataFileFormat As clsDbGeneral.enumFileFormat

#End Region
#Region "Constructor"
    Public Sub New()
        ' Add any initialization after the InitializeComponent() call.
        AddHandler myPdaRapi.ActiveSync.Active, AddressOf EvntPdaConnectionActivated
        ' AddHandler sPDAException.InnerException, AddressOf evnterrorhandling

    End Sub
    ''' <summary>
    ''' Initialize the connection.
    ''' Check if the PDA is connected.
    ''' Used after create an instance.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InitializeConnection()
        If myPdaRapi.DevicePresent Then myPdaRapi.Connect()
    End Sub

#End Region

#Region "PDA Handling"
#Region "PDA connection Handling"
    ''' <summary>
    ''' Handling of RAPIConnected.
    ''' Raise event RapiConnected to higher level
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntPdaAConnection() Handles myPdaRapi.RAPIConnected
        RaiseEvent EvntRapiConnected()
    End Sub
    ''' <summary>
    ''' Handling of RAPIDisconnected.
    ''' Raise event RAPIDisconnected to higher level
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntPdaDisconnection() Handles myPdaRapi.RAPIDisconnected
        RaiseEvent EvntRapiDisconnected()
    End Sub
    ''' <summary>
    ''' Addhandler of myPDARapi.ActiveSync.Active
    ''' If the device if pressent, establish connection
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EvntPdaConnectionActivated()
        If myPdaRapi.DevicePresent Then myPdaRapi.Connect()
    End Sub
#End Region
#Region "PDA File copy handling"
    ''' <summary>
    ''' Copy mulitple files with extension from INSPECTOR to desktop PC
    ''' </summary>
    ''' <param name="localFileDir">Directory on the desktop PC</param>
    ''' <param name="remoteFileDir">Directory on the PDA</param>
    ''' <param name="fileExtenstion">File extension to copy (*.* = copy all files)</param>
    ''' <remarks></remarks>
    Private Sub CopyMultipleFilesFromDevice(localFileDir As String, remoteFileDir As String, fileExtenstion As String)
        Dim f As FileList = myPdaRapi.EnumFiles(Path.Combine(remoteFileDir, fileExtenstion))

        For i = 0 To f.Count - 1
            Try
                myPdaRapi.CopyFileFromDevice(localFileDir & f(i).FileName, remoteFileDir & f(i).FileName, True)
            Catch ex As Exception
            End Try
        Next
    End Sub
    ''' <summary>
    ''' Copy a file from from INSPECTOR to the desktop PC
    ''' </summary>
    ''' <param name="localFileName">Name of destination file on desktop PC</param>
    ''' <param name="remoteFileName">Name of source file on the PDA</param>
    ''' <remarks></remarks>
    Private Sub CopyFileFromDevice(localFileName As String, remoteFileName As String)
        Try
            myPdaRapi.CopyFileFromDevice(localFileName, remoteFileName, True)
        Catch ex As Exception
        End Try
    End Sub
    ''' <summary>
    ''' Delete a file at INSPECTOR
    ''' </summary>
    ''' <param name="remoteFileName">Name of source file on the PDA</param>
    ''' <remarks></remarks>
    Private Sub DeleFileFromDevice(remoteFileName As String)
        Try
            myPdaRapi.DeleteDeviceFile(remoteFileName)
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
    Private Sub MoveMultipleFilesFromDevice(localFileDir As String, remoteFileDir As String, fileExtenstion As String)
        Dim f As FileList = myPdaRapi.EnumFiles(Path.Combine(remoteFileDir, fileExtenstion))
        For i = 0 To f.Count - 1
            Try
                myPdaRapi.CopyFileFromDevice(Path.Combine(localFileDir, f(i).FileName), Path.Combine(remoteFileDir, f(i).FileName), True)
                myPdaRapi.DeleteDeviceFile(Path.Combine(remoteFileDir, f(i).FileName))
            Catch ex As Exception
            End Try
        Next
    End Sub
    ''' <summary>
    ''' Display the status\ progress of copy a file from device to desktop
    ''' </summary>
    ''' <param name="totalbytes"></param>
    ''' <param name="bytesLeft"></param>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Private Sub EventHandlingFileCopyProgress(totalbytes As Integer, bytesLeft As Integer, fileName As String) Handles myPdaRapi.FileCopyProcessStatus
        RaiseEvent EvntCopyFileProgressStatus(totalbytes, bytesLeft, fileName)
    End Sub
#End Region
#Region "Inspector Paths determine"
    ''' <summary>
    ''' Determine the INSPECTOR path on the PDA. 
    ''' If no path is found return False
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetInspectorPathOnIP()
        If Not myPdaRapi.Connected Then
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")
            Exit Sub
        End If
        If myPdaRapi.DeviceFileExists("\SD Card\INSPECTOR V5\") Then
            'For Ecom i.roc 520 and Ecom CN 70
            modCommunicationPaths.IPGeneralDataDir = "\SD Card\INSPECTOR V5\"
        ElseIf myPdaRapi.DeviceFileExists("\Storage Card\INSPECTOR V5\") Then
            'For Ecom i.roc 511
            modCommunicationPaths.IPGeneralDataDir = "\Storage Card\INSPECTOR V5\"
        Else
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoInspectorInstalled, "")
            Exit Sub
        End If

        modCommunicationPaths.IPConfigDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, "Config")
        modCommunicationPaths.IPProgramDir = modCommunicationPaths.IPGeneralDataDir

        modCommunicationPaths.IPDataDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, "Data")
        modCommunicationPaths.IPMeasurementDataDir = Path.Combine(modCommunicationPaths.IPGeneralDataDir, "Measurementdata")
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")

    End Sub
#End Region
#Region "Get pda information"
    Public Function GetDeviceSystemInfo() As SYSTEM_INFO
        Try
            Dim returnValue As SYSTEM_INFO
            myPdaRapi.GetDeviceSystemInfo(returnValue)
            Return returnValue
        Catch ex As Exception
        End Try
    End Function
    Public Function GetDeviceMemoryStatus() As MEMORYSTATUS
        Try
            Dim returnValue As MEMORYSTATUS
            myPdaRapi.GetDeviceMemoryStatus(returnValue)
            Return returnValue
        Catch ex As Exception
        End Try
    End Function
    Public Function GetDeviceStoreInformation() As STORE_INFORMATION
        Try
            Dim returnValue As STORE_INFORMATION
            myPdaRapi.GetDeviceStoreInformation(returnValue)
            Return returnValue
        Catch ex As Exception

        End Try
    End Function
    Public Function GetDeviceSystemPowerStatus() As SYSTEM_POWER_STATUS_EX
        Try
            Dim returnValue As SYSTEM_POWER_STATUS_EX
            myPdaRapi.GetDeviceSystemPowerStatus(returnValue)
            Return returnValue
        Catch ex As Exception
        End Try
    End Function


#End Region
#End Region

#Region "Functions to copy/Delete files INSPECTOR to/ from desktop"
    ''' <summary>
    ''' Copy the data from INSPECTOR to working dir
    ''' Move the files measurementdata files (*.fpr)
    ''' XML files (StationInformation, InspectionStatus, InspectionProcedure)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransferDataIPToDpc()
        'check if device is present
        If myPdaRapi.DevicePresent = False Then RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")

        'Check dir exits. if not create
        If Not modFileHandling.DirExistsPC(modCommunicationPaths.MeasurementDataDirPC) Then Directory.CreateDirectory(modCommunicationPaths.MeasurementDataDirPC)

        'Copy all measurementdata files to desktop PC; Delete all measurementdata files from PDA
        MoveMultipleFilesFromDevice(modCommunicationPaths.MeasurementDataDirPC, modCommunicationPaths.IPMeasurementDataDir, "*.fpr")

        Try
            If m_DataFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatXml.ToString Then
                CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"))
                CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Inspectionstatus.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"))
                CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.xml"), Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"))
            ElseIf m_DataFileFormat.ToString = clsDbGeneral.enumFileFormat.FormatSdf.ToString Then
                CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.sdf"), Path.Combine(modCommunicationPaths.IPDataDir, "Results.sdf"))
                CopyFileFromDevice(Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.sdf"), Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.sdf"))
            End If
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
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "StationInformation.sdf")
        If CheckFileExistsPC(fileName) Then FileCopy(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "StationInformation_" & Trim(Format(Now, "yyyymmddHHmm")) & ".sdf"))
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_IP2DPC, "Results.sdf")
        If CheckFileExistsPC(fileName) Then FileCopy(fileName, Path.Combine(modCommunicationPaths.ArchiveDataDirPC, "Results_" & Trim(Format(Now, "yyyymmddHHmm")) & ".sdf"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Delete the results file at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteResultsIP()
        'Always delete xml and SDF files
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "Results.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "ResultsLast.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "Results.sdf"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Delete the inspection procedure, stationinformation and inspectionstatus at INSPECTOR
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteFilesIP()
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionProcedure.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "testprog.txt"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "progidx.txt"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "errorcodes.txt"))

        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.xml"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "StationInformation.sdf"))
        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "InspectionStatus.xml"))

        DeleFileFromDevice(Path.Combine(modCommunicationPaths.IPDataDir, "PLEXOR.xml"))
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
    End Sub
    ''' <summary>
    ''' Copy the files from desktop PC to INSPECTOR
    ''' The files are copied from the temporary directory TempSubDataDirPC_DPC2IP
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransferDataDpcToIP()
        Dim fileNamePda As String = ""
        Dim fileNamePC As String

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "StationInformation.xml")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.xml")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")) Then
            fileNamePda = Path.Combine(IPDataDir, "StationInformation.sdf")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
            If CheckFileExistsPC(fileNamePC) Then Kill(fileNamePC)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "InspectionProcedure.xml")
            FileCopy(Path.Combine(modCommunicationPaths.InspProcDataDirPC, "InspectionProcedure.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$InspectionProcedure.xml")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "ResultsLast.xml")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$ResultsLast.xml")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")) Then
            fileNamePda = Path.Combine(IPDataDir, "Results.sdf")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
        End If

        'Copy file to local work directory
        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml")) Then
            fileNamePda = Path.Combine(IPDataDir, "PLEXOR.xml")
            FileCopy(Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXOR.xml"), Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml"))
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$PLexor.xml")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
        End If

        If CheckFileExistsPC(Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")) Then
            fileNamePda = Path.Combine(IPDataDir, "LastUpload.txt")
            fileNamePC = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
            myPdaRapi.CopyFileToDevice(fileNamePC, fileNamePda, True)
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
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$StationInformation.sdf")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$Results.sdf")
        If CheckFileExistsPC(fileName) Then Kill(fileName)
        fileName = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP, "~$LastUpload.txt")
        If CheckFileExistsPC(fileName) Then Kill(fileName)

        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")

    End Sub
#End Region



#Region "Get Inspector Information"
    ''' <summary>
    ''' Getting the file InspectorInfo.xml from the PDA.
    ''' If retreived the file is read and InspectionInformation is set with the information
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetInspectorInformation()
        'If the PDA is not connected to the desktop PC
        If Not myPdaRapi.Connected Then RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoDeviceConnected, "")

        Dim pcfileNamePdaSnr As String = Path.Combine(modCommunicationPaths.TempMainDataDirPC, "~$InspectorInfo" & Format(Now, "yyyymmddHHmmss") & ".XML")

        'Getting version information from PDA
        Dim pdafileNamePdaSnr As String = ""
        If myPdaRapi.DeviceFileExists("\Windows\" & "InspectorInfo.XML") Then
            pdafileNamePdaSnr = Path.Combine("\Windows", "InspectorInfo.XML")
        ElseIf myPdaRapi.DeviceFileExists("\Windows\" & "InspectorInfo.XML") Then
            pdafileNamePdaSnr = Path.Combine(IPDataDir, "InspectorInfo.XML")
        Else
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoFileInspectorInfo, "") '.CommunicatorPDAresx.str_InspectorInfoFileNotFound)
            Exit Sub
        End If

        'Copy the INSPECTOR information file to desktop
        Try
            If myPdaRapi.DeviceFileExists(pdafileNamePdaSnr) = True Then myPdaRapi.CopyFileFromDevice(pcfileNamePdaSnr, pdafileNamePdaSnr, True)
        Catch ex As Exception
            RaiseEvent EvntStatus(SyncStatus.SError, enumErrorMessages.NoSpecificMessage, ex.Message)
        End Try


        'Reading the xml file into the object
        Dim xmlFile As XmlReader
        xmlFile = XmlReader.Create(pcfileNamePdaSnr, New XmlReaderSettings())
        Dim ds As New DataSet
        ds.ReadXml(xmlFile)


        InspectionInformation.SerialNumber = ds.Tables(0).Rows(0).Item("PDASerialNumber").ToString
        InspectionInformation.Version = ds.Tables(0).Rows(0).Item("Version").ToString
        InspectionInformation.SubVersion = ds.Tables(0).Rows(0).Item("SubVersion").ToString
        xmlFile.Close()

        'Delete temp file
        If CheckFileExistsPC(pcfileNamePdaSnr) Then Kill(pcfileNamePdaSnr)
        RaiseEvent EvntStatus(SyncStatus.Succes, enumErrorMessages.NoError, "")
        Exit Sub
    End Sub
#End Region



End Class
