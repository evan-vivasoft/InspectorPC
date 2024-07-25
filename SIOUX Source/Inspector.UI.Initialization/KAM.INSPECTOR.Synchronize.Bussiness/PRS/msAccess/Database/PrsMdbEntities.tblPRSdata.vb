Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tblPRSdata")> _
Partial Public Class tblPRSdata
    Private _PRSCode As System.String
    Private _PRSName As System.String
    Private _PRSIdentification As System.String
    Private _Route As System.String
    Private _Information As System.String
    Private _InspectionProcedure As System.String
    Private _PrsId As Nullable(Of System.Int32)
    Private _tblGCLdatas As EntitySet(Of tblGCLdata)
    Private _tblPRSObjects As EntitySet(Of tblPRSObject)
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
        Me._tblGCLdatas = New EntitySet(Of tblGCLdata)()
        Me._tblPRSObjects = New EntitySet(Of tblPRSObject)()
    End Sub

    <Column(Storage:="_PRSCode", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="PRSCode")> _
    Public Property PRSCode() As System.String
        Get
            Return _PRSCode
        End Get
        Set(value As System.String)
            _PRSCode = value
        End Set
    End Property

    <Column(Storage:="_PRSName", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="PRSName")> _
    Public Property PRSName() As System.String
        Get
            Return _PRSName
        End Get
        Set(value As System.String)
            _PRSName = value
        End Set
    End Property

    <Column(Storage:="_PRSIdentification", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="PRSIdentification")> _
    Public Property PRSIdentification() As System.String
        Get
            Return _PRSIdentification
        End Get
        Set(value As System.String)
            _PRSIdentification = value
        End Set
    End Property

    <Column(Storage:="_Route", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Route")> _
    Public Property Route() As System.String
        Get
            Return _Route
        End Get
        Set(value As System.String)
            _Route = value
        End Set
    End Property

    <Column(Storage:="_Information", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="Information")> _
    Public Property Information() As System.String
        Get
            Return _Information
        End Get
        Set(value As System.String)
            _Information = value
        End Set
    End Property

    <Column(Storage:="_InspectionProcedure", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="InspectionProcedure")> _
    Public Property InspectionProcedure() As System.String
        Get
            Return _InspectionProcedure
        End Get
        Set(value As System.String)
            _InspectionProcedure = value
        End Set
    End Property

    <Column(Storage:="_PrsId", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, IsPrimaryKey:=True, Name:="PRS_ID")> _
    Public Property PrsId() As Nullable(Of System.Int32)
        Get
            Return _PrsId
        End Get
        Set(value As Nullable(Of System.Int32))
            _PrsId = value
        End Set
    End Property

    <Association(Storage:="_tblGCLdatas", ThisKey:="PrsId", OtherKey:="PRSLinkID", IsForeignKey:=False, Name:="tblPRSdata_tblGCLdata")> _
    Public Property tblGCLdatas() As EntitySet(Of tblGCLdata)
        Get
            Return Me._tblGCLdatas
        End Get
        Set(value As EntitySet(Of tblGCLdata))
            Me._tblGCLdatas.Assign(value)
        End Set
    End Property

    <Association(Storage:="_tblPRSObjects", ThisKey:="PrsId", OtherKey:="PRSLinkID", IsForeignKey:=False, Name:="tblPRSdata_tblPRSObject")> _
    Public Property tblPRSObjects() As EntitySet(Of tblPRSObject)
        Get
            Return Me._tblPRSObjects
        End Get
        Set(value As EntitySet(Of tblPRSObject))
            Me._tblPRSObjects.Assign(value)
        End Set
    End Property
End Class
