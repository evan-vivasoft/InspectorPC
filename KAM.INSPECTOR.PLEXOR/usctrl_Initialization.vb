'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls.UI
Imports Inspector.Model
Imports Inspector.Infra
Imports KAM.INSPECTOR.Infra
Imports System.Drawing

''' <summary>
''' User control for initialization of manometer. The status is displayed in a grid
''' Waiting bar for displaying initilization if progress
''' </summary>
''' <remarks></remarks>
Public Class usctrl_Initialization
#Region "Class members"
    Private deviceSelect As String
    Private Enum enumDeviceSelect
        DevicePlexor = 0
        DeviceManometerTH1 = 1
        DeviceManometerTH2 = 2
    End Enum
    Private deviceSelectStepNumber As Integer = 0
    Private _plexorCalibarionDate As Date
    Private infraResources As New clsResourceConvert
    Private _manometerTh1Unit As String = "-"
    Private _manometerTh2Unit As String = "-"
    Private _stepNumberManometerUnit As Integer = -1
    Private _displayDesktopAlert As Boolean = True
    Private _initErrorCode As Integer = 0

#End Region
#Region "Form initialization"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        InitializeDesktopAlert()
    End Sub
    Private Sub usctrl_Initialization_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadGrid()
        StartWaitingBar()
    End Sub
    Private Sub StartWaitingBar()
        rdWaitBar.StartWaiting()
        rdWaitBar.Show()
    End Sub
    ''' <summary>
    ''' Stops the waiting bar
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StopWaitingBar()
        rdWaitBar.StopWaiting()
        rdWaitBar.Hide()
    End Sub
#End Region
#Region "Desktop Alert"
    ''' <summary>
    ''' Initialize desktop alert
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeDesktopAlert()
        Me.RadDesktopAlert1.AutoCloseDelay = 10
        Me.RadDesktopAlert1.ScreenPosition = AlertScreenPosition.BottomRight
        Me.RadDesktopAlert1.ShowCloseButton = True
        Me.RadDesktopAlert1.ShowPinButton = True
        Me.RadDesktopAlert1.ShowOptionsButton = False
        Me.RadDesktopAlert1.SoundToPlay = Media.SystemSounds.Exclamation
        Me.RadDesktopAlert1.PlaySound = True
        Me.RadDesktopAlert1.FixedSize = New System.Drawing.Size(330, 100)

        Me.RadDesktopAlert1.ContentImage = ResizeImage(My.Resources.Images.warning, 50)

        Me.RadDesktopAlert1.Opacity = 0.8
    End Sub
    ''' <summary>
    ''' Show the desktopallert. Calibration date expired
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowAllertCDOutOfDate()
        If _displayDesktopAlert = False Then Exit Sub
        Me.RadDesktopAlert1.CaptionText = [String].Format("<html><Size=10><b>" & My.Resources.KamPLEXORresx.str_Calibration_Header & "</b></html>")
        Me.RadDesktopAlert1.ContentText = "<html>" & My.Resources.KamPLEXORresx.str_Calibration_out_of_date & vbCrLf & My.Resources.KamPLEXORresx.str_Calibration_date & _plexorCalibarionDate & "</html>" '"<html><i>This will be the alert's content text</i>You can place HTML formatted text here as well.</html>" 
        Me.RadDesktopAlert1.Show()
    End Sub
    'MOD 07
    ''' <summary>
    ''' Show the desktopallert. Calibration date about to expire
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowAllertCDToExpire()
        If _displayDesktopAlert = False Then Exit Sub
        Me.RadDesktopAlert1.CaptionText = [String].Format("<html><Size=10><b>" & My.Resources.KamPLEXORresx.str_Calibration_Header & "</b></html>")
        Me.RadDesktopAlert1.ContentText = "<html>" & My.Resources.KamPLEXORresx.str_Calibration_to_expire & vbCrLf & My.Resources.KamPLEXORresx.str_Calibration_date & _plexorCalibarionDate & "</html>"
        Me.RadDesktopAlert1.Show()
    End Sub


    ''' <summary>
    ''' Resize an image
    ''' </summary>
    ''' <param name="image">Image source</param>
    ''' <param name="size">New image size</param>
    ''' <returns>New image with desired size</returns>
    ''' <remarks></remarks>
    Public Function ResizeImage(ByVal image As Image, ByVal size As Integer)
        'following code resizes picture to fit

        Dim bm As New Bitmap(image)

        Dim width As Integer = Val(size) 'image width. 
        Dim height As Integer = Val(size) 'image height

        Dim thumb As New Bitmap(width, height)
        Dim g As Graphics = Graphics.FromImage(thumb)

        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        g.DrawImage(bm, New Rectangle(0, 0, width, height), New Rectangle(0, 0, bm.Width, bm.Height), GraphicsUnit.Pixel)
        g.Dispose()

        Return thumb

        bm.Dispose()
        thumb.Dispose()
    End Function

