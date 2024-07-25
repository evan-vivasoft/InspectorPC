Imports System.IO
Imports KAM.Infra

''' <summary>
''' module of paths on desktop Pc and Inspector (PC and PDA version of INSPECTOR)
''' </summary>
''' <remarks></remarks>
Public Module modCommunicationPaths
#Region "Constants"
    'Main = definition/ relation to CONNEXION general paths
    'IP = definiteion/ relation to INSPECTOR PC/ PDA paths

    'Paths on desktop PC
    Private Const PcDataDir_IP2DPC As String = "IP2DPC"
    Private Const PcDataDir_DPC2IP As String = "DPC2IP"
#End Region

#Region "Properties"
    ''' <summary>
    ''' The serial number of INSPECTOR; 
    ''' This serial number is used to create a temporary directory 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Is set by COMMUNICATOR</remarks>
    Public Property InspectorSerialnumber As String
        Get
            Return m_InspectorSerialNumber
        End Get
        Set(value As String)
            m_InspectorSerialNumber = value
        End Set
    End Property
    Private m_InspectorSerialNumber As String

#Region "INSPECTOR settings/ Paths"
    ''' <summary>
    ''' Path of the general dir of INSPECTOR PC/ PDA
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Is set by COMMUNICATOR</remarks>
    Public Property IPGeneralDataDir As String
        Get
            Return m_IP_GeneralDir
        End Get
        Set(value As String)
            m_IP_GeneralDir = value
        End Set
    End Property
    Private m_IP_GeneralDir As String

    ''' <summary>
    ''' Path of the data directory of INSPECTOR PC/ PDA
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IPDataDir As String
        Get
            Return m_IP_DataDir
        End Get
        Set(value As String)
            m_IP_DataDir = value
        End Set
    End Property
    Private m_IP_DataDir As String

    ''' <summary>
    ''' Path of the configuration directory of INSPECTOR PC/ PDA
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IPConfigDir As String
        Get
            Return m_IP_ConfigDir
        End Get
        Set(value As String)
            m_IP_ConfigDir = value
        End Set
    End Property
    Private m_IP_ConfigDir As String

    ''' <summary>
    ''' Path of the measurement data directory of INSPECTOR PC/ PDA
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IPMeasurementDataDir As String
        Get
            Return m_IP_MeasurementDataDir
        End Get
        Set(value As String)
            m_IP_MeasurementDataDir = value
        End Set
    End Property
    Private m_IP_MeasurementDataDir As String
    ''' <summary>
    ''' Path of the program directory of INSPECTOR PC/ PDA
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IPProgramDir As String
        Get
            Return m_IP_ProgramDir
        End Get
        Set(value As String)
            m_IP_ProgramDir = value
        End Set
    End Property
    Private m_IP_ProgramDir As String
#End Region
#Region "PC paths"

    ''' <summary>
    ''' Path of the XSD for COMMUNICATOR. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR</remarks>
    Public Property XsdDirPC As String
        Get
            Return m_XsdDirPC
        End Get
        Set(value As String)
            m_XsdDirPC = value
        End Set
    End Property
    Private m_XsdDirPC As String

    ''' <summary>
    ''' Path of the measurement data directory on the desktop Pc
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property MeasurementDataDirPC As String
        Get
            Return m_MeasurementDataDirPC
        End Get
        Set(value As String)
            m_MeasurementDataDirPC = value
        End Set
    End Property
    Private m_MeasurementDataDirPC As String

    ''' <summary>
    ''' Path of the archive directory on the desktop PC
    ''' The files of INSPECTOR are copied to this directory
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property ArchiveDataDirPC As String
        Get
            Return m_ArchiveDataDirPC
        End Get
        Set(value As String)
            m_ArchiveDataDirPC = value
        End Set
    End Property
    Private m_ArchiveDataDirPC As String

    ''' <summary>
    ''' Path of the stationinformation file on the desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property PrsLoadDataDirPC As String
        Get
            Return m_PRSLoadDataDirPC
        End Get
        Set(value As String)
            m_PRSLoadDataDirPC = value
        End Set
    End Property
    Private m_PRSLoadDataDirPC As String

    ''' <summary>
    ''' Path of the inspection procedure file on the desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property InspProcDataDirPC As String
        Get
            Return m_InspProcDataDirPC
        End Get
        Set(value As String)
            m_InspProcDataDirPC = value
        End Set
    End Property
    Private m_InspProcDataDirPC As String

    ''' <summary>
    ''' Path of the PLEXOR file on the desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property PlexorDataDirPC As String
        Get
            Return m_PlexorDataDirPC
        End Get
        Set(value As String)
            m_PlexorDataDirPC = value
        End Set
    End Property
    Private m_PlexorDataDirPC As String

    ''' <summary>
    '''  Path of the results file on the desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Set by COMMUNICATOR; Retreived for the "COMMUNICATORdata.mdb" database</remarks>
    Public Property ResultDataDirPC As String
        Get
            Return m_ResultDataDirPC
        End Get
        Set(value As String)
            m_ResultDataDirPC = value
        End Set
    End Property
    Private m_ResultDataDirPC As String

