Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tblGCLdata")> _
Partial Public Class tblGCLdata
    Private _PRSLinkID As Nullable(Of System.Int32)
    Private _GCLName As System.String
    Private _PeMin As System.String
    Private _PeMax As System.String
    Private _VolumeVA As System.String
    Private _VolumeVAK As System.String
    Private _PdRangeDM As System.String
    Private _PuRangeDM As System.String
    Private _GCLIdentification As System.String
    Private _InspectionProcedure As System.String
    Private _FSDStartPosition As Nullable(Of System.Int16)
    Private _GclId As System.Int32
    Private _tblGCLObjects As EntitySet(Of tblGCLObject)
    Private _tblPRSdata As EntityRef(Of tblPRSdata)
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
        Me._tblGCLObjects = New EntitySet(Of tblGCLObject)()
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

    <Column(Storage:="_GCLName", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="GCLName")> _
    Public Property GCLName() As System.String
        Get
            Return _GCLName
        End Get
        Set(value As System.String)
            _GCLName = value
        End Set
    End Property

    <Column(Storage:="_PeMin", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="PeMin")> _
    Public Property PeMin() As System.String
        Get
            Return _PeMin
        End Get
        Set(value As System.String)
            _PeMin = value
        End Set
    End Property

    <Column(Storage:="_PeMax", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="PeMax")> _
    Public Property PeMax() As System.String
        Get
            Return _PeMax
        End Get
        Set(value As System.String)
            _PeMax = value
        End Set
    End Property

    <Column(Storage:="_VolumeVA", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="VolumeVA")> _
    Public Property VolumeVA() As System.String
        Get
            Return _VolumeVA
        End Get
        Set(value As System.String)
            _VolumeVA = value
        End Set
    End Property

    <Column(Storage:="_VolumeVAK", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="VolumeVAK")> _
    Public Property VolumeVAK() As System.String
        Get
            Return _VolumeVAK
        End Get
        Set(value As System.String)
            _VolumeVAK = value
        End Set
    End Property

    <Column(Storage:="_PdRangeDM", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="PdRangeDM")> _
    Public Property PdRangeDM() As System.String
        Get
            Return _PdRangeDM
        End Get
        Set(value As System.String)
            _PdRangeDM = value
        End Set
    End Property

    <Column(Storage:="_PuRangeDM", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="PuRangeDM")> _
    Public Property PuRangeDM() As System.String
        Get
            Return _PuRangeDM
        End Get
        Set(value As System.String)
            _PuRangeDM = value
        End Set
    End Property

    <Column(Storage:="_GCLIdentification", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="GCLIdentification")> _
    Public Property GCLIdentification() As System.String
        Get
            Return _GCLIdentification
        End Get
        Set(value As System.String)
            _GCLIdentification = value
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

    <Column(Storage:="_FSDStartPosition", DbType:="SmallInt", UpdateCheck:=UpdateCheck.Never, Name:="FSDStartPosition")> _
    Public Property FSDStartPosition() As Nullable(Of System.Int16)
        Get
            Return _FSDStartPosition
        End Get
        Set(value As Nullable(Of System.Int16))
            _FSDStartPosition = value
        End Set
    End Property

    <Column(Storage:="_GclId", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, IsPrimaryKey:=True, Name:="GCL_ID")> _
    Public Property GclId() As System.Int32
        Get
            Return _GclId
        End Get
        Set(value As System.Int32)
            _GclId = value
        End Set
    End Property

    <Association(Storage:="_tblGCLObjects", ThisKey:="GclId", OtherKey:="GCLLinkID", IsForeignKey:=False, Name:="tblGCLdata_tblGCLObject")> _
    Public Property tblGCLObjects() As EntitySet(Of tblGCLObject)
        Get
            Return Me._tblGCLObjects
        End Get
        Set(value As EntitySet(Of tblGCLObject))
            Me._tblGCLObjects.Assign(value)
        End Set
    End Property

    <Association(Storage:="_tblPRSdata", ThisKey:="PRSLinkID", OtherKey:="PrsId", IsForeignKey:=True, Name:="tblPRSdata_tblGCLdata")> _
    Public Property tblPRSdata() As tblPRSdata
        Get
            Return Me._tblPRSdata.Entity
        End Get
        Set(value As tblPRSdata)
            Me._tblPRSdata.Entity = value
        End Set
    End Property
End Class
