Imports ALinq
Imports ALinq.Mapping

<Table(Name:="tlbIdentificationCode")> _
Partial Public Class tlbIdentificationCode
    Private _PRSIdentificationNumber As Nullable(Of System.Int32)
    Private _PRSIdentificationSub As System.String
    Private _GCLIdentificationNumber As Nullable(Of System.Int32)
    Private _GLCIdentificationSub As System.String
    Private _IcId As Nullable(Of System.Int32)
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub

    Public Sub New()
    End Sub

    <Column(Storage:="_PRSIdentificationNumber", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, Name:="PRSIdentificationNumber")> _
    Public Property PRSIdentificationNumber() As Nullable(Of System.Int32)
        Get
            Return _PRSIdentificationNumber
        End Get
        Set(value As Nullable(Of System.Int32))
            _PRSIdentificationNumber = value
        End Set
    End Property

    <Column(Storage:="_PRSIdentificationSub", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="PRSIdentificationSub")> _
    Public Property PRSIdentificationSub() As System.String
        Get
            Return _PRSIdentificationSub
        End Get
        Set(value As System.String)
            _PRSIdentificationSub = value
        End Set
    End Property

    <Column(Storage:="_GCLIdentificationNumber", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, Name:="GCLIdentificationNumber")> _
    Public Property GCLIdentificationNumber() As Nullable(Of System.Int32)
        Get
            Return _GCLIdentificationNumber
        End Get
        Set(value As Nullable(Of System.Int32))
            _GCLIdentificationNumber = value
        End Set
    End Property

    <Column(Storage:="_GLCIdentificationSub", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="GLCIdentificationSub")> _
    Public Property GLCIdentificationSub() As System.String
        Get
            Return _GLCIdentificationSub
        End Get
        Set(value As System.String)
            _GLCIdentificationSub = value
        End Set
    End Property

    <Column(Storage:="_IcId", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, IsPrimaryKey:=True, Name:="IC_ID")> _
    Public Property IcId() As Nullable(Of System.Int32)
        Get
            Return _IcId
        End Get
        Set(value As Nullable(Of System.Int32))
            _IcId = value
        End Set
    End Property
End Class
