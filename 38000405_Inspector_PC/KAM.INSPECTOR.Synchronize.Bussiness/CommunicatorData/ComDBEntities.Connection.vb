Imports ALinq
Imports ALinq.Mapping
Imports ALinq.Access
Imports System.ComponentModel
Imports System.Reflection
Imports System.IO
Imports System.Data.OleDb

Namespace Model.CommunicatorData
    Partial Public Class clsLinqToMDB
        Inherits ALinq.DataContext
#Region "Extensibility Method Definitions"
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub InserttblPDA(instance As tblPDA)
        End Sub
        Partial Private Sub UpdatetblPDA(instance As tblPDA)
        End Sub
        Partial Private Sub DeletetblPDA(instance As tblPDA)
        End Sub
        Partial Private Sub InserttblRevision(instance As tblRevision)
        End Sub
        Partial Private Sub UpdatetblRevision(instance As tblRevision)
        End Sub
        Partial Private Sub DeletetblRevision(instance As tblRevision)
        End Sub
#End Region
        Public Sub New(connection As String)
            MyBase.New(connection)
        End Sub
        Public ReadOnly Property tblPDAs() As ALinq.Table(Of tblPDA)
            Get
                Return Me.GetTable(Of tblPDA)()
            End Get
        End Property
        Public ReadOnly Property tblRevisions() As ALinq.Table(Of tblRevision)
            Get
                Return Me.GetTable(Of tblRevision)()
            End Get
        End Property
    End Class

End Namespace
