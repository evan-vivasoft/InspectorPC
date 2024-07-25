'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'===============================================================================

Namespace Model.Result
    Module clsSdfHelpers
#Region "Public functions"
        Public Function TransferSdfDateTime(ByVal SdfDateTime As Date, ByRef rDate As Date, ByRef rTime As Date) As Boolean
            ' Change the date for add to database 
            Dim ldDateStart As Date = Now
            Dim ldTimeStart As Date = Now
            Try
                ldDateStart = SdfDateTime
                ldTimeStart = CType(DateValue("December 30, 1899") + ldDateStart.TimeOfDay, Date)
                ldDateStart = ldDateStart.Date
            Catch ex As Exception
                Return False
            End Try
            rDate = ldDateStart
            rTime = ldTimeStart
            Return True
        End Function

#End Region
    End Module

End Namespace
