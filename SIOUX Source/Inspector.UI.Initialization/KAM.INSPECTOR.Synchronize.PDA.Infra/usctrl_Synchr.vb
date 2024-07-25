'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports OpenNETCF.Desktop.Communication
Imports System.IO
Imports System.Math
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports Telerik.WinControls.UI
Imports KAM.COMMUNICATOR.Synchronize.Bussiness
Imports Telerik.WinControls.Data
Imports System.ComponentModel
Imports KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
Imports KAM.COMMUNICATOR.Synchronize.Infra.My.Resources
Imports KAM.COMMUNICATOR.Synchronize.Infra.COMMUNICATOR.Status.Model

Public Class usctrl_Synchr
#Region "Class members"
    'MOD 06
    Public Event EvntSyncRunning(status As Boolean)

    Private toolTipImage1 As ToolTip = New ToolTip

    Private WithEvents InspectorSyncHandling As clsSyncManager
    Private pdaConnectStatus As clsSyncManager.enumConnectionStatus = clsSyncManager.enumConnectionStatus.Disconnected

#End Region
#Region "Properties"
    Public ReadOnly Property InspectorPcInstalled As Boolean
        Get
            Return InspectorSyncHandling.InspectorPcInstalled
        End Get
    End Property

    Public ReadOnly Property InspectorPdaInstalled As Boolean
        Get
            Return InspectorSyncHandling.InspectorPdaInstalled
        End Get
    End Property

#End Region


#Region "Constructor"
    ''' <summary>
    ''' Load the form with information
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_FormInspector_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        DebugLogger = log4net.LogManager.GetLogger("LogCommunicatorAppl1")
        'Determine Result (ACCESS or XML) 
        'Determine PRS (ACCESS or XML) 
        SyncStatusLoadGrid()
        PdaStatusLoadGrid()
        pictureConnectStatus.Image = Fit2PictureBox(Images.connectionIdle, pictureConnectStatus)

        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False

        InspectorSyncHandling = New clsSyncManager

        'Initialize connection
        InspectorSyncHandling.InitializeConnection()

        If InspectorSyncHandling.InspectorPcInstalled = True Then toolTipImage1.SetToolTip(pictureConnectStatus, CommunicatorPDAresx.str_ClickButtonToStartSyncInspectorPC)


    End Sub

#End Region
#Region "GUI Handling"
    ''' <summary>
    ''' Start the synchronisation of INSPECTOR PC.
    ''' The user clicks on the sync icon in the GUI
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pictureConnectStatus_Click(sender As System.Object, e As System.EventArgs) Handles pictureConnectStatus.Click
        StartSync()
    End Sub

    ''' <summary>
    ''' Handling of starting the synchronisation
    ''' Some logic tests
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartSync()
        If pdaConnectStatus = clsSyncManager.enumConnectionStatus.Connected Then
            MsgBox(CommunicatorPDAresx.str_SyncNotStartedPdaConnect, vbInformation, Application.ProductName)
            Exit Sub
        End If
        If InspectorSyncHandling.InspectorPcInstalled = True Then InspectorSyncHandling.StartSynchronisationPc()
        'MsgBox(CommunicatorPDAresx.str_SyncNotStartedNoInspectorPC, vbInformation)
    End Sub

    ''' <summary>
    ''' Handling of short cuts keys
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, _
      ByVal keyData As Keys) As Boolean
        Const WM_KEYDOWN As Integer = &H100
        Const WM_SYSKEYDOWN As Integer = &H104

        If ((msg.Msg = WM_KEYDOWN) Or (msg.Msg = WM_SYSKEYDOWN)) Then
            Select Case (keyData)
                Case Keys.F9
                    Console.WriteLine("F9 Captured")
                    StartSync()
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

#End Region

