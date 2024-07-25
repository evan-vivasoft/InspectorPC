'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Globalization
Namespace Model.Result

    ''' <summary>
    ''' Helpers for exporting the data from the MsAccess database.
    ''' </summary>
    ''' <remarks></remarks>
    Module clsMsAccessHelpers

#Region "Members"
        'List with Units; assign value instead of text
        'MOD 23
        Private lsUnitList As List(Of String) = GetPlexorUnits()   'MOD 34 From {"mbar", "bar", "dm3/h", "mbar/min"}
#End Region

#Region "Public"
        ''' <summary>
        ''' General structure for the Results
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure structResult
            Dim Hit As Boolean
            Dim Text As String
            Dim Value As Double
            Dim UOM As String
        End Structure
#End Region

#Region "Public functions"
        ''' <summary>
        ''' Helper; Retrieve a value from a record depending on fieldnummer "lifield" and, if a value, split up in Text or value. 
        ''' It will NOT handle the List item
        ''' </summary>
        ''' <param name="lsResult"></param>
        ''' <returns>structResult; as value</returns>
        ''' <remarks></remarks>
        Public Function GetRecordValueResult(ByVal lsResult As String) As structResult
            GetRecordValueResult.Hit = False
            GetRecordValueResult.Text = ""
            GetRecordValueResult.UOM = ""
            GetRecordValueResult.Value = Double.NaN

            Dim lbUnitHit As Boolean = False

            'Check the value 
            If lsResult <> "" And lsResult.ToLower <> "n.m." Then
                'A value is found
                GetRecordValueResult.Hit = True
                For i As Integer = 0 To lsUnitList.Count - 1
                    'Check if the text ends with a unit
                    If lsResult.ToLower.EndsWith(lsUnitList(i).ToLower) = True Then
                        Dim val As Double
                        Dim tmpValue As String = ""
                        tmpValue = lsResult.Substring(0, lsResult.Length - lsUnitList(i).Length)

                        Dim culture As New CultureInfo(CultureInfo.CurrentCulture.Name)
                        'MOD 31
                        If culture.NumberFormat.NumberDecimalSeparator = "." Then
                            tmpValue = tmpValue.Replace(",", culture.NumberFormat.CurrencyDecimalSeparator)
                        Else
                            tmpValue = tmpValue.Replace(".", culture.NumberFormat.CurrencyDecimalSeparator)
                        End If
                        'check if the remaining is a valid value
                        If Double.TryParse(tmpValue, val) = True Then
                            GetRecordValueResult.UOM = lsUnitList(i)
                            'Is set as value; Decimal seperator is handled correct
                            GetRecordValueResult.Value = tmpValue
                            lbUnitHit = True
                            Exit For
                        Else
                            'If not a value; it is a text
                            Exit For
                        End If
                    End If
                Next
                'If not a value; it is a text
                If lbUnitHit = False Then GetRecordValueResult.Text = lsResult

                lbUnitHit = False
            End If
        End Function


        ''' <summary>
        ''' Get value from record depeding on FieldNo. If a value then split up in Text or value.
        ''' A list is never generated.
        ''' </summary>
        ''' <param name="gclMsaRecord"></param>
        ''' <param name="lsResultFields"></param>
        ''' <remarks></remarks>
        Public Sub SetRecordsValueResultGcl(ByVal gclMsaRecord As ResultsMDBEntities.DataAccess.tblResultGCL, ByRef lsResultFields() As String)

            ReDim lsResultFields(60)

            Dim lsResult As String = ""
            Try
                lsResultFields(1) = gclMsaRecord.Result1
                lsResultFields(2) = gclMsaRecord.Result2
                lsResultFields(3) = gclMsaRecord.Result3
                lsResultFields(4) = gclMsaRecord.Result4
                lsResultFields(5) = gclMsaRecord.Result5
                lsResultFields(6) = gclMsaRecord.Result6
                lsResultFields(7) = gclMsaRecord.Result7
                lsResultFields(8) = gclMsaRecord.Result8
                lsResultFields(9) = gclMsaRecord.Result9
                lsResultFields(10) = gclMsaRecord.Result10
                lsResultFields(11) = gclMsaRecord.Result11
                lsResultFields(12) = gclMsaRecord.Result12
                lsResultFields(13) = gclMsaRecord.Result13
                lsResultFields(14) = gclMsaRecord.Result14
                lsResultFields(15) = gclMsaRecord.Result15
                lsResultFields(16) = gclMsaRecord.Result16
                lsResultFields(17) = gclMsaRecord.Result17
                lsResultFields(18) = gclMsaRecord.Result18
                lsResultFields(19) = gclMsaRecord.Result19
                lsResultFields(20) = gclMsaRecord.Result20
                lsResultFields(21) = gclMsaRecord.Result21
                lsResultFields(22) = gclMsaRecord.Result22
                lsResultFields(23) = gclMsaRecord.Result23
                lsResultFields(24) = gclMsaRecord.Result24
                lsResultFields(25) = gclMsaRecord.Result25
                lsResultFields(26) = gclMsaRecord.Result26
                lsResultFields(27) = gclMsaRecord.Result27
                lsResultFields(28) = gclMsaRecord.Result28
                lsResultFields(29) = gclMsaRecord.Result29
                lsResultFields(30) = gclMsaRecord.Result30
                lsResultFields(31) = gclMsaRecord.Result31
                lsResultFields(32) = gclMsaRecord.Result32
                lsResultFields(33) = gclMsaRecord.Result33
                lsResultFields(34) = gclMsaRecord.Result34
                lsResultFields(35) = gclMsaRecord.Result35
                lsResultFields(36) = gclMsaRecord.Result36
                lsResultFields(37) = gclMsaRecord.Result37
                lsResultFields(38) = gclMsaRecord.Result38
                lsResultFields(39) = gclMsaRecord.Result39
                lsResultFields(40) = gclMsaRecord.Result40
                lsResultFields(41) = gclMsaRecord.Result41
                lsResultFields(42) = gclMsaRecord.Result42
                lsResultFields(43) = gclMsaRecord.Result43
                lsResultFields(44) = gclMsaRecord.Result44
                lsResultFields(45) = gclMsaRecord.Result45
                lsResultFields(46) = gclMsaRecord.Result46
                lsResultFields(47) = gclMsaRecord.Result47
                lsResultFields(48) = gclMsaRecord.Result48
                lsResultFields(49) = gclMsaRecord.Result49
                lsResultFields(50) = gclMsaRecord.Result50
                lsResultFields(51) = gclMsaRecord.Result51
                lsResultFields(52) = gclMsaRecord.Result52
                lsResultFields(53) = gclMsaRecord.Result53
                lsResultFields(54) = gclMsaRecord.Result54
                lsResultFields(55) = gclMsaRecord.Result55
                lsResultFields(56) = gclMsaRecord.Result56
                lsResultFields(57) = gclMsaRecord.Result57
                lsResultFields(58) = gclMsaRecord.Result58
                lsResultFields(59) = gclMsaRecord.Result59
                ' lsResultFields(60)=gclMsaRecord.Result60
            Catch ex As Exception
            End Try
        End Sub
        ''' <summary>
        ''' Get value from record depeding on FieldNo. If a value then split up in Text or value.
        ''' A list is never generated.
        ''' </summary>
        ''' <param name="prsMsaRecord"></param>
        ''' <param name="lsResultFields"></param>
        ''' <remarks></remarks>
        Public Sub SetRecordsValueResultPrs(ByVal prsMsaRecord As ResultsMDBEntities.DataAccess.tblResultPRS, ByRef lsResultFields() As String)

            ReDim lsResultFields(60)
            Dim lsResult As String = ""
            Try
                lsResultFields(1) = prsMsaRecord.Result1
                lsResultFields(2) = prsMsaRecord.Result2
                lsResultFields(3) = prsMsaRecord.Result3
                lsResultFields(4) = prsMsaRecord.Result4
                lsResultFields(5) = prsMsaRecord.Result5
                lsResultFields(6) = prsMsaRecord.Result6
                lsResultFields(7) = prsMsaRecord.Result7
                lsResultFields(8) = prsMsaRecord.Result8
                lsResultFields(9) = prsMsaRecord.Result9
                lsResultFields(10) = prsMsaRecord.Result10
                lsResultFields(11) = prsMsaRecord.Result11
                lsResultFields(12) = prsMsaRecord.Result12
                lsResultFields(13) = prsMsaRecord.Result13
                lsResultFields(14) = prsMsaRecord.Result14
                lsResultFields(15) = prsMsaRecord.Result15
                lsResultFields(16) = prsMsaRecord.Result16
                lsResultFields(17) = prsMsaRecord.Result17
                lsResultFields(18) = prsMsaRecord.Result18
                lsResultFields(19) = prsMsaRecord.Result19
                lsResultFields(20) = prsMsaRecord.Result20
                lsResultFields(21) = prsMsaRecord.Result21
                lsResultFields(22) = prsMsaRecord.Result22
                lsResultFields(23) = prsMsaRecord.Result23
                lsResultFields(24) = prsMsaRecord.Result24
                lsResultFields(25) = prsMsaRecord.Result25
                lsResultFields(26) = prsMsaRecord.Result26
                lsResultFields(27) = prsMsaRecord.Result27
                lsResultFields(28) = prsMsaRecord.Result28
                lsResultFields(29) = prsMsaRecord.Result29
                lsResultFields(30) = prsMsaRecord.Result30
                lsResultFields(31) = prsMsaRecord.Result31
                lsResultFields(32) = prsMsaRecord.Result32
                lsResultFields(33) = prsMsaRecord.Result33
                lsResultFields(34) = prsMsaRecord.Result34
                lsResultFields(35) = prsMsaRecord.Result35
                lsResultFields(36) = prsMsaRecord.Result36
                lsResultFields(37) = prsMsaRecord.Result37
                lsResultFields(38) = prsMsaRecord.Result38
                lsResultFields(39) = prsMsaRecord.Result39
                lsResultFields(40) = prsMsaRecord.Result40
                lsResultFields(41) = prsMsaRecord.Result41
                lsResultFields(42) = prsMsaRecord.Result42
                lsResultFields(43) = prsMsaRecord.Result43
                lsResultFields(44) = prsMsaRecord.Result44
                lsResultFields(45) = prsMsaRecord.Result45
                lsResultFields(46) = prsMsaRecord.Result46
                lsResultFields(47) = prsMsaRecord.Result47
                lsResultFields(48) = prsMsaRecord.Result48
                lsResultFields(49) = prsMsaRecord.Result49
                lsResultFields(50) = prsMsaRecord.Result50
                lsResultFields(51) = prsMsaRecord.Result51
                lsResultFields(52) = prsMsaRecord.Result52
                lsResultFields(53) = prsMsaRecord.Result53
                lsResultFields(54) = prsMsaRecord.Result54
                lsResultFields(55) = prsMsaRecord.Result55
                lsResultFields(56) = prsMsaRecord.Result56
                lsResultFields(57) = prsMsaRecord.Result57
                lsResultFields(58) = prsMsaRecord.Result58
                lsResultFields(59) = prsMsaRecord.Result59
                ' lsResultFields(60)=prsMsaRecord.Result60
            Catch ex As Exception

            End Try
        End Sub
#End Region
    End Module
End Namespace