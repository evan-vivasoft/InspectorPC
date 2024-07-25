'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports System.IO
Public Class clsSettings

#Region "Class members"
    Private dsSettings As DataSet
    'MOD 10 Public ReadOnly SettingFilename As String = My.Application.Info.DirectoryPath & "\Config\INSPECTORSettings.xml"
    Public ReadOnly NoValue As String = "<No value>"
    Private m_SettingFileName As String = ""

#End Region
#Region "Properties"

    ''' <summary>
    ''' Reading the settings from the file INSEPCTORSettings.xml
    ''' </summary>
    ''' <param name="section"></param>
    ''' <param name="settingName"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSetting(ByVal section As String, ByVal settingName As String)
        Get
            Try
                Dim dsRow() As Data.DataRow
                dsRow = dsSettings.Tables("Settings").Select("Section = '" & section & "'")
                Dim SettingsID As Integer
                Try
                    SettingsID = dsRow(0)("Settings_id").ToString()
                Catch ex As Exception
                    Return NoValue
                End Try

                Dim dsRow2() As Data.DataRow
                dsRow2 = dsSettings.Tables("Setting").Select("Settings_id = " & SettingsID & "and name = '" & settingName & "'")
                Try
                    Return (dsRow2(0)("Value").ToString)
                Catch ex As Exception
                    Return NoValue
                End Try

            Catch ex As Exception
                Return (NoValue)
            End Try

        End Get
    End Property
    ''' <summary>
    ''' Save the settings to the file INSEPCTORSettings.xml
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="SettingName"></param>
    ''' <param name="fileSettingName">Optional; Read </param>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SaveSetting(ByVal section As String, ByVal settingName As String, Optional ByVal fileSettingName As String = "")
        Set(value)
            Dim dsRow() As Data.DataRow
            dsRow = dsSettings.Tables("Settings").Select("Section = '" & section & "'")
            'Check if the Section (row) already exists. If not add to table and select it.
            If dsRow.Length = 0 Then
                dsSettings.Tables("Settings").Rows.Add(dsSettings.Tables("Settings").Rows.Count, section)
                dsRow = dsSettings.Tables("Settings").Select("Section = '" & section & "'")
            End If

            Dim SettingsID As Integer
            SettingsID = dsRow(0)("Settings_id").ToString()
            Dim dsRow2() As Data.DataRow
            dsRow2 = dsSettings.Tables("Setting").Select("Settings_id = " & SettingsID & "and name = '" & settingName & "'")
            'Check if the Setting_id already exists. If not add to table and set value.
            If dsRow2.Length = 0 Then
                'MOD 80
                Dim newRow As DataRow = dsSettings.Tables("Setting").NewRow
                newRow("Name") = settingName
                newRow("Value") = value
                newRow("Settings_id") = SettingsID

                'dsSettings.Tables("Setting").Rows.Add(settingName, value, SettingsID)
                dsSettings.Tables("Setting").Rows.Add(newRow)
            Else
                dsRow2(0)("Value") = value
            End If
            'Save the settingsfile.

            WriteFileFromDataset(m_SettingFileName)
        End Set
    End Property

#End Region
#Region "Constructor"

    ''' <summary>
    ''' Read the settings file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            ReadFileIntoDataset(ModuleSettings.SettingFilename)
            m_SettingFileName = ModuleSettings.SettingFilename
        Catch ex As Exception
            ' MsgBox(SettingFilename)
        End Try
    End Sub

    ''' <summary>
    ''' Read the settings file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(fileSettingName As String)
        Try
            ReadFileIntoDataset(fileSettingName)
            m_SettingFileName = fileSettingName
        Catch ex As Exception
            ' MsgBox(SettingFilename)
        End Try
    End Sub
#End Region
    ''' <summary>
    ''' Reload the settings file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReloadSettingsFile()
        ReadFileIntoDataset(m_SettingFileName)
    End Sub
    ''' <summary>
    ''' Reload the settings file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReloadSettingsFile(fileSettingName As String)
        ReadFileIntoDataset(fileSettingName)
        m_SettingFileName = fileSettingName
    End Sub

#Region "Data set handling"
    ''' <summary>
    ''' Read the settings file into the dataset
    ''' </summary>
    ''' <param name="fileName">File name of the settings file</param>
    ''' <remarks></remarks>
    Private Sub ReadFileIntoDataset(ByVal fileName As String)
        dsSettings = New DataSet

        Dim fs As FileStream = Nothing
        Try
            fs = New FileStream(fileName, FileMode.Open, FileAccess.Read)
        Catch ex As Exception
        End Try
        Try
            dsSettings.ReadXml(fs)
        Catch ex As Exception
        End Try
        fs.Close()
    End Sub
    ''' <summary>
    ''' Write the settings file from the data set
    ''' </summary>
    ''' <param name="fileName">Settings file file name</param>
    ''' <remarks></remarks>
    Private Sub WriteFileFromDataset(ByVal fileName As String)
        Dim fs As FileStream = Nothing
        Try
            fs = New FileStream(fileName, FileMode.Create, FileAccess.Write)
        Catch ex As Exception

        End Try
        Try
            dsSettings.WriteXml(fs)
        Catch ex As Exception

        End Try
        fs.Close()
    End Sub
#End Region
End Class
