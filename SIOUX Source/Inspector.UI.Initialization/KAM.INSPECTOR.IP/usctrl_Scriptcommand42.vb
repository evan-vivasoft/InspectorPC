Imports Inspector.Model

Public Class usctrl_Scriptcommand42
#Region "Class members"
    Public Event evntNext(stepSequenceNumber As Integer)
    Public Event evntNextWithResult(stepSequenceNumber As Integer, resultText As String)
    Public _scriptcommand42 As New InspectionProcedure.ScriptCommand42
#End Region
#Region "Command"
    ''' <summary>
    ''' Intialized
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        'Capture clicking the the next button of the main screen
    End Sub

    ''' <summary>
    ''' Sending the information with the inspection result
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveResult()
        RaiseEvent evntNextWithResult(_scriptcommand42.SequenceNumber, radTxtBRemarks.Text.ToString)
    End Sub
#End Region
#Region "Properties"
    ''' <summary>
    ''' Set script command 42 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand42() As InspectionProcedure.ScriptCommand42
        Set(ByVal value As InspectionProcedure.ScriptCommand42)
            _scriptcommand42 = value
        End Set
    End Property

    Public Property RemarkValue As string
        'MOD 61
        Set(value As String)
            radTxtBRemarks.Text = value
        End Set
        Get
            Return radTxtBRemarks.Text.ToString
        End Get
    End Property

#End Region
End Class
