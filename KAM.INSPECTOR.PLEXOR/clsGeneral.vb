'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Public Module clsGeneral
    ''' <summary>
    ''' The main version of the application INSPECTOR PC
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ComponentVersion() As String
        Dim versionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        Return versionInfo.Major & "." & versionInfo.Minor & "." & versionInfo.Build & "." & versionInfo.Revision

    End Function
    ''' <summary>
    ''' Assembly information
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function AssemblyInformation() As System.Reflection.Assembly
        Return System.Reflection.Assembly.GetExecutingAssembly
    End Function
End Module
