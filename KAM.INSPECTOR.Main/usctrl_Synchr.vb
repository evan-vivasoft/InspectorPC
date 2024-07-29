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
Imports KAM.INSPECTOR.Main.My.Resources
Imports KAM.INSPECTOR.Main.SyncErrorMessages

Public Class usctrl_Synchr
#Region "Class members"
    'MOD 06
    Public Event EvntSyncRunning(status As Boolean)

    Private toolTipImage1 As ToolTip = New ToolTip

    Private WithEvents InspectorSyncHandling As SyncManager

#End Region

#Region "Constructor"
    ''' <summary>
    ''' Load the form with information
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_FormInspector_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load
        SyncStatusLoadGrid()
        pictureConnectStatus.Image = Fit2PictureBox(Images.connectionIdle, pictureConnectStatus)

        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False

        InspectorSyncHandling = New SyncManager

        'Initialize connection
        InspectorSyncHandling.InitializeConnection()

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
    Private Sub SyncButton_Click(sender As Object, e As EventArgs) Handles SyncButton.Click
        StartSync()
    End Sub

    ''' <summary>
    ''' Handling of starting the synchronisation
    ''' Some logic tests
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartSync()
        rdWaitingSync.Visible = True
        rdProgressCopyfile.Visible = True
        InspectorSyncHandling.StartSynchronisation()
    End Sub

    ''' <summary>
    ''' Handling of short cuts keys
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message,
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
        BeginInvoke(New Action(AddressOf InvokeSynchronizeStarted))
    End Sub

    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeStarted()
        'MOD 06
        pictureConnectStatus.Enabled = False
        RaiseEvent EvntSyncRunning(True)
        'rdGridPDA.Rows.Clear()
        'rdGridSyncStatus.Rows.Clear()
        rdWaitingSync.Visible = True
        'rdWaitingSync.StartWaiting()
        rdProgressCopyfile.Visible = True
        '  pictureConnectStatus
        'pictureConnectStatus.Image = Fit2PictureBox(Images.connectionStart, pictureConnectStatus)
    End Sub

    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>

    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeFinished()
        'MOD 06
        RaiseEvent EvntSyncRunning(False)
        'pictureConnectStatus.Image = Fit2PictureBox(Images.connectionComplete, pictureConnectStatus)
        'rdWaitingSync.StopWaiting()
        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False
        pictureConnectStatus.Enabled = True
    End Sub

    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>

    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeError()
        'MOD 06
        RaiseEvent EvntSyncRunning(False)
        'pictureConnectStatus.Image = Fit2PictureBox(Images.connectionError, pictureConnectStatus)
        'rdWaitingSync.StopWaiting()
        rdWaitingSync.Visible = False
        rdProgressCopyfile.Visible = False
        pictureConnectStatus.Enabled = True
    End Sub

#End Region

#Region "Event handling of synchronization process status"
    ''' <summary>
    ''' Handling the event of synchronization step status
    ''' </summary>
    ''' <param name="info"></param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSyncStatus(info As SyncStatusStepModel) Handles InspectorSyncHandling.EvntsyncStepStatus
        BeginInvoke(New Action(Of SyncStatusStepModel)(AddressOf InvokeSyncStatus), info)
    End Sub
    ''' <summary>
    ''' Handling the invoke of synchronization step status
    ''' This will update the grid
    ''' </summary>
    ''' <param name="info"></param>
    ''' <remarks></remarks>
    Private Sub InvokeSyncStatus(info As SyncStatusStepModel)
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
    Private Sub SyncStatusGridRowChange(ByVal stepInformation As SyncStatusStepModel)
        Dim transStepErrorMessage = ""
        Dim row As Integer
        For row = 0 To rdGridSyncStatus.RowCount - 1
            If rdGridSyncStatus.Rows(row).Cells(GridSyncStatusFields.FldStepID).Value.ToString().ToUpper().Equals(stepInformation.StepId.ToString) Then
                'MOD 07
                SyncStatusUpdateGridImage(row, stepInformation)
                If stepInformation.StepErrorCode.ToString = enumErrorMessages.NoError.ToString Then Exit Sub
                transStepErrorMessage = ResourceConvert.GetSyncErrorTranslation(stepInformation.StepErrorCode.ToString)
                Me.rdGridSyncStatus.Rows(row).Cells(GridSyncStatusFields.FldStepStatus).Value = transStepErrorMessage & stepInformation.SetpAdditionalMessage
                MsgBox(transStepErrorMessage & stepInformation.SetpAdditionalMessage, MsgBoxStyle.Critical, Application.ProductName)
                Exit Sub
            End If
        Next
        If row >= rdGridSyncStatus.RowCount - 1 Then
            Dim StepIDText = ResourceConvert.GetSyncStepTranslation(stepInformation.StepId.ToString)
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
    Private Sub SyncStatusUpdateGridImage(ByVal rowIndex As Integer, ByVal stepInformation As SyncStatusStepModel)
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
    ''' '' ''' <summary>
    '' '' ''' Add items to grid; Set column text form resource
    '' '' ''' </summary>
    '' '' ''' <remarks></remarks>
    Private Sub SyncStatusAddGridColumns()
        With rdGridSyncStatus
            Dim dateTimeColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepDateTime)
            dateTimeColumn.HeaderText = "StepStatusDateTime"
            dateTimeColumn.MaxWidth = 55
            dateTimeColumn.MinWidth = 55

            Dim imageColumn As New GridViewImageColumn(GridSyncStatusFields.FldStatusImage)
            imageColumn.ImageLayout = Windows.Forms.ImageLayout.Zoom
            imageColumn.MaxWidth = 18
            imageColumn.MinWidth = 18
            imageColumn.HeaderText = "StatusImage"

            Dim stepIDColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepID)
            stepIDColumn.HeaderText = "StepID"
            stepIDColumn.IsVisible = False

            Dim stepIDTextColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepIDText)
            stepIDTextColumn.HeaderText = "StepIDText"
            stepIDTextColumn.MinWidth = 250

            Dim stepStatusColumn As New GridViewTextBoxColumn(GridSyncStatusFields.FldStepStatus)
            stepStatusColumn.HeaderText = "StepStatus"
            stepStatusColumn.IsVisible = True

            .Columns.Add(dateTimeColumn)
            .Columns.Add(imageColumn)
            .Columns.Add(stepIDColumn)
            .Columns.Add(stepIDTextColumn)
            .Columns.Add(stepStatusColumn)
            .BestFitColumns()

        End With
    End Sub
    'Private Sub rdGridSyncStatus_ToolTipTextNeeded(sender As Object, e As Telerik.WinControls.ToolTipTextNeededEventArgs) Handles rdGridSyncStatus.ToolTipTextNeeded
    '    Dim dataCell As GridDataCellElement = TryCast(sender, GridDataCellElement)
    '    If dataCell IsNot Nothing Then
    '        Dim toolTipText As String = ""
    '        toolTipText = dataCell.RowInfo.Cells(GridSyncStatusFields.FldStepStatus).Value.ToString

    '        e.ToolTipText = toolTipText
    '    End If
    'End Sub
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
End Class