#End Region
#Region "Temporary files on desktop PC"
    ''' <summary>
    ''' Path of the temporary directory on the desktop PC.
    ''' Used as working directory
    ''' System.IO.Path.GetTempPath, "WS-gasCON5"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TempMainDataDirPC As String
        Get
            'MOD 08
            Dim tempDir As String = Path.Combine(System.IO.Path.GetTempPath, "WS-gasCON5")
            If Not CheckFileExistsPC(tempDir) Then Directory.CreateDirectory(tempDir)
            Return tempDir
        End Get

    End Property
    ''' <summary>
    ''' Path of the temporary directory on the Desktop PC.
    ''' System.IO.Path.GetTempPath, "WS-gasCON5"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TempSubDataDirPC As String
        Get

            Return Path.Combine(System.IO.Path.GetTempPath, "WS-gasCON5", stripSerialnumber(m_InspectorSerialNumber))
        End Get
    End Property
    ''' <summary>
    ''' Path of the temporary directory on the desktop PC
    ''' This directory is used as working directory
    ''' Used for files from Inspector to desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TempSubDataDirPC_IP2DPC As String
        Get
            Return Path.Combine(System.IO.Path.GetTempPath, "WS-gasCON5", stripSerialnumber(m_InspectorSerialNumber), PcDataDir_IP2DPC)
        End Get
    End Property
    ''' <summary>
    ''' Path of the temporary directory on the desktop PC
    ''' This directory is used as working directory
    ''' Used for files from Inspector to desktop PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TempSubDataDirPC_DPC2IP As String
        Get
            Return Path.Combine(System.IO.Path.GetTempPath, "WS-gasCON5", stripSerialnumber(m_InspectorSerialNumber), PcDataDir_DPC2IP)
        End Get
    End Property

#End Region
#End Region
#Region "Serialnumber strip"
    ''' <summary>
    ''' Strip a text 
    ''' </summary>
    ''' <param name="textToStrip"></param>
    ''' <returns>The stripped text</returns>
    ''' <remarks></remarks>
    Private Function stripSerialnumber(textToStrip) As String
        Dim stripText As String
        stripText = Replace(textToStrip, ":", "")
        stripText = Replace(stripText, "-", "")
        stripText = Replace(stripText, "\", "")
        stripText = Replace(stripText, "/", "")
        Return stripText
    End Function
#End Region

#Region "General file Handling"
    ''' <summary>
    ''' Create a temporary direcotry for copying the files between PDA and PC
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateTempDirectory() As Boolean
        'Create temp dir for connected PDA
        Dim directoryTemp As String = ""
        If Not CheckFileExistsPC(TempSubDataDirPC) Then Directory.CreateDirectory(TempSubDataDirPC)

        directoryTemp = Path.Combine(modCommunicationPaths.TempSubDataDirPC_DPC2IP)
        If Not CheckFileExistsPC(directoryTemp) Then Directory.CreateDirectory(directoryTemp)
        DeleteMultipleFilePC(directoryTemp, "*.xml")
        DeleteMultipleFilePC(directoryTemp, "*.txt")
        DeleteMultipleFilePC(directoryTemp, "*.sdf")

        directoryTemp = Path.Combine(modCommunicationPaths.TempSubDataDirPC, modCommunicationPaths.PcDataDir_IP2DPC)
        If Not CheckFileExistsPC(directoryTemp) Then Directory.CreateDirectory(directoryTemp)
        DeleteMultipleFilePC(directoryTemp, "*.xml")
        DeleteMultipleFilePC(directoryTemp, "*.txt")
        DeleteMultipleFilePC(directoryTemp, "*.sdf")
        Return True
    End Function
#End Region



End Module