#Region "Event handling Sync proces"
    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSynchronizeStarted() Handles InspectorSyncHandling.EvntSyncStarted
        BeginInvoke(New action(AddressOf InvokeSynchronizeStarted))
    End Sub
    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeStarted()
        'MOD 06
        pictureConnectStatus.Enabled = False
        RaiseEvent EvntSyncRunning(True)
        rdGridPDA.Rows.Clear()
        rdGridSyncStatus.Rows.Clear()
        rdWaitingSync.Visible = True
        rdWaitingSync.StartWaiting()
        rdProgressCopyfile.Visible = True
        '  pictureConnectStatus
        pictureConnectStatus.Image = Fit2PictureBox(Images.connectionStart, pictureConnectStatus)
    End Sub

    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSynchronizeFinished() Handles InspectorSyncHandling.EvntSyncFinished
        BeginInvoke(New action(AddressOf InvokeSynchronizeFinished))
    End Sub
    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeFinished()
        'MOD 06
        RaiseEvent EvntSyncRunning(False)
        pictureConnectStatus.Image = Fit2PictureBox(Images.connectionComplete, pictureConnectStatus)
        rdWaitingSync.StopWaiting()
        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False
        pictureConnectStatus.Enabled = True
    End Sub

    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSynchronizeError() Handles InspectorSyncHandling.EvntSyncError
        BeginInvoke(New action(AddressOf InvokeSynchronizeError))
    End Sub
    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeError()
        'MOD 06
        RaiseEvent EvntSyncRunning(False)
        pictureConnectStatus.Image = Fit2PictureBox(Images.connectionError, pictureConnectStatus)
        rdWaitingSync.StopWaiting()
        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False
        pictureConnectStatus.Enabled = True
    End Sub

