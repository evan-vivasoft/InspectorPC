'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================

Public Module clsGeneral
    ''' <summary>
    ''' The main version of the application INSPECTOR PC
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ComponentVersion() As String
        Get
            Dim versionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
            Return versionInfo.Major & "." & versionInfo.Minor & "." & versionInfo.Build & "." & versionInfo.Revision
        End Get
    End Property
    ''' <summary>
    ''' Assembly information
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AssemblyInformation() As System.Reflection.Assembly
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly
        End Get
    End Property
End Module