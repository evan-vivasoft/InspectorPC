'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Public Class clsDbGeneral
    ''' <summary>
    ''' Event for showing how many records are processed
    ''' </summary>
    ''' <param name="total">Total amount of records to be processed</param>
    ''' <param name="processed">Amount of records which are processed</param>
    ''' <param name="recordInfo">Information about record</param>
    ''' <remarks></remarks>
    Public Delegate Sub EvntdbFileProcessStatus(typeFile As DbCreateType, total As Integer, processed As Integer, status As DbCreateStatus, recordInfo As String)
    Public Delegate Sub EventdbFileError(errorCode As enumFileErrorMessages, additionalMessage As String)

    ''' <summary>
    ''' Status of exporting the file
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DbCreateStatus
        StartedCreate
        StartedWrite
        SuccesWrite
        TimeOut
        SError
        Unset
        Warning
        ErrorWrite
        FileNotExists
        ErrorXsd 'MOD 21
    End Enum

    ''' <summary>
    ''' Status of exporting the file
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DbCreateType
        Prs
        Result
    End Enum

    ''' <summary>
    ''' Class enum file format.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class enumFileFormat
        Private key As String

        Public Shared ReadOnly FormatMsAccess As New enumFileFormat("MsAccess")
        Public Shared ReadOnly FormatSdf As New enumFileFormat("SDF")
        Public Shared ReadOnly FormatXml As New enumFileFormat("XML")

        Private Sub New(key As String)
            Me.key = key
        End Sub

        Public Overrides Function ToString() As String
            Return Me.key
        End Function
    End Class

    Public Class enumFileErrorMessages
        Private key As Integer

        Public Shared ReadOnly NoError As New enumFileErrorMessages(60000)
        Public Shared ReadOnly Error01 As New enumFileErrorMessages(61000)
        Public Shared ReadOnly Error02 As New enumFileErrorMessages(61001)

        Private Sub New(key As String)
            Me.key = key
        End Sub

        Public Overrides Function ToString() As String
            Return Me.key
        End Function

    End Class

End Class