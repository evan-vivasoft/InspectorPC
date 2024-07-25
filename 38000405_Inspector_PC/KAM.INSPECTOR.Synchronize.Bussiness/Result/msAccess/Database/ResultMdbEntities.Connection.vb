Imports ALinq
Imports ALinq.Mapping
Imports System.ComponentModel
Imports System.Reflection
Imports System.IO

Namespace ResultMdbEntities.DataAccess
    <ALinq.Mapping.Provider(GetType(ALinq.Access.AccessDbProvider))> _
    Public Class AccessInspectionResults
        Inherits ALinq.DataContext

#Region "Extensibility Method Definitions"
        Partial Private Sub OnCreated()
        End Sub
        'Partial Private Sub InserttblDBVersion(ByVal instance As tblDBVersion)
        'End Sub
        'Partial Private Sub UpdatetblDBVersion(ByVal instance As tblDBVersion)
        'End Sub
        'Partial Private Sub DeletetblDBVersion(ByVal instance As tblDBVersion)
        'End Sub
        'Partial Private Sub InserttblGCLdata(ByVal instance As tblGCLdata)
        'End Sub
        'Partial Private Sub UpdatetblGCLdata(ByVal instance As tblGCLdata)
        'End Sub
        'Partial Private Sub DeletetblGCLdata(ByVal instance As tblGCLdata)
        'End Sub
        'Partial Private Sub InserttblGCLObject(ByVal instance As tblGCLObject)
        'End Sub
        'Partial Private Sub UpdatetblGCLObject(ByVal instance As tblGCLObject)
        'End Sub
        'Partial Private Sub DeletetblGCLObject(ByVal instance As tblGCLObject)
        'End Sub
        'Partial Private Sub InserttblPRSdata(ByVal instance As tblPRSdata)
        'End Sub
        'Partial Private Sub UpdatetblPRSdata(ByVal instance As tblPRSdata)
        'End Sub
        'Partial Private Sub DeletetblPRSdata(ByVal instance As tblPRSdata)
        'End Sub
        'Partial Private Sub InserttblPRSObject(ByVal instance As tblPRSObject)
        'End Sub
        'Partial Private Sub UpdatetblPRSObject(ByVal instance As tblPRSObject)
        'End Sub
        'Partial Private Sub DeletetblPRSObject(ByVal instance As tblPRSObject)
        'End Sub
        'Partial Private Sub InserttlbIdentificationCode(ByVal instance As tlbIdentificationCode)
        'End Sub
        'Partial Private Sub UpdatetlbIdentificationCode(ByVal instance As tlbIdentificationCode)
        'End Sub
        'Partial Private Sub DeletetlbIdentificationCode(ByVal instance As tlbIdentificationCode)
        'End Sub
#End Region

        Public Sub New(ByVal conn As String)
            MyBase.New(conn)
        End Sub


        Public ReadOnly Property tblResultGCL() As ALinq.Table(Of ResultsMDBEntities.DataAccess.tblResultGCL)
            Get
                Return Me.GetTable(Of ResultsMDBEntities.DataAccess.tblResultGCL)()
            End Get
        End Property

        Public ReadOnly Property tblResultPRS() As ALinq.Table(Of ResultsMDBEntities.DataAccess.tblResultPRS)
            Get
                Return Me.GetTable(Of ResultsMDBEntities.DataAccess.tblResultPRS)()
            End Get
        End Property

    End Class
End Namespace