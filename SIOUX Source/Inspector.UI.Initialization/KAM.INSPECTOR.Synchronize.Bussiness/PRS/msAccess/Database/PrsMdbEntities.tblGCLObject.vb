Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tblGCLObjects")> _
Partial Public Class tblGCLObject
    Private _ObjectName As System.String
    Private _ObjectID As System.String
    Private _MeasurePoint As System.String
    Private _MeasurePointID As System.String
    Private _FieldNo As System.Decimal
    Private _Value As System.String
    Private _Percentage As System.String
    Private _ValueMin As System.String
    Private _ValueMax As System.String
    Private _Uov As System.String
    Private _GCLLinkID As System.Int32
    Private _GclobjectId As Nullable(Of System.Int32)
    Private _tblGCLdata As EntityRef(Of tblGCLdata)
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
        Me._tblGCLdata = Nothing
    End Sub

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

    <Column(Storage:="_Value", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="[Value]")> _
    Public Property Value() As System.String
        Get
            Return _Value
        End Get
        Set(value As System.String)
            _Value = value
        End Set
    End Property

    <Column(Storage:="_Percentage", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Percentage")> _
    Public Property Percentage() As System.String
        Get
            Return _Percentage
        End Get
        Set(value As System.String)
            _Percentage = value
        End Set
    End Property

    <Column(Storage:="_ValueMin", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="ValueMin")> _
    Public Property ValueMin() As System.String
        Get
            Return _ValueMin
        End Get
        Set(value As System.String)
            _ValueMin = value
        End Set
    End Property

    <Column(Storage:="_ValueMax", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="ValueMax")> _
    Public Property ValueMax() As System.String
        Get
            Return _ValueMax
        End Get
        Set(value As System.String)
            _ValueMax = value
        End Set
    End Property

    <Column(Storage:="_Uov", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="UOV")> _
    Public Property Uov() As System.String
        Get
            Return _Uov
        End Get
        Set(value As System.String)
            _Uov = value
        End Set
    End Property

    <Column(Storage:="_GCLLinkID", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="GCLLinkID")> _
    Public Property GCLLinkID() As System.Int32
        Get
            Return _GCLLinkID
        End Get
        Set(value As System.Int32)
            _GCLLinkID = value
        End Set
    End Property

    <Column(Storage:="_GclobjectId", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, IsPrimaryKey:=True, Name:="GCLObject_ID")> _
    Public Property GclobjectId() As Nullable(Of System.Int32)
        Get
            Return _GclobjectId
        End Get
        Set(value As Nullable(Of System.Int32))
            _GclobjectId = value
        End Set
    End Property

    <Association(Storage:="_tblGCLdata", ThisKey:="GCLLinkID", OtherKey:="GclId", IsForeignKey:=True, Name:="tblGCLdata_tblGCLObject")> _
    Public Property tblGCLdata() As tblGCLdata
        Get
            Return Me._tblGCLdata.Entity
        End Get
        Set(value As tblGCLdata)
            Me._tblGCLdata.Entity = value
        End Set
    End Property
End Class