#End Region
#Region "Grid control"
    ''' <summary>
    ''' Initialize the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadGrid()
        AddGridColumns()
        With Me.rdGridInitStatus

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

            .BestFitColumns()
            .AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        End With

    End Sub
    ''' <summary>
    ''' Add items to grid; Set column text form resource
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddGridColumns()
        With rdGridInitStatus
            Dim stepNumberColumn As New GridViewTextBoxColumn(InitDataFields.FldStepNumber)
            stepNumberColumn.HeaderText = My.Resources.KamPLEXORresx.str_StepNumber
            stepNumberColumn.IsVisible = False

            Dim stepNumberDeviceColumn As New GridViewTextBoxColumn(InitDataFields.FldstepNumberDevice)
            stepNumberDeviceColumn.HeaderText = My.Resources.KamPLEXORresx.str_StepNumberLastDevice
            stepNumberDeviceColumn.IsVisible = False

            Dim manometerColumn As New GridViewTextBoxColumn(InitDataFields.FLdDevice)
            manometerColumn.HeaderText = My.Resources.KamPLEXORresx.str_Device
            Dim initStepColumn As New GridViewTextBoxColumn(InitDataFields.FldStepID)
            initStepColumn.HeaderText = My.Resources.KamPLEXORresx.str_StepID

            Dim stepStatusColumn As New GridViewTextBoxColumn(InitDataFields.FldStepStatus)
            stepStatusColumn.HeaderText = My.Resources.KamPLEXORresx.str_StepStatus
            Dim initDescription As New GridViewTextBoxColumn(InitDataFields.FldStepMessage)
            initDescription.HeaderText = My.Resources.KamPLEXORresx.str_StepMessage
            Dim imageColumn As New GridViewImageColumn(InitDataFields.FldStatusImage)
            imageColumn.ImageLayout = Windows.Forms.ImageLayout.Zoom
            imageColumn.MaxWidth = 20
            imageColumn.MinWidth = 20

            imageColumn.HeaderText = My.Resources.KamPLEXORresx.str_Status
            .Columns.Add(stepNumberColumn)
            .Columns.Add(stepNumberDeviceColumn)
            .Columns.Add(manometerColumn)
            .Columns.Add(initStepColumn)
            .Columns.Add(imageColumn)
            .Columns.Add(stepStatusColumn)
            .Columns.Add(initDescription)
        End With
    End Sub

    ''' <summary>
    ''' Update the grid information; Stepmessage an icon
    ''' </summary>
    ''' <param name="stepInformation"></param>
    ''' <remarks></remarks>
    Private Sub GridRowChange(ByVal stepInformation As Initialization.Model.InitializationStepModel)
        Dim transStepErrorMessage As String = ""
        Dim transDeviceSelected As String = ""
        Dim transStepInformation As String = ""

        transDeviceSelected = infraResources.getDeviceCommandsTranslation(deviceSelect.ToString)
        transStepInformation = infraResources.getDeviceCommandsTranslation(stepInformation.StepId)

        For row As Integer = 0 To rdGridInitStatus.RowCount - 1
            If rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStepID).Value.ToString().ToUpper().Equals(transStepInformation.ToUpper()) And rdGridInitStatus.Rows(row).Cells(InitDataFields.FldstepNumberDevice).Value.Equals(deviceSelectStepNumber.ToString) Then
                'If rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStepID).Value.ToString().ToUpper().Equals(transStepInformation.ToUpper()) And rdGridInitStatus.Rows(row).Cells(InitDataFields.FLdDevice).Value.ToString().ToUpper().Equals(transDeviceSelected.ToString.ToUpper) Then
                Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStepMessage).Value = stepInformation.StepMessage

                transStepErrorMessage = infraResources.getErrorTranslation(stepInformation.StepErrorCode)
                Debug.Print(Now & stepInformation.StepResult.ToString)

                Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStepStatus).Value = transStepErrorMessage
                Select Case stepInformation.StepResult
                    Case InitializationStepResult.ERROR
                        _initErrorCode = stepInformation.StepErrorCode
                        Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStatusImage).Value = My.Resources.Images.Delete
                    Case InitializationStepResult.SUCCESS
                        If _stepNumberManometerUnit = stepInformation.StepNumber Then
                            'Store the unit of the manometer
                            Select Case deviceSelect
                                Case enumDeviceSelect.DeviceManometerTH1
                                    _manometerTh1Unit = stepInformation.StepMessage
                                Case enumDeviceSelect.DeviceManometerTH2
                                    _manometerTh2Unit = stepInformation.StepMessage
                            End Select
                        End If
                        Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStatusImage).Value = My.Resources.Images.Green_Check
                    Case InitializationStepResult.TIMEOUT
                        _initErrorCode = stepInformation.StepErrorCode
                        Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStatusImage).Value = My.Resources.Images.Delete
                    Case InitializationStepResult.UNSET
                        _initErrorCode = stepInformation.StepErrorCode
                    Case InitializationStepResult.WARNING
                        _initErrorCode = stepInformation.StepErrorCode
                        Me.rdGridInitStatus.Rows(row).Cells(InitDataFields.FldStatusImage).Value = My.Resources.Images.warning
                End Select
                Exit Sub
            End If
        Next
    End Sub
    ''' <summary>
    ''' Handling of witch initialization step will be displayed in the grid.
    ''' Also link step to device (plexor, TH1 or TH2)
    ''' </summary>
    ''' <param name="stepInformation"></param>
    ''' <remarks></remarks>
    Private Sub gridRowAdd(ByVal stepInformation As Initialization.Model.InitializationStepModel)
        Select Case stepInformation.StepId
            Case DeviceCommand.CheckBatteryStatus.ToString
                AddRow(stepInformation)
            Case DeviceCommand.None.ToString
            Case DeviceCommand.Connect.ToString
                deviceSelectStepNumber = stepInformation.StepNumber
                deviceSelect = enumDeviceSelect.DevicePlexor
                AddRow(stepInformation)
            Case DeviceCommand.Disconnect.ToString
                AddRow(stepInformation)
            Case DeviceCommand.EnterRemoteLocalCommandMode.ToString
                deviceSelectStepNumber = stepInformation.StepNumber
                deviceSelect = enumDeviceSelect.DevicePlexor
            Case DeviceCommand.ExitRemoteLocalCommandMode.ToString
                'MOD 65  Case DeviceCommand.SwitchInitializationLedOff.ToString
                'MOD 65 AddRow(stepInformation)
                'MOD 65  Case DeviceCommand.SwitchInitializationLedOn.ToString
                'MOD 65  AddRow(stepInformation)
            Case DeviceCommand.SwitchToManometerTH1.ToString
                deviceSelectStepNumber = stepInformation.StepNumber
                deviceSelect = enumDeviceSelect.DeviceManometerTH1
                AddRow(stepInformation)
            Case DeviceCommand.SwitchToManometerTH2.ToString
                deviceSelectStepNumber = stepInformation.StepNumber
                deviceSelect = enumDeviceSelect.DeviceManometerTH2
                AddRow(stepInformation)
            Case DeviceCommand.FlushManometerCache.ToString
                'MOD 19
            Case DeviceCommand.CheckManometerPresent.ToString
                AddRow(stepInformation)
            Case DeviceCommand.CheckBatteryStatus.ToString
                AddRow(stepInformation)
            Case DeviceCommand.CheckIdentification.ToString
                AddRow(stepInformation)
            Case DeviceCommand.CheckPressureUnit.ToString
                _stepNumberManometerUnit = stepInformation.StepNumber
                AddRow(stepInformation)
            Case DeviceCommand.CheckRange.ToString
                AddRow(stepInformation)
            Case DeviceCommand.CheckSCPIInterface.ToString
            Case DeviceCommand.InitiateSelfTest.ToString
            Case DeviceCommand.SetPressureUnit.ToString
            Case Else
        End Select
    End Sub
    ''' <summary>
    ''' Add a row with initialization information to the grid
    ''' </summary>
    ''' <param name="stepInformation"></param>
    ''' <remarks></remarks>
    Private Sub AddRow(ByVal stepInformation As Initialization.Model.InitializationStepModel)
        Dim transDeviceSelected As String = ""
        Dim transStepInformation As String = ""

        transDeviceSelected = infraResources.getDeviceCommandsTranslation("DeviceNumber" & deviceSelect)
        transStepInformation = infraResources.getDeviceCommandsTranslation(stepInformation.StepId)
        Me.rdGridInitStatus.Rows.Add(stepInformation.StepNumber, deviceSelectStepNumber, transDeviceSelected, transStepInformation, "", stepInformation.StepMessage, "")
    End Sub
    ''' <summary>
    ''' Display a tooltiptext
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridInitStatus_ToolTipTextNeeded(sender As Object, e As Telerik.WinControls.ToolTipTextNeededEventArgs) Handles rdGridInitStatus.ToolTipTextNeeded
        Dim gridCell As GridDataCellElement = TryCast(sender, GridDataCellElement)
        Dim tooltipText As String = String.Empty
        If gridCell IsNot Nothing AndAlso String.IsNullOrWhiteSpace(gridCell.ToolTipText) Then
            If Not String.IsNullOrWhiteSpace(gridCell.Text) Then
                tooltipText = Me.rdGridInitStatus.Rows(gridCell.RowIndex).Cells(InitDataFields.FldStepStatus).Value
            End If
            e.ToolTipText = tooltipText
        End If
    End Sub
