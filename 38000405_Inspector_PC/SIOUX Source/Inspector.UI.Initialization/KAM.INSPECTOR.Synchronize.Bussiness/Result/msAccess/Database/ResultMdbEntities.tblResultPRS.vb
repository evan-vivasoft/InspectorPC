Imports ALinq
Imports ALinq.Mapping

Namespace ResultsMDBEntities.DataAccess

    <Table(Name:="tblResultPRS")> _
    Partial Public Class tblResultPRS
        Private _Id As System.Int32
        'Private _Crc As System.String
        Private _PRSCode As System.String
        Private _PRSName As System.String
        Private _InspectionProcedure As System.String
        Private _InspectionProcedureVersion As System.String
        Private _Dst As System.String
        Private _TimeZone As System.String
        Private _DateStart As Nullable(Of System.DateTime)
        Private _TimeStart As Nullable(Of System.DateTime)
        Private _DateEnd As Nullable(Of System.DateTime)
        Private _TimeEnd As Nullable(Of System.DateTime)
        Private _PRSIdentification As System.String
        Private _Result1 As System.String
        Private _Result2 As System.String
        Private _Result3 As System.String
        Private _Result4 As System.String
        Private _Result5 As System.String
        Private _Result6 As System.String
        Private _Result7 As System.String
        Private _Result8 As System.String
        Private _Result9 As System.String
        Private _Result10 As System.String
        Private _Result11 As System.String
        Private _Result12 As System.String
        Private _Result13 As System.String
        Private _Result14 As System.String
        Private _Result15 As System.String
        Private _Result16 As System.String
        Private _Result17 As System.String
        Private _Result18 As System.String
        Private _Result19 As System.String
        Private _Result20 As System.String
        Private _Result21 As System.String
        Private _Result22 As System.String
        Private _Result23 As System.String
        Private _Result24 As System.String
        Private _Result25 As System.String
        Private _Result26 As System.String
        Private _Result27 As System.String
        Private _Result28 As System.String
        Private _Result29 As System.String
        Private _Result30 As System.String
        Private _Result31 As System.String
        Private _Result32 As System.String
        Private _Result33 As System.String
        Private _Result34 As System.String
        Private _Result35 As System.String
        Private _Result36 As System.String
        Private _Result37 As System.String
        Private _Result38 As System.String
        Private _Result39 As System.String
        Private _Result40 As System.String
        Private _Result41 As System.String
        Private _Result42 As System.String
        Private _Result43 As System.String
        Private _Result44 As System.String
        Private _Result45 As System.String
        Private _Result46 As System.String
        Private _Result47 As System.String
        Private _Result48 As System.String
        Private _Result49 As System.String
        Private _Result50 As System.String
        Private _Result51 As System.String
        Private _Result52 As System.String
        Private _Result53 As System.String
        Private _Result54 As System.String
        Private _Result55 As System.String
        Private _Result56 As System.String
        Private _Result57 As System.String
        Private _Result58 As System.String
        Private _Result59 As System.String
        Private _Reserved1 As System.String
        Private _Reserved2 As System.String
        Private _Reserved3 As System.String
        Private _Reserved4 As System.String
        Private _Reserved5 As System.String
        Private _Reserved6 As System.String
        Private _Reserved7 As System.String
        Private _Reserved8 As System.String
        Private _Reserved9 As System.String
        Private _Reserved10 As System.String
        Partial Private Sub OnLoaded()
        End Sub
        Partial Private Sub OnValidate(action As ChangeAction)
        End Sub
        Partial Private Sub OnCreated()
        End Sub

        Public Sub New()
        End Sub

        <Column(Storage:="_Id", DbType:="Integer", UpdateCheck:=UpdateCheck.Never, CanBeNull:=False, IsPrimaryKey:=True, Name:="Id")> _
        Public Property Id() As System.Int32
            Get
                Return _Id
            End Get
            Set(value As System.Int32)
                _Id = value
            End Set
        End Property

        '<Column(Storage:="_Crc", DbType:="VarChar(50)", UpdateCheck:=UpdateCheck.Never, Name:="CRC")> _
        'Public Property Crc() As System.String
        '    Get
        '        Return _Crc
        '    End Get
        '    Set(value As System.String)
        '        _Crc = value
        '    End Set
        'End Property

        <Column(Storage:="_PRSCode", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="PRSCode")> _
        Public Property PRSCode() As System.String
            Get
                Return _PRSCode
            End Get
            Set(value As System.String)
                _PRSCode = value
            End Set
        End Property

        <Column(Storage:="_PRSName", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="PRSName")> _
        Public Property PRSName() As System.String
            Get
                Return _PRSName
            End Get
            Set(value As System.String)
                _PRSName = value
            End Set
        End Property

        <Column(Storage:="_InspectionProcedure", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="InspectionProcedure")> _
        Public Property InspectionProcedure() As System.String
            Get
                Return _InspectionProcedure
            End Get
            Set(value As System.String)
                _InspectionProcedure = value
            End Set
        End Property

        <Column(Storage:="_InspectionProcedureVersion", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="InspectionProcedureVersion")> _
        Public Property InspectionProcedureVersion() As System.String
            Get
                Return _InspectionProcedureVersion
            End Get
            Set(value As System.String)
                _InspectionProcedureVersion = value
            End Set
        End Property

        <Column(Storage:="_Dst", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="DST")> _
        Public Property Dst() As System.String
            Get
                Return _Dst
            End Get
            Set(value As System.String)
                _Dst = value
            End Set
        End Property

        <Column(Storage:="_TimeZone", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="TimeZone")> _
        Public Property TimeZone() As System.String
            Get
                Return _TimeZone
            End Get
            Set(value As System.String)
                _TimeZone = value
            End Set
        End Property

        <Column(Storage:="_DateStart", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="DateStart")> _
        Public Property DateStart() As Nullable(Of System.DateTime)
            Get
                Return _DateStart
            End Get
            Set(value As Nullable(Of System.DateTime))
                _DateStart = value
            End Set
        End Property

        <Column(Storage:="_TimeStart", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="TimeStart")> _
        Public Property TimeStart() As Nullable(Of System.DateTime)
            Get
                Return _TimeStart
            End Get
            Set(value As Nullable(Of System.DateTime))
                _TimeStart = value
            End Set
        End Property

        <Column(Storage:="_DateEnd", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="DateEnd")> _
        Public Property DateEnd() As Nullable(Of System.DateTime)
            Get
                Return _DateEnd
            End Get
            Set(value As Nullable(Of System.DateTime))
                _DateEnd = value
            End Set
        End Property

        <Column(Storage:="_TimeEnd", DbType:="DateTime", UpdateCheck:=UpdateCheck.Never, Name:="TimeEnd")> _
        Public Property TimeEnd() As Nullable(Of System.DateTime)
            Get
                Return _TimeEnd
            End Get
            Set(value As Nullable(Of System.DateTime))
                _TimeEnd = value
            End Set
        End Property

        <Column(Storage:="_PRSIdentification", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="PRSIdentification")> _
        Public Property PRSIdentification() As System.String
            Get
                Return _PRSIdentification
            End Get
            Set(value As System.String)
                _PRSIdentification = value
            End Set
        End Property

        <Column(Storage:="_Result1", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result1")> _
        Public Property Result1() As System.String
            Get
                Return _Result1
            End Get
            Set(value As System.String)
                _Result1 = value
            End Set
        End Property

        <Column(Storage:="_Result2", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result2")> _
        Public Property Result2() As System.String
            Get
                Return _Result2
            End Get
            Set(value As System.String)
                _Result2 = value
            End Set
        End Property

        <Column(Storage:="_Result3", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result3")> _
        Public Property Result3() As System.String
            Get
                Return _Result3
            End Get
            Set(value As System.String)
                _Result3 = value
            End Set
        End Property

        <Column(Storage:="_Result4", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result4")> _
        Public Property Result4() As System.String
            Get
                Return _Result4
            End Get
            Set(value As System.String)
                _Result4 = value
            End Set
        End Property

        <Column(Storage:="_Result5", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result5")> _
        Public Property Result5() As System.String
            Get
                Return _Result5
            End Get
            Set(value As System.String)
                _Result5 = value
            End Set
        End Property

        <Column(Storage:="_Result6", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result6")> _
        Public Property Result6() As System.String
            Get
                Return _Result6
            End Get
            Set(value As System.String)
                _Result6 = value
            End Set
        End Property

        <Column(Storage:="_Result7", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result7")> _
        Public Property Result7() As System.String
            Get
                Return _Result7
            End Get
            Set(value As System.String)
                _Result7 = value
            End Set
        End Property

        <Column(Storage:="_Result8", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result8")> _
        Public Property Result8() As System.String
            Get
                Return _Result8
            End Get
            Set(value As System.String)
                _Result8 = value
            End Set
        End Property

        <Column(Storage:="_Result9", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result9")> _
        Public Property Result9() As System.String
            Get
                Return _Result9
            End Get
            Set(value As System.String)
                _Result9 = value
            End Set
        End Property

        <Column(Storage:="_Result10", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result10")> _
        Public Property Result10() As System.String
            Get
                Return _Result10
            End Get
            Set(value As System.String)
                _Result10 = value
            End Set
        End Property

        <Column(Storage:="_Result11", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result11")> _
        Public Property Result11() As System.String
            Get
                Return _Result11
            End Get
            Set(value As System.String)
                _Result11 = value
            End Set
        End Property

        <Column(Storage:="_Result12", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result12")> _
        Public Property Result12() As System.String
            Get
                Return _Result12
            End Get
            Set(value As System.String)
                _Result12 = value
            End Set
        End Property

        <Column(Storage:="_Result13", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result13")> _
        Public Property Result13() As System.String
            Get
                Return _Result13
            End Get
            Set(value As System.String)
                _Result13 = value
            End Set
        End Property

        <Column(Storage:="_Result14", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result14")> _
        Public Property Result14() As System.String
            Get
                Return _Result14
            End Get
            Set(value As System.String)
                _Result14 = value
            End Set
        End Property

        <Column(Storage:="_Result15", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result15")> _
        Public Property Result15() As System.String
            Get
                Return _Result15
            End Get
            Set(value As System.String)
                _Result15 = value
            End Set
        End Property

        <Column(Storage:="_Result16", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result16")> _
        Public Property Result16() As System.String
            Get
                Return _Result16
            End Get
            Set(value As System.String)
                _Result16 = value
            End Set
        End Property

        <Column(Storage:="_Result17", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result17")> _
        Public Property Result17() As System.String
            Get
                Return _Result17
            End Get
            Set(value As System.String)
                _Result17 = value
            End Set
        End Property

        <Column(Storage:="_Result18", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result18")> _
        Public Property Result18() As System.String
            Get
                Return _Result18
            End Get
            Set(value As System.String)
                _Result18 = value
            End Set
        End Property

        <Column(Storage:="_Result19", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result19")> _
        Public Property Result19() As System.String
            Get
                Return _Result19
            End Get
            Set(value As System.String)
                _Result19 = value
            End Set
        End Property

        <Column(Storage:="_Result20", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result20")> _
        Public Property Result20() As System.String
            Get
                Return _Result20
            End Get
            Set(value As System.String)
                _Result20 = value
            End Set
        End Property

        <Column(Storage:="_Result21", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result21")> _
        Public Property Result21() As System.String
            Get
                Return _Result21
            End Get
            Set(value As System.String)
                _Result21 = value
            End Set
        End Property

        <Column(Storage:="_Result22", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result22")> _
        Public Property Result22() As System.String
            Get
                Return _Result22
            End Get
            Set(value As System.String)
                _Result22 = value
            End Set
        End Property

        <Column(Storage:="_Result23", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result23")> _
        Public Property Result23() As System.String
            Get
                Return _Result23
            End Get
            Set(value As System.String)
                _Result23 = value
            End Set
        End Property

        <Column(Storage:="_Result24", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result24")> _
        Public Property Result24() As System.String
            Get
                Return _Result24
            End Get
            Set(value As System.String)
                _Result24 = value
            End Set
        End Property

        <Column(Storage:="_Result25", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result25")> _
        Public Property Result25() As System.String
            Get
                Return _Result25
            End Get
            Set(value As System.String)
                _Result25 = value
            End Set
        End Property

        <Column(Storage:="_Result26", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result26")> _
        Public Property Result26() As System.String
            Get
                Return _Result26
            End Get
            Set(value As System.String)
                _Result26 = value
            End Set
        End Property

        <Column(Storage:="_Result27", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result27")> _
        Public Property Result27() As System.String
            Get
                Return _Result27
            End Get
            Set(value As System.String)
                _Result27 = value
            End Set
        End Property

        <Column(Storage:="_Result28", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result28")> _
        Public Property Result28() As System.String
            Get
                Return _Result28
            End Get
            Set(value As System.String)
                _Result28 = value
            End Set
        End Property

        <Column(Storage:="_Result29", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result29")> _
        Public Property Result29() As System.String
            Get
                Return _Result29
            End Get
            Set(value As System.String)
                _Result29 = value
            End Set
        End Property

        <Column(Storage:="_Result30", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result30")> _
        Public Property Result30() As System.String
            Get
                Return _Result30
            End Get
            Set(value As System.String)
                _Result30 = value
            End Set
        End Property

        <Column(Storage:="_Result31", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result31")> _
        Public Property Result31() As System.String
            Get
                Return _Result31
            End Get
            Set(value As System.String)
                _Result31 = value
            End Set
        End Property

        <Column(Storage:="_Result32", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result32")> _
        Public Property Result32() As System.String
            Get
                Return _Result32
            End Get
            Set(value As System.String)
                _Result32 = value
            End Set
        End Property

        <Column(Storage:="_Result33", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result33")> _
        Public Property Result33() As System.String
            Get
                Return _Result33
            End Get
            Set(value As System.String)
                _Result33 = value
            End Set
        End Property

        <Column(Storage:="_Result34", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result34")> _
        Public Property Result34() As System.String
            Get
                Return _Result34
            End Get
            Set(value As System.String)
                _Result34 = value
            End Set
        End Property

        <Column(Storage:="_Result35", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result35")> _
        Public Property Result35() As System.String
            Get
                Return _Result35
            End Get
            Set(value As System.String)
                _Result35 = value
            End Set
        End Property

        <Column(Storage:="_Result36", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result36")> _
        Public Property Result36() As System.String
            Get
                Return _Result36
            End Get
            Set(value As System.String)
                _Result36 = value
            End Set
        End Property

        <Column(Storage:="_Result37", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result37")> _
        Public Property Result37() As System.String
            Get
                Return _Result37
            End Get
            Set(value As System.String)
                _Result37 = value
            End Set
        End Property

        <Column(Storage:="_Result38", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result38")> _
        Public Property Result38() As System.String
            Get
                Return _Result38
            End Get
            Set(value As System.String)
                _Result38 = value
            End Set
        End Property

        <Column(Storage:="_Result39", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result39")> _
        Public Property Result39() As System.String
            Get
                Return _Result39
            End Get
            Set(value As System.String)
                _Result39 = value
            End Set
        End Property

        <Column(Storage:="_Result40", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result40")> _
        Public Property Result40() As System.String
            Get
                Return _Result40
            End Get
            Set(value As System.String)
                _Result40 = value
            End Set
        End Property

        <Column(Storage:="_Result41", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result41")> _
        Public Property Result41() As System.String
            Get
                Return _Result41
            End Get
            Set(value As System.String)
                _Result41 = value
            End Set
        End Property

        <Column(Storage:="_Result42", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result42")> _
        Public Property Result42() As System.String
            Get
                Return _Result42
            End Get
            Set(value As System.String)
                _Result42 = value
            End Set
        End Property

        <Column(Storage:="_Result43", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result43")> _
        Public Property Result43() As System.String
            Get
                Return _Result43
            End Get
            Set(value As System.String)
                _Result43 = value
            End Set
        End Property

        <Column(Storage:="_Result44", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result44")> _
        Public Property Result44() As System.String
            Get
                Return _Result44
            End Get
            Set(value As System.String)
                _Result44 = value
            End Set
        End Property

        <Column(Storage:="_Result45", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result45")> _
        Public Property Result45() As System.String
            Get
                Return _Result45
            End Get
            Set(value As System.String)
                _Result45 = value
            End Set
        End Property

        <Column(Storage:="_Result46", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result46")> _
        Public Property Result46() As System.String
            Get
                Return _Result46
            End Get
            Set(value As System.String)
                _Result46 = value
            End Set
        End Property

        <Column(Storage:="_Result47", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result47")> _
        Public Property Result47() As System.String
            Get
                Return _Result47
            End Get
            Set(value As System.String)
                _Result47 = value
            End Set
        End Property

        <Column(Storage:="_Result48", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result48")> _
        Public Property Result48() As System.String
            Get
                Return _Result48
            End Get
            Set(value As System.String)
                _Result48 = value
            End Set
        End Property

        <Column(Storage:="_Result49", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result49")> _
        Public Property Result49() As System.String
            Get
                Return _Result49
            End Get
            Set(value As System.String)
                _Result49 = value
            End Set
        End Property

        <Column(Storage:="_Result50", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result50")> _
        Public Property Result50() As System.String
            Get
                Return _Result50
            End Get
            Set(value As System.String)
                _Result50 = value
            End Set
        End Property

        <Column(Storage:="_Result51", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result51")> _
        Public Property Result51() As System.String
            Get
                Return _Result51
            End Get
            Set(value As System.String)
                _Result51 = value
            End Set
        End Property

        <Column(Storage:="_Result52", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result52")> _
        Public Property Result52() As System.String
            Get
                Return _Result52
            End Get
            Set(value As System.String)
                _Result52 = value
            End Set
        End Property

        <Column(Storage:="_Result53", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result53")> _
        Public Property Result53() As System.String
            Get
                Return _Result53
            End Get
            Set(value As System.String)
                _Result53 = value
            End Set
        End Property

        <Column(Storage:="_Result54", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result54")> _
        Public Property Result54() As System.String
            Get
                Return _Result54
            End Get
            Set(value As System.String)
                _Result54 = value
            End Set
        End Property

        <Column(Storage:="_Result55", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result55")> _
        Public Property Result55() As System.String
            Get
                Return _Result55
            End Get
            Set(value As System.String)
                _Result55 = value
            End Set
        End Property

        <Column(Storage:="_Result56", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result56")> _
        Public Property Result56() As System.String
            Get
                Return _Result56
            End Get
            Set(value As System.String)
                _Result56 = value
            End Set
        End Property

        <Column(Storage:="_Result57", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result57")> _
        Public Property Result57() As System.String
            Get
                Return _Result57
            End Get
            Set(value As System.String)
                _Result57 = value
            End Set
        End Property

        <Column(Storage:="_Result58", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result58")> _
        Public Property Result58() As System.String
            Get
                Return _Result58
            End Get
            Set(value As System.String)
                _Result58 = value
            End Set
        End Property

        <Column(Storage:="_Result59", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Result59")> _
        Public Property Result59() As System.String
            Get
                Return _Result59
            End Get
            Set(value As System.String)
                _Result59 = value
            End Set
        End Property

        <Column(Storage:="_Reserved1", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved1")> _
        Public Property Reserved1() As System.String
            Get
                Return _Reserved1
            End Get
            Set(value As System.String)
                _Reserved1 = value
            End Set
        End Property

        <Column(Storage:="_Reserved2", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved2")> _
        Public Property Reserved2() As System.String
            Get
                Return _Reserved2
            End Get
            Set(value As System.String)
                _Reserved2 = value
            End Set
        End Property

        <Column(Storage:="_Reserved3", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved3")> _
        Public Property Reserved3() As System.String
            Get
                Return _Reserved3
            End Get
            Set(value As System.String)
                _Reserved3 = value
            End Set
        End Property

        <Column(Storage:="_Reserved4", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved4")> _
        Public Property Reserved4() As System.String
            Get
                Return _Reserved4
            End Get
            Set(value As System.String)
                _Reserved4 = value
            End Set
        End Property

        <Column(Storage:="_Reserved5", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved5")> _
        Public Property Reserved5() As System.String
            Get
                Return _Reserved5
            End Get
            Set(value As System.String)
                _Reserved5 = value
            End Set
        End Property

        <Column(Storage:="_Reserved6", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved6")> _
        Public Property Reserved6() As System.String
            Get
                Return _Reserved6
            End Get
            Set(value As System.String)
                _Reserved6 = value
            End Set
        End Property

        <Column(Storage:="_Reserved7", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved7")> _
        Public Property Reserved7() As System.String
            Get
                Return _Reserved7
            End Get
            Set(value As System.String)
                _Reserved7 = value
            End Set
        End Property

        <Column(Storage:="_Reserved8", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved8")> _
        Public Property Reserved8() As System.String
            Get
                Return _Reserved8
            End Get
            Set(value As System.String)
                _Reserved8 = value
            End Set
        End Property

        <Column(Storage:="_Reserved9", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved9")> _
        Public Property Reserved9() As System.String
            Get
                Return _Reserved9
            End Get
            Set(value As System.String)
                _Reserved9 = value
            End Set
        End Property

        <Column(Storage:="_Reserved10", DbType:="VarChar(255)", UpdateCheck:=UpdateCheck.Never, Name:="Reserved10")> _
        Public Property Reserved10() As System.String
            Get
                Return _Reserved10
            End Get
            Set(value As System.String)
                _Reserved10 = value
            End Set
        End Property
    End Class

End Namespace
