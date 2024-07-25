Imports ALinq
Imports ALinq.Mapping
Imports System.ComponentModel
Imports System.Reflection

Namespace PRSMDB.DataAccess
    Public Class StationInformation

        Inherits ALinq.DataContext
#Region "Extensibility Method Definitions"
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub InserttblDBVersion(instance As tblDBVersion)
        End Sub
        Partial Private Sub UpdatetblDBVersion(instance As tblDBVersion)
        End Sub
        Partial Private Sub DeletetblDBVersion(instance As tblDBVersion)
        End Sub
        Partial Private Sub InserttblGCLdata(instance As tblGCLdata)
        End Sub
        Partial Private Sub UpdatetblGCLdata(instance As tblGCLdata)
        End Sub
        Partial Private Sub DeletetblGCLdata(instance As tblGCLdata)
        End Sub
        Partial Private Sub InserttblGCLObject(instance As tblGCLObject)
        End Sub
        Partial Private Sub UpdatetblGCLObject(instance As tblGCLObject)
        End Sub
        Partial Private Sub DeletetblGCLObject(instance As tblGCLObject)
        End Sub
        Partial Private Sub InserttblPRSdata(instance As tblPRSdata)
        End Sub
        Partial Private Sub UpdatetblPRSdata(instance As tblPRSdata)
        End Sub
        Partial Private Sub DeletetblPRSdata(instance As tblPRSdata)
        End Sub
        Partial Private Sub InserttblPRSObject(instance As tblPRSObject)
        End Sub
        Partial Private Sub UpdatetblPRSObject(instance As tblPRSObject)
        End Sub
        Partial Private Sub DeletetblPRSObject(instance As tblPRSObject)
        End Sub
        Partial Private Sub InserttlbIdentificationCode(instance As tlbIdentificationCode)
        End Sub
        Partial Private Sub UpdatetlbIdentificationCode(instance As tlbIdentificationCode)
        End Sub
        Partial Private Sub DeletetlbIdentificationCode(instance As tlbIdentificationCode)
        End Sub
#End Region
        Public Sub New(connection As String)
            MyBase.New(connection)
        End Sub

        Public ReadOnly Property tblDBVersions() As ALinq.Table(Of tblDBVersion)
            Get
                Return Me.GetTable(Of tblDBVersion)()
            End Get
        End Property

        Public ReadOnly Property tblGCLdatas() As ALinq.Table(Of tblGCLdata)
            Get
                Return Me.GetTable(Of tblGCLdata)()
            End Get
        End Property

        Public ReadOnly Property tblGCLObjects() As ALinq.Table(Of tblGCLObject)
            Get
                Return Me.GetTable(Of tblGCLObject)()
            End Get
        End Property

        Public ReadOnly Property tblPRSdatas() As ALinq.Table(Of tblPRSdata)
            Get
                Return Me.GetTable(Of tblPRSdata)()
            End Get
        End Property

        Public ReadOnly Property tblPRSObjects() As ALinq.Table(Of tblPRSObject)
            Get
                Return Me.GetTable(Of tblPRSObject)()
            End Get
        End Property

        Public ReadOnly Property tlbIdentificationCodes() As ALinq.Table(Of tlbIdentificationCode)
            Get
                Return Me.GetTable(Of tlbIdentificationCode)()
            End Get
        End Property
    End Class
End Namespace