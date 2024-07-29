Public Class SyncStatusStepModel
#Region "Class members"

#End Region
#Region "Properties"
    ''' <summary>
    ''' Gets or sets the step id.
    ''' </summary>
    ''' <value>
    ''' The step id.
    ''' </value>
    Public Property StepDateTime() As DateTime
        Get
            Return m_StepDateTime
        End Get
        Set(value As DateTime)
            If m_StepDateTime <> value Then
                m_StepDateTime = value
            End If
        End Set
    End Property
    Private m_StepDateTime As DateTime
    ''' <summary>
    ''' Gets or sets the step id.
    ''' </summary>
    ''' <value>
    ''' The step id.
    ''' </value>
    Public Property StepId() As SynchronisationStep
        Get
            Return m_StepId
        End Get
        Set(value As SynchronisationStep)
            m_StepId = value
        End Set
    End Property
    Private m_StepId As SynchronisationStep
    ''' <summary>
    ''' Gets or sets the step result.
    ''' </summary>
    ''' <value>
    ''' The step result.
    ''' </value>
    Public Property StepResult() As SynchronisationStep.SyncStatus
        Get
            Return m_StepResult
        End Get
        Set(value As SynchronisationStep.SyncStatus)
            If m_StepResult <> value Then
                m_StepResult = value
            End If
        End Set
    End Property
    Private m_StepResult As SynchronisationStep.SyncStatus
    ''' <summary>
    ''' Gets or sets the step error code.
    ''' </summary>
    ''' <value>
    ''' The step error code.
    ''' </value>
    Public Property StepErrorCode() As SyncErrorMessages.enumErrorMessages
        Get
            Return m_StepErrorCode
        End Get
        Set(value As SyncErrorMessages.enumErrorMessages)
            m_StepErrorCode = value
        End Set
    End Property
    Private m_StepErrorCode As SyncErrorMessages.enumErrorMessages
    ''' <summary>
    ''' Gets or sets the step additional message.
    ''' </summary>
    ''' <value>
    ''' The additional message.
    ''' </value>
    Public Property SetpAdditionalMessage() As String
        Get
            Return m_StepAdditionalMessage
        End Get
        Set(value As String)
            m_StepAdditionalMessage = value

        End Set
    End Property
    Private m_StepAdditionalMessage As String

#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initializes a new instance of the class.
    ''' </summary>
    ''' <param name="stepID">The id.</param>
    ''' <param name="result">The result.</param>
    ''' <param name="errorCode">The error code.</param>
    Public Sub New(stepID As SynchronisationStep, result As SynchronisationStep.SyncStatus, errorCode As SyncErrorMessages.enumErrorMessages, additionalMessage As String)
        Me.StepDateTime = Now
        Me.StepId = stepID
        Me.StepResult = result
        Me.StepErrorCode = errorCode
        Me.SetpAdditionalMessage = additionalMessage
    End Sub
    ''' <summary>
    ''' Initializes a new instance of the class.
    ''' </summary>
    ''' <param name="stepID">The id.</param>
    Public Sub New(stepID As SynchronisationStep)
        Me.StepDateTime = Now
        Me.StepId = stepID
        Me.StepResult = SynchronisationStep.SyncStatus.Unset
        Me.StepErrorCode = SyncErrorMessages.enumErrorMessages.NoError
        Me.SetpAdditionalMessage = ""
    End Sub
#End Region
End Class

Public NotInheritable Class SynchronisationStep
    Private Sub New()
    End Sub


#Region "Copy File steps"
    Private key As Integer


    Private Sub New(key As Integer)
        Me.key = key
    End Sub

    Public Overrides Function ToString() As String
        Return Me.key
    End Function

    ''' <summary>
    ''' An unexpected error occurred in the HAL
    ''' </summary>
    Public Shared Inspector_to_PC_Start_Copy_FPR As New SynchronisationStep(1000)
    Public Shared Inspector_to_PC_Complete_Copy_FPR As New SynchronisationStep(1001)


    ''' <summary>
    ''' Login using plexorOnline
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Connect_with_plexoronline As New SynchronisationStep(5000)

    ''' <summary>
    ''' Get Updated license status and verify with stored licenseInfo
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Check_license_information As New SynchronisationStep(5001)

    ''' <summary>
    ''' Check if there's any result
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Check_result As New SynchronisationStep(5002)

    ''' <summary>
    ''' If there's result then transfer the result
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Transfer_result As New SynchronisationStep(5003)

    ''' <summary>
    ''' Remove result.json file and the fpr data
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Remove_result As New SynchronisationStep(5004)

    ''' <summary>
    ''' Getting PRS info, InspectionProcedureInfo, Plexor Device info, IPCSettings file
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Get_updated_data As New SynchronisationStep(5005)


    ''' <summary>
    '''  Get last result
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Get_last_result As New SynchronisationStep(5006)

    ''' <summary>
    '''  Data Sync Complete
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Data_sync_complete As New SynchronisationStep(5007)

    ''' <summary>
    ''' Set last upload information
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Set_last_upload As New SynchronisationStep(5008)

    Public Shared Transfer_error As New SynchronisationStep(5009)

    Public Enum SyncStatus
        Started
        Succes
        TimeOut
        SError
        Unset
        Warning
    End Enum

#End Region



End Class

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
        Public Shared ReadOnly PlexorLoginError As New enumErrorMessages(63006)
        Public Shared ReadOnly LicenseInformationNotValid As New enumErrorMessages(63007)


        Private Sub New(key As Integer)
            Me.key = key
        End Sub

        Public Overrides Function ToString() As String
            Return Me.key
        End Function

    End Class
End Class