#End Region
#Region "Event Handling Connection status"
    ''' <summary>
    ''' Handling of event PDA connection status
    ''' </summary>
    ''' <param name="ConnectStatus">The status of the connection</param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingDeviceConnectionStatus(ConnectStatus As clsSyncManager.enumConnectionStatus) Handles InspectorSyncHandling.EvntConnectionStatus
        BeginInvoke(New Action(Of clsSyncManager.enumConnectionStatus)(AddressOf InvokeDeviceConnectStatus), ConnectStatus)
    End Sub
    ''' <summary>
    ''' Invoke of PDA connection status
    ''' </summary>
    ''' <param name="ConnectStatus">The status of the connection</param>
    ''' <remarks></remarks>
    Private Sub InvokeDeviceConnectStatus(ConnectStatus As clsSyncManager.enumConnectionStatus)
        pdaConnectStatus = ConnectStatus
        If ConnectStatus = clsSyncManager.enumConnectionStatus.Disconnected Then
            pictureConnectStatus.Image = Fit2PictureBox(Images.connectionIdle, pictureConnectStatus)
            rdWaitingSync.StopWaiting()
            rdWaitingSync.Visible = False
            rdProgressCopyfile.Visible = False
        End If
    End Sub

#End Region
#Region "Event Handling Copy file progress"
    ''' <summary>
    ''' Handling of event file copy process
    ''' </summary>
    ''' <param name="totalFileSize">Total file size</param>
    ''' <param name="copiedFileSize">File size copied</param>
    ''' <param name="fileName">name of file to be copied</param>
    ''' <remarks></remarks>
    Public Sub EvntHandlingCopyFileProgress(totalFileSize As Integer, copiedFileSize As Integer, fileName As String) Handles InspectorSyncHandling.EvntCopyFileProgressStatus
        BeginInvoke(New Action(Of Integer, Integer, String)(AddressOf InvokeCopyFileProgress), totalFileSize, copiedFileSize, fileName)
    End Sub
    ''' <summary>
    ''' Invoke of event file copy process
    ''' </summary>
    ''' <param name="totalFileSize">Total file size</param>
    ''' <param name="copiedFileSize">File size copied</param>
    ''' <param name="fileName">name of file to be copied</param>
    ''' <remarks></remarks>
    Private Sub InvokeCopyFileProgress(totalFileSize As Integer, copiedFileSize As Integer, fileName As String)
        'If 100% copied then hide copy file process bar
        If copiedFileSize >= totalFileSize Then
            rdProgressCopyfile.Visible = False
            Exit Sub
        Else
            rdProgressCopyfile.Visible = True
        End If

        rdProgressCopyfile.Maximum = totalFileSize
        rdProgressCopyfile.Value1 = copiedFileSize
        rdProgressCopyfile.Text = CommunicatorPDAresx.str_Copy_file & Path.GetFileNameWithoutExtension(fileName) & " - " & Round(((100 / totalFileSize) * copiedFileSize), 0) & CommunicatorPDAresx.str_Completed
    End Sub
#End Region

#Region "Event handling PDA information"
    Private Sub EvntHandlingDeviceMemoryStatus(info As MEMORYSTATUS) Handles InspectorSyncHandling.EvntDeviceMemoryStatus
        BeginInvoke(New Action(Of MEMORYSTATUS)(AddressOf InvokeDeviceMemoryStatus), info)
    End Sub
    Private Sub InvokeDeviceMemoryStatus(info As MEMORYSTATUS)
        'RadTextBoxControl1.AppendText(info.dwTotalVirtual.ToString & vbCrLf)
    End Sub

    Private Sub EvntHandlingDeviceStorInformation(info As STORE_INFORMATION) Handles InspectorSyncHandling.EvntDeviceStoreInformation
        BeginInvoke(New Action(Of STORE_INFORMATION)(AddressOf InvokeDeviceStorInformation), info)
    End Sub
    Private Sub InvokeDeviceStorInformation(info As STORE_INFORMATION)
        PdaStatusGridRowChange(4, CommunicatorPDAresx.str_Storage, CommunicatorPDAresx.str_Device_free_storage, info.dwFreeSize)
    End Sub
    Private Sub EvntHandlingDeviceSystemInfo(info As SYSTEM_INFO) Handles InspectorSyncHandling.EvntDeviceSystemInfo
        BeginInvoke(New Action(Of SYSTEM_INFO)(AddressOf InvokeDeviceSystemInfo), info)
    End Sub
    Private Sub InvokeDeviceSystemInfo(info As SYSTEM_INFO)
        PdaStatusGridRowChange(3, CommunicatorPDAresx.str_Processor, CommunicatorPDAresx.str_ProcessorType, info.dwProcessorType.ToString)
    End Sub
    Private Sub EvntHandlingDeviceSystemPowerStatus(info As SYSTEM_POWER_STATUS_EX) Handles InspectorSyncHandling.EvntDeviceSystemPowerStatus
        BeginInvoke(New Action(Of SYSTEM_POWER_STATUS_EX)(AddressOf InvokeDeviceSystemPowerStatus), info)
    End Sub
    Private Sub InvokeDeviceSystemPowerStatus(info As SYSTEM_POWER_STATUS_EX)
        Dim hmsBatteryLife = TimeSpan.FromSeconds(info.BatteryLifeTime)
        Dim hBatteryLife = hmsBatteryLife.Hours.ToString
        Dim mBatteryLife = hmsBatteryLife.Minutes.ToString
        Dim sBatteryLife = hmsBatteryLife.Seconds.ToString

        Dim hmsBatteryFull = TimeSpan.FromSeconds(info.BatteryFullLifeTime)
        Dim hBatteryFull = hmsBatteryFull.Hours.ToString
        Dim mBatteryFull = hmsBatteryFull.Minutes.ToString
        Dim sBatteryFull = hmsBatteryFull.Seconds.ToString

        PdaStatusGridRowChange(2, CommunicatorPDAresx.str_Battery, CommunicatorPDAresx.str_BatteryLifeTime, hBatteryLife & " " & CommunicatorPDAresx.str_hours & " " & mBatteryLife & " " & CommunicatorPDAresx.str_minutes)
        PdaStatusGridRowChange(2, CommunicatorPDAresx.str_Battery, CommunicatorPDAresx.str_BatteryLifePercent, info.BatteryLifePercent & "%")
        'PdaStatusGridRowChange(2, "!Battery", "!BatteryFullLifeTime", mBatteryFull & " " & "!minutes")
        'PdaStatusGridRowChange(2, "!Battery", "!BatteryFlag", info.BatteryFlag)

    End Sub
    Private Sub EvntHandlingInspectorInformation(info As clsInspectorCommunication.StructInspectionInformation) Handles InspectorSyncHandling.EvntInspectionInformation
        BeginInvoke(New Action(Of clsInspectorCommunication.StructInspectionInformation)(AddressOf InvokeInspectorInformation), info)
    End Sub
    Private Sub InvokeInspectorInformation(info As clsInspectorCommunication.StructInspectionInformation)
        PdaStatusGridRowChange(1, CommunicatorPDAresx.str_INSPECTOR, CommunicatorPDAresx.str_INSPECTORSerialNumber, info.SerialNumber.ToString)
        PdaStatusGridRowChange(1, CommunicatorPDAresx.str_INSPECTOR, CommunicatorPDAresx.str_INSPECTORVersion, info.Version.ToString)
        If info.SubVersion <> "" Then PdaStatusGridRowChange(1, CommunicatorPDAresx.str_INSPECTOR, CommunicatorPDAresx.str_INSPECTORSubVersion, info.SubVersion.ToString)
    End Sub
#End Region

#Region "Grid PDA status handling"
    ''' <summary>
    ''' Initialize the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PdaStatusLoadGrid()
        PdaStatusAddGridColumns()
        With Me.rdGridPDA

            .EnableSorting = False
            .EnableGrouping = True
            .EnableFiltering = False
            .AllowColumnChooser = True

            .AllowAddNewRow = False
            .AllowRowReorder = False
            .ReadOnly = True
            .MultiSelect = False

            .AllowColumnHeaderContextMenu = False
            .AllowCellContextMenu = False

            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = False

            .EnableAlternatingRowColor = True

            .AutoSizeRows = True
            .ShowItemToolTips = True
            .AutoExpandGroups = True
            .ShowGroupPanel = False
            .ShowGroupedColumns = False

            .ShowColumnHeaders = False

            .BestFitColumns()
            .AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
            .TableElement.ShowSelfReferenceLines = True
        End With

    End Sub

    ''' <summary>
    ''' Changing the group text
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridView1_GroupSummaryEvaluate(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GroupSummaryEvaluationEventArgs) Handles rdGridPDA.GroupSummaryEvaluate
        If e.SummaryItem.Name = GridPDAStatus.FldStepMainIDText Then
            e.FormatString = [String].Format(e.Value)
        End If
    End Sub

    '' '' ''' <summary>
    '' '' ''' Update the grid information; Stepmessage an icon
    '' '' ''' </summary>
    '' '' ''' <param name="stepInformation"></param>
    '' '' ''' <remarks></remarks>
    Private Sub PdaStatusGridRowChange(stepMainID As Integer, stepMainIDText As String, stepIdText As String, stepMessage As String)
        Me.rdGridPDA.Rows.Add(stepMainID, stepMainIDText, stepIdText, stepMessage)
    End Sub
    '' '' ''' <summary>
    '' '' ''' Add items to grid; Set column text form resource
    '' '' ''' </summary>
    '' '' ''' <remarks></remarks>
    Private Sub PdaStatusAddGridColumns()
        With rdGridPDA
            Dim stepMainIDColumn As New GridViewTextBoxColumn(GridPDAStatus.FldStepMainID)
            stepMainIDColumn.HeaderText = CommunicatorPDAresx.str_InspectorInfoStepMainID
            stepMainIDColumn.IsVisible = True

            Dim stepMainIDTextColumn As New GridViewTextBoxColumn(GridPDAStatus.FldStepMainIDText)
            stepMainIDTextColumn.HeaderText = CommunicatorPDAresx.str_InspectorInfoStepMainIDText


            Dim stepIDTextColumn As New GridViewTextBoxColumn(GridPDAStatus.FldStepIDText)
            stepIDTextColumn.HeaderText = CommunicatorPDAresx.str_InspectorInfoStepIDText

            Dim stepMessagetColumn As New GridViewTextBoxColumn(GridPDAStatus.FldStepMessage)
            stepMessagetColumn.HeaderText = CommunicatorPDAresx.str_InspectorInfoMessage

            .Columns.Add(stepMainIDColumn)
            .Columns.Add(stepMainIDTextColumn)
            .Columns.Add(stepIDTextColumn)
            .Columns.Add(stepMessagetColumn)

            Dim descriptor As New GroupDescriptor()
            descriptor.GroupNames.Add(GridPDAStatus.FldStepMainIDText, ListSortDirection.Ascending)
            .GroupDescriptors.Add(descriptor)

            '.GroupDescriptors.Add(New GridGroupByExpression("StepID as StepID format ""{0}: {1}"" Group By StepID"))
        End With
    End Sub
    ''' <summary>
    ''' Clas for column setting of PDA status
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GridPDAStatus
        Public Const FldStepMainID As String = "StepMainID"
        Public Const FldStepMainIDText As String = "StepMainIDText"
        Public Const FldStepIDText As String = "StepIDText"
        Public Const FldStepMessage As String = "StepMessage"
    End Class

