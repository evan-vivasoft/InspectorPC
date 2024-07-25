Public Class SyncErrorMessages

    Public Delegate Sub EventFileError(errorCode As enumErrorMessages, additionalMessage As String)

    Public Class enumErrorMessages
        Private key As Integer

        Public Shared ReadOnly NoError As New enumErrorMessages(60000)
        Public Shared ReadOnly NoSpecificMessage As New enumErrorMessages(61000)
        Public Shared ReadOnly Error02 As New enumErrorMessages(61001)

        'INSPECTOR PDA
        Public Shared ReadOnly NoDeviceConnected As New enumErrorMessages(62001)
        Public Shared ReadOnly NoInspectorInstalled As New enumErrorMessages(62002)

        'File Errors
        Public Shared ReadOnly NoFileInspectorInfo As New enumErrorMessages(63001)
        Public Shared ReadOnly NoInspectorInCommunicatorDb As New enumErrorMessages(63002)
        Public Shared ReadOnly NonSupportedFileFormat As New enumErrorMessages(63003)
        Public Shared ReadOnly NonExistingFile As New enumErrorMessages(63004)
        Public Shared ReadOnly NonDirectoryDefined As New enumErrorMessages(63005)


        Private Sub New(key As Integer)
            Me.key = key
        End Sub

        Public Overrides Function ToString() As string
            Return Me.key
        End Function

    End Class
End Class
