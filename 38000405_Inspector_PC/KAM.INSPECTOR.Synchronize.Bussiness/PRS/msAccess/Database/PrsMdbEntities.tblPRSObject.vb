Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tblPRSObjects")> _
Partial Public Class tblPRSObject
    Private _PRSLinkID As Nullable(Of System.Int32)
    Private _ObjectName As System.String
    Private _ObjectID As System.String
    Private _MeasurePoint As System.String
    Private _MeasurePointID As System.String
    Private _FieldNo As System.Decimal
    Private _PrsobjectId As Nullable(Of System.Int32)
    Private _tblPRSdata As EntityRef(Of tblPRSdata)
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
        Me._tblPRSdata = Nothing
    End Sub

    <Column(Storage:="_PRSLinkID", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, Name:="PRSLinkID")> _
    Public Property PRSLinkID() As Nullable(Of System.Int32)
        Get
            Return _PRSLinkID
        End Get
        Set(value As Nullable(Of System.Int32))
            _PRSLinkID = value
        End Set
    End Property

    <Column(Storage:="_ObjectName", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="ObjectName")> _
    Public Property ObjectName() As System.String
        Get
            Return _ObjectName
        End Get
        Set(value As System.String)
            _ObjectName = value
        End Set
    End Property

    <Column(Storage:="_ObjectID", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="ObjectID")> _
    Public Property ObjectID() As System.String
        Get
            Return _ObjectID
        End Get
        Set(value As System.String)
            _ObjectID = value
        End Set
    End Property

    <Column(Storage:="_MeasurePoint", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="MeasurePoint")> _
    Public Property MeasurePoint() As System.String
        Get
            Return _MeasurePoint
        End Get
        Set(value As System.String)
            _MeasurePoint = value
        End Set
    End Property

    <Column(Storage:="_MeasurePointID", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="MeasurePointID")> _
    Public Property MeasurePointID() As System.String
        Get
            Return _MeasurePointID
        End Get
        Set(value As System.String)
            _MeasurePointID = value
        End Set
    End Property

    <Column(Storage:="_FieldNo", DbType:="Numeric", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="FieldNo")> _
    Public Property FieldNo() As System.Decimal
        Get
            Return _FieldNo
        End Get
        Set(value As System.Decimal)
            _FieldNo = value
        End Set
    End Property

    <Column(Storage:="_PrsobjectId", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, IsPrimaryKey:=True, Name:="PRSObject_ID")> _
    Public Property PrsobjectId() As Nullable(Of System.Int32)
        Get
            Return _PrsobjectId
        End Get
        Set(value As Nullable(Of System.Int32))
            _PrsobjectId = value
        End Set
    End Property

    <Association(Storage:="_tblPRSdata", ThisKey:="PRSLinkID", OtherKey:="PrsId", IsForeignKey:=True, Name:="tblPRSdata_tblPRSObject")> _
    Public Property tblPRSdata() As tblPRSdata
        Get
            Return Me._tblPRSdata.Entity
        End Get
        Set(value As tblPRSdata)
            Me._tblPRSdata.Entity = value
        End Set
    End Property
End Class
