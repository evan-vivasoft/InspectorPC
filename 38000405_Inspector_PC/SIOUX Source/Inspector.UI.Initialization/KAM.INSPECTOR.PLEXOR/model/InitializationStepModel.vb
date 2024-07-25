'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Inspector.Model

Namespace Initialization.Model
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class InitializationStepModel
#Region "Class members"
        Private m_StepId As String
        Private m_StepName As String
        Private m_StepResult As InitializationStepResult
        Private m_StepMessage As String
        Private m_StepErrorCode As Integer
        Private m_StepNumber As Integer
#End Region
#Region "Properties"
        ''' <summary>
        ''' Gets or sets the step id.
        ''' </summary>
        ''' <value>
        ''' The step id.
        ''' </value>
        Public Property StepNumber() As String
            Get
                Return m_StepNumber
            End Get
            Set(value As String)
                If m_StepNumber <> value Then
                    m_StepNumber = value
                    '****OnPropertyChanged("StepId")
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the step id.
        ''' </summary>
        ''' <value>
        ''' The step id.
        ''' </value>
        Public Property StepId() As String
            Get
                Return m_StepId
            End Get
            Set(value As String)
                If m_StepId <> value Then
                    m_StepId = value
                    '****OnPropertyChanged("StepId")
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the step.
        ''' </summary>
        ''' <value>
        ''' The name of the step.
        ''' </value>
        Public Property StepName() As String
            Get
                Return m_StepName
            End Get
            Set(value As String)
                If m_StepName <> value Then
                    m_StepName = value
                    '****OnPropertyChanged("StepName")
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the step result.
        ''' </summary>
        ''' <value>
        ''' The step result.
        ''' </value>
        Public Property StepResult() As InitializationStepResult
            Get
                Return m_StepResult
            End Get
            Set(value As InitializationStepResult)
                If m_StepResult <> value Then
                    m_StepResult = value
                    '****OnPropertyChanged("StepResult")
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the step message.
        ''' </summary>
        ''' <value>
        ''' The step message.
        ''' </value>
        Public Property StepMessage() As String
            Get
                Return m_StepMessage
            End Get
            Set(value As String)
                If m_StepMessage <> value Then
                    m_StepMessage = value
                    '****OnPropertyChanged("StepMessage")
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the step error code.
        ''' </summary>
        ''' <value>
        ''' The step error code.
        ''' </value>
        Public Property StepErrorCode() As Integer
            Get
                Return m_StepErrorCode
            End Get
            Set(value As Integer)
                If m_StepErrorCode <> value Then
                    m_StepErrorCode = value
                    '****OnPropertyChanged("StepErrorCode")
                End If
            End Set
        End Property
#End Region
#Region "Constructor"
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="name">The name.</param>
        ''' <param name="result">The result.</param>
        ''' <param name="message">The message.</param>
        ''' <param name="errorCode">The error code.</param>
        Public Sub New(stepNumber As Integer, id As String, name As String, result As InitializationStepResult, message As String, errorCode As Integer)
            Me.StepNumber = stepNumber
            Me.StepId = id
            Me.StepName = name
            Me.StepResult = result
            Me.StepMessage = message
            Me.StepErrorCode = errorCode
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="name">The name.</param>
        Public Sub New(stepNumber As String, id As String, name As String)
            Me.StepNumber = stepNumber
            Me.StepId = id
            Me.StepName = name
            Me.StepResult = InitializationStepResult.UNSET
            Me.StepMessage = [String].Empty
            Me.StepErrorCode = -1
        End Sub
#End Region
    End Class


End Namespace