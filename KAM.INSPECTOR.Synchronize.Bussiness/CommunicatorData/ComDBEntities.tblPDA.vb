Imports ALinq
Imports ALinq.Mapping
Namespace Model.CommunicatorData
    <Table(Name:="tblPDA")> _
    Partial Public Class tblPDA
        Private _Id As System.Int32
        Private _Pdasnr As System.String
        Private _INSPECTORVersion As System.String
        Private _INSPECTORUpdate As System.Boolean
        Private _INSPECTORUpdateDate As Nullable(Of System.DateTime)
        Private _INSPECTORLastSyncDate As Nullable(Of System.DateTime)
        Private _DirDataPRSLoad As System.String
        Private _DirDataInspProc As System.String
        Private _DirDataPLEXOR As System.String
        Private _DirDataFPR As System.String
        Private _DirDataPRSSave As System.String
        Private _DirDataResult As System.String
        Private _DirDataTemp As System.String
        Private _DirDataArchive As System.String
        Private _StorageCardINSPECTOR As System.String
        Private _SystemIntegration As System.String
        Partial Private Sub OnLoaded()
        End Sub
        Partial Private Sub OnValidate(action As ChangeAction)
        End Sub
        Partial Private Sub OnCreated()
        End Sub

        Public Sub New()
        End Sub

        <Column(Storage:="_Id", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, IsPrimaryKey:=True, Name:="ID")> _
        Public Property Id() As System.Int32
            Get
                Return _Id
            End Get
            Set(value As System.Int32)
                _Id = value
            End Set
        End Property

        <Column(Storage:="_Pdasnr", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="PDASNR")> _
        Public Property Pdasnr() As System.String
            Get
                Return _Pdasnr
            End Get
            Set(value As System.String)
                _Pdasnr = value
            End Set
        End Property

        <Column(Storage:="_INSPECTORVersion", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="INSPECTORVersion")> _
        Public Property INSPECTORVersion() As System.String
            Get
                Return _INSPECTORVersion
            End Get
            Set(value As System.String)
                _INSPECTORVersion = value
            End Set
        End Property

        <Column(Storage:="_INSPECTORUpdate", DbType:="Bit", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, Name:="INSPECTORUpdate")> _
        Public Property INSPECTORUpdate() As System.Boolean
            Get
                Return _INSPECTORUpdate
            End Get
            Set(value As System.Boolean)
                _INSPECTORUpdate = value
            End Set
        End Property

        <Column(Storage:="_INSPECTORUpdateDate", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="INSPECTORUpdateDate")> _
        Public Property INSPECTORUpdateDate() As Nullable(Of System.DateTime)
            Get
                Return _INSPECTORUpdateDate
            End Get
            Set(value As Nullable(Of System.DateTime))
                _INSPECTORUpdateDate = value
            End Set
        End Property

        <Column(Storage:="_INSPECTORLastSyncDate", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="INSPECTORLastSyncDate")> _
        Public Property INSPECTORLastSyncDate() As Nullable(Of System.DateTime)
            Get
                Return _INSPECTORLastSyncDate
            End Get
            Set(value As Nullable(Of System.DateTime))
                _INSPECTORLastSyncDate = value
            End Set
        End Property

        <Column(Storage:="_DirDataPRSLoad", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataPRSLoad")> _
        Public Property DirDataPRSLoad() As System.String
            Get
                Return _DirDataPRSLoad
            End Get
            Set(value As System.String)
                _DirDataPRSLoad = value
            End Set
        End Property

        <Column(Storage:="_DirDataInspProc", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataInspProc")> _
        Public Property DirDataInspProc() As System.String
            Get
                Return _DirDataInspProc
            End Get
            Set(value As System.String)
                _DirDataInspProc = value
            End Set
        End Property

        <Column(Storage:="_DirDataPLEXOR", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataPLEXOR")> _
        Public Property DirDataPLEXOR() As System.String
            Get
                Return _DirDataPLEXOR
            End Get
            Set(value As System.String)
                _DirDataPLEXOR = value
            End Set
        End Property

        <Column(Storage:="_DirDataFPR", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataFPR")> _
        Public Property DirDataFPR() As System.String
            Get
                Return _DirDataFPR
            End Get
            Set(value As System.String)
                _DirDataFPR = value
            End Set
        End Property

        <Column(Storage:="_DirDataPRSSave", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataPRSSave")> _
        Public Property DirDataPRSSave() As System.String
            Get
                Return _DirDataPRSSave
            End Get
            Set(value As System.String)
                _DirDataPRSSave = value
            End Set
        End Property

        <Column(Storage:="_DirDataResult", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataResult")> _
        Public Property DirDataResult() As System.String
            Get
                Return _DirDataResult
            End Get
            Set(value As System.String)
                _DirDataResult = value
            End Set
        End Property

        <Column(Storage:="_DirDataTemp", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataTemp")> _
        Public Property DirDataTemp() As System.String
            Get
                Return _DirDataTemp
            End Get
            Set(value As System.String)
                _DirDataTemp = value
            End Set
        End Property

        <Column(Storage:="_DirDataArchive", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DirDataArchive")> _
        Public Property DirDataArchive() As System.String
            Get
                Return _DirDataArchive
            End Get
            Set(value As System.String)
                _DirDataArchive = value
            End Set
        End Property

        <Column(Storage:="_StorageCardINSPECTOR", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="StorageCardINSPECTOR")> _
        Public Property StorageCardINSPECTOR() As System.String
            Get
                Return _StorageCardINSPECTOR
            End Get
            Set(value As System.String)
                _StorageCardINSPECTOR = value
            End Set
        End Property

        <Column(Storage:="_SystemIntegration", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="SystemIntegration")> _
        Public Property SystemIntegration() As System.String
            Get
                Return _SystemIntegration
            End Get
            Set(value As System.String)
                _SystemIntegration = value
            End Set
        End Property
    End Class

End Namespace