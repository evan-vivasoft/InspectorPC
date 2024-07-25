'
'//===============================================================================
'// Copyright Wigermsa
'// All rights reserved.
'//===============================================================================
'


Imports System.Globalization

Namespace Model.Result.formats
    ''' <summary>
    ''' This Class represents part XML model used to create the InspectionResultsData Report.
    ''' Do Not set properties via the setters!
    ''' Use the constructor or a specific Set function to ensure proper setting of a value.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId:="TimeStamp")> _
    Public Class DateTimeStamp
        ''' <summary>
        ''' 
        ''' </summary>
        Public Const DATE_FORMAT As String = "yyyy-MM-dd"
        ''' <summary>
        ''' 
        ''' </summary>
        Public Const TIME_FORMAT As String = "HH:mm:ss"

  
    End Class
End Namespace

