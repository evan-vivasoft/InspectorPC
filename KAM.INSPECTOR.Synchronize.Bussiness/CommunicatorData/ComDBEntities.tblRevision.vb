Imports ALinq
Imports ALinq.Mapping
Namespace Model.CommunicatorData
    <Table(Name:="tblRevision")> _
    Partial Public Class tblRevision
        Private _RevStatus As System.String
        Private _Date As Nullable(Of System.DateTime)
        Private _Comments As System.String
        Private _DatabaseNumber As System.String
        Partial Private Sub OnLoaded()
        End Sub
        Partial Private Sub OnValidate(action As ChangeAction)
        End Sub
        Partial Private Sub OnCreated()
        End Sub

        Public Sub New()
        End Sub

        <Column(Storage:="_RevStatus", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="[Rev status]")> _
        Public Property RevStatus() As System.String
            Get
                Return _RevStatus
            End Get
            Set(value As System.String)
                _RevStatus = value
            End Set
        End Property

        <Column(Storage:="_Date", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="[Date]")> _
        Public Property [Date]() As Nullable(Of System.DateTime)
            Get
                Return _Date
            End Get
            Set(value As Nullable(Of System.DateTime))
                _Date = value
            End Set
        End Property

        <Column(Storage:="_Comments", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="Comments")> _
        Public Property Comments() As System.String
            Get
                Return _Comments
            End Get
            Set(value As System.String)
                _Comments = value
            End Set
        End Property

        <Column(Storage:="_DatabaseNumber", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="[Database number]")> _
        Public Property DatabaseNumber() As System.String
            Get
                Return _DatabaseNumber
            End Get
            Set(value As System.String)
                _DatabaseNumber = value
            End Set
        End Property
    End Class


End Namespace