#End Region
#Region "Event handling of synchronization process status"
    ''' <summary>
    ''' Handling the event of synchronization step status
    ''' </summary>
    ''' <param name="info"></param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSyncStatus(info As COMMUNICATOR.Status.Model.SyncStatusStepModel) Handles InspectorSyncHandling.EvntSyncStepStatus
        BeginInvoke(New Action(Of COMMUNICATOR.Status.Model.SyncStatusStepModel)(AddressOf InvokeSyncStatus), info)
    End Sub
    ''' <summary>
    ''' Handling the invoke of synchronization step status
    ''' This will update the grid
    ''' </summary>
    ''' <param name="info"></param>
    ''' <remarks></remarks>
    Private Sub InvokeSyncStatus(info As COMMUNICATOR.Status.Model.SyncStatusStepModel)
        Debug.Print("Status info: " & info.StepId.ToString & " " & info.StepResult.ToString & vbCrLf)
        SyncStatusGridRowChange(info)
    End Sub
#End Region

#Region "Grid Sync status handling"
    ''' <summary>
    ''' Initialize the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SyncStatusLoadGrid()

        With Me.rdGridSyncStatus

            .EnableSorting = False
            .EnableGrouping = False
            .EnableFiltering = False
            .AllowColumnChooser = True

            .AllowAddNewRow = False
            .AllowRowReorder = False
            .ReadOnly = True
            .MultiSelect = False


            .AllowColumnHeaderContextMenu = False
            .AllowCellContextMenu = False

            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = False

            .EnableAlternatingRowColor = True

            .AutoSizeRows = True
            .ShowItemToolTips = True
            .ShowColumnHeaders = False

            .BestFitColumns()
            .AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        End With
        SyncStatusAddGridColumns()
    End Sub

    '' '' ''' <summary>
    '' '' ''' Update the grid information; Stepmessage an icon
    '' '' ''' </summary>
    '' '' ''' <param name="stepInformation"></param>
    '' '' ''' <remarks></remarks>
    Private Sub SyncStatusGridRowChange(ByVal stepInformation As COMMUNICATOR.Status.Model.SyncStatusStepModel)
        Dim transStepErrorMessage = ""
        Dim row As Integer
        For row = 0 To rdGridSyncStatus.RowCount - 1
            If rdGridSyncStatus.Rows(row).Cells(GridSyncStatusFields.FldStepID).Value.ToString().ToUpper().Equals(stepInformation.StepId.ToString) Then
                'MOD 07
                SyncStatusUpdateGridImage(row, stepInformation)
                If stepInformation.StepErrorCode.ToString = enumErrorMessages.NoError.ToString Then Exit Sub
                transStepErrorMessage = clsResourceConvert.GetSyncErrorTranslation(stepInformation.StepErrorCode.ToString)
                Me.rdGridSyncStatus.Rows(row).Cells(GridSyncStatusFields.FldStepStatus).Value = transStepErrorMessage & stepInformation.SetpAdditionalMessage
                MsgBox(transStepErrorMessage & stepInformation.SetpAdditionalMessage, MsgBoxStyle.Critical, Application.ProductName)
                Exit Sub
            End If
        Next
        If row >= rdGridSyncStatus.RowCount - 1 Then
            Dim StepIDText = clsResourceConvert.GetSyncStepTranslation(stepInformation.StepId.ToString)
            Me.rdGridSyncStatus.Rows.Add(Format(stepInformation.StepDateTime, "HH:mm:ss"), "", stepInformation.StepId.ToString, StepIDText, "")
            SyncStatusUpdateGridImage(rdGridSyncStatus.RowCount - 1, stepInformation)
        End If
    End Sub
    ''' <summary>
    ''' Set the image of the sync status
    ''' </summary>
    ''' <param name="rowIndex">Row for apply image</param>
    ''' <param name="stepInformation">Status information</param>
    ''' <remarks></remarks>
    Private Sub SyncStatusUpdateGridImage(ByVal rowIndex As Integer, ByVal stepInformation As COMMUNICATOR.Status.Model.SyncStatusStepModel)
        Select Case stepInformation.StepResult
            Case SynchronisationStep.SyncStatus.SError
                Me.rdGridSyncStatus.Rows(rowIndex).Cells(GridSyncStatusFields.FldStatusImage).Value = Images.Delete
            Case SynchronisationStep.SyncStatus.Succes
                Me.rdGridSyncStatus.Rows(rowIndex).Cells(GridSyncStatusFields.FldStatusImage).Value = Images.Green_Check
            Case SynchronisationStep.SyncStatus.Started
                Me.rdGridSyncStatus.Rows(rowIndex).Cells(GridSyncStatusFields.FldStatusImage).Value = ""
            Case SynchronisationStep.SyncStatus.TimeOut
                Me.rdGridSyncStatus.Rows(rowIndex).Cells(GridSyncStatusFields.FldStatusImage).Value = Images.Delete
            Case SynchronisationStep.SyncStatus.Unset
            Case SynchronisationStep.SyncStatus.Warning
                Me.rdGridSyncStatus.Rows(rowIndex).Cells(GridSyncStatusFields.FldStatusImage).Value = Images.warning
        End Select
    End Sub
    '' '' ''' <summary>
    '' '' ''' Add items to grid; Set column text form resource
    '' '' ''' </summary>
    '' '' ''' <remarks></remarks>
    Private Sub SyncStatusAddGridColumns()
        With rdGridSyncStatus
            Dim dateTimeColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepDateTime)
            dateTimeColumn.HeaderText = CommunicatorPDAresx.str_SyncStatusDateTime
            dateTimeColumn.MaxWidth = 55
            dateTimeColumn.MinWidth = 55

            Dim imageColumn As New GridViewImageColumn(GridSyncStatusFields.FldStatusImage)
            imageColumn.ImageLayout = Windows.Forms.ImageLayout.Zoom
            imageColumn.MaxWidth = 18
            imageColumn.MinWidth = 18
            imageColumn.HeaderText = CommunicatorPDAresx.str_SyncStatusStatusIcon

            Dim stepIDColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepID)
            stepIDColumn.HeaderText = CommunicatorPDAresx.str_SyncStatusStepID
            stepIDColumn.IsVisible = False

            Dim stepIDTextColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepIDText)
            stepIDTextColumn.HeaderText = CommunicatorPDAresx.str_SyncStatusStepMessage
            stepIDTextColumn.MinWidth = 180

            Dim stepStatusColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepStatus)
            stepStatusColumn.HeaderText = CommunicatorPDAresx.str_SyncStatusStatusText
            stepStatusColumn.IsVisible = True

            .Columns.Add(dateTimeColumn)
            .Columns.Add(imageColumn)
            .Columns.Add(stepIDColumn)
            .Columns.Add(stepIDTextColumn)
            .Columns.Add(stepStatusColumn)
            .BestFitColumns()

        End With
    End Sub
    Private Sub rdGridSyncStatus_ToolTipTextNeeded(sender As Object, e As Telerik.WinControls.ToolTipTextNeededEventArgs) Handles rdGridSyncStatus.ToolTipTextNeeded
        Dim dataCell As GridDataCellElement = TryCast(sender, GridDataCellElement)
        If dataCell IsNot Nothing Then
            Dim toolTipText As String = ""
            toolTipText = dataCell.RowInfo.Cells(GridSyncStatusFields.FldStepStatus).Value.ToString

            e.ToolTipText = toolTipText
        End If
    End Sub
    ''' <summary>
    ''' The different fields"(columns) in the initstatus grid. Used in the grid of displaying the data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GridSyncStatusFields
        Public Const FldStepDateTime As String = "StepStatusDateTime"
        Public Const FldStepID As String = "StepID"
        Public Const FldStepIDText As String = "StepIDText"
        Public Const FldStatusImage As String = "StatusImage"
        Public Const FldStepStatus As String = "StepStatus"
    End Class

#End Region

#Region "Bitmap Fit to Picture box"
    ''' <summary>
    ''' Resize the image to the dimensions of the picturebox
    ''' </summary>
    ''' <param name="image">Image to resize</param>
    ''' <param name="picBox">Picturebox to fit</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Fit2PictureBox(image As Image, picBox As PictureBox) As Image
        Dim bmp As Bitmap = Nothing
        Dim g As Graphics

        ' Scale:
        Dim scaleY As Double = CDbl(image.Width) / picBox.Width
        Dim scaleX As Double = CDbl(image.Height) / picBox.Height
        Dim scale As Double = If(scaleY < scaleX, scaleX, scaleY)

        ' Create new bitmap:
        bmp = New Bitmap(CInt(Math.Truncate(CDbl(image.Width) / scale)), CInt(Math.Truncate(CDbl(image.Height) / scale)))

        ' Set resolution of the new image:
        bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution)

        ' Create graphics:
        g = Graphics.FromImage(bmp)

        ' Set interpolation mode:
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        ' Draw the new image:
        ' Destination
        ' Source
        g.DrawImage(image, New Rectangle(0, 0, bmp.Width, bmp.Height), New Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel)

        ' Release the resources of the graphics:
        g.Dispose()

        ' Release the resources of the origin image:
        image.Dispose()

        Return bmp
    End Function
#End Region

#Region "Event Handlig of Database creation"
    ''' <summary>
    ''' Event handling of Database creation
    ''' </summary>
    ''' <param name="fileType">File type (PRS or Result</param>
    ''' <param name="total">Total amount of records</param>
    ''' <param name="processed">Records process</param>
    ''' <param name="status">status of creation</param>
    ''' <param name="recordInfo">Optional record information</param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingCreateDbFileProgress(fileType As String, total As Integer, processed As Integer, status As KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral.DbCreateStatus, recordInfo As String) Handles InspectorSyncHandling.EvntdbFileProcessStatusEventHandler
        Debug.Print(fileType & " " & total & " " & processed & " " & recordInfo)
        Invoke(New Action(Of Integer, Integer, KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral.DbCreateStatus, String)(AddressOf InvokeCreateDbFileProgress), total, processed, status, recordInfo)
    End Sub
    ''' <summary>
    ''' Invoke handling of event EvntHandlingCreateDbFileProgress
    ''' </summary>
    ''' <param name="total"></param>
    ''' <param name="processed"></param>
    ''' <param name="status"></param>
    ''' <param name="infoString"></param>
    ''' <remarks></remarks>
    Private Sub InvokeCreateDbFileProgress(total As Integer, processed As Integer, status As KAM.COMMUNICATOR.Synchronize.Bussiness.clsDbGeneral.DbCreateStatus, infoString As String)
        If status = Bussiness.clsDbGeneral.DbCreateStatus.SuccesWrite Then
            rdProgressCopyfile.Visible = False
        Else
            rdProgressCopyfile.Visible = True
        End If
        rdProgressCopyfile.Visible = True
        rdProgressCopyfile.Maximum = total
        rdProgressCopyfile.Value1 = processed
        Select Case status
            Case Bussiness.clsDbGeneral.DbCreateStatus.StartedWrite
                rdProgressCopyfile.Text = CommunicatorPDAresx.str_SavingFile
            Case clsDbGeneral.DbCreateStatus.StartedCreate
                rdProgressCopyfile.Text = infoString & "  " & CommunicatorPDAresx.str_Amount_of_data & "    " & Round(((100 / total) * processed), 0) & CommunicatorPDAresx.str_Completed
            Case clsDbGeneral.DbCreateStatus.ErrorWrite
                MsgBox(CommunicatorPDAresx.str_ErrorWriteFile & ": " & infoString, vbCritical, Application.ProductName)
                rdProgressCopyfile.Text = "Error"
            Case clsDbGeneral.DbCreateStatus.ErrorXsd
                'MOD 21
                MsgBox(CommunicatorPDAresx.str_ErrorXsdValidation & ": " & infoString, vbCritical, Application.ProductName)
                rdProgressCopyfile.Text = "Error"
            Case clsDbGeneral.DbCreateStatus.SError
                rdProgressCopyfile.Text = "Error"
            Case clsDbGeneral.DbCreateStatus.SuccesWrite
                rdProgressCopyfile.Text = ""
            Case clsDbGeneral.DbCreateStatus.TimeOut
            Case clsDbGeneral.DbCreateStatus.Warning
                rdProgressCopyfile.Text = "Warning"
            Case clsDbGeneral.DbCreateStatus.FileNotExists
                MsgBox(CommunicatorPDAresx.str_FileNotExists & ": " & infoString, vbCritical, Application.ProductName)
        End Select
    End Sub
#End Region




End Class