#End Region
#Region "PLEXOR initialization handling"
    ''' <summary>
    ''' Interface: Creating a new iniitialization step to the grid
    ''' </summary>
    ''' <param name="stepInformation"></param>
    ''' <remarks></remarks>
    Public Sub InitilizationNewStep(ByVal stepInformation As Initialization.Model.InitializationStepModel)
        Dim newRadListDataItem As New RadListDataItem
        newRadListDataItem.Text = Now & ": " & stepInformation.StepId & " " & stepInformation.StepMessage
        lstInitializationSteps.Items.Add(newRadListDataItem)
        gridRowAdd(stepInformation)
    End Sub
    ''' <summary>
    ''' Interface: Handling of initialization finished; Only displayed in list
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Sub InitializationFinished(ByVal message As String)
        Try
            Dim newRadListDataItem As New RadListDataItem
            newRadListDataItem.Text = message
            lstInitializationSteps.Items.Add(newRadListDataItem)
            StopWaitingBar()
        Catch ex As Exception

        End Try


    End Sub
    ''' <summary>
    ''' Interface: Update a iniitialization step to the grid
    ''' </summary>
    ''' <param name="stepInformation"></param>
    ''' <remarks></remarks>
    Public Sub InitializationUpdateStep(ByVal stepInformation As Initialization.Model.InitializationStepModel)
        GridRowChange(stepInformation)
    End Sub


