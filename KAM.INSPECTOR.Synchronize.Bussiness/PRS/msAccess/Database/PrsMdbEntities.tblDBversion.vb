Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tblDBVersion")> _
Partial Public Class tblDBVersion
    Private _VersionMajor As System.String
    Private _VersionMinor As System.String
    Private _RevisionDate As Nullable(Of System.DateTime)
    Private _DbId As Integer
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
    End Sub

    <Column(Storage:="_VersionMajor", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="VersionMajor")> _
    Public Property VersionMajor() As System.String
        Get
            Return _VersionMajor
        End Get
        Set(value As System.String)
            _VersionMajor = value
        End Set
    End Property

    <Column(Storage:="_VersionMinor", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="VersionMinor")> _
    Public Property VersionMinor() As System.String
        Get
            Return _VersionMinor
        End Get
        Set(value As System.String)
            _VersionMinor = value
        End Set
    End Property

    <Column(Storage:="_RevisionDate", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="RevisionDate")> _
    Public Property RevisionDate() As Nullable(Of System.DateTime)
        Get
            Return _RevisionDate
        End Get
        Set(value As Nullable(Of System.DateTime))
            _RevisionDate = value
        End Set
    End Property

    <Column(Name:="db_ID", Storage:="_DbId", DbType:="Integer", CanBeNull:=False, IsPrimaryKey:=True, UpdateCheck:=UpdateCheck.Never)> _
Public Property DbId() As Integer
        Get
            Return Me._DbId
        End Get
        Set(value As Integer)
            If ((Me._DbId = value) _
               = False) Then
                Me._DbId = value
            End If
        End Set
    End Property
End Class
