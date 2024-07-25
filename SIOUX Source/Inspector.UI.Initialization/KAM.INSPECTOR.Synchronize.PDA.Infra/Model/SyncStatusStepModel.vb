'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
'Imports COMMUNICATOR.Model
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Namespace COMMUNICATOR.Status.Model
    ''' <summary>
    ''' 
    ''' </summary>
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
            Me.StepErrorCode = enumErrorMessages.NoError
            Me.SetpAdditionalMessage = ""
        End Sub
#End Region
    End Class



    '//===============================================================================
    '// Copyright Wigersma
    '// All rights reserved.
    '//===============================================================================
    '
    ''' <summary>
    ''' Synchronisation step information
    ''' </summary>
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
        ''' Getting path information from PDA
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Get_Path_Pda As New SynchronisationStep(5000)

        ''' <summary>
        ''' Getting inspector information
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Get_Inspector_Information As New SynchronisationStep(5001)

        ''' <summary>
        ''' Updating software version in database
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Update_Software_in_Database As New SynchronisationStep(5002)

        ''' <summary>
        ''' Getting the paths
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Getting_Paths_From_Database As New SynchronisationStep(5003)

        ''' <summary>
        ''' Creating temp directory
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Creating_temp_directory As New SynchronisationStep(5004)

        ''' <summary>
        ''' Getting PDA information
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Getting_pda_information As New SynchronisationStep(5005)


        ''' <summary>
        '''  Copy data from Inspector to working dir
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Copy_files_From_Inspector As New SynchronisationStep(5006)

        ''' <summary>
        '''  Process result files from Inspector
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Process_Result_file_Inspector As New SynchronisationStep(5007)

        ''' <summary>
        '''  Deleting results files at Inspector
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Delete_Result_files_Inspector As New SynchronisationStep(5008)

        ''' <summary>
        '''  Creating station information file
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Create_Station_information_File As New SynchronisationStep(5009)

        ''' <summary>
        '''  Deleting station files at Inspector
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Delete_station_File_Inspector As New SynchronisationStep(5010)

        ''' <summary>
        '''  Copy data files to Inspector
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Copy_files_to_Inspector As New SynchronisationStep(5011)

        ''' <summary>
        ''' Transfer Completed
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Transfer_Completed As New SynchronisationStep(5012)

        ''' <summary>
        ''' Transfer Completed
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Create_Results_File As New SynchronisationStep(5013)

        ''' <summary>
        ''' Delete temp files
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Delete_Temp_File As New SynchronisationStep(5014)

        ''' <summary>
        ''' Transfer Error
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Transfer_Error As New SynchronisationStep(5015)


        ''' <summary>
        ''' Set last upload information
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Set_last_Upload As New SynchronisationStep(5016)

        ''' <summary>
        ''' Getting the inspector synchronisation options
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Get_Inspector_Sync_options As New SynchronisationStep(5017)


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
End Namespace