#End Region
#Region "Properties"
    ''' <summary>
    ''' Set PLEXOR calibration date. 
    ''' If date is before current date, a desktop allert is displayed.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PlexorCalibarionDate As Date
        Get
            Return _plexorCalibarionDate
        End Get
        Set(value As Date)
            _plexorCalibarionDate = value

            If _plexorCalibarionDate < Now Then
                ShowAllertCDOutOfDate()
            ElseIf _plexorCalibarionDate < Now.AddDays(30) Then
                'MOD 07
                ShowAllertCDToExpire()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Collected manometer unit of TH1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ManometerTH1unit As String
        Get
            Return _manometerTh1Unit
        End Get
        Set(value As String)
            _manometerTh1Unit = value
        End Set
    End Property
    ''' <summary>
    ''' Collected manometer unit of TH1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ManometerTH2unit As String
        Get
            Return _manometerTh2Unit
        End Get
        Set(value As String)
            _manometerTh2Unit = value
        End Set
    End Property

    ''' <summary>
    ''' Set if a new PLEXOR bluetooth device is selected
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property NewPlexorDevice As Boolean
        Set(value As Boolean)
            _displayDesktopAlert = value
        End Set
    End Property

    'MOD 17
    ''' <summary>
    ''' The error code during the initialization process of an individual step
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property InitErrorCode As Integer
        Get
            Return _initErrorCode
        End Get
    End Property

#End Region

    ''' <summary>
    ''' The different fields"(columns) in the initstatus grid. Used in the grid of displaying the data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InitDataFields
        Public Const FldStepStatus As String = "StepStatus"
        Public Const FldstepNumberDevice As String = "stepNumberDevice"
        Public Const FldStatusImage As String = "StatusImage"
        Public Const FldStepID As String = "StepID"
        Public Const FldStepMessage As String = "StepMessage"
        Public Const FLdDevice As String = "Device"
        Public Const FldStepNumber As String = "StepNumber"
    End Class


